using System;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Predictors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests.Predictors
{
    [TestClass]
    public class ABasedPredictorTests
    {
        private AbstractPredictor _predictor;

        [TestInitialize]
        public void Setup()
        {
            _predictor = new ABasedPredictor();
            byte[,] matrix =
            {
                {1, 2, 3},
                {1, 2, 3},
                {1, 2, 3}
            };

            _predictor.SetOriginalMatrix(matrix);
        }

        [TestMethod]
        public void GetPredictionValueForLeftTopPixelIs128()
        {
            var predictionValue = _predictor.GetPredictionValue(0,0);

            Assert.AreEqual(128, predictionValue);
        }

        [TestMethod]
        public void GetPredictionValueForAPixelReturnsThePreviousPixelValue()
        {
            var predictionValue = _predictor.GetPredictionValue(1, 0);

            Assert.AreEqual(1, predictionValue);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetPredictionValueForOutOfBoundsPixelThrowsIndexOutOfRangeException()
        {
            var predictionValue = _predictor.GetPredictionValue(10, 0);

            Assert.AreEqual(1, predictionValue);
        }
    }
}
