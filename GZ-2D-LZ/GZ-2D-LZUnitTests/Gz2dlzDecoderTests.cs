using System;
using G2_2D_LZ;
using G2_2D_LZ.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzDecoderTests
    {
        private string inputTestFile = Environment.CurrentDirectory + "\\TestData\\test.bmp.mat";
        private string input4x4Block = Environment.CurrentDirectory + "\\TestData\\4x4Block.bmp.mat";
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

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchFlagCorrectlyFor4x4Block()
        {
            bool[,] expected = new bool[,] {
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, true , false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false },
                {false, false, false, false, false, false, false, false, false, false } 
            };
            decoder = new Gz2dlzDecoder(input4x4Block);

            decoder.LoadMatrixFromTxtFile();

            var matchFlag = decoder.GetMatchFlag();

            var height = matchFlag.GetLength(0);
            var width = matchFlag.GetLength(1);

            Assert.AreEqual(10, matchFlag.GetLength(0));
            Assert.AreEqual(10, matchFlag.GetLength(1));
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(expected[y,x], matchFlag[y, x]);
                }
            }
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchLocationCorrectly4x4Block()
        {
           /*x y x y..
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 1 2 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 }*/

            decoder = new Gz2dlzDecoder(input4x4Block);

            decoder.LoadMatrixFromTxtFile();
            var matchLocation = decoder.GetMatchLocation();

            var height = matchLocation.GetLength(0);
            var width = matchLocation.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);
            Assert.AreEqual(1, matchLocation[6, 5].X);
            Assert.AreEqual(2, matchLocation[6, 5].Y);

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.AreEqual(0, matchLocation[y, x].Y);
                    Assert.AreEqual(0, matchLocation[y, x].X);
                }
            }

            for (int y = 6; y < height; y++)
            {
                for (int x = 6; x < width; x++)
                {
                    Assert.AreEqual(0, matchLocation[y, x].Y);
                    Assert.AreEqual(0, matchLocation[y, x].X);
                }
            }
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchDimensionsCorrectly4x4Block()
        {
           /*x y x y ..
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 4 3 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 
             */

            decoder = new Gz2dlzDecoder(input4x4Block);

            decoder.LoadMatrixFromTxtFile();
            var matchDimensions = decoder.GetMatchDimensions();

            var height = matchDimensions.GetLength(0);
            var width = matchDimensions.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);

            Assert.AreEqual(4, matchDimensions[6, 5].Width);
            Assert.AreEqual(3, matchDimensions[6, 5].Height);

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.AreEqual(0, matchDimensions[y, x].Width);
                    Assert.AreEqual(0, matchDimensions[y, x].Height);
                }
            }

            for (int y = 6; y < height; y++)
            {
                for (int x = 6; x < width; x++)
                {
                    Assert.AreEqual(0, matchDimensions[y, x].Width);
                    Assert.AreEqual(0, matchDimensions[y, x].Height);
                }
            }
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsResidualCorrectly4x4Block()
        {
            decoder = new Gz2dlzDecoder(input4x4Block);

            decoder.LoadMatrixFromTxtFile();
            var residual = decoder.GetResidual();

            var height = residual.GetLength(0);
            var width = residual.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(0, residual[y, x]);
                }
            }
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsPredictionErrorCorrectly4x4Block()
        {
            decoder = new Gz2dlzDecoder(input4x4Block);
            int[,] expectedValues = new int[,] {
                {64, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                {-32, -96, 0, 0, 0, 192, -96, 96, -96, -96},
                {64, -192, 0, 0, 0, 96, 96, -192, 255, -159},
                {-64, -64, 0, 0, 0, 255, -159, 159, -159, -96},
                {127, -255, 0, 0, 0, 96, 96, -96, 96, -96 },
                {64, -96, 159, -63, 63, -255, 0, 0, 0, 64 },
                {-64, 128, -96, 159, -159, 0, 0, 0, 0, 64 },
                {64, -128, 128, -96, 96, -192, 0, 0, 0, 64},
                {-64, 191, -159, 159, -159, -96, 0, 0, 0, 64},
                {-128, 96, 96, -128, 191, -191, 191, -191, 191, -63 }};

            decoder.LoadMatrixFromTxtFile();
            var predictionError = decoder.GetPredictionError();

            var height = predictionError.GetLength(0);
            var width = predictionError.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(expectedValues[y, x], predictionError[y, x]);
                }
            }
        }
    }
}
