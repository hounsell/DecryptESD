using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WIMCore.Exceptions;

namespace WIMCore.Tests
{
    [TestClass]
    public class IntegrityTests
    {
        [TestMethod]
        public void CheckIntegrityWithNoIntegrityData()
        {
            WimIntegrityException ex = Assert.ThrowsException<WimIntegrityException>(() =>
            {
                using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-CompressNone.wim"))
                {
                    bool succcess = wf.CheckIntegrity();
                }
            });

            Assert.AreEqual(WimIntegrityExceptionType.NoIntegrityData, ex.Type);
        }

        [TestMethod]
        public void LoadWin10WithIntegrityAndCheck()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-Integrity.wim"))
            {
                bool success = wf.CheckIntegrity();
                Assert.IsTrue(success);
            }
        }

        [TestMethod]
        public void LoadWin10WithBrokenIntegrityAndCheck()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-BrokenIntegrity.wim"))
            {
                bool success = wf.CheckIntegrity();
                Assert.IsTrue(!success);
            }
        }

        [TestMethod]
        public void LoadWin10MultipleImagesWithIntegrityAndCheck()
        {
            using (WimFile wf = new WimFile($"{AppContext.BaseDirectory}\\TestFiles\\Win10-RS1-MultiImage-Integrity.wim"))
            {
                Assert.IsTrue(wf.ImageCount == 2);

                bool success = wf.CheckIntegrity();
                Assert.IsTrue(success);
            }
        }
    }
}