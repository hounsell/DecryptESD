using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WIMCore.Tests
{
    [TestClass]
    public class ReaderTests
    {
        [TestMethod]
        public void LoadInvalidSizeWithError()
        {
            WimNotValidException wnvex = Assert.ThrowsException<WimNotValidException>(() =>
            {
                using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Error-InvalidSize.wim"))
                {
                }
            });

            Assert.AreEqual(WimNotValidException.ErrorType.InvalidSize, wnvex.Type);
        }
        [TestMethod]
        public void LoadInvalidMagicWithError()
        {
            WimNotValidException wnvex = Assert.ThrowsException<WimNotValidException>(() =>
            {
                using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Error-InvalidMagic.wim"))
                {
                }
            });

            Assert.AreEqual(WimNotValidException.ErrorType.InvalidMagic, wnvex.Type);
        }

        [TestMethod]
        public void LoadWin10Rs1CompressNone()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressNone.wim"))
            {
                Assert.IsNotNull(wf);
                Assert.IsTrue((wf.Flags & WimHeaderFlags.Compressed) == 0);
            }
        }

        [TestMethod]
        public void LoadWin10Rs1CompressFast()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressFast.wim"))
            {
                Assert.IsNotNull(wf);
                Assert.IsTrue((wf.Flags & WimHeaderFlags.CompressXpress) > 0);
            }
        }

        [TestMethod]
        public void LoadWin10Rs1CompressMax()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressMax.wim"))
            {
                Assert.IsNotNull(wf);
                Assert.IsTrue((wf.Flags & WimHeaderFlags.CompressLzx) > 0);
            }
        }
    }
}