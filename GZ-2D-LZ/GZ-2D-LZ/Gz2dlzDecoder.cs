using System;
using System.IO;
using System.Security.Permissions;
using System.Xml;
using G2_2D_LZ.Helpers;

namespace G2_2D_LZ
{
    public class Gz2dlzDecoder : BaseClass
    {
        private int _height;
        private int _width;

        public Gz2dlzDecoder(string inputFileName) : base(inputFileName)
        {
            
        }

        public void LoadMatrixFromTxtFile()
        {
            using (StreamReader reader = new StreamReader(InputFileName))
            {
                var fileContent = reader.ReadToEnd();
                string[] values = fileContent.Split(' ');
                _height = Convert.ToInt32(values[1]);
                _width = Convert.ToInt32(values[0]);
                InitializeTables(_height, _width);
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
            var height = MatchDimensions.GetLength(0);
            var width = MatchDimensions.GetLength(1);
            int i = 2 + 3 * height * width;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var valueWidth = Convert.ToInt32(values[i++]);
                    var valueHeight = Convert.ToInt32(values[i++]);

                    MatchDimensions[y, x] = new BlockDimension(valueWidth, valueHeight);
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
                    var valueX = Convert.ToInt32(values[i++]);
                    var valueY = Convert.ToInt32(values[i++]);

                    MatchLocation[y, x] = new PixelLocation(valueX, valueY);
                }
            }
        }

        private void LoadMatchFlagFromFile(string[] values)
        {
            int i = 2;

            for (int y = 0; y < MatchFlag.GetLength(0); y++)
            {
                for (int x = 0; x < MatchFlag.GetLength(1); x++)
                {
                    var value = Convert.ToInt32(values[i++]);
                    MatchFlag[y, x] = value != 0;
                }
            }
        }

        public void Decode()
        {
            DecodeMatchingTables();
            DecodeFirstRow();

            for (int y = 1; y < WorkImage.GetLength(0); y++)
            {
                for (int x = 0; x < WorkImage.GetLength(1); x++)
                {
                    if (IsPixelEncoded[y, x])
                    {
                        var hasMatch = MatchFlag[y, x];

                        if (hasMatch)
                        {

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
            //if (match falg is false)
            //predict a block of pixels and correct the prediction errors using the prediction error table
            //get the next unencoded pixel
            //end while
            //output the reconstructed image
        }

        private void PredictNoMatchBlock(int y, int x)
        {
            for (int i = 0; i < Constants.NoMatchBlockHeight; i++)
            {
                for (int j = 0; j < Constants.NoMatchBlockWidth; j++)
                {
                    if (y + i < WorkImage.GetLength(0) && x + j < WorkImage.GetLength(1))
                    {
                        WorkImage[y + i, x + j] = (byte) (GetPredictionValue(x + j, y + i) + PredictionError[y + i, x + j]);
                    }
                }
            }
        }

        private void DecodeFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = false;
                WorkImage[0, i] = (byte) (GetPredictionValue(i, 0) + PredictionError[0, i]);
            }
        }

        private void DecodeMatchingTables()
        {
            
        }
    }
}
