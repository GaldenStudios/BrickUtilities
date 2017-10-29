#----------------------------------------------------------------------------------------------------------
# Script start
#----------------------------------------------------------------------------------------------------------

param (
    [string]$command
)

Set-PSDebug -Strict
$ErrorActionPreference = "Stop"

$BuildOutputDirectory = "Drop"
$BuildTestOutputDirectory = "TestDrop"
$VersionFileName = "GitInfo.txt"
[string]$GitExecutable = ""
[string]$NuGetExecutable = ""

Add-Type -AssemblyName Microsoft.VisualBasic

#----------------------------------------------------------------------------------------------------------
# Executes command and fails if an error is returned
#----------------------------------------------------------------------------------------------------------
function Exec([scriptblock]$cmd, [string]$errorMessage = "Error executing command: " + $cmd)
{ 
    & $cmd 
    if ($LastExitCode -ne 0)
    {
        throw $errorMessage
    } 
}

#----------------------------------------------------------------------------------------------------------
# Executes command and returns true if succeeded, or false if error
#----------------------------------------------------------------------------------------------------------
function ExecReturnCode([scriptblock]$cmd)
{ 
    & $cmd 
    if ($LastExitCode -ne 0)
    {
        $false
    }
    else
    {
        $true
    }
}

#----------------------------------------------------------------------------------------------------------
# Finds a file in PATH
#----------------------------------------------------------------------------------------------------------
function FindInPath([string]$filename)
{
    $paths = $Env:Path.Split(';')
    foreach($path in $paths)
    {
       if(Test-Path -Path "$path\$filename")
       {
           Write-Host "Using executable: $path\$filename"
           "$path\$filename"
           return
       }
    }
    throw "Path to executable not found: $filename"
}

#----------------------------------------------------------------------------------------------------------
# Verifies that we are in a git repository
#----------------------------------------------------------------------------------------------------------
function VerifyInRepository()
{
    $result = Exec { & $GitExecutable status -b -s 2>&1 }
    $result = $result | Select-Object -First 1
    if(-not $result.StartsWith("##"))
    {
        throw "Not in git repository"
    }
}

#----------------------------------------------------------------------------------------------------------
# Gets the repository root
#----------------------------------------------------------------------------------------------------------
function GetRepositoryRoot()
{
    VerifyInRepository
    $result = Exec { & $GitExecutable rev-parse --show-toplevel 2>&1 }
    $result
}

#----------------------------------------------------------------------------------------------------------
# Gets the repository name
#----------------------------------------------------------------------------------------------------------
function GetRepositoryName()
{
    $root = GetRepositoryRoot
    $result = [System.IO.Path]::GetFileNameWithoutExtension($root).Split(".")[0]
    $result
}

#----------------------------------------------------------------------------------------------------------
# Get .sln file path
#----------------------------------------------------------------------------------------------------------
function GetSlnFilePath()
{
    $path = Get-ChildItem $repositoryRoot -Filter *.sln | % { $_.FullName }
    $path
}

#----------------------------------------------------------------------------------------------------------
# Get .sln configurations
#----------------------------------------------------------------------------------------------------------
function GetSlnConfigurations()
{
    $path = GetSlnFilePath
    $contents = [System.IO.File]::ReadLines($path)
    $inSection = $false
    $configurations = @()
    foreach($content in $contents)
    {
        if($content -match "GlobalSection\(SolutionConfigurationPlatforms\) = preSolution")
        {
            $inSection = $true
        }
        elseif ($content -match "EndGlobalSection")
        {
            $inSection = $false
        }
        elseif ($inSection)
        {
            $configuration = $content.Split("=")[0].Trim()
            $configurations = $configurations + $configuration
        } 
    }

    $configurations
}

#----------------------------------------------------------------------------------------------------------
# Get default debug .sln configurations
#----------------------------------------------------------------------------------------------------------
function GetDefaultDebugConfigurations()
{
    $configurations = GetSlnConfigurations
    $selectedConfigurations = @()
    if ($configurations -contains "Debug|Any CPU")
    {
        $selectedConfigurations = $selectedConfigurations + "Debug|Any CPU"
    }
    if ($configurations -contains "Debug|x86")
    {
        $selectedConfigurations = $selectedConfigurations + "Debug|x86"
    }
    if ($configurations -contains "Debug|Mixed Platforms")
    {
        $selectedConfigurations = $selectedConfigurations + "Debug|Mixed Platforms"
    }

    $selectedConfigurations 
}

#----------------------------------------------------------------------------------------------------------
# Get default release .sln configurations
#----------------------------------------------------------------------------------------------------------
function GetDefaultReleaseConfigurations()
{
    $configurations = GetSlnConfigurations
    $selectedConfigurations = @()
    if ($configurations -contains "Release|Any CPU")
    {
        $selectedConfigurations = $selectedConfigurations + "Release|Any CPU"
    }
    if ($configurations -contains "Release|x86")
    {
        $selectedConfigurations = $selectedConfigurations + "Release|x86"
    }
    if ($configurations -contains "Release|Mixed Platforms")
    {
        $selectedConfigurations = $selectedConfigurations + "Release|Mixed Platforms"
    }

    $selectedConfigurations 
}

#----------------------------------------------------------------------------------------------------------
# Get platform from configuration
#----------------------------------------------------------------------------------------------------------
function GetPlatformFromConfiguration([string]$configuration)
{
    $configuration.Split("|")[1]
}

#----------------------------------------------------------------------------------------------------------
# Get config from configuration
#----------------------------------------------------------------------------------------------------------
function GetConfigFromConfiguration([string]$configuration)
{
    $configuration.Split("|")[0]
}

#----------------------------------------------------------------------------------------------------------
# Build
#----------------------------------------------------------------------------------------------------------
function Build([string]$command,[string]$buildConfiguration,[bool]$officialBuild)
{
    $slnArg = GetSlnFilePath
    $typeArg = "/t:$command"
    $config = GetConfigFromConfiguration($buildConfiguration)
    $platform = GetPlatformFromConfiguration($buildConfiguration)
    $configurationArg = "/p:Configuration=$config"
    $platformArg = "/p:Platform=$platform"
    $officialBuildArg = ""
    if($officialBuild)
    {
        $officialBuildArg = "/p:OfficialBuild=true"
    }
    Exec { & "msbuild" $slnArg $typeArg $configurationArg $platformArg $officialBuildArg}
}

#----------------------------------------------------------------------------------------------------------
# Delete to recycle bin
#----------------------------------------------------------------------------------------------------------
function DeleteToRecycleBin([string]$path)
{
    $fullpath = [System.IO.Path]::GetFullPath($path)
    [Microsoft.VisualBasic.FileIO.FileSystem]::DeleteFile($fullpath,'OnlyErrorDialogs','SendToRecycleBin')
}

#----------------------------------------------------------------------------------------------------------
# Backs up and cleans extra files
#----------------------------------------------------------------------------------------------------------
function CleanExtraFiles([bool]$skipvsfiles)
{
    Push-Location $RepositoryRoot

    $extraFiles = Exec { & $GitExecutable ls-files --other 2>&1 }
    foreach($extraFile in $extraFiles)
    {
        if($skipvsfiles)
        {
            if($extraFile.StartsWith(".vs/"))
            {
                continue
            }
        }
        $result = ExecReturnCode { & $GitExecutable check-ignore $extraFile 2>&1 }
        if ($result -eq $false)
        {
            throw "Need to add file to git?: $extraFile"
        }
        else
        {
            "Deleting file: $RepositoryRoot\$extraFile"
            if($extraFile.StartsWith("packages/") -or
               $extraFiles.StartsWith("Build/BuildDocs/obj/.cache"))
            {
                Remove-Item "$RepositoryRoot\$extraFile"
            }
            else
            {
                DeleteToRecycleBin("$RepositoryRoot\$extraFile")
            }
        }
    }

    Get-ChildItem -recurse $RepositoryRoot | Where {$_.PSIsContainer -and `
        @(Get-ChildItem -Force -Lit $_.Fullname -r | Where {!$_.PSIsContainer}).Length -eq 0} |
        Remove-Item -recurse

    Pop-Location
}

#----------------------------------------------------------------------------------------------------------
# Restore packages
#----------------------------------------------------------------------------------------------------------
function RestorePackages()
{
    Push-Location $RepositoryRoot
    Exec { & $NuGetExecutable restore }
    $slnArg = GetSlnFilePath
    Exec { & "msbuild" "/T:Restore" $slnArg}
    Pop-Location
}

#----------------------------------------------------------------------------------------------------------
# RunTests
#----------------------------------------------------------------------------------------------------------
function RunTests()
{
    $platformDirectories = Get-ChildItem "$RepositoryRoot\$BuildTestOutputDirectory" -Name
    foreach ($platformDirectory in $platformDirectories)
    {
        if ($platformDirectory -eq "ARM")
        {
            continue
        }
        elseif ($platformDirectory -eq "x86")
        {        
        }
        elseif ($platformDirectory -eq "AnyCPU")
        {
        }
        elseif ($platformDirectory -eq "$SourceDirectory")
        {
            continue
        }
        else
        {
            throw "Unknown platform directory: $platformDirectory"
        }

        $configurationDirectories = Get-ChildItem "$RepositoryRoot\$BuildTestOutputDirectory\$platformDirectory" -Name
        foreach ($configurationDirectory in $configurationDirectories)
        {
            if ($configurationDirectory -eq "Debug")
            {
            }
            elseif ($configurationDirectory -eq "Release")
            {
            }
            else
            {
                throw "Unknown configuration directory: $configurationDirectory"
            }

            $subdir = "$RepositoryRoot\$BuildTestOutputDirectory\$platformDirectory\$configurationDirectory"
            if (Test-Path "$subdir")
            {
                $testDirectories = Get-ChildItem "$subdir" -Name
                foreach ($testDirectory in $testDirectories)
                {
                    $fullTestDirectory = "$subdir\$testDirectory"
                    Write-Host "Running tests at $fullTestDirectory"
                    $testFile = "$fullTestDirectory\$testDirectory.dll"
                    Exec { vstest.console.exe $testFile }
                }
            }
        }
    }
}

#----------------------------------------------------------------------------------------------------------
# Build several types
#----------------------------------------------------------------------------------------------------------
function BuildSeveral($configurations, [bool]$skipcleaningvsfiles)
{
    CleanExtraFiles $skipcleaningvsfiles
    RestorePackages
    foreach($configuration in $configurations)
    {
        Write-Host "Building $configuration" 
        Build "build" $configuration $false
    }
    RunTests
}

#----------------------------------------------------------------------------------------------------------
# Verifies that there are no uncommitted files
#----------------------------------------------------------------------------------------------------------
function VerifyNoUncommittedFiles()
{
    $result = Exec { & $GitExecutable status -s 2>&1 }
    if ($result)
    {
        throw "Error -- One or more uncommitted files"    
    }
}

#----------------------------------------------------------------------------------------------------------
# Gets the version file path
#----------------------------------------------------------------------------------------------------------
function GetVersionFilePath()
{
    $path = "$RepositoryRoot/$VersionFileName"
    $path
}

#----------------------------------------------------------------------------------------------------------
# Read version information
#----------------------------------------------------------------------------------------------------------
function GetVersionNumber()
{
    $path = GetVersionFilePath
    $contents = [System.IO.File]::ReadLines($path)
    if ($contents -notmatch "^\d+\.\d+\.\d+$")
    {
        throw "Unable to find current version"
    }

    $contents
}

#----------------------------------------------------------------------------------------------------------
# Increment version number in version file
#----------------------------------------------------------------------------------------------------------
function IncrementVersionNumber()
{
    $path = GetVersionFilePath
    $contents = [System.IO.File]::ReadAllText($path)

    $match = [RegEx]::Match($contents,"(\d+\.\d+\.)(\d+)")
    $start = $match.Groups[1]
    $end = $match.Groups[2]
    $end= [int]$end.Value +  1

    $contents = "$start$end"
    [System.IO.File]::WriteAllText($path, $contents)
}

#----------------------------------------------------------------------------------------------------------
# Get suffix for configuration
#----------------------------------------------------------------------------------------------------------
function GetSuffixForPlatformAndConfiguration([string]$platform, [string]$configuration)
{
    if ($configuration -eq "Release")
    {
        if ($platform -eq "AnyCPU")
        {
            ""
        }
        else
        {
            "-$platform"
        }
    }
    else
    {
        if ($platform -eq "AnyCPU")
        {
            "-$configuration"
        }
        else
        {
            "-$platform-$configuration"
        }
    }
}

#----------------------------------------------------------------------------------------------------------
# Drop
#----------------------------------------------------------------------------------------------------------
function Drop()
{
    $dropPath = "$env:BuildDropPath"
    if ($dropPath -ne '')
    {
        $repositoryName = GetRepositoryName
        $repositoryDropPath = "$dropPath/$repositoryName"

        New-Item -ItemType Directory -Force -Path "$repositoryDropPath" | Out-Null

        $platformDirectories = Get-ChildItem "$RepositoryRoot\$BuildOutputDirectory" -Name
        foreach ($platformDirectory in $platformDirectories)
        {
            $configurationDirectories = Get-ChildItem "$RepositoryRoot\$BuildOutputDirectory\$platformDirectory" -Name
            foreach ($configurationDirectory in $configurationDirectories)
            {
                if ($configurationDirectory -eq "Debug")
                {
                }
                elseif ($configurationDirectory -eq "Release")
                {
                }
                else
                {
                    throw "Unknown configuration directory: $configurationDirectory"
                }

                $subdir = "$RepositoryRoot\$BuildOutputDirectory\$platformDirectory\$configurationDirectory\$DropDirectory"
                if (Test-Path "$subdir")
                {
                    Write-Host "Dropping files at $subdir"
                    $files = Get-ChildItem "$subdir" -Name
                    foreach ($file in $files)
                    {
                        $sourcePath = "$subdir\$file"
                        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($sourcePath)
                        $extension = [System.IO.Path]::GetExtension($sourcePath)
                        $suffix= GetSuffixForPlatformAndConfiguration $platformDirectory $configurationDirectory
                        $destinationPath = "$repositoryDropPath/$fileName$suffix$extension"

                        Copy-Item "$sourcePath" "$destinationPath"
                    }
                }
            }
        }
    }
}

#----------------------------------------------------------------------------------------------------------
# DropNuget
#----------------------------------------------------------------------------------------------------------
function DropNuget()
{
    $dropNugetPath = "$env:BuildDropNugetPath"
    if ($dropNugetPath -ne '')
    {
        $repositoryName = GetRepositoryName

        New-Item -ItemType Directory -Force -Path "$dropNugetPath" | Out-Null

        $platformDirectories = Get-ChildItem "$RepositoryRoot\$BuildOutputDirectory" -Name
        foreach ($platformDirectory in $platformDirectories)
        {
            $configurationDirectories = Get-ChildItem "$RepositoryRoot\$BuildOutputDirectory\$platformDirectory" -Name
            foreach ($configurationDirectory in $configurationDirectories)
            {
                if ($configurationDirectory -ne "Release")
                {
                    continue
                }

                $subdir = "$RepositoryRoot\$BuildOutputDirectory\$platformDirectory\$configurationDirectory\$DropDirectory"
                if (Test-Path "$subdir")
                {
                    Write-Host "Dropping nuget files at $subdir"
                    $files = Get-ChildItem "$subdir" -Name
                    foreach ($file in $files)
                    {
                        $sourcePath = "$subdir\$file"
                        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($sourcePath)
                        $extension = [System.IO.Path]::GetExtension($sourcePath)
                        $suffix= GetSuffixForPlatformAndConfiguration $platformDirectory $configurationDirectory
                        $destinationPath = "$dropNugetPath/$fileName$suffix$extension"

                        Copy-Item "$sourcePath" "$destinationPath"
                    }
                }
            }
        }
    }
}

#----------------------------------------------------------------------------------------------------------
# Add all modified files to commit list
#----------------------------------------------------------------------------------------------------------
function AddModifiedFiles()
{
    $result = Exec { & $GitExecutable add --all 2>&1 }
}

#----------------------------------------------------------------------------------------------------------
# Submit all files
#----------------------------------------------------------------------------------------------------------
function SubmitAllFilesForOfficialBuild([string]$buildVersion)
{
    $result = Exec { & $GitExecutable commit -m "Official build: $buildVersion" 2>&1 }
}

#----------------------------------------------------------------------------------------------------------
# Tag with version
#----------------------------------------------------------------------------------------------------------
function TagWithVersion([string]$buildVersion)
{
    $result = Exec { & $GitExecutable tag "v$buildVersion" 2>&1 }
}

#----------------------------------------------------------------------------------------------------------
# Clean command
#----------------------------------------------------------------------------------------------------------
function CleanCommand()
{
    $configurations = GetSlnConfigurations
    foreach($configuration in $configurations)
    {
        Build "clean" "$configuration" $false
    }
    CleanExtraFiles $true
}

#----------------------------------------------------------------------------------------------------------
# FullClean command
#----------------------------------------------------------------------------------------------------------
function FullCleanCommand()
{
    $configurations = GetSlnConfigurations
    foreach($configuration in $configurations)
    {
        Build "clean" "$configuration" $false
    }
    CleanExtraFiles $false
}

#----------------------------------------------------------------------------------------------------------
# Debug command
#----------------------------------------------------------------------------------------------------------
function DebugCommand()
{
    $configurations = GetDefaultDebugConfigurations
    BuildSeveral $configurations $true
}

#----------------------------------------------------------------------------------------------------------
# Release command
#----------------------------------------------------------------------------------------------------------
function ReleaseCommand()
{
    $configurations = GetDefaultReleaseConfigurations
    BuildSeveral $configurations $true
}

#----------------------------------------------------------------------------------------------------------
# Full command
#----------------------------------------------------------------------------------------------------------
function FullCommand()
{
    $configurations = GetSlnConfigurations
    BuildSeveral $configurations $true
}

#----------------------------------------------------------------------------------------------------------
# Official command
#----------------------------------------------------------------------------------------------------------
function OfficialCommand()
{
    CleanExtraFiles $true
    VerifyNoUncommittedFiles

    RestorePackages
    $configurations = GetSlnConfigurations
    foreach($configuration in $configurations)
    {
        Build "Build" "$configuration" $true
    }
    RunTests
    Drop
    DropNuget

    $versionBeingBuilt = GetVersionNumber
    IncrementVersionNumber
    AddModifiedFiles
    SubmitAllFilesForOfficialBuild $versionBeingBuilt
    TagWithVersion $versionBeingBuilt
}

#----------------------------------------------------------------------------------------------------------
# Nuget command
#----------------------------------------------------------------------------------------------------------
function NugetCommand()
{
    $configurations = GetDefaultReleaseConfigurations
    BuildSeveral $configurations $true
    DropNuget
}

#----------------------------------------------------------------------------------------------------------
# Script code
#----------------------------------------------------------------------------------------------------------

$GitExecutable = FindInPath "git.exe"
$NuGetExecutable = FindInPath "nuget.exe"
$RepositoryRoot = GetRepositoryRoot

if ($command -eq "clean")
{
    CleanCommand
}
elseif ($command -eq "fullclean")
{
    FullCleanCommand
}
elseif ($command -eq "debug")
{
    DebugCommand
}
elseif ($command -eq "release")
{
    ReleaseCommand
}
elseif ($command -eq "full")
{
    FullCommand
}
elseif ($command -eq "official")
{
    OfficialCommand
}
elseif ($command -eq "nuget")
{
    NugetCommand
}
else
{
    Write-Host "ERROR: Unknown command"

    Write-Host "Commands:"
    Write-Host "  debug: Build/test debug version of code"
    Write-Host "  release: Build/test release version of code"
    Write-Host "  full: Build/test both versions of code"
    Write-Host "  official: Increment version number and build/test both versions of code"
    Write-Host "  clean: Delete all generated/built files"
    Write-Host "  fullclean: Delete all generated/built files, including those locked by visual studio"
    Write-Host "  nuget: Build/test release version of code, and then copy it to local nuget directory"
}
