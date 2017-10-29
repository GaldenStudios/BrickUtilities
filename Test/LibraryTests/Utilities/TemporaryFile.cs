using System;
using System.IO;

namespace LibraryTests.Utilities
{
    /// <summary>
    /// Temporary file
    /// </summary>
    internal class TemporaryFile: IDisposable
    {
        public string Path { get; }

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

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}
