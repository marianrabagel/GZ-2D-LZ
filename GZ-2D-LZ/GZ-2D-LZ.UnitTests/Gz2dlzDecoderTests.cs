﻿using System;
using System.Diagnostics.CodeAnalysis;
using G2_2D_LZ;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Facades;
using G2_2D_LZ.Predictors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZ.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Gz2DlzDecoderTests
    {
        private readonly string basePath = "\\TestData\\Decoder\\";
        private string _testBmpMatFileName = "test_a.bmp.mat";
        private string _one3X4BlockBmpMatFileName = "3x4Block_a.bmp.mat";
        private string _2PossibleMatchBlocksBmpMatFileName = "2PossibleMatchBlocks_a.bmp.mat";

        private Gz2DlzDecoder _decoder;
        private IGz2DlzDecoderFacade gz2DlzDecoderFacade;
        
        [TestInitialize]
        public void Setup()
        {
            _testBmpMatFileName = GetFullPath(_testBmpMatFileName);
            _one3X4BlockBmpMatFileName = GetFullPath(_one3X4BlockBmpMatFileName);
            _2PossibleMatchBlocksBmpMatFileName = GetFullPath(_2PossibleMatchBlocksBmpMatFileName);
        }

        private string GetFullPath(string fileName)
        {
            return Environment.CurrentDirectory + $"{basePath}{fileName}";
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchFlagCorrectly()
        {
            SetTestBmpGz2DlzDecoderFacade();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();

            var matchFlag = _decoder.IsMatchFound;

            Assert.AreEqual(5, matchFlag.GetLength(0));
            Assert.AreEqual(6, matchFlag.GetLength(1));

            for (int y = 0; y < matchFlag.GetLength(0); y++)
            {
                for (int x = 0; x < matchFlag.GetLength(1); x++)
                {
                    Assert.AreEqual(false, matchFlag[y, x]);
                }
            }
        }

        private void SetTestBmpGz2DlzDecoderFacade()
        {
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _testBmpMatFileName;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchLocationCorrectly()
        {
            SetTestBmpGz2DlzDecoderFacade();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();
            var matchLocation = _decoder.MatchLocation;

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
            SetTestBmpGz2DlzDecoderFacade();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();
            var matchDimensions = _decoder.MatchDimension;

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
            SetTestBmpGz2DlzDecoderFacade();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();
            var residual = _decoder.Residual;

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
            SetTestBmpGz2DlzDecoderFacade();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);
            int[,] expectedValues = new int[,]
            {
                {-128, 255, -255, 255, -255, 255},
                {-128, 255, -255, 255, -255, 0},
                {127, -255, 0, 0, 255, 0},
                {-128, 255, -255, 255, -255, 255},
                {127, -255, 255, 0, -255, 0}
            };

            _decoder.LoadMatrixFromTxtFile();
            var predictionError = gz2DlzDecoderFacade.AbstractPredictor.PredictionError;

            var height = predictionError.GetLength(0);
            var width = predictionError.GetLength(1);

            Assert.AreEqual(5, height);
            Assert.AreEqual(6, width);

            AssertEachValue(expectedValues, predictionError);
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchFlagCorrectlyFor3X4Block()
        {
            bool[,] expected =
            {
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, true, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false}
            };
            SetTestBmpGz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _one3X4BlockBmpMatFileName;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();

            var matchFlag = _decoder.IsMatchFound;

            Assert.AreEqual(10, matchFlag.GetLength(0));
            Assert.AreEqual(10, matchFlag.GetLength(1));
            AssertEachValue(expected, matchFlag);
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsMatchLocationCorrectly3X4Block()
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

            SetTestBmpGz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _one3X4BlockBmpMatFileName;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();
            var matchLocation = _decoder.MatchLocation;

            var height = matchLocation.GetLength(0);
            var width = matchLocation.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);
            Assert.AreEqual( 1, matchLocation[6, 5].X);
            Assert.AreEqual( 2, matchLocation[6, 5].Y);

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    Assert.AreEqual( 0, matchLocation[y, x].Y);
                    Assert.AreEqual( 0, matchLocation[y, x].X);
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
        public void LoadMatrixFromTxtFileReadsMatchDimensionsCorrectly3X4Block()
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
            SetTestBmpGz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _one3X4BlockBmpMatFileName;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();
            var matchDimensions = _decoder.MatchDimension;

            var height = matchDimensions.GetLength(0);
            var width = matchDimensions.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);

            Assert.AreEqual( 4, matchDimensions[6, 5].Width);
            Assert.AreEqual( 3, matchDimensions[6, 5].Height);

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
        public void LoadMatrixFromTxtFileReadsResidualCorrectly3X4Block()
        {
            SetTestBmpGz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _one3X4BlockBmpMatFileName;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _decoder.LoadMatrixFromTxtFile();
            var residual = _decoder.Residual;

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
        public void LoadMatrixFromTxtFileReadsPredictionErrorCorrectly2PossibleMatchBlocks()
        {
            SetTestBmpGz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _2PossibleMatchBlocksBmpMatFileName;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            int[,] expectedValues = new int[,]
            {
                {64, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, -64, 64, -64, -128},
                {0, 0, 0, 0, 0, 0, 0, -192, 255, -127},
                {0, 0, 0, -192, 0, 255, -127, 127, -127, -128},
                {0, 0, 0, -192, 0, 128, 64, -64, 64, -64},
                {0, 0, 0, -192, 0, 0, 0, 0, 0, 128},
                {-32, 96, -64, 127, -127, -128, 0, 0, 0, 96},
                {64, -96, 96, -64, 64, -192, 0, 0, 0, 0},
                {-32, 159, -127, 127, -127, -128, 0, 0, 0, 128},
                {-128, 128, 64, -96, 159, -127, 127, -127, 127, -127}
            };

            _decoder.LoadMatrixFromTxtFile();
            var predictionError = gz2DlzDecoderFacade.AbstractPredictor.PredictionError;

            var height = predictionError.GetLength(0);
            var width = predictionError.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);
            AssertEachValue(expectedValues, predictionError);
        }

        [TestMethod]
        public void LoadMatrixFromTxtFileReadsPredictionErrorCorrectly3X4Block()
        {
            SetTestBmpGz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = _one3X4BlockBmpMatFileName;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);
            int[,] expectedValues = new int[,]
            {
                {64, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {-32, -96, 0, 0, 0, 192, -96, 96, -96, -96},
                {64, -192, 0, 0, 0, 96, 96, -192, 255, -159},
                {-64, -64, 0, 0, 0, 255, -159, 159, -159, -96},
                {127, -255, 0, 0, 0, 96, 96, -96, 96, -96},
                {64, -96, 159, -63, 63, -255, 0, 0, 0, 64},
                {-64, 128, -96, 159, -159, 0, 0, 0, 0, 64},
                {64, -128, 128, -96, 96, 0, 0, 0, 0, 64},
                {-64, 191, -159, 159, -159, 0, 0, 0, 0, 64},
                {-128, 96, 96, -128, 191, -191, 191, -191, 191, -63}
            };

            _decoder.LoadMatrixFromTxtFile();
            var predictionError = gz2DlzDecoderFacade.AbstractPredictor.PredictionError;

            var height = predictionError.GetLength(0);
            var width = predictionError.GetLength(1);

            Assert.AreEqual(10, height);
            Assert.AreEqual(10, width);
            AssertEachValue(expectedValues, predictionError);
        }

        private void DisplayImageToOutputWindow<T>(T[,] bytes)
        {
            int height1 = bytes.GetLength(0);
            int width1 = bytes.GetLength(1);

            for (int y = 0; y < height1; y++)
            {
                for (int x = 0; x < width1; x++)
                {
                    System.Diagnostics.Debug.Write(bytes[y, x] + " ");
                }

                System.Diagnostics.Debug.WriteLine(" ");
            }
        }

        private void AssertEachValue<T>(T[,] expectedValues, T[,] actualValues)
        {
            int height = actualValues.GetLength(0);
            int width = actualValues.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(expectedValues[y, x], actualValues[y, x]);
                }
            }
        }
    }
}
