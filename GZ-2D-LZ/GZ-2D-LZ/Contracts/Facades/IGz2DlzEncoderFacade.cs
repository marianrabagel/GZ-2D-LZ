namespace G2_2D_LZ.Contracts.Facades
{
    public interface IGz2DlzEncoderFacade
    {
        string InputFilePath { get; set; }
        AbstractPredictor AbstractPredictor { get; set; }
        IImageReader ImageReader { get; set; }
    }
}
