using System;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Predictors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests.Predictors
{
    [TestClass]
    public class CalicPredictorTests
    {
        private AbstractPredictor _predictor;

        [TestInitialize]
        public void Setup()
        {
            _predictor = new CalicPredictor();
            byte[,] matrix =
            {
                {100, 200, 255},
                {150, 150, 150},
                {100, 100, 255}
            };

            _predictor.SetOriginalMatrix(matrix);
        }

        [TestMethod]
        public void GetPredictionValue1()
        {
            var predictionValue = _predictor.GetPredictionValue(0, 0);

            Assert.AreEqual(100, predictionValue);
        }

        [TestMethod]
        public void GetPredictionValue2()
        {
            var predictionValue = _predictor.GetPredictionValue(1, 0);

            Assert.AreEqual(200, predictionValue);
        }

        [TestMethod]
        public void GetPredictionValue3()
        {
            var predictionValue = _predictor.GetPredictionValue(1, 1);

            Assert.AreEqual(200, predictionValue);
        }

        [TestMethod]
        public void GetPredictionValue4()
        {
            var predictionValue = _predictor.GetPredictionValue(2, 2);

            Assert.AreEqual(100, predictionValue);
        }

        [TestMethod]
        public void GetPredictionValue5()
        {
            var predictionValue = _predictor.GetPredictionValue(1, 2);

            Assert.AreEqual(100, predictionValue);
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
