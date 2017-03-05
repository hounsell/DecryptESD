using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WIMCore.Exceptions;

namespace WIMCore.Tests
{
    [TestClass]
    public class LoadTests
    {
        [TestMethod]
        public void LoadInvalidSizeWithError()
        {
            WimInvalidException wnvex = Assert.ThrowsException<WimInvalidException>(() =>
            {
                using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Error-InvalidSize.wim"))
                {
                }
            });

            Assert.AreEqual(WimInvalidExceptionType.InvalidSize, wnvex.Type);
        }

        [TestMethod]
        public void LoadInvalidMagicWithError()
        {
            WimInvalidException wnvex = Assert.ThrowsException<WimInvalidException>(() =>
            {
                using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Error-InvalidMagic.wim"))
                {
                }
            });

            Assert.AreEqual(WimInvalidExceptionType.InvalidMagic, wnvex.Type);
        }

        [TestMethod]
        public void LoadWin10Rs1CompressNone()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressNone.wim"))
            {
                Assert.IsTrue((wf.Flags & WimHeaderFlags.Compressed) == 0);
            }
        }

        [TestMethod]
        public void LoadWin10Rs1CompressFast()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressFast.wim"))
            {
                Assert.IsTrue((wf.Flags & WimHeaderFlags.CompressXpress) > 0);
            }
        }

        [TestMethod]
        public void LoadWin10Rs1CompressMax()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressMax.wim"))
            {
                Assert.IsTrue((wf.Flags & WimHeaderFlags.CompressLzx) > 0);
            }
        }

        [TestMethod]
        public void LoadWin10Rs1MultipleImages()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-MultiImage-Integrity.wim"))
            {
                Assert.IsTrue(wf.ImageCount == 2);
            }
        }
    }
}