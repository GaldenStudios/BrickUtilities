using System;

// ReSharper disable once CheckNamespace
namespace BrickUtilities
{
    /// <summary>
    /// Exception thrown with the parsing of a file fails
    /// </summary>
    public class FileParseException: Exception
    {
        /// <summary>
        /// Line number of the parsing error
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="lineNumber">Line number of command in the file</param>
        public FileParseException(string message, int lineNumber):
            base(message)
        {
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public FileParseException(string message, Exception innerException):
            base(message, innerException)
        {
        }

    }
}
