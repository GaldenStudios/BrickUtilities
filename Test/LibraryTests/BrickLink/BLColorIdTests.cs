using BrickUtilities.BrickLink;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests.BrickLink
{
    [TestClass]
    public class BLColorIdTests
    {
        [TestMethod]
        public void BLColorIdTests_Constructor()
        {
            var id = new BLColorId(10);
            Assert.AreEqual(10, id);
        }

        [TestMethod]
        public void BLColorIdTests_Equals()
        {
            var id1 = new BLColorId(10);
            var id2 = new BLColorId(10);
            var id3 = new BLColorId(11);
            Assert.IsTrue(id1.Equals(id2));
            Assert.IsFalse(id1.Equals(id3));
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.IsFalse(id1.Equals(10));
        }

        [TestMethod]
        public void BLColorIdTests_GetHashCode()
        {
            var id1 = new BLColorId(10);
            var id2 = new BLColorId(10);
            var id3 = new BLColorId(11);
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
            Assert.AreNotEqual(id1.GetHashCode(), id3.GetHashCode());
        }

        [TestMethod]
        public void BLColorIdTests_EqualsOperator()
        {
            var id1 = new BLColorId(10);
            var id2 = new BLColorId(10);
            var id3 = new BLColorId(11);
            Assert.IsTrue(id1 == id2);
            Assert.IsFalse(id1 == id3);
            Assert.IsTrue(id1 == 10);
            Assert.IsFalse(id1 == 11);
        }

        [TestMethod]
        public void BLColorIdTests_NotEqualsOperator()
        {
            var id1 = new BLColorId(10);
            var id2 = new BLColorId(10);
            var id3 = new BLColorId(11);
            Assert.IsFalse(id1 != id2);
            Assert.IsTrue(id1 != id3);
            Assert.IsFalse(id1 != 10);
            Assert.IsTrue(id1 != 11);
        }

        [TestMethod]
        public void BLColorIdTests_ToString()
        {
            var id = new BLColorId(10);
            Assert.AreEqual("10", id.ToString());
        }
    }
}