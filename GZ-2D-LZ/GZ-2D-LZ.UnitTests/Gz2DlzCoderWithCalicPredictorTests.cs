using G2_2D_LZ;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Constants = GZ_2D_LZ.UnitTests.Common.Constants;

namespace GZ_2D_LZ.UnitTests
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

        //todo pair encode-decode tests
        [TestMethod]
        public void WriteMatrixesToFileAsTextTestImageNoMatchOnlyPrediction()
        {
            _encoder = new Gz2DlzEncoder(Constants.TestBmpPath, _predictor, _bmpReader);

            _encoder.Encode();
            _encoder.WriteMatrixToFileAsText();
        }
    }
}
