using System;
using System.Diagnostics.CodeAnalysis;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Facades;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using GZ_2D_LZ.Archiver;
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
        public string One3X4MatchBlockBmpPath = Environment.CurrentDirectory + $"{basePath}3x4Block.bmp";
        public string One6X3MatchBlockBmpPath = Environment.CurrentDirectory + $"{basePath}6x3Block.bmp";
        public string VerticalMirrorBmpPath = Environment.CurrentDirectory + $"{basePath}VerticalMirror.bmp";

        private IGz2DlzEncoderFacade _gz2DlzEncoderFacade;

        [TestInitialize]
        public void Setup()
        {
            _bmpReader = new BmpImageReader();
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 17;
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
            _gz2DlzEncoderFacade.InputFilePath = One3X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            var originalImage = _encoder.WorkImage;
            MarkFirstRowAsPredicted(originalImage);
            var encoderPosition = new PixelLocation(5, 5);
            var rootPosition = new PixelLocation(1, 1);

            BestMatch bestMatch = _encoder.LocateTheBestAproximateMatchForGivenRootPixel(encoderPosition, rootPosition);

            Assert.AreEqual(4, bestMatch.Height);
            Assert.AreEqual(4, bestMatch.Width);
            Assert.AreEqual(16, bestMatch.Size);
        }

        private void MarkFirstRowAsPredicted(byte[,] originalImage)
        {
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
        }

        [TestMethod]
        public void EncodeDoesntFindTheExpectedBlockMatchForMinMatchSize17()
        {
            _gz2DlzEncoderFacade.InputFilePath = One3X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            
            _encoder.Encode();

            Assert.IsNull(_encoder.MatchLocation[6, 5]);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchForMinMatchSize17()
        {
            _gz2DlzEncoderFacade.InputFilePath = One6X3MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            Assert.IsNotNull(_encoder.MatchLocation[6, 5]);
            Assert.AreEqual(1, _encoder.MatchLocation[6, 5].X);
            Assert.AreEqual(2, _encoder.MatchLocation[6, 5].Y);
        }
        
        [TestMethod]
        public void EncodeDoesntFindsTheExpectedBlockMatchIfThereAre2BlockAtTheSameOriginForMinMatchSize17()
        {
            _gz2DlzEncoderFacade.InputFilePath = TwoPossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            Assert.IsNull(_encoder.MatchLocation[1, 0]);
        }



        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchForIdentityTransform()
        {
            _gz2DlzEncoderFacade.InputFilePath = VerticalMirrorBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation = (int) G2_2D_LZ.Helpers.Constants.GeometricTransformation.Identity;

            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;

            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            _encoder.WorkImage = new byte[10,12]
            {  //0      1      2      3      4      5      6      7      8      9      10    11
                { 50,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0}, //0
                {  0,   200,     0,     0,     0,    50,   100,   100,     0,     0,    50,    50}, //1
                {  0,   250,   250,   250,     0,     0,     0,     0,     0,     0,     0,     0}, //2
                {  0,   250,   250,   250,     0,     0,     0,     0,     0,     0,     0,     0}, //3
                {  0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0}, //4
                {  0,     0,     0,     0,     0,     0,     0,     0,   150,     0,     0,     0}, //5
                {  0,     0,     0,     0,     0,   250,   250,   250,   150,     0,     0,     0}, //6
                {  0,     0,     0,     0,     0,   250,   250,   250,     0,     0,     0,     0}, //7
                {  0,     0,     0,     0,     0,   150,   150,   150,     0,     0,     0,     0}, //8
                {  0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0}  //9
            };
            _encoder.Encode();

            Assert.IsNotNull(_encoder.MatchLocation[6, 5]);
            Assert.AreEqual(1, _encoder.MatchLocation[6, 5].X);
            Assert.AreEqual(2, _encoder.MatchLocation[6, 5].Y);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchForVerticalMirrorTransform()
        {
            _gz2DlzEncoderFacade.InputFilePath = VerticalMirrorBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;

            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;

            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            /*_encoder.WorkImage = new byte[10, 12]
            {  //0      1      2      3      4      5      6      7      8      9      10    11
                { 50,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0}, //0
                {  0,   200,     0,     0,     0,    50,   100,   100,     0,     0,    50,    50}, //1
                {  0,     0,     0,   250,   250,   250,     0,     0,     0,     0,     0,     0}, //2
                {  0,     0,     0,   250,   250,   250,     0,     0,     0,     0,     0,     0}, //3
                {  0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0}, //4
                {  0,     0,     0,     0,     0,     0,     0,     0,   150,     0,     0,     0}, //5
                {100,     0,    100,     0,     0,   250,   250,   250,   150,     0,     0,     0}, //6
                {  0,   100,     0,     0,     0,   250,   250,   250,     0,     0,     0,     0}, //7
                {  0,     0,     0,     0,     0,   150,   150,   150,     0,     0,     0,     0}, //8
                {  0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0}  //9
            };*/
            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            Assert.IsNotNull(_encoder.MatchLocation[6, 5]);
            Assert.AreEqual(5, _encoder.MatchLocation[6, 5].X);
            Assert.AreEqual(2, _encoder.MatchLocation[6, 5].Y);
        }

        [TestMethod]
        public void EncodeForVerticalMirror()
        {
            _gz2DlzEncoderFacade.InputFilePath = One3X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();

            Assert.IsNull(_encoder.MatchLocation[6, 5]);
        }

        [TestMethod]
        public void EncodeForVerticalMirror2()
        {
            _gz2DlzEncoderFacade.InputFilePath = One6X3MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            Assert.IsNull(_encoder.MatchLocation[6, 5]);
        }

        [TestMethod]
        public void EncodeForVerticalMirror3()
        {
            _gz2DlzEncoderFacade.InputFilePath = TwoPossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            _encoder.Encode();

            Assert.IsNull(_encoder.MatchLocation[6, 5]);
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
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation =
                (int) G2_2D_LZ.Helpers.Constants.GeometricTransformation.Identity;
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
    }
}
