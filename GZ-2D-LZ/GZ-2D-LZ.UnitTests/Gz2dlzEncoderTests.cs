using System;
using System.Diagnostics.CodeAnalysis;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Facades;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Constants = GZ_2D_LZ.UnitTests.Common.Constants;

namespace GZ_2D_LZ.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Gz2DlzEncoderTests
    {
        private Gz2DlzEncoder _encoder;
        IImageReader _bmpReader;

        private static readonly string basePath = "\\TestData\\Encoder\\";
        public string TwoPossibleMatchBlocksBmpPath = Environment.CurrentDirectory + $"{basePath}2PossibleMatchBlocks.bmp";
        public string TestBmpPath = Environment.CurrentDirectory + $"{basePath}test.bmp";
        public string One4X4MatchBlockBmpPath = Environment.CurrentDirectory + $"{basePath}4x4Block.bmp";

        private IGz2DlzEncoderFacade _gz2DlzEncoderFacade;

        [TestInitialize]
        public void Setup()
        {
            _bmpReader = new BmpImageReader();
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
        }

        [TestMethod]
        public void ImageIsLoadedIntoMemory()
        {
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            byte[,] originalImage = _encoder.WorkImage;

            Assert.AreEqual(5, originalImage.GetLength(0));
            Assert.AreEqual(6, originalImage.GetLength(1));

            AssertEachValue(Constants.TestBmp, originalImage);
        }
        
        [TestMethod]
        public void EncodesTheFirstRowIsMarkedAsEncoded()
        {
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var isEncodedPixel = _encoder.IsPixelEncoded;
            int y = 0;

            for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
            {
                Assert.IsTrue(isEncodedPixel[y, x]);
            }
        }

        [TestMethod]
        public void EncodeEncodesAllPixelsForA5X6Image()
        {
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var isEncodedPixel = _encoder.IsPixelEncoded;
            for (int y = 0; y < isEncodedPixel.GetLength(0); y++)
            {
                for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
                {
                    Assert.IsTrue(isEncodedPixel[y, x]);
                }
            }
        }

        [TestMethod]
        public void EncodeEncodesAllPixelsForAnImageWithAMatchingBlock()
        {
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var isEncodedPixel = _encoder.IsPixelEncoded;
            for (int y = 0; y < isEncodedPixel.GetLength(0); y++)
            {
                for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
                {
                    Assert.IsTrue(isEncodedPixel[y, x]);
                }
            }
        }

        [TestMethod]
        public void EncodesHasNoMatchesFound()
        {
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var matchFlag = _encoder.IsMatchFound;
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
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var residual = _encoder.Residual;
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
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var matchLocation = _encoder.MatchLocation;
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
            SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            var matchDimensions = _encoder.MatchDimension;
            for (int y = 0; y < matchDimensions.GetLength(0); y++)
            {
                for (int x = 0; x < matchDimensions.GetLength(1); x++)
                {
                    Assert.AreEqual(null, matchDimensions[y, x]);
                }
            }
        }

        [TestMethod]
        public void LocateTheBestAproximateMatchForGivenRootPixelGivesTheExpectedBestMatch()
        {
            _gz2DlzEncoderFacade.InputFilePath = One4X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var originalImage = _encoder.WorkImage;
            var width = originalImage.GetLength(0);

            for (int i = 0; i < width; i++)
            {
                _encoder.IsPixelEncoded[0, i] = true;
                _encoder.IsPixelEncoded[1, i] = true;
                _encoder.IsPixelEncoded[2, i] = true;
                _encoder.IsPixelEncoded[3, i] = true;
                _encoder.IsPixelEncoded[4, i] = true;
            }
            for (int i = 0; i < 5; i++)
            {
                _encoder.IsPixelEncoded[0, i] = true;
            }

            var encoderPosition = new PixelLocation(5, 5);
            var rootPosition = new PixelLocation(1, 1);
            BestMatch bestMatch = _encoder.LocateTheBestAproximateMatchForGivenRootPixel(encoderPosition, rootPosition);

            Assert.AreEqual((uint) 4, bestMatch.Height);
            Assert.AreEqual((uint) 4, bestMatch.Width);
            Assert.AreEqual((uint) 16, bestMatch.Size);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatch()
        {
            _gz2DlzEncoderFacade.InputFilePath = One4X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            Assert.IsNotNull(_encoder.MatchLocation[6, 5]);
            Assert.AreEqual((uint) 1, _encoder.MatchLocation[6, 5].X);
            Assert.AreEqual((uint) 2, _encoder.MatchLocation[6, 5].Y);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchIfThereAre2BlockAtTheSameOrigin()
        {
            _gz2DlzEncoderFacade.InputFilePath = TwoPossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            Assert.IsNotNull(_encoder.MatchLocation[1, 0]);
            Assert.AreEqual((uint) 0, _encoder.MatchLocation[1, 0].X);
            Assert.AreEqual((uint) 0, _encoder.MatchLocation[1, 0].Y);
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

        private void SetGz2DLzEncoderFacadeWithTestBmpABasedPredictorAndBmpReader()
        {
            _gz2DlzEncoderFacade.InputFilePath = TestBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
        }
    }
}
