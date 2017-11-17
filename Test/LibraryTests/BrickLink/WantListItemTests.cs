// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

using System;
using BrickUtilities.BrickLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.BrickLink
{
    [TestClass]
    public class WantListItemTests
    {
        [TestMethod]
        public void WantListItemTests_Create()
        {
            var item = new WantListItem(WantListItemType.Part, "3001", new BrickUtilities.BrickLink.WantListColorId(20), 5.1, 100, 50);
            Assert.AreEqual(WantListItemType.Part, item.ItemType);
            Assert.AreEqual("3001", item.ItemNumber);
            Assert.IsNotNull(item.ColorId);
            Assert.AreEqual(20, item.ColorId.Value);
            Assert.AreEqual(5.1, item.MaximumPrice);
            Assert.AreEqual(100, item.MinimumDesiredQuantity);
            Assert.AreEqual(50, item.QuantityFilled);
        }

        [TestMethod]
        public void WantListItemTests_CreateWithInvalidItemNumber()
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new WantListItem(WantListItemType.Part, null, new BrickUtilities.BrickLink.WantListColorId(20), 5.1, 100, 50);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("itemNumber", e.ParamName);
            }
        }

        [TestMethod]
        public void WantListItemTests_UpdateMinimumDesiredQuantity()
        {
            var item = new WantListItem(WantListItemType.Part, "3001", new BrickUtilities.BrickLink.WantListColorId(20), 5.1, 100, 50);

            var item2 = item.UpdateMinimimDesiredQuantity(150);

            Assert.AreEqual(item.ItemType, item2.ItemType);
            Assert.AreEqual(item.ItemNumber, item2.ItemNumber);
            Assert.AreEqual(item.ColorId, item2.ColorId);
            Assert.AreEqual(item.MaximumPrice, item2.MaximumPrice);
            Assert.AreEqual(150, item2.MinimumDesiredQuantity);
            Assert.AreEqual(item.QuantityFilled, item2.QuantityFilled);
        }

        [TestMethod]
        public void WantListItemTests_UpdateQuantityFilled()
        {
            var item = new WantListItem(WantListItemType.Part, "3001", new BrickUtilities.BrickLink.WantListColorId(20), 5.1, 100, 50);

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