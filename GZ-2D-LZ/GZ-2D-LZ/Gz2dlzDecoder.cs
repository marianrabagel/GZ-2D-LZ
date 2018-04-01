using System;
using System.Drawing;
using System.IO;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Readers;

namespace G2_2D_LZ
{
    public class Gz2DlzDecoder
    {
        private readonly string _inputFileName;

        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimension[,] MatchDimension { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public byte[,] WorkImage { get; private set; }

        private int _height;
        private int _width;

        private readonly AbstractPredictor _abstractPredictor;

        public Gz2DlzDecoder(string inputFileName, AbstractPredictor abstractPredictor)
        {
            _inputFileName = inputFileName;
            _abstractPredictor = abstractPredictor;
        }

        public void Decode()
        {
            _abstractPredictor.SetOriginalMatrix(WorkImage);

            DecodeFirstRow();

            for (int y = 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (IsPixelEncoded[y, x])
                    {
                        var hasMatch = IsMatchFound[y, x];

                        if (hasMatch)
                        {
                            ReproduceImage(y, x);
                        }
                        else
                        {
                            ReconstructWithPredictonNoMatchBlock(y, x);
                            x += Constants.NoMatchBlockWidth - 1;
                        }
                    }
                }
            }
        }

        public void LoadMatrixFromTxtFile()
        {
            using (StreamReader reader = new StreamReader(_inputFileName))
            {
                var fileContent = reader.ReadToEnd();
                string[] values = fileContent.Split(Constants.Separator);
                _height = Convert.ToInt32(values[1]);
                _width = Convert.ToInt32(values[0]);

                TxtReader txtReader = new TxtReader(new Dimension(_width, _height));

                WorkImage = new byte[_height, _width];
                SetIsPixelEncodedToTrue();

                IsMatchFound = txtReader.GetMatchFlagFromString(values);
                MatchLocation = txtReader.GetMatchLocationFromString(values);
                MatchDimension = txtReader.GetMatchDimensionsFromString(values);
                Residual = txtReader.ReadResidualFromTxtFile(values);
                _abstractPredictor.PredictionError = txtReader.ReadPredicionErrorFromTxtFile(values);
            }
        }

        public void SaveAsTxtFile()
        {
            using (StreamWriter writer = new StreamWriter(_inputFileName + ".decoded.txt"))
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        byte value = WorkImage[y, x];
                        writer.Write(value);
                        writer.Write(Constants.Separator.ToString());
                    }
                    writer.WriteLine();
                }
            }
        }


        private void SetIsPixelEncodedToTrue()
        {
            IsPixelEncoded = new bool[_height, _width];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    IsPixelEncoded[y, x] = true;
                }
            }
        }

        private void ReproduceImage(int y, int x)
        {
            var matchLocation = MatchLocation[y, x];
            var matchDimension = MatchDimension[y, x];

            for (int i = 0; i < matchDimension.Height; i++)
            {
                for (int j = 0; j < matchDimension.Width; j++)
                {
                    WorkImage[y + i, x + j] =
                        (byte) (WorkImage[matchLocation.Y + i, matchLocation.X + j] + Residual[y + i, x + j]);
                    IsPixelEncoded[y + i, x + j] = false;
                }
            }
        }

        private void ReconstructWithPredictonNoMatchBlock(int y, int x)
        {
            for (int i = 0; i < Constants.NoMatchBlockHeight; i++)
            {
                for (int j = 0; j < Constants.NoMatchBlockWidth; j++)
                {
                    if (y + i < _height && x + j < _width)
                    {
                        WorkImage[y + i, x + j] = (byte) (_abstractPredictor.GetPredictionValue(x + j, y + i) + _abstractPredictor.PredictionError[y + i, x + j]);
                    }
                }
            }
        }

        private void DecodeFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = false;
                WorkImage[0, i] = (byte) (_abstractPredictor.GetPredictionValue(i, 0) + _abstractPredictor.PredictionError[0, i]);
            }
        }
    }
}
