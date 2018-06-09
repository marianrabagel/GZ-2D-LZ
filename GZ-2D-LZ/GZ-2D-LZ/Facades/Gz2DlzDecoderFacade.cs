using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;
using GZ_2D_LZ.Archiver.Contracts;

namespace G2_2D_LZ.Facades
{
    public class Gz2DlzDecoderFacade : IGz2DlzDecoderFacade
    {
        public string InputFilePath { get; set; }
        public AbstractPredictor AbstractPredictor { get; set; }
        public IArchiver Archiver { get; set; }
        //public int GeometricTransformation { get; set; }
    }
}
