using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;

namespace G2_2D_LZ.Facades
{
    public class Gz2DlzEncoderFacade : IGz2DlzEncoderFacade
    {
        public string InputFilePath { get; set; }
        public AbstractPredictor AbstractPredictor { get; set; }
        public IImageReader ImageReader { get; set; }
    }
}
