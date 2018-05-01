using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;

namespace G2_2D_LZ.Facades
{
    public class Gz2DlzDecoderFacade : IGz2DlzDecoderFacade
    {
        public string _inputFilePath { get; set; }
        public AbstractPredictor _abstractPredictor { get; set; }
    }
}
