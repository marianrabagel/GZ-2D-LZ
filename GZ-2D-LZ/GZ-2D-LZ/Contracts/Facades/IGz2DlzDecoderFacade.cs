namespace G2_2D_LZ.Contracts.Facades
{
    public interface IGz2DlzDecoderFacade
    {
        string _inputFilePath { get; set; }
        AbstractPredictor _abstractPredictor { get; set; }
    }
}
