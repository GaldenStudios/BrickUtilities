// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using BrickUtilities;
using BrickUtilities.BrickLink;
using LibraryTests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.BrickLink
{
    [TestClass]
    public class WantListFileTests
    {
        #region --------------------------------------------- Helpers

        /// <summary>
        /// GetItemTypeString
        /// </summary>
        private static string GetItemTypeString(WantListItemType itemType)
        {
            switch (itemType)
            {
                case WantListItemType.Set:
                    return "S";
                case WantListItemType.Part:
                    return "P";
                case WantListItemType.Minifig:
                    return "M";
                case WantListItemType.Book:
                    return "B";
                case WantListItemType.Gear:
                    return "G";
                case WantListItemType.Catalog:
                    return "C";
                case WantListItemType.Instruction:
                    return "I";
                case WantListItemType.OriginalBox:
                    return "O";
                case WantListItemType.UnsortedLot:
                    return "U";
                default:
                    throw new InvalidOperationException("Invalid itemtype: '" + itemType + "'");
            }
        }

        #endregion

        #region --------------------------------------------- Constructor

        [TestMethod]
        public void WantListFileTests_ConstructorSingleItem()
        {
            var item = new WantListItem(
                WantListItemType.Part, "3001", new BrickUtilities.BrickLink.WantListColorId(5), 5.1, 100, 50);
            var itemList = new List<WantListItem> {item};

            var blfile = new WantListFile(itemList);
            Assert.AreEqual(1, blfile.Items.Count);
            Assert.AreSame(item, blfile.Items[0]);
        }

        #endregion

        #region --------------------------------------------- Load

        [TestMethod]
        public void WantListFileTests_LoadOneItem()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3001</ITEMID>
                    <COLOR>5</COLOR>
                    <MAXPRICE>1.00</MAXPRICE>
                    <MINQTY>100</MINQTY>
                    <QTYFILLED>50</QTYFILLED>
                    <CONDITION>N</CONDITION>
                    <REMARKS>for MOC AB154A</REMARKS>
                    <NOTIFY>N</NOTIFY>
                    <WANTEDSHOW>N</WANTEDSHOW>
                    <WANTEDLIST>X</WANTEDLIST>
                  </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile(data))
            {
                var blfile = WantListFile.Load(file.Path);

                Assert.AreEqual(1, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.AreEqual(WantListItemType.Part, item.ItemType);
                Assert.AreEqual("3001", item.ItemNumber);
                Assert.AreEqual(1.00, item.MaximumPrice);
                Assert.AreEqual(100, item.MinimumDesiredQuantity);
                Assert.AreEqual(50, item.QuantityFilled);
                Assert.IsNotNull(item.ColorId);
                Assert.AreEqual(5, item.ColorId.Value);
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadOneItemMinimumData()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3622</ITEMID> 
                  </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile(data))
            {
                var blfile = WantListFile.Load(file.Path);

                Assert.AreEqual(1, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(WantListItemType.Part, item.ItemType);
                Assert.AreEqual("3622", item.ItemNumber);
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadOneItemWithEmptyOptionalElements()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3622</ITEMID>
                    <COLOR></COLOR>
                    <MAXPRICE></MAXPRICE>
                    <MINQTY></MINQTY>
                    <QTYFILLED></QTYFILLED>
                    <CONDITION></CONDITION>
                    <REMARKS></REMARKS>
                    <NOTIFY></NOTIFY>
                    <WANTEDSHOW></WANTEDSHOW>
                    <WANTEDLIST></WANTEDLIST>
                  </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile(data))
            {
                var blfile = WantListFile.Load(file.Path);

                Assert.AreEqual(1, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(WantListItemType.Part, item.ItemType);
                Assert.AreEqual("3622", item.ItemNumber);
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadMultipleItems()
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

            using (var file = new TemporaryFile(data))
            {
                var blfile = WantListFile.Load(file.Path);

                Assert.AreEqual(2, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(WantListItemType.Part, item.ItemType);
                Assert.AreEqual("3622", item.ItemNumber);

                item = blfile.Items[1];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(WantListItemType.Part, item.ItemType);
                Assert.AreEqual("3623", item.ItemNumber);
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadMissingRootItem()
        {
            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, "");

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("XML exception", e.Message);
                    Assert.AreEqual(0, e.LineNumber);
                    Assert.AreEqual("Root element is missing.", e.InnerException?.Message);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadMissingInventoryItem()
        {
            string data = @"
                <X>
                </X>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Missing root 'INVENTORY' element", e.Message);
                    Assert.AreEqual(2, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadMissingItemType()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                    <ITEMID>3622</ITEMID> 
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Missing 'ITEMTYPE' element", e.Message);
                    Assert.AreEqual(3, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadInvalidItemType()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                    <ITEMTYPE>X</ITEMTYPE>
                    <ITEMID>3622</ITEMID> 
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Invalid 'ITEMTYPE' value: 'X'", e.Message);
                    Assert.AreEqual(3, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadAllItemTypes()
        {
            foreach (WantListItemType itemType in Enum.GetValues(typeof(WantListItemType)))
            {
                var itemTypeString = GetItemTypeString(itemType);

                string data = @"
                  <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>" +
                              itemTypeString +
                              @"</ITEMTYPE>
                    <ITEMID>3622</ITEMID> 
                  </ITEM>
                  </INVENTORY>";

                using (var file = new TemporaryFile())
                {
                    File.WriteAllText(file.Path, data);

                    var blfile = WantListFile.Load(file.Path);

                    Assert.AreEqual(1, blfile.Items.Count);
                    Assert.AreEqual(itemType, blfile.Items[0].ItemType);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadMissingItemId()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Missing 'ITEMID' element", e.Message);
                    Assert.AreEqual(3, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadEmptyItemId()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                <ITEMID></ITEMID>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Invalid 'ITEMID' value: ''", e.Message);
                    Assert.AreEqual(5, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadDuplicateItemId()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                <ITEMID>1</ITEMID>
                <ITEMID>2</ITEMID>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Duplicate 'ITEMID' elements", e.Message);
                    Assert.AreEqual(3, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadInvalidColorId()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                <ITEMID>1</ITEMID>
                <COLOR>x</COLOR>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Invalid 'COLOR' value: 'x'", e.Message);
                    Assert.AreEqual(6, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadInvalidMinQuantity()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                <ITEMID>1</ITEMID>
                <MINQTY>x</MINQTY>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Invalid 'MINQTY' value: 'x'", e.Message);
                    Assert.AreEqual(6, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadInvalidQuantityFilled()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                <ITEMID>1</ITEMID>
                <QTYFILLED>x</QTYFILLED>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Invalid 'QTYFILLED' value: 'x'", e.Message);
                    Assert.AreEqual(6, e.LineNumber);
                }
            }
        }

        [TestMethod]
        public void WantListFileTests_LoadInvalidMaxPrice()
        {
            string data = @"
                <INVENTORY>
                <ITEM>
                <ITEMTYPE>P</ITEMTYPE>
                <ITEMID>1</ITEMID>
                <MAXPRICE>x</MAXPRICE>
                </ITEM>
                </INVENTORY>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    WantListFile.Load(file.Path);
                    Assert.Fail();
                }
                catch (FileParseException e)
                {
                    Assert.AreEqual("Invalid 'MAXPRICE' value: 'x'", e.Message);
                    Assert.AreEqual(6, e.LineNumber);
                }
            }
        }

        #endregion

        #region --------------------------------------------- Save

        [TestMethod]
        public void WantListFileTests_SaveSingleItem()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3001</ITEMID>
                    <COLOR>5</COLOR>
                    <MAXPRICE>5.1</MAXPRICE>
                    <MINQTY>100</MINQTY>
                    <QTYFILLED>50</QTYFILLED>
                  </ITEM>
                </INVENTORY>";

            var item = new WantListItem(
                WantListItemType.Part, "3001", new BrickUtilities.BrickLink.WantListColorId(5), 5.1, 100, 50);
            var itemList = new List<WantListItem> {item};

            var blfile = new WantListFile(itemList);
            using (var temporaryFile = new TemporaryFile())
            {
                blfile.Save(temporaryFile.Path);

                var fileData = File.ReadAllText(temporaryFile.Path);
                var doc1 = XDocument.Parse(fileData);
                var doc2 = XDocument.Parse(data);
                Assert.IsTrue(XNode.DeepEquals(doc1, doc2));
            }
        }

        [TestMethod]
        public void WantListFileTests_SaveMultipleItems()
        {
            const string data = @"
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

            var item1 = new WantListItem(
                WantListItemType.Part, "3622");
            var item2 = new WantListItem(
                WantListItemType.Part, "3623");
            var itemList = new List<WantListItem> {item1, item2};

            var blfile = new WantListFile(itemList);
            using (var temporaryFile = new TemporaryFile())
            {
                blfile.Save(temporaryFile.Path);

                var fileData = File.ReadAllText(temporaryFile.Path);
                var doc1 = XDocument.Parse(fileData);
                var doc2 = XDocument.Parse(data);
                Assert.IsTrue(XNode.DeepEquals(doc1, doc2));
            }
        }

        [TestMethod]
        public void WantListFileTests_SaveMinimumItem()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3001</ITEMID>
                  </ITEM>
                </INVENTORY>";

            var item = new WantListItem(
                WantListItemType.Part, "3001");
            var itemList = new List<WantListItem> {item};

            var blfile = new WantListFile(itemList);
            using (var temporaryFile = new TemporaryFile())
            {
                blfile.Save(temporaryFile.Path);

                var fileData = File.ReadAllText(temporaryFile.Path);
                var doc1 = XDocument.Parse(fileData);
                var doc2 = XDocument.Parse(data);
                Assert.IsTrue(XNode.DeepEquals(doc1, doc2));
            }
        }

        [TestMethod]
        public void WantListFileTests_SaveAllItemTypes()
        {
            foreach (WantListItemType itemType in Enum.GetValues(typeof(WantListItemType)))
            {
                var itemTypeString = GetItemTypeString(itemType);

                string data = @"
                  <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>" +
                              itemTypeString +
                              @"</ITEMTYPE>
                    <ITEMID>X</ITEMID> 
                  </ITEM>
                  </INVENTORY>";

                var item = new WantListItem(itemType, "X");
                var itemList = new List<WantListItem> {item};
                var blfile = new WantListFile(itemList);

                using (var temporaryFile = new TemporaryFile())
                {
                    blfile.Save(temporaryFile.Path);

                    var fileData = File.ReadAllText(temporaryFile.Path);
                    var doc1 = XDocument.Parse(fileData);
                    var doc2 = XDocument.Parse(data);
                    Assert.IsTrue(XNode.DeepEquals(doc1, doc2));
                }
            }
        }

        #endregion
    }
}