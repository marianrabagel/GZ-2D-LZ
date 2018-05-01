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
        private string _one4X4MatchBlockBmpPath = "4x4Block.bmp";
        private string _2PossibleMatchBlocksBmpPath = "2PossibleMatchBlocks.bmp";
        private string _lenna256anBmpPath = "Lenna256an.bmp";

        private IGz2DlzEncoderFacade _gz2DlzEncoderFacade;
        private IGz2DlzDecoderFacade gz2DlzDecoderFacade;

        [TestInitialize]
        public void Setup()
        {
            _bmpImageReader = new BmpImageReader();

            _2PosibleMatchBlocksTxtFileName = _basePath + "TxtMatrices\\"+ _2PosibleMatchBlocksTxtFileName;
            _lenna256anTxtFileName = _basePath + "TxtMatrices\\" + _lenna256anTxtFileName;

            _testBmpPath = _basePath + _testBmpPath;
            _one4X4MatchBlockBmpPath = _basePath + _one4X4MatchBlockBmpPath;
            _2PossibleMatchBlocksBmpPath = _basePath + _2PossibleMatchBlocksBmpPath;
            _lenna256anBmpPath = _basePath + _lenna256anBmpPath;

            _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
        }
        
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
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = aBasedPredictor;
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
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
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
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new CalicPredictor();
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
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new CalicPredictor();
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

            var inputFilePath = $"{_testBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor4X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _one4X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_one4X4MatchBlockBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one4X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictor2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _2PossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_2PossibleMatchBlocksBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
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

            var inputFilePath = $"{_lenna256anBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
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

            var inputFilePath = $"{_testBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new CalicPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic4X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _one4X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_one4X4MatchBlockBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new CalicPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one4X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithCalic2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _2PossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new CalicPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_2PossibleMatchBlocksBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new CalicPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
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

            var inputFilePath = $"{_lenna256anBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new CalicPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            //_encoder.WriteMatrixToFileAsText();
            //_decoder.LoadMatrixFromTxtFile();
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

            var inputFilePath = $"{_testBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
        }
        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperations4X4BlockBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _one4X4MatchBlockBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_one4X4MatchBlockBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_one4X4MatchBlockBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperations2PossibleMatchBlockskBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _2PossibleMatchBlocksBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_2PossibleMatchBlocksBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_2PossibleMatchBlocksBmpPath, workImage);
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsLenaBmpResultsTheSamePixels()
        {
            _gz2DlzEncoderFacade.InputFilePath = _lenna256anBmpPath;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = _bmpImageReader;
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);

            var inputFilePath = $"{_lenna256anBmpPath}.mat";
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade._inputFilePath = inputFilePath;
            gz2DlzDecoderFacade._abstractPredictor = new ABasedPredictor();
            _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);

            _encoder.Encode();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256anBmpPath, workImage);
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
