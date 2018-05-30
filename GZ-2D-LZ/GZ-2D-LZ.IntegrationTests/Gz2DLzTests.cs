using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Facades;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using GZ_2D_LZ.Archiver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZ.IntegrationTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class Gz2DLzTests
    {
        private Gz2DlzEncoder _encoder;
        IImageReader _bmpImageReader;

        private Gz2DlzDecoder _decoder;

        private readonly string _basePath = Environment.CurrentDirectory + "\\TestData\\";
        private string _2PosibleMatchBlocksTxtFileName = "2PossibleMatchBlocks.txt";
        private string _lenna256anTxtFileName = "Lenna256an.txt";
        private string _testBmpPath = "test.bmp";
        private string _one3X4MatchBlockBmpPath = "3x4Block.bmp";
        private string verticalMirrorBmp = "VerticalMirror.bmp";
        private string _2PossibleMatchBlocksBmpPath = "2PossibleMatchBlocks.bmp";
        private string _lenna256anBmpPath = "Lenna256an.bmp";
        private string _peppers512 = "Peppers512an.BMP ";

        private IGz2DlzEncoderFacade _gz2DlzEncoderFacade;
        private IGz2DlzDecoderFacade gz2DlzDecoderFacade;

        [TestInitialize]
        public void Setup()
        {
            _bmpImageReader = new BmpImageReader();

            _2PosibleMatchBlocksTxtFileName = _basePath + "TxtMatrices\\"+ _2PosibleMatchBlocksTxtFileName;
            _lenna256anTxtFileName = _basePath + "TxtMatrices\\" + _lenna256anTxtFileName;

            _testBmpPath = _basePath + _testBmpPath;
            _one3X4MatchBlockBmpPath = _basePath + _one3X4MatchBlockBmpPath;
            verticalMirrorBmp = _basePath + verticalMirrorBmp;
            _2PossibleMatchBlocksBmpPath = _basePath + _2PossibleMatchBlocksBmpPath;
            _lenna256anBmpPath = _basePath + _lenna256anBmpPath;
            _peppers512 = _basePath + _peppers512;

            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
        }
        
        [Ignore]
        [TestMethod]
        public void EncodeAndDecodeWithAPredictorCustomTxtMatrixFileCreatesAFileIndenticalWithTheOriginal()
        {
            IImageReader txtImageReader = new TxtAsImageReader();
            var aBasedPredictor = new ABasedPredictor();

            _gz2DlzEncoderFacade.InputFilePath = _2PosibleMatchBlocksTxtFileName;
            _gz2DlzEncoderFacade.AbstractPredictor = aBasedPredictor;
            _gz2DlzEncoderFacade.ImageReader = txtImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_2PosibleMatchBlocksTxtFileName}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = aBasedPredictor;
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveOriginalImageAsTxtFile();

            var filename = $"{_2PosibleMatchBlocksTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_2PosibleMatchBlocksTxtFileName).Replace("10 10 ", "");
            var decodedFile = GetContentWithoutNewLines(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [Ignore]
        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorCustomTxtLena256AnCreatesAFileIndenticalWithTheOriginal()
        {
            IImageReader txtImageReader = new TxtAsImageReader();

            _gz2DlzEncoderFacade.InputFilePath = _lenna256anTxtFileName;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = txtImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_lenna256anTxtFileName}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveOriginalImageAsTxtFile();

            var filename = $"{_lenna256anTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_lenna256anTxtFileName).Replace("256 256 ", "");
            var decodedFile = GetContentWithoutNewLines(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [Ignore]
        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorCustomTxtMatrixFileCreatesAFileIndenticalWithTheOriginal()
        {
            IImageReader txtImageReader = new TxtAsImageReader();
            _gz2DlzEncoderFacade.InputFilePath = _2PosibleMatchBlocksTxtFileName;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = txtImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_2PosibleMatchBlocksTxtFileName}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new CalicPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveOriginalImageAsTxtFile();

            var filename = $"{_2PosibleMatchBlocksTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_2PosibleMatchBlocksTxtFileName).Replace("10 10 ", "");
            var decodedFile = GetContentWithoutNewLines(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [Ignore]
        [TestMethod]
        public void EncodeAndDecodeWithCalicCustomTxtLena256AnCreatesAFileIndenticalWithTheOriginal()
        {
            IImageReader txtImageReader = new TxtAsImageReader();
            _gz2DlzEncoderFacade.InputFilePath = _lenna256anTxtFileName;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = txtImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_lenna256anTxtFileName}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new CalicPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveOriginalImageAsTxtFile();

            var filename = $"{_lenna256anTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_lenna256anTxtFileName).Replace("256 256 ", "");
            var decodedFile = GetContentWithoutNewLines(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorTestBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _testBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor3X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _one3X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorVerticalMirrorBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = verticalMirrorBmp;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade.GeometricTransformation = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(verticalMirrorBmp, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _2PossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorLenaBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _lenna256anBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256anBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256anBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorTestBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _testBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new CalicPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic3X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _one3X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new CalicPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _2PossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new CalicPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorLenaBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _lenna256anBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256anBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new CalicPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256anBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsTestBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _testBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperations3X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _one3X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperations2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _2PossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsLenaBmpResultsTheSamePixels()
        {
            var aBasedPredictor = new ABasedPredictor();
            var paq6V2Archiver = new Paq6V2Archiver();

            _gz2DlzEncoderFacade.InputFilePath = _lenna256anBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = aBasedPredictor;
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = paq6V2Archiver;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256anBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = aBasedPredictor;
            gz2DlzDecoderFacade.Archiver = paq6V2Archiver;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256anBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithPaqAndWithAPredictorAndBitOperationsTestBmpResultsTheSamePixels()
        {
            var aBasedPredictor = new ABasedPredictor();
            var paq6V2Archiver = new Paq6V2Archiver();

            _gz2DlzEncoderFacade.InputFilePath = _testBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = aBasedPredictor;
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = paq6V2Archiver;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = aBasedPredictor;
            gz2DlzDecoderFacade.Archiver = paq6V2Archiver;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsPeppersBmpResultsTheSamePixels()
        {
            var aBasedPredictor = new ABasedPredictor();
            var paq6V2Archiver = new Paq6V2Archiver();

            _gz2DlzEncoderFacade.InputFilePath = _peppers512;
            _gz2DlzEncoderFacade.AbstractPredictor = aBasedPredictor;
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = paq6V2Archiver;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _peppers512 + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = inputFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = aBasedPredictor;
            gz2DlzDecoderFacade.Archiver = paq6V2Archiver;
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_peppers512, workImage);
        }

        private static string GetContentWithoutNewLines(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                var fileContent = reader.ReadToEnd();
                var contentWithoutNewLine = fileContent.Replace("\r", "").Replace("\n", "");

                return contentWithoutNewLine.Trim();
            }
        }

        private void CompareValueWithPixelFromBmp(string originalImageFilePath, byte[,] workImage)
        {
            using (Bitmap bitmap = new Bitmap(originalImageFilePath))
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Assert.AreEqual(bitmap.GetPixel(x, y).R, workImage[y, x]);
                    }
                }
            }
        }

        private void DisplayImageToOutputWindow(byte[,] bytes)
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
