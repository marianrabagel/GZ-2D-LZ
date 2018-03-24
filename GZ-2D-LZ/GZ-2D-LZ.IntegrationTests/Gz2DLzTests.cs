using System;
using System.IO;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZ.IntegrationTests
{
    [TestClass]
    public class Gz2DLzTests
    {
        private Gz2DlzEncoder _encoder;
        private AbstractPredictor _calicPredictor;
        private AbstractPredictor _aPredictor;
        IReader _bmpReader;

        private Gz2DlzDecoder _decoder;

        public string TestBmpPath = "";
        private readonly string _basePath = Environment.CurrentDirectory + "\\TestData\\TxtMatrices\\";
        private string _2PosibleMatchBlocksTxtFileName = "2PossibleMatchBlocks.txt";
        private string _lenna256anTxtFileName = "Lenna256an.txt";
        private string _lenna256anBmpFileName = "Lenna256an.bmp";

        [TestInitialize]
        public void Setup()
        {
            _calicPredictor = new CalicPredictor();
            _aPredictor = new ABasedPredictor();
            _bmpReader = new BmpReader();

            _2PosibleMatchBlocksTxtFileName = _basePath + _2PosibleMatchBlocksTxtFileName;
            _lenna256anTxtFileName = _basePath + _lenna256anTxtFileName;
        }

        [TestMethod]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            _encoder = new Gz2DlzEncoder(TestBmpPath, _calicPredictor, _bmpReader);
            _encoder.Encode();

            var predictionError = _encoder.PredictionError;
            int y = 0;
            double[] firstRow = {0, 255, 0, 255, 0, 255};

            for (int x = 0; x < predictionError.GetLength(1); x++)
            {
                Assert.AreEqual(firstRow[x], predictionError[y, x]);
            }
        }

        [TestMethod]
        public void EncodeAndDecodeACustomTxtMatrixFileCreatesAFileIndenticalWithTheOriginal()
        {
            IReader txtReader = new TxtReader();
            _encoder = new Gz2DlzEncoder(_2PosibleMatchBlocksTxtFileName, _aPredictor, txtReader);
            _decoder = new Gz2DlzDecoder($"{_2PosibleMatchBlocksTxtFileName}.mat", _aPredictor);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

            var filename = $"{_2PosibleMatchBlocksTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLine(_2PosibleMatchBlocksTxtFileName).Replace("10 10 ", "");
            var decodedFile = GetContentWithoutNewLine(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }

        [TestMethod]
        public void EncodeAndDecodeACustomeTxtLena256AnCreatesAFileIndenticalWithTheOriginal()
        {
            IReader txtReader = new TxtReader();
            _encoder = new Gz2DlzEncoder(_lenna256anTxtFileName, _aPredictor, txtReader);
            _decoder = new Gz2DlzDecoder($"{_lenna256anTxtFileName}.mat", _aPredictor);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

            var filename = $"{_lenna256anTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLine(_lenna256anTxtFileName).Replace("256 256 ", "");
            var decodedFile = GetContentWithoutNewLine(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }


        [TestMethod]
        public void EncodeAndWriteMatrixesToLenaFileAsTextFromLenaTxtFile()
        {
            IReader txtReader = new TxtReader();
            _encoder = new Gz2DlzEncoder(_lenna256anTxtFileName, _calicPredictor, txtReader);
            _decoder = new Gz2DlzDecoder($"{_lenna256anTxtFileName}.mat", _calicPredictor);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
            _decoder.LoadMatrixFromTxtFile();
            _decoder.Decode();
            _decoder.SaveAsTxtFile();

            var filename = $"{_lenna256anTxtFileName}.mat.decoded.txt";
            var originalFile = GetContentWithoutNewLine(_lenna256anTxtFileName).Replace("256 256 ", "");
            var decodedFile = GetContentWithoutNewLine(filename);

            Assert.AreEqual(originalFile.Length, decodedFile.Length);
            Assert.IsTrue(string.Equals(originalFile, decodedFile));
        }


        [TestMethod]
        [Ignore]
        public void DecodeForLenaAPredictor()
        {
            _decoder = new Gz2DlzDecoder(_lenna256anBmpFileName, _aPredictor);
            _decoder.LoadMatrixFromTxtFile();

            _decoder.Decode();
        }

        private static string GetContentWithoutNewLine(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                var fileContent = reader.ReadToEnd();
                var contentWithoutNewLine = fileContent.Replace("\r", "").Replace("\n", "");

                return contentWithoutNewLine.Trim();
            }
        }
    }
}
