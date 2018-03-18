using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZUnitTests
{
    [TestClass]
    public class Gz2DlzCoderWithCalicPredictorTests
    {
        private Gz2DlzEncoder _encoder;
        AbstractPredictor _predictor;
        IReader _bmpReader;

        [TestInitialize]
        public void Setup()
        {
            _predictor = new CalicPredictor();
            _bmpReader = new BmpReader();
        }

        [TestMethod]
        public void EncodesTheFirstRowWithTheExpectedValues()
        {
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);
            _encoder.Encode();

            var predictionError = _encoder.PredictionError;
            int y = 0;
            double[] firstRow = {0, 255, 0, 255, 0, 255};

            for (int x = 0; x < predictionError.GetLength(1); x++)
            {
                Assert.AreEqual(firstRow[x], predictionError[y, x]);
            }
        }

        //todo pair encode-decode tests
        [TestMethod]
        public void WriteMatrixesToFileAsTextTestImageNoMatchOnlyPrediction()
        {
            _encoder = new Gz2DlzEncoder(Constants.InputTestImagePath, _predictor, _bmpReader);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
        }


        [TestMethod]
        public void EncodeAndWriteMatrixesToLenaFileAsTextFromLenaTxtFile()
        {
            IReader txtReader = new TxtReader();
            _encoder = new Gz2DlzEncoder(Constants.LenaTxtImagePath, _predictor, txtReader);
            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
        }
    }
}
