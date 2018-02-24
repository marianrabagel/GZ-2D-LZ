using G2_2D_LZ;
using G2_2D_LZ.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzCoderTests
    {
        private Gz2DlzEncoder encoder;

        [TestMethod]
        public void ImageIsLoadedIntoMemory()
        {
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);
            
            byte[,] originalImage = encoder.WorkImage;

            Assert.AreEqual(5, originalImage.GetLength(0));
            Assert.AreEqual(6, originalImage.GetLength(1));

            AssertEachValue(Constants.TestImage, originalImage);
        }

        [TestMethod]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);
            encoder.Encode();

            var predictionError = encoder.PredictionError;
            int y = 0;
            double[] firstRow = {-128, 255, -255, 255, -255, 255};

            for (int x = 0; x < predictionError.GetLength(1); x++)
            {
                Assert.AreEqual(firstRow[x], predictionError[y, x]);
            }
        }
        
        [TestMethod]
        public void EncodesTheFirstRowIsMarkedAsEncoded()
        {
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var isEncodedPixel = encoder.IsPixelEncoded;
            int y = 0;

            for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
            {
                Assert.IsTrue(isEncodedPixel[y, x]);
            }
        }
        
        [TestMethod]
        public void EncodeEncodesAllPixelsForA5X6Image()
        {
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var isEncodedPixel = encoder.IsPixelEncoded;
            for (int y = 0; y < isEncodedPixel.GetLength(0); y++)
            {
                for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
                {
                    Assert.IsTrue(isEncodedPixel[y,x]);
                }    
            }
        }

        [TestMethod]
        public void EncodeEncodesAllPixelsForAnImageWithAmatchingBlock()
        {
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var isEncodedPixel = encoder.IsPixelEncoded;
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
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var matchFlag = encoder.MatchFlag;
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
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var residual = encoder.Residual;
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
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var matchLocation = encoder.MatchLocation;
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
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();

            var matchDimensions = encoder.MatchDimensions;
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
            encoder = new Gz2DlzEncoder(Constants.Input4x4MatchBlockPath);
            var originalImage = encoder.WorkImage;
            var width = originalImage.GetLength(0);

            for (int i = 0; i < width; i++)
            {
                encoder.IsPixelEncoded[0, i] = true;
                encoder.IsPixelEncoded[1, i] = true;
                encoder.IsPixelEncoded[2, i] = true;
                encoder.IsPixelEncoded[3, i] = true;
                encoder.IsPixelEncoded[4, i] = true;
            }
            for (int i = 0; i < 5; i++)
            {
                encoder.IsPixelEncoded[0, i] = true;
            }

            DisplayImageToOutputWindow(originalImage);

            var encoderPosition = new PixelLocation(5,5);
            var rootPosition = new PixelLocation(1, 1);
            BestMatch bestMatch = encoder.LocateTheBestAproximateMatchForGivenRootPixel(encoderPosition, rootPosition);

            Assert.AreEqual(4, bestMatch.Height);
            Assert.AreEqual(4, bestMatch.Width);
            Assert.AreEqual(16, bestMatch.Size);
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

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatch()
        {
            encoder = new Gz2DlzEncoder(Constants.Input4x4MatchBlockPath);

            encoder.Encode();
            
            Assert.IsNotNull(encoder.MatchLocation[6,5]);
            Assert.AreEqual(1, encoder.MatchLocation[6,5].X);
            Assert.AreEqual(2, encoder.MatchLocation[6,5].Y);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchIfThereAre2BlockAtTheSameOrigin()
        {
            encoder = new Gz2DlzEncoder(Constants.Input2PosibleMatchBlocksPath);
            DisplayImageToOutputWindow(encoder.WorkImage);
            encoder.Encode();

            Assert.IsNotNull(encoder.MatchLocation[1, 0]);
            Assert.AreEqual(0, encoder.MatchLocation[1, 0].X);
            Assert.AreEqual(0, encoder.MatchLocation[1, 0].Y);
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextTestImageNoMatchOnlyPrediction()
        {
            encoder = new Gz2DlzEncoder(Constants.InputTestImagePath);

            encoder.Encode();
            encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextOne4X4MatchBlock()
        {
            encoder = new Gz2DlzEncoder(Constants.Input4x4MatchBlockPath);

            encoder.Encode();
            encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextBestMatchOutOfTwo()
        {
            encoder = new Gz2DlzEncoder(Constants.Input2PosibleMatchBlocksPath);

            encoder.Encode();
            encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextLena()
        {
            encoder = new Gz2DlzEncoder(Constants.LenaImagePath);

            encoder.Encode();
            encoder.WriteMatrixToFileAsText();
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
