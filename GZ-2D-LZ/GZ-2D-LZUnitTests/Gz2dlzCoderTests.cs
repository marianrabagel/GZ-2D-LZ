using System;
using G2_2D_LZ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzCoderTests
    {
        string inputFileName = @"E:\Workspaces\GZ-2D-LZ\GZ-2D-LZ\GZ-2D-LZUnitTests\TestData\test.bmp";
        private Gz2dlzEncoder encoder;
        [TestInitialize]
        public void Setup()
        {
            encoder = new Gz2dlzEncoder(inputFileName);
        }

        [TestMethod]
        public void TestThatImageIsLoadedIntoMemory()
        {
            byte[,] testImage =
            {
                {0, 255, 0, 255, 0, 255},
                {0, 255, 0, 255, 0, 0},
                {255, 0, 0, 0, 255, 255},
                {0, 255, 0, 255, 0, 255},
                {255, 0, 255, 255, 0, 0}
            };
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

        [TestMethod]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            encoder.Encode();

            var predictionError = encoder.GetPredictionError();
            int y = 0;
            double[] firstRow = {-128, 255, -255, 255, -255, 255};

            for (int x = 0; x < predictionError.GetLength(1); x++)
            {
                Assert.AreEqual(firstRow[x], predictionError[y, x]);
            }
        }


        [TestMethod]
        public void EncodesTheFirstRowIsMarkedAsEncoded()
        {
            encoder.Encode();

            var isEncodedPixel = encoder.GetEncodedPixels();
            int y = 0;

            for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
            {
                Assert.IsTrue(isEncodedPixel[y, x]);
            }
        }
    }
}
