using System;
using BrickUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.Core
{
    [TestClass]
    public class FileParseExceptionTests
    {
        [TestMethod]
        public void FileParseExceptionTests_Constructor()
        {
            var e = new FileParseException("Message", 101);
            Assert.AreEqual("Message", e.Message);
            Assert.AreEqual(101, e.LineNumber);
        }

        [TestMethod]
        public void FileParseExceptionTests_ConstructorWithInnerException()
        {
            var innerException = new InvalidOperationException();
            var e = new FileParseException("Message", innerException);
            Assert.AreEqual("Message", e.Message);
            Assert.AreSame(innerException, e.InnerException);
        }
    }
}