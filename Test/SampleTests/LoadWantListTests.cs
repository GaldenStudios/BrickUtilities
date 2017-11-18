using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleTests
{
    [TestClass]
    public class LoadWantListTests
    {
        [TestMethod]
        public void LoadWantListTests_Test()
        {
            var process = new Process();
            Assert.IsNotNull(process);

            process.StartInfo.FileName = "LoadWantList.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            var result = process.StandardOutput.ReadToEnd();

            process.WaitForExit(30000);

            Assert.AreEqual(
                "Loading file LoadWantList.xml...\r\n" +
                "File contents:\r\n" +
                "3622\r\n" +
                "3039\r\n" +
                "3001\r\n", result);
        }
    }
}
