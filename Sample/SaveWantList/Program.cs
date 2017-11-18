using System;
using BrickUtilities.BrickLink;

namespace SaveWantList
{
    class Program
    {
        private const string FileName = "SaveWantList.xml";

        static void Main()
        {
            // <SampleCode>
            var item = new WantListItem(WantListItemType.Part, "3622", new WantListColorId(11));
            var file = new WantListFile(new [] { item });

            Console.WriteLine("Saving file " + FileName + "...");
            file.Save(FileName);
            // </SampleCode>
        }
    }
}
