using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
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
        }
        
        [TestMethod]
        public void EncodeAndDecodeWithAPredictorCustomTxtMatrixFileCreatesAFileIndenticalWithTheOriginal()
        {
            IImageReader txtImageReader = new TxtAsImageReader();
            _encoder = new Gz2DlzEncoder(_2PosibleMatchBlocksTxtFileName, new ABasedPredictor(), txtImageReader);
            _decoder = new Gz2DlzDecoder($"{_2PosibleMatchBlocksTxtFileName}.mat", new ABasedPredictor());

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

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
            _encoder = new Gz2DlzEncoder(_lenna256anTxtFileName, new ABasedPredictor(), txtImageReader);
            _decoder = new Gz2DlzDecoder($"{_lenna256anTxtFileName}.mat", new ABasedPredictor());

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

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
            _encoder = new Gz2DlzEncoder(_2PosibleMatchBlocksTxtFileName, new CalicPredictor(), txtImageReader);
            _decoder = new Gz2DlzDecoder($"{_2PosibleMatchBlocksTxtFileName}.mat", new CalicPredictor());

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

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
            _encoder = new Gz2DlzEncoder(_lenna256anTxtFileName, new CalicPredictor(), txtImageReader);
            _decoder = new Gz2DlzDecoder($"{_lenna256anTxtFileName}.mat", new CalicPredictor());

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

            var filename = $"{_lenna256anTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLines(_lenna256anTxtFileName).Replace("256 256 ", "");
            var decodedFile = GetContentWithoutNewLines(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [TestMethod]
        public void EncodeAndDecodeWithAPredictorTestBmpResultsTheSamePixels()
        {
            _encoder = new Gz2DlzEncoder(_testBmpPath, new ABasedPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_testBmpPath}.mat", new ABasedPredictor());

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
            _encoder = new Gz2DlzEncoder(_one4X4MatchBlockBmpPath, new ABasedPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_one4X4MatchBlockBmpPath}.mat", new ABasedPredictor());

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
            _encoder = new Gz2DlzEncoder(_2PossibleMatchBlocksBmpPath, new ABasedPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_2PossibleMatchBlocksBmpPath}.mat", new ABasedPredictor());

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
            _encoder = new Gz2DlzEncoder(_lenna256anBmpPath, new ABasedPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_lenna256anBmpPath}.mat", new ABasedPredictor());

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
            _encoder = new Gz2DlzEncoder(_testBmpPath, new CalicPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_testBmpPath}.mat", new CalicPredictor());

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
            _encoder = new Gz2DlzEncoder(_one4X4MatchBlockBmpPath, new CalicPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_one4X4MatchBlockBmpPath}.mat", new CalicPredictor());

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
            _encoder = new Gz2DlzEncoder(_2PossibleMatchBlocksBmpPath, new CalicPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_2PossibleMatchBlocksBmpPath}.mat", new CalicPredictor());

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
            _encoder = new Gz2DlzEncoder(_lenna256anBmpPath, new CalicPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_lenna256anBmpPath}.mat", new CalicPredictor());

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_lenna256anBmpPath, workImage);
        }

        [TestMethod]
        [Ignore]
        public void EncodeAndDecodeWithAPredictorAndBitOperationsTestBmpResultsTheSamePixels()
        {
            _encoder = new Gz2DlzEncoder(_testBmpPath, new ABasedPredictor(), _bmpImageReader);
            _decoder = new Gz2DlzDecoder($"{_testBmpPath}.mat", new ABasedPredictor());

            _encoder.Encode();
            _encoder.WriteResultingMatricesToIndividualFiles();
            //_decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();

            var workImage = _decoder.WorkImage;
            CompareValueWithPixelFromBmp(_testBmpPath, workImage);
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
