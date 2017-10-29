using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using BrickUtilities.BrickLink;
using LibraryTests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrickUtilities;

namespace LibraryTests.BrickLink
{
    [TestClass]
    public class BLPartsListFileTests
    {

        #region --------------------------------------------- Helpers

        /// <summary>
        /// GetItemTypeString
        /// </summary>
        private static string GetItemTypeString(BLItemType itemType)
        {
            switch (itemType)
            {
                case BLItemType.Set:
                    return "S";
                case BLItemType.Part:
                    return "P";
                case BLItemType.Minifig:
                    return "M";
                case BLItemType.Book:
                    return "B";
                case BLItemType.Gear:
                    return "G";
                case BLItemType.Catalog:
                    return "C";
                case BLItemType.Instruction:
                    return "I";
                case BLItemType.OriginalBox:
                    return "O";
                case BLItemType.UnsortedLot:
                    return "U";
                default:
                    throw new InvalidOperationException("Invalid itemtype: '" + itemType + "'");
            }
        }

        #endregion

        #region --------------------------------------------- Constructor

        [TestMethod]
        public void BLPartsListFileTests_ConstructorSingleItem()
        {
            var item = new BLPartsListItem(
                BLItemType.Part, "3001", new BLColorId(5), 5.1, 100, 50);
            var itemList = new List<BLPartsListItem> { item };

            var blfile = new BLPartsListFile(itemList);
            Assert.AreEqual(1, blfile.Items.Count);
            Assert.AreSame(item, blfile.Items[0]);
        }

        #endregion

        #region --------------------------------------------- Load

        [TestMethod]
        public void BLPartsListFileTests_LoadOneItem()
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
                var blfile = BLPartsListFile.Load(file.Path);

                Assert.AreEqual(1, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.AreEqual(BLItemType.Part, item.ItemType);
                Assert.AreEqual("3001", item.ItemNumber);
                Assert.AreEqual(1.00, item.MaximumPrice);
                Assert.AreEqual(100, item.MinimumDesiredQuantity);
                Assert.AreEqual(50, item.QuantityFilled);
                Assert.IsNotNull(item.ColorId);
                Assert.AreEqual(5, item.ColorId.Value);
            }
        }

        [TestMethod]
        public void BLPartsListFileTests_LoadOneItemMinimumData()
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
                var blfile = BLPartsListFile.Load(file.Path);

                Assert.AreEqual(1, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(BLItemType.Part, item.ItemType);
                Assert.AreEqual("3622", item.ItemNumber);
            }
        }

        [TestMethod]
        public void BLPartsListFileTests_LoadOneItemWithEmptyOptionalElements()
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
                var blfile = BLPartsListFile.Load(file.Path);

                Assert.AreEqual(1, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(BLItemType.Part, item.ItemType);
                Assert.AreEqual("3622", item.ItemNumber);
            }
        }

        [TestMethod]
        public void BLPartsListFileTests_LoadMultipleItems()
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
                var blfile = BLPartsListFile.Load(file.Path);

                Assert.AreEqual(2, blfile.Items.Count);

                var item = blfile.Items[0];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(BLItemType.Part, item.ItemType);
                Assert.AreEqual("3622", item.ItemNumber);

                item = blfile.Items[1];
                Assert.IsNull(item.MaximumPrice);
                Assert.IsNull(item.ColorId);
                Assert.IsNull(item.MinimumDesiredQuantity);
                Assert.IsNull(item.QuantityFilled);
                Assert.AreEqual(BLItemType.Part, item.ItemType);
                Assert.AreEqual("3623", item.ItemNumber);
            }
        }

        [TestMethod]
        public void BLPartsListFileTests_LoadMissingRootItem()
        {
            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, "");

                try
                {
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadMissingInventoryItem()
        {
            string data = @"
                <X>
                </X>";

            using (var file = new TemporaryFile())
            {
                File.WriteAllText(file.Path, data);

                try
                {
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadMissingItemType()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadInvalidItemType()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadAllItemTypes()
        {
            foreach (BLItemType itemType in Enum.GetValues(typeof(BLItemType)))
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

                    var blfile = BLPartsListFile.Load(file.Path);

                    Assert.AreEqual(1, blfile.Items.Count);
                    Assert.AreEqual(itemType, blfile.Items[0].ItemType);
                }
            }
        }

        [TestMethod]
        public void BLPartsListFileTests_LoadMissingItemId()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadEmptyItemId()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadDuplicateItemId()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadInvalidColorId()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadInvalidMinQuantity()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadInvalidQuantityFilled()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_LoadInvalidMaxPrice()
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
                    BLPartsListFile.Load(file.Path);
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
        public void BLPartsListFileTests_SaveSingleItem()
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

            var item = new BLPartsListItem(
                BLItemType.Part, "3001", new BLColorId(5), 5.1, 100, 50);
            var itemList = new List<BLPartsListItem> { item };

            var blfile = new BLPartsListFile(itemList);
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
        public void BLPartsListFileTests_SaveMultipleItems()
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

            var item1 = new BLPartsListItem(
                BLItemType.Part, "3622");
            var item2 = new BLPartsListItem(
                BLItemType.Part, "3623");
            var itemList = new List<BLPartsListItem> { item1, item2 };

            var blfile = new BLPartsListFile(itemList);
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
        public void BLPartsListFileTests_SaveMinimumItem()
        {
            string data = @"
                <INVENTORY>
                  <ITEM>
                    <ITEMTYPE>P</ITEMTYPE>
                    <ITEMID>3001</ITEMID>
                  </ITEM>
                </INVENTORY>";

            var item = new BLPartsListItem(
                BLItemType.Part, "3001");
            var itemList = new List<BLPartsListItem> { item };

            var blfile = new BLPartsListFile(itemList);
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
        public void BLPartsListFileTests_SaveAllItemTypes()
        {
            foreach (BLItemType itemType in Enum.GetValues(typeof(BLItemType)))
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

                var item = new BLPartsListItem(itemType, "X");
                var itemList = new List<BLPartsListItem> { item };
                var blfile = new BLPartsListFile(itemList);

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