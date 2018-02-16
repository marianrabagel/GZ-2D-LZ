using System;
using System.IO;
using System.Security.Permissions;
using System.Xml;
using G2_2D_LZ.Helpers;

namespace G2_2D_LZ
{
    public class Gz2dlzDecoder : BaseClass
    {

        public Gz2dlzDecoder(string inputFileName) : base(inputFileName)
        {
            int height = 0;
            int width = 0;
            InitializeTables(height, width);
        }

        public void LoadMatrixFromTxtFile()
        {
            using (StreamReader reader = new StreamReader(InputFileName))
            {
                var fileContent = reader.ReadToEnd();
                string[] values = fileContent.Split(' ');
                int height = Convert.ToInt32(values[1]);
                int width = Convert.ToInt32(values[0]);
                InitializeTables(height, width);

                LoadMatchFlagFromFile(values);
                LoadMatchLocationFromFile(values);
                LoadMatchDimensionsFromFile(values);

                int i = 2 + 5 * height * width;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var value = Convert.ToInt32(values[i++]);
                        Residual[y, x] = value;
                    }
                }

                int j = 2 + 6 * height * width;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var value = Convert.ToInt32(values[j++]);
                        PredictionError[y, x] = value;
                    }
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
            //decode the matching tables
            //predict one row of pixels and correct each prediction error using the prediction error table
            //advance the decoder to the next undecoded pixel
            //while ( more pixeld to be decoded)
            //read a value from the match flag table
            //if (match flag is true)
            //reproduce a block o pixels and correct the prediction errors using the prediction error table
            //if (match falg is false)
            //predict a block of pixells and correct the prediction errors using the prediction error table
            //get the next unencoded pixel
            //end while
            //output the reconstructed image
        }

        private void DecodeMatchingTables()
        {
            
        }
    }
}
