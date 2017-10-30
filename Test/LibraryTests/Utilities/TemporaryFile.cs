// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

using System;
using System.IO;

namespace LibraryTests.Utilities
{
    /// <summary>
    /// Temporary file
    /// </summary>
    internal class TemporaryFile : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TemporaryFile()
        {
            Path = System.IO.Path.GetTempFileName();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TemporaryFile(string contents)
        {
            Path = System.IO.Path.GetTempFileName();
            File.WriteAllText(Path, contents);
        }

        public string Path { get; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}