using System;
using System.CodeDom;
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
        private string _lenna256AnTxtFileName = "Lenna256an.txt";
        private string _testBmpPath = "test.bmp";
        private string _one3X4MatchBlockBmpPath = "3x4Block.bmp";
        private string _verticalMirrorBmp = "VerticalMirror.bmp";
        private string _horizontalMirrorBmp = "HorizontalMirror.bmp";
        private string _firstDiagonalMirrorBmp = "FirstDiagonalMirror.bmp";
        private string _greyBmp = "Grey.bmp";
        private string _grey256Bmp = "Grey_256.bmp";
        private string _twoColorsBmp = "TwoColors.bmp";

        private string _2PossibleMatchBlocksBmpPath = "2PossibleMatchBlocks.bmp";
        private string _lenna256AnBmpPath = "Lenna256an.bmp";
        private string _barbBmpPath = "barb.bmp";
        private string _franceBmpPath = "france.bmp";
        private string _peppers512 = "Peppers512an.BMP ";

        readonly int? specificGeometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.Identity;

        private IGz2DlzEncoderFacade _gz2DlzEncoderFacade;
        private IGz2DlzDecoderFacade _gz2DlzDecoderFacade;

        [TestInitialize]
        public void Setup()
        {
            _bmpImageReader = new BmpImageReader();

            _2PosibleMatchBlocksTxtFileName = _basePath + "TxtMatrices\\"+ _2PosibleMatchBlocksTxtFileName;
            _lenna256AnTxtFileName = _basePath + "TxtMatrices\\" + _lenna256AnTxtFileName;

            _testBmpPath = _basePath + _testBmpPath;
            _one3X4MatchBlockBmpPath = _basePath + _one3X4MatchBlockBmpPath;
            _verticalMirrorBmp = _basePath + _verticalMirrorBmp;
            _horizontalMirrorBmp = _basePath + _horizontalMirrorBmp;
            _firstDiagonalMirrorBmp = _basePath + _firstDiagonalMirrorBmp;
            _greyBmp = _basePath + _greyBmp;
            _grey256Bmp = _basePath + _grey256Bmp;
            _twoColorsBmp = _basePath + _twoColorsBmp;
            _2PossibleMatchBlocksBmpPath = _basePath + _2PossibleMatchBlocksBmpPath;
            _lenna256AnBmpPath = _basePath + _lenna256AnBmpPath;
            _barbBmpPath = _basePath + _barbBmpPath;
            _franceBmpPath = _basePath + _franceBmpPath;
            _peppers512 = _basePath + _peppers512;
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
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = aBasedPredictor,
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode(specificGeometricTransform);
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

            _gz2DlzEncoderFacade.InputFilePath = _lenna256AnTxtFileName;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = txtImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_lenna256AnTxtFileName}.mat";
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode(specificGeometricTransform);
            _decoder.SaveOriginalImageAsTxtFile();

            var filename = $"{_lenna256AnTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_lenna256AnTxtFileName).Replace("256 256 ", "");
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
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode(specificGeometricTransform);
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
            _gz2DlzEncoderFacade.InputFilePath = _lenna256AnTxtFileName;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = txtImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_lenna256AnTxtFileName}.mat";
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode(specificGeometricTransform);
            _decoder.SaveOriginalImageAsTxtFile();

            var filename = $"{_lenna256AnTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_lenna256AnTxtFileName).Replace("256 256 ", "");
            var decodedFile = GetContentWithoutNewLines(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorTestBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _testBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor3X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _one3X4MatchBlockBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

             _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor3X4BlockBmpResultsTheSamePixelsNoGeometricTransformation()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _one3X4MatchBlockBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var noGeometricTransformation = (int) G2_2D_LZ.Helpers.Constants.GeometricTransformation.NoGeometricTransformation;

            _encoder.Encode(noGeometricTransformation);
            _decoder.Decode(noGeometricTransformation);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorVerticalMirrorBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _verticalMirrorBmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _verticalMirrorBmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver(),
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var minMatchSize = G2_2D_LZ.Helpers.Constants.MinMatchSize;
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;
            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            G2_2D_LZ.Helpers.Constants.MinMatchSize = minMatchSize;
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_verticalMirrorBmp, workImage);
        }
        
        [TestMethod]
        public void EncodeAndDecodeWithAPredictorGreyBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _greyBmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _greyBmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var minMatchSize = G2_2D_LZ.Helpers.Constants.MinMatchSize;
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.HorizontalMirror;
            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            G2_2D_LZ.Helpers.Constants.MinMatchSize = minMatchSize;
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_greyBmp, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorGrey256BmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _grey256Bmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _grey256Bmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var minMatchSize = G2_2D_LZ.Helpers.Constants.MinMatchSize;
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.HorizontalMirror;
            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            G2_2D_LZ.Helpers.Constants.MinMatchSize = minMatchSize;
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_grey256Bmp, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorTwoColorsBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _twoColorsBmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _twoColorsBmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var minMatchSize = G2_2D_LZ.Helpers.Constants.MinMatchSize;
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.Identity;
            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            G2_2D_LZ.Helpers.Constants.MinMatchSize = minMatchSize;
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_twoColorsBmp, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorFirstDiagonalMirrorBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _firstDiagonalMirrorBmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _firstDiagonalMirrorBmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var minMatchSize = G2_2D_LZ.Helpers.Constants.MinMatchSize;
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.FirstDiagonalMirror;
            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            G2_2D_LZ.Helpers.Constants.MinMatchSize = minMatchSize;
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_firstDiagonalMirrorBmp, workImage);
        }


        [TestMethod]
        public void EncodeAndDecodeWithAPredictorVerticalMirrorBmpResultsTheSamePixelsAllGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _verticalMirrorBmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _verticalMirrorBmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_verticalMirrorBmp, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor2PossibleMatchBlockskBmpResultsTheSamePixelsForVerticalGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;

            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor2PossibleMatchBlockskBmpResultsTheSamePixelsForHorizontalGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.HorizontalMirror;

            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);
            
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor2PossibleMatchBlockskBmpResultsTheSamePixelsAllGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorLenaBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _lenna256AnBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256AnBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256AnBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorLenaBmpResultsTheSamePixelsVerticalGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _lenna256AnBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256AnBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;

            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256AnBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorLenaBmpResultsTheSamePixelsHorizontalGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _lenna256AnBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256AnBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.HorizontalMirror;

            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256AnBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorHorizontalMirrorBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _horizontalMirrorBmp,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _horizontalMirrorBmp + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var minMatchSize = G2_2D_LZ.Helpers.Constants.MinMatchSize;
            G2_2D_LZ.Helpers.Constants.MinMatchSize = 5;
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.HorizontalMirror;
            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            G2_2D_LZ.Helpers.Constants.MinMatchSize = minMatchSize;
            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_horizontalMirrorBmp, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorLenaBmpResultsTheSamePixelsAllGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _lenna256AnBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256AnBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256AnBmpPath, workImage);
        }
        
        [TestMethod]
        public void EncodeAndDecodeWithAPredictorBarbBmpResultsTheSamePixelsAllGeometricTransformations()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _barbBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _barbBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_barbBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorTestBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
            _gz2DlzEncoderFacade.InputFilePath = _testBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic3X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _one3X4MatchBlockBmpPath,
                AbstractPredictor = new CalicPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic2PossibleMatchBlockskBmpResultsTheSamePixelsIdentity()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new CalicPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }
        
        [TestMethod]
        public void EncodeAndDecodeWithCalic2PossibleMatchBlockskBmpResultsTheSamePixelsVertical()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new CalicPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.VerticalMirror;

            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic2PossibleMatchBlockskBmpResultsTheSamePixelsHorizontal()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new CalicPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder +
                                Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);
            var geometricTransform = (int)G2_2D_LZ.Helpers.Constants.GeometricTransformation.HorizontalMirror;

            _encoder.Encode(geometricTransform);
            _decoder.Decode(geometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }


        [TestMethod]
        public void EncodeAndDecodeWithCalic2PossibleMatchBlockskBmpResultsTheSamePixelsHorizontalAll()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new CalicPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorLenaBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _lenna256AnBmpPath,
                AbstractPredictor = new CalicPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256AnBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256AnBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalicPredictorBarbBmpResultsTheSamePixelsAll()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
            _gz2DlzEncoderFacade.InputFilePath = _barbBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _barbBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_barbBmpPath, workImage);
        }

        [TestMethod]
        [Ignore]
        public void EncodeAndDecodeWithCalicPredictorFranceBmpResultsTheSamePixelsAll()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
            _gz2DlzEncoderFacade.InputFilePath = _franceBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _franceBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new CalicPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(null);
            _decoder.Decode(null);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_franceBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsTestBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _testBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperations3X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _one3X4MatchBlockBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _one3X4MatchBlockBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one3X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperations2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _2PossibleMatchBlocksBmpPath,
                AbstractPredictor = new ABasedPredictor(),
                ImageReader = _bmpImageReader,
                Archiver = new Paq6V2Archiver()
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            var inputFilePath = _2PossibleMatchBlocksBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = new ABasedPredictor(),
                Archiver = new Paq6V2Archiver()
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsLenaBmpResultsTheSamePixels()
        {
            var aBasedPredictor = new ABasedPredictor();
            var paq6V2Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _lenna256AnBmpPath,
                AbstractPredictor = aBasedPredictor,
                ImageReader = _bmpImageReader,
                Archiver = paq6V2Archiver
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _lenna256AnBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = aBasedPredictor,
                Archiver = paq6V2Archiver
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256AnBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithPaqAndWithAPredictorAndBitOperationsTestBmpResultsTheSamePixels()
        {
            var aBasedPredictor = new ABasedPredictor();
            var paq6V2Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
            _gz2DlzEncoderFacade.InputFilePath = _testBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = aBasedPredictor;
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = paq6V2Archiver;
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _testBmpPath + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = aBasedPredictor,
                Archiver = paq6V2Archiver
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsPeppersBmpResultsTheSamePixels()
        {
            var aBasedPredictor = new ABasedPredictor();
            var paq6V2Archiver = new Paq6V2Archiver();
            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade
            {
                InputFilePath = _peppers512,
                AbstractPredictor = aBasedPredictor,
                ImageReader = _bmpImageReader,
                Archiver = paq6V2Archiver
            };
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = _peppers512 + G2_2D_LZ.Helpers.Constants.Folder + Constants.Paq6Extension;
            _gz2DlzDecoderFacade = new Gz2DlzDecoderFacade
            {
                InputFilePath = inputFilePath,
                AbstractPredictor = aBasedPredictor,
                Archiver = paq6V2Archiver
            };
            _decoder = new Gz2DlzDecoder(_gz2DlzDecoderFacade);

            _encoder.Encode(specificGeometricTransform);
            _decoder.Decode(specificGeometricTransform);

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
