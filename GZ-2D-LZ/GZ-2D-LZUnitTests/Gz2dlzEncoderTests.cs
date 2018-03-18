using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Predictors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzEncoderTests
    {
        private Gz2DlzEncoder _encoder;
        AbstractPredictor _predictor;
        IReader _bmpReader;

        [TestInitialize]
        public void Setup()
        {
            _predictor = new ABasedPredictor();
            _bmpReader = new BmpReader();
        }

        [TestMethod]
        public void ImageIsLoadedIntoMemory()
        {
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

            byte[,] originalImage = _encoder.WorkImage;

            Assert.AreEqual(5, originalImage.GetLength(0));
            Assert.AreEqual(6, originalImage.GetLength(1));

            AssertEachValue(Constants.TestBmp, originalImage);
        }

        [TestMethod]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);
            _encoder.Encode();

            var predictionError = _encoder.PredictionError;
            int y = 0;
            double[] firstRow = { -128, 255, -255, 255, -255, 255 };

            for (int x = 0; x < predictionError.GetLength(1); x++)
            {
                Assert.AreEqual(firstRow[x], predictionError[y, x]);
            }
        }

        [TestMethod]
        public void EncodesTheFirstRowIsMarkedAsEncoded()
        {
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

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
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

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
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

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
        public void EncodesHasNoMatchedPoints()
        {
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

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
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

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
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

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
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

            _encoder.Encode();

            var matchDimensions = _encoder.MatchDimensions;
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
            _encoder = new Gz2DlzEncoder(Constants.Input4x4MatchBlockPath, _predictor, _bmpReader);
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

            Assert.AreEqual(4, bestMatch.Height);
            Assert.AreEqual(4, bestMatch.Width);
            Assert.AreEqual(16, bestMatch.Size);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatch()
        {
            _encoder = new Gz2DlzEncoder(Constants.Input4x4MatchBlockPath, _predictor, _bmpReader);

            _encoder.Encode();

            Assert.IsNotNull(_encoder.MatchLocation[6, 5]);
            Assert.AreEqual(1, _encoder.MatchLocation[6, 5].X);
            Assert.AreEqual(2, _encoder.MatchLocation[6, 5].Y);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchIfThereAre2BlockAtTheSameOrigin()
        {
            _encoder = new Gz2DlzEncoder(Constants.Input2PosibleMatchBlocksPath, _predictor, _bmpReader);
            _encoder.Encode();

            Assert.IsNotNull(_encoder.MatchLocation[1, 0]);
            Assert.AreEqual(0, _encoder.MatchLocation[1, 0].X);
            Assert.AreEqual(0, _encoder.MatchLocation[1, 0].Y);
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
