using System;
using G2_2D_LZ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzCoderTests
    {
        string inputImage5x6 = @"E:\Workspaces\GZ-2D-LZ\GZ-2D-LZ\GZ-2D-LZUnitTests\TestData\test.bmp";
        string inputImage512x512 = @"E:\Workspaces\GZ-2D-LZ\GZ-2D-LZ\GZ-2D-LZUnitTests\TestData\test200.bmp";
        string inputImageChess = @"E:\Workspaces\GZ-2D-LZ\GZ-2D-LZ\GZ-2D-LZUnitTests\TestData\testChess.bmp";

        private Gz2dlzEncoder encoder;

        [TestInitialize]
        public void Setup()
        {
            
        }

        [TestMethod]
        public void ImageIsLoadedIntoMemory()
        {
            encoder = new Gz2dlzEncoder(inputImage5x6);
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
        [Ignore]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            //encoder.PredictFirstRow();

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
            encoder = new Gz2dlzEncoder(inputImage5x6);

            encoder.Encode();

            var isEncodedPixel = encoder.GetEncodedPixels();
            int y = 0;

            for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
            {
                Assert.IsTrue(isEncodedPixel[y, x]);
            }
        }
        
        [TestMethod]
        public void EncodeEncodesAllPixels()
        {
            encoder = new Gz2dlzEncoder(inputImage5x6);

            encoder.Encode();

            var isEncodedPixel = encoder.GetEncodedPixels();
            for (int y = 0; y < isEncodedPixel.GetLength(0); y++)
            {
                for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
                {
                    Assert.IsTrue(isEncodedPixel[y,x]);
                }    
            }
        }
        
        [TestMethod]
        public void EncodesHasNoMatchedPoints()
        {
            encoder = new Gz2dlzEncoder(inputImage5x6);

            encoder.Encode();

            var matchFlag = encoder.GetMatchFlag();
            for (int y = 0; y < matchFlag.GetLength(0); y++)
            {
                for (int x = 0; x < matchFlag.GetLength(1); x++)
                {
                    Assert.IsFalse(matchFlag[y, x]);
                }
            }
        }

        [TestMethod]
        public void EncodesHasNoResidualValues()
        {
            encoder = new Gz2dlzEncoder(inputImage5x6);

            encoder.Encode();

            var residual = encoder.GetResidual();
            for (int y = 0; y < residual.GetLength(0); y++)
            {
                for (int x = 0; x < residual.GetLength(1); x++)
                {
                    Assert.AreEqual(0, residual[y, x]);
                }
            }
        }

        [TestMethod]
        public void EncodesHasNoMatchLocation()
        {
            encoder = new Gz2dlzEncoder(inputImage5x6);

            encoder.Encode();

            var matchLocation = encoder.GetMatchLocation();
            for (int y = 0; y < matchLocation.GetLength(0); y++)
            {
                for (int x = 0; x < matchLocation.GetLength(1); x++)
                {
                    Assert.AreEqual(null, matchLocation[y, x]);
                }
            }
        }

        [TestMethod]
        public void EncodesHasNoMatchDimensions()
        {
            encoder = new Gz2dlzEncoder(inputImage5x6);

            encoder.Encode();

            var matchDimensions = encoder.GetMatchDimensions();
            for (int y = 0; y < matchDimensions.GetLength(0); y++)
            {
                for (int x = 0; x < matchDimensions.GetLength(1); x++)
                {
                    Assert.AreEqual(null, matchDimensions[y, x]);
                }
            }
        }

        [TestMethod]
        public void Test()
        {
            encoder = new Gz2dlzEncoder(inputImage512x512);
            
            encoder.Encode();
        }
    }
}
