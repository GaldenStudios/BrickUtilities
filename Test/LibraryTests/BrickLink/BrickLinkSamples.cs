// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using BrickUtilities.BrickLink;
using LibraryTests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.BrickLink
{
    /// <summary>
    /// Tests that contain sample code displayed in the docs
    /// </summary>
    [TestClass]
    public class BrickLinkSamples
    {
        [TestMethod]
        public void BrickLinkSamples_LoadWantList()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3622</ITEMID> 
                  </ITEM>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3623</ITEMID> 
                  </ITEM>
                </INVENTORY>";

            using (var temporaryFile = new TemporaryFile(data))
            {
                string fileName = temporaryFile.Path;

                // <LoadWantList>
                var file = WantListFile.Load(fileName);
                foreach (var item in file.Items)
                {
                    Console.WriteLine(item.ItemNumber);
                }
                // </LoadWantList>

                Assert.AreEqual(2, file.Items.Count);
                Assert.AreEqual("3622", file.Items[0].ItemNumber);
                Assert.AreEqual("3623", file.Items[1].ItemNumber);
            }
        }

        [TestMethod]
        public void BrickLinkSamples_CreateWantList()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                string fileName = temporaryFile.Path;

                // <CreateWantList>
                var item = new WantListItem(WantListItemType.Part, "3622", new WantListColorId(11));
                var file = new WantListFile(new[] { item });
                file.Save(fileName);
                // </CreateWantList>

                var savedFile = File.ReadAllText(temporaryFile.Path);
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

        [TestMethod]
        public void BrickLinkSamples_UpdateWantList()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3622</ITEMID> 
                    <MINQTY>2</MINQTY>
                  </ITEM>
                </INVENTORY>";

            using (var temporaryFile = new TemporaryFile(data))
            {
                string fileName = temporaryFile.Path;

                // <UpdateWantList>
                var file = WantListFile.Load(fileName);
                var newItems = new List<WantListItem>();
                foreach (var item in file.Items)
                {
                    if (item.ItemNumber != "3622")
                        newItems.Add(item);
                    else
                        newItems.Add(item.UpdateMinimimDesiredQuantity(item.MinimumDesiredQuantity+1));
                }
                var newFile = new WantListFile(newItems);
                newFile.Save(fileName);
                // </UpdateWantList>

                var savedFile = File.ReadAllText(temporaryFile.Path);
                Assert.AreEqual(
                    "<INVENTORY>\r\n" +
                    "  <ITEM>\r\n" +
                    "    <ITEMTYPE>P</ITEMTYPE>\r\n" +
                    "    <ITEMID>3622</ITEMID>\r\n" +
                    "    <MINQTY>3</MINQTY>\r\n" +
                    "  </ITEM>\r\n" +
                    "</INVENTORY>",
                    savedFile);
            }
        }
    }
}