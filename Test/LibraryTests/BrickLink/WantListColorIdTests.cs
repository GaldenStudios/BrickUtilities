// -----------------------------------------------------------------------
// BrickUtilities
// Copyright (c) 2017 Galden Studios
// -----------------------------------------------------------------------

using BrickUtilities.BrickLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.BrickLink
{
    [TestClass]
    public class WantListColorIdTests
    {
        [TestMethod]
        public void WantListColorIdTests_Constructor()
        {
            var id = new BrickUtilities.BrickLink.WantListColorId(10);
            Assert.AreEqual(10, id);
        }

        [TestMethod]
        public void WantListColorIdTests_Equals()
        {
            var id1 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id2 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id3 = new BrickUtilities.BrickLink.WantListColorId(11);
            Assert.IsTrue(id1.Equals(id2));
            Assert.IsFalse(id1.Equals(id3));
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(id1.Equals(10));
        }

        [TestMethod]
        public void WantListColorIdTests_GetHashCode()
        {
            var id1 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id2 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id3 = new BrickUtilities.BrickLink.WantListColorId(11);
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
            Assert.AreNotEqual(id1.GetHashCode(), id3.GetHashCode());
        }

        [TestMethod]
        public void WantListColorIdTests_EqualsOperator()
        {
            var id1 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id2 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id3 = new BrickUtilities.BrickLink.WantListColorId(11);
            Assert.IsTrue(id1 == id2);
            Assert.IsFalse(id1 == id3);
            Assert.IsTrue(id1 == 10);
            Assert.IsFalse(id1 == 11);
        }

        [TestMethod]
        public void WantListColorIdTests_NotEqualsOperator()
        {
            var id1 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id2 = new BrickUtilities.BrickLink.WantListColorId(10);
            var id3 = new BrickUtilities.BrickLink.WantListColorId(11);
            Assert.IsFalse(id1 != id2);
            Assert.IsTrue(id1 != id3);
            Assert.IsFalse(id1 != 10);
            Assert.IsTrue(id1 != 11);
        }

        [TestMethod]
        public void WantListColorIdTests_ToString()
        {
            var id = new BrickUtilities.BrickLink.WantListColorId(10);
            Assert.AreEqual("10", id.ToString());
        }
    }
}