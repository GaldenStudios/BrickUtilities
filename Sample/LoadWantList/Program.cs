using System;
using BrickUtilities.BrickLink;

namespace LoadWantList
{
    class Program
    {
        private const string FileName = "LoadWantList.xml";

        static void Main()
        {
            // <SampleCode>
            Console.WriteLine("Loading file " + FileName + "...");
            var file = WantListFile.Load(FileName);

            Console.WriteLine("File contents:");
            foreach (var item in file.Items)
            {
                Console.WriteLine(item.ItemNumber);
            }
            // </SampleCode>
        }
    }
}
