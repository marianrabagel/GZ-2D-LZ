using System;
using System.Drawing;
using System.IO;
using BitOperations;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Readers;

namespace G2_2D_LZ
{
    public class Gz2DlzDecoder
    {
        private readonly string _inputFilePath;

        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimension[,] MatchDimension { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public byte[,] WorkImage { get; private set; }

        private uint _height;
        private uint _width;

        private readonly AbstractPredictor _abstractPredictor;

        public Gz2DlzDecoder(string inputFilePath, AbstractPredictor abstractPredictor)
        {
            _inputFilePath = inputFilePath;
            _abstractPredictor = abstractPredictor;
        }

        public void Decode()
        {
            DecodeMatchingTables();
            WorkImage = new byte[_height, _width];
            SetIsPixelEncodedToTrue();
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

        private void DecodeMatchingTables()
        {
            IsMatchFound = LoadIsMatchFoundFromFile(GetInputFileName(_inputFilePath, nameof(IsMatchFound)));
            MatchLocation = LoadMatchLocationFromFile(GetInputFileName(_inputFilePath, nameof(MatchLocation)));
            MatchDimension = LoadMatchDimensionFromFile(GetInputFileName(_inputFilePath, nameof(MatchDimension)));
            Residual = LoadResidualFromFile(GetInputFileName(_inputFilePath, nameof(Residual)));
            _abstractPredictor.PredictionError = LoadResidualFromFile(GetInputFileName(_inputFilePath, nameof(_abstractPredictor.PredictionError)));

        }

        private int[,] LoadResidualFromFile(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                _height = bitReader.ReadNBits(Constants.NumberOfBitsForSize);

                var matrix = new int[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var positivePredictionError = bitReader.ReadNBits(Constants.NumberOfBitsForPredictionError);
                        matrix[y, x] = Convert.ToInt32(positivePredictionError) - 255;
                    }
                }

                return matrix;
            }
        }

        private Dimension[,] LoadMatchDimensionFromFile(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                _height = bitReader.ReadNBits(Constants.NumberOfBitsForSize);

                var matrix = new Dimension[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var xValue = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                        var yValue = bitReader.ReadNBits(Constants.NumberOfBitsForSize);

                        matrix[y, x] = new Dimension(xValue, yValue);
                    }
                }

                return matrix;
            }
        }

        private PixelLocation[,] LoadMatchLocationFromFile(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                _height = bitReader.ReadNBits(Constants.NumberOfBitsForSize);

                var matrix = new PixelLocation[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var xValue = bitReader.ReadNBits(Constants.NumberOfBitsForX);
                        var yValue = bitReader.ReadNBits(Constants.NumberOfBitsForX);

                        matrix[y, x] = new PixelLocation(xValue, yValue);
                    }
                }

                return matrix;
            }
        }

        private bool[,] LoadIsMatchFoundFromFile(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                _height = bitReader.ReadNBits(Constants.NumberOfBitsForSize);

                var matrix = new bool[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        byte bit = bitReader.ReadBit();
                        matrix[y, x] = bit == 0x01;
                    }
                }

                return matrix;
            }
        }

        private string GetInputFileName(string filePath, string matrixName)
        {
            filePath = filePath.Replace(Constants.IntermediaryFileExtension, "");
            string fileName = Path.GetFileName(filePath);

            return filePath + $"{Constants.Folder}\\" + fileName + "." + matrixName + Constants.IntermediaryFileExtension;
        }

        public void LoadMatrixFromTxtFile()
        {
            using (StreamReader reader = new StreamReader(_inputFilePath))
            {
                var fileContent = reader.ReadToEnd();
                string[] values = fileContent.Split(Constants.Separator);
                _height = Convert.ToUInt32(values[1]);
                _width = Convert.ToUInt32(values[0]);

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

        public void SaveOriginalImageAsTxtFile()
        {
            using (StreamWriter streamWriter = new StreamWriter(_inputFilePath + ".decoded.txt"))
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        byte value = WorkImage[y, x];
                        streamWriter.Write(value);
                        streamWriter.Write(Constants.Separator.ToString());
                    }
                    streamWriter.WriteLine();
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
