using System.IO;
using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Constants = GZ_2D_LZ.UnitTests.Common.Constants;

namespace GZ_2D_LZ.UnitTests
{
    [TestClass]
    public class Gz2DlzCoderWithAPredictorTests
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
        public void WriteMatrixesToFileAsTextTestImageNoMatchOnlyPrediction()
        {
            _encoder = new Gz2DlzEncoder(Constants.TestBmpPath, _predictor, _bmpReader);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextOne4X4MatchBlock()
        {
            _encoder = new Gz2DlzEncoder(Constants.One4X4MatchBlockBmpPath, _predictor, _bmpReader);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextBestMatchOutOfTwo()
        {
            _encoder = new Gz2DlzEncoder(Constants.TwoPossibleMatchBlocksBmpPath, _predictor, _bmpReader);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void EncodeAndWriteMatrixesToFileAsTextLena()
        {
            _encoder = new Gz2DlzEncoder(Constants.LenaImagePath, _predictor, _bmpReader);
            WritToFile(_encoder.WorkImage);
            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
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

        private void WritToFile(byte[,] bytes)
        {
            using (StreamWriter writer = new StreamWriter("test.txt"))
            {
                int height1 = bytes.GetLength(0);
                int width1 = bytes.GetLength(1);
                writer.WriteLine(height1 + " " + width1);

                for (int y = 0; y < height1; y++)
                {
                    for (int x = 0; x < width1; x++)
                    {
                        writer.Write(bytes[y, x] + " ");
                    }

                }
            }
        }
    }
}
