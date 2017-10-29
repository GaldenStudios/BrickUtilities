using System;
using BrickUtilities.BrickLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.BrickLink
{
    [TestClass]
    public class BLPartsListItemTests
    {
        [TestMethod]
        public void BLPartsListItemTests_Create()
        {
            var item = new BLPartsListItem(BLItemType.Part, "3001", new BLColorId(20), 5.1, 100, 50);
            Assert.AreEqual(BLItemType.Part, item.ItemType);
            Assert.AreEqual("3001", item.ItemNumber);
            Assert.IsNotNull(item.ColorId);
            Assert.AreEqual(20, item.ColorId.Value);
            Assert.AreEqual(5.1, item.MaximumPrice);
            Assert.AreEqual(100, item.MinimumDesiredQuantity);
            Assert.AreEqual(50, item.QuantityFilled);
        }

        [TestMethod]
        public void BLPartsListItemTests_CreateWithInvalidItemNumber()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new BLPartsListItem(BLItemType.Part, null, new BLColorId(20), 5.1, 100, 50);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("itemNumber", e.ParamName);
            }
        }

        [TestMethod]
        public void BLPartsListItemTests_UpdateMinimumDesiredQuantity()
        {
            var item = new BLPartsListItem(BLItemType.Part, "3001", new BLColorId(20), 5.1, 100, 50);

            var item2 = item.UpdateMinimimDesiredQuantity(150);

            Assert.AreEqual(item.ItemType, item2.ItemType);
            Assert.AreEqual(item.ItemNumber, item2.ItemNumber);
            Assert.AreEqual(item.ColorId, item2.ColorId);
            Assert.AreEqual(item.MaximumPrice, item2.MaximumPrice);
            Assert.AreEqual(150, item2.MinimumDesiredQuantity);
            Assert.AreEqual(item.QuantityFilled, item2.QuantityFilled);
        }

        [TestMethod]
        public void BLPartsListItemTests_UpdateQuantityFilled()
        {
            var item = new BLPartsListItem(BLItemType.Part, "3001", new BLColorId(20), 5.1, 100, 50);

            var item2 = item.UpdateQuantityFilled(75);

            Assert.AreEqual(item.ItemType, item2.ItemType);
            Assert.AreEqual(item.ItemNumber, item2.ItemNumber);
            Assert.AreEqual(item.ColorId, item2.ColorId);
            Assert.AreEqual(item.MaximumPrice, item2.MaximumPrice);
            Assert.AreEqual(item.MinimumDesiredQuantity, item2.MinimumDesiredQuantity);
            Assert.AreEqual(75, item2.QuantityFilled);
        }
    }
}
