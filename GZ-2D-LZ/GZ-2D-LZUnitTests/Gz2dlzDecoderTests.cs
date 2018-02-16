using System;
using G2_2D_LZ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzDecoderTests
    {
        private string inputTestFile = Environment.CurrentDirectory + "\\TestData\\test.bmp.mat";
        Gz2dlzDecoder decoder;

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchFlagCorrectly()
        {
            decoder = new Gz2dlzDecoder(inputTestFile);

            decoder.LoadMatrixFromTxtFile();

            var matchFlag = decoder.GetMatchFlag();

            Assert.AreEqual(5, matchFlag.GetLength(0));
            Assert.AreEqual(6, matchFlag.GetLength(1));

            for (int y = 0; y < matchFlag.GetLength(0); y++)
            {
                for (int x = 0; x < matchFlag.GetLength(1); x++)
                {
                    Assert.AreEqual(false, matchFlag[y,x]);
                }
            }
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchLocationCorrectly()
        {
            decoder = new Gz2dlzDecoder(inputTestFile);

            decoder.LoadMatrixFromTxtFile();
            var matchLocation = decoder.GetMatchLocation();

            var height = matchLocation.GetLength(0);
            var width = matchLocation.GetLength(1);

            Assert.AreEqual(5, height);
            Assert.AreEqual(6, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(0, matchLocation[y, x].Y);
                    Assert.AreEqual(0, matchLocation[y, x].X);
                }
            }
        }
        
        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchDimensionsCorrectly()
        {
            decoder = new Gz2dlzDecoder(inputTestFile);

            decoder.LoadMatrixFromTxtFile();
            var matchDimensions = decoder.GetMatchDimensions();

            var height = matchDimensions.GetLength(0);
            var width = matchDimensions.GetLength(1);

            Assert.AreEqual(5, height);
            Assert.AreEqual(6, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(0, matchDimensions[y, x].Width);
                    Assert.AreEqual(0, matchDimensions[y, x].Height);
                }
            }
        }


        [TestMethod]
        public void LoadMatrixFromTxtFileReadsResidualCorrectly()
        {
            decoder = new Gz2dlzDecoder(inputTestFile);

            decoder.LoadMatrixFromTxtFile();
            var residual = decoder.GetResidual();

            var height = residual.GetLength(0);
            var width = residual.GetLength(1);

            Assert.AreEqual(5, height);
            Assert.AreEqual(6, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(0, residual[y, x]);
                }
            }
        }


        [TestMethod]
        public void LoadMatrixFromTxtFileReadsPredictionErrorCorrectly()
        {
            decoder = new Gz2dlzDecoder(inputTestFile);
            int[,] expectedValues = new int[,] {
                {-128,  255, -255,  255, -255,  255 },
                {-128,  255, -255,  255, -255,    0 },
                { 127, -255,    0,    0,  255,    0  },
                {-128,  255, -255,  255, -255,  255 },
                { 127, -255,  255,    0, -255,    0 }};

            decoder.LoadMatrixFromTxtFile();
            var predictionError = decoder.GetPredictionError();

            var height = predictionError.GetLength(0);
            var width = predictionError.GetLength(1);

            Assert.AreEqual(5, height);
            Assert.AreEqual(6, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
                    Assert.AreEqual(expectedValues[y,x], predictionError[y, x]);
                }
            }
        }
    }
}
