using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleTests
{
    [TestClass]
    public class SaveWantListTests
    {
        [TestMethod]
        public void SaveWantListTests_Test()
        {
            var process = new Process();
            Assert.IsNotNull(process);

            process.StartInfo.FileName = "SaveWantList.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            var result = process.StandardOutput.ReadToEnd();

            process.WaitForExit(30000);

            Assert.AreEqual(
                "Saving file SaveWantList.xml...\r\n", result);

            var savedFile = File.ReadAllText("SaveWantList.xml");
            Assert.AreEqual(
                "<INVENTORY>\r\n" +
                "  <ITEM>\r\n" +
                "    <ITEMTYPE>P</ITEMTYPE>\r\n" +
                "    <ITEMID>3622</ITEMID>\r\n" +
                "    <COLOR>11</COLOR>\r\n" +
                "  </ITEM>\r\n" +
                "</INVENTORY>",
                savedFile);
        }
    }
}
