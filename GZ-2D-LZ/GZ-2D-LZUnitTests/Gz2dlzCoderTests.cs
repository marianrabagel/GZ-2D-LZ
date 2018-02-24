using System;
using System.IO;
using G2_2D_LZ;
using G2_2D_LZ.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2dlzCoderTests
    {
        private Gz2dlzEncoder encoder;

        [TestMethod]
        public void ImageIsLoadedIntoMemory()
        {
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);
            
            byte[,] originalImage = encoder.GetOriginalImage();

            Assert.AreEqual(5, originalImage.GetLength(0));
            Assert.AreEqual(6, originalImage.GetLength(1));

            AssertEachValue(Constants.TestImage, originalImage);
        }

        [TestMethod]
        [Ignore]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            //encoder.PredictFirstRow();

            var predictionError = encoder.GetPredictionError();
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
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var isEncodedPixel = encoder.GetEncodedPixels();
            int y = 0;

            for (int x = 0; x < isEncodedPixel.GetLength(1); x++)
            {
                Assert.IsTrue(isEncodedPixel[y, x]);
            }
        }
        
        [TestMethod]
        public void EncodeEncodesAllPixelsForA5x6Image()
        {
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var isEncodedPixel = encoder.GetEncodedPixels();
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
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var isEncodedPixel = encoder.GetEncodedPixels();
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
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var matchFlag = encoder.GetMatchFlag();
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
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var residual = encoder.GetResidual();
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
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var matchLocation = encoder.GetMatchLocation();
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
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();

            var matchDimensions = encoder.GetMatchDimensions();
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
            encoder = new Gz2dlzEncoder(Constants.Input4x4MatchBlock);
            var originalImage = encoder.GetOriginalImage();
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
            encoder = new Gz2dlzEncoder(Constants.Input4x4MatchBlock);

            encoder.Encode();
            
            Assert.IsNotNull(encoder.GetMatchLocation()[6,5]);
            Assert.AreEqual(1, encoder.GetMatchLocation()[6,5].X);
            Assert.AreEqual(2, encoder.GetMatchLocation()[6,5].Y);
        }

        [TestMethod]
        public void EncodeFindsTheExpectedBlockMatchIfThereAre2BlockAtTheSameOrigin()
        {
            encoder = new Gz2dlzEncoder(Constants.Input2PosibleMatchBlocks);
            //DisplayImageToOutputWindow(encoder.GetOriginalImage());
            encoder.Encode();

            Assert.IsNotNull(encoder.GetMatchLocation()[1, 0]);
            Assert.AreEqual(0, encoder.GetMatchLocation()[1, 0].X);
            Assert.AreEqual(0, encoder.GetMatchLocation()[1, 0].Y);
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextTestImageNoMatchOnlyPrediction()
        {
            encoder = new Gz2dlzEncoder(Constants.InputTestImage);

            encoder.Encode();
            encoder.WriteMatrixToFileAsText();
        }

        [TestMethod]
        public void WriteMatrixesToFileAsTextOne4x4MatchBlock()
        {
            encoder = new Gz2dlzEncoder(Constants.Input4x4MatchBlock);

            encoder.Encode();
            encoder.WriteMatrixToFileAsText();
        }


        [TestMethod]
        public void WriteMatrixesToFileAsTextBestMatchOutOfTwo()
        {
            encoder = new Gz2dlzEncoder(Constants.Input2PosibleMatchBlocks);

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
