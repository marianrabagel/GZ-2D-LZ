using System;
using System.Drawing;
using System.IO;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Helpers;

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
        public int[,] PredictionError { get; private set; } // prediction error values
        public byte[,] WorkImage { get; private set; }

        private int _height;
        private int _width;

        private readonly AbstractPredictor _abstractPredictor;

        protected Gz2DlzDecoder(string inputFileName) 
        {
            _inputFileName = inputFileName;
        }

        public Gz2DlzDecoder(string inputFileName, AbstractPredictor abstractPredictor) : this(inputFileName)
        {
            _abstractPredictor = abstractPredictor;
        }

        public void LoadMatrixFromTxtFile()
        {
            using (StreamReader reader = new StreamReader(_inputFileName))
            {
                var fileContent = reader.ReadToEnd();
                string[] values = fileContent.Split(' ');
                _height = Convert.ToInt32(values[1]);
                _width = Convert.ToInt32(values[0]);

                InitializeTables();
                WorkImage = new byte[_height, _width];
                SetIsPixelEncodedToTrue();

                LoadMatchFlagFromFile(values);
                LoadMatchLocationFromFile(values);
                LoadMatchDimensionsFromFile(values);

                int i = 2 + 5 * _height * _width;
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var value = Convert.ToInt32(values[i++]);
                        Residual[y, x] = value;
                    }
                }

                int j = 2 + 6 * _height * _width;
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var value = Convert.ToInt32(values[j++]);
                        PredictionError[y, x] = value;
                    }
                }
            }
        }

        protected void InitializeTables()
        {
            IsMatchFound = new bool[_height, _width];
            IsPixelEncoded = new bool[_height, _width];
            MatchLocation = new PixelLocation[_height, _width];
            MatchDimension = new Dimension[_height, _width];
            Residual = new int[_height, _width];
            PredictionError = new int[_height, _width];
        }

        private void SetIsPixelEncodedToTrue()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    IsPixelEncoded[y, x] = true;
                }
            }
        }

        private void LoadMatchDimensionsFromFile(string[] values)
        {
            var height = MatchDimension.GetLength(0);
            var width = MatchDimension.GetLength(1);
            int i = 2 + 3 * height * width;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var valueWidth = Convert.ToInt32(values[i++]);
                    var valueHeight = Convert.ToInt32(values[i++]);

                    MatchDimension[y, x] = new Dimension(valueWidth, valueHeight);
                }
            }
        }

        private void LoadMatchLocationFromFile(string[] values)
        {
            var height = MatchLocation.GetLength(0);
            var width = MatchLocation.GetLength(1);
            int i = 2 + height * width;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var valueX = Convert.ToUInt32(values[i++]);
                    var valueY = Convert.ToUInt32(values[i++]);

                    MatchLocation[y, x] = new PixelLocation(valueX, valueY);
                }
            }
        }

        private void LoadMatchFlagFromFile(string[] values)
        {
            int i = 2;

            for (int y = 0; y < IsMatchFound.GetLength(0); y++)
            {
                for (int x = 0; x < IsMatchFound.GetLength(1); x++)
                {
                    var value = Convert.ToInt32(values[i++]);
                    IsMatchFound[y, x] = value != 0;
                }
            }
        }

        public void Decode()
        {
            _abstractPredictor.SetOriginalMatrix(WorkImage);

            DecodeMatchingTables();
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
                            PredictNoMatchBlock(y, x);
                            x += Constants.NoMatchBlockWidth - 1;
                        }
                    }
                }
            }

            

            //decode the matching tables
            //predict one row of pixels and correct each prediction error using the prediction error table
            //advance the decoder to the next undecoded pixel
            //while ( more pixels to be decoded)
            //read a value from the match flag table
            //if (match flag is true)
            //reproduce a block of pixels using the match dimension, location and residual tables
            //if (match flag is false)
            //predict a block of pixels and correct the prediction errors using the prediction error table
            //get the next unencoded pixel
            //end while
            //output the reconstructed image
        }

        public void SaveAsBitmap()
        {
            Bitmap bitmap = new Bitmap(_width, _height);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var color = WorkImage[y, x];
                    bitmap.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }

            bitmap.Save(_inputFileName + ".decoded.bmp");
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
                        writer.Write(" ");
                    }
                    writer.WriteLine();
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

        private void PredictNoMatchBlock(int y, int x)
        {
            for (int i = 0; i < Constants.NoMatchBlockHeight; i++)
            {
                for (int j = 0; j < Constants.NoMatchBlockWidth; j++)
                {
                    if (y + i < _height && x + j < _width)
                    {
                        WorkImage[y + i, x + j] = (byte) (_abstractPredictor.GetPredictionValue(x + j, y + i) + PredictionError[y + i, x + j]);
                    }
                }
            }
        }

        private void DecodeFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = false;
                WorkImage[0, i] = (byte) (_abstractPredictor.GetPredictionValue(i, 0) + PredictionError[0, i]);
            }
        }

        private void DecodeMatchingTables()
        {
            
        }
    }
}
