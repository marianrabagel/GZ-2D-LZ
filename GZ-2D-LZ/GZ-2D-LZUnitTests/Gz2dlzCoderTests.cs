using System;
using G2_2D_LZ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzCoderTests
    {
        [TestMethod]
        public void TestThatImageIsLoadedIntoMemory()
        {
            string inputFileName = @"E:\Workspaces\GZ-2D-LZ\GZ-2D-LZ\GZ-2D-LZUnitTests\TestData\test.bmp";
            byte[,] testImage =
            {
                {0, 255, 0, 255, 0, 255},
                {0, 255, 0, 255, 0, 0},
                {255, 0, 0, 0, 255, 255},
                {0, 255, 0, 255, 0, 255},
                {255, 0, 255, 255, 0, 0}
            };
            Gz2dlzEncoder encoder = new Gz2dlzEncoder(inputFileName);

            byte[,] originalImage = encoder.GetOriginalImage();

            Assert.AreEqual(5, originalImage.GetLength(0));
            Assert.AreEqual(6, originalImage.GetLength(1));

            for (int y = 0; y < originalImage.GetLength(0); y++)
            {
                for (int x = 0; x < originalImage.GetLength(1); x++)
                {
                    Assert.AreEqual(testImage[y,x], originalImage[y,x]);
                }
            }
        }
    }
}
