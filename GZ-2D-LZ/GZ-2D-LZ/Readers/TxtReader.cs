using System;
using System.IO;
using G2_2D_LZ.Contracts;

namespace G2_2D_LZ.Readers
{
    public class TxtReader : IReader
    {
        public byte[,] LoadImageInMemory(string inputFileName)
        {
            using (StreamReader reader = new StreamReader(inputFileName))
            {
                var fileContent = reader.ReadToEnd();
                string[] values = fileContent.Split(' ');
                var height = Convert.ToInt32(values[1]);
                var width = Convert.ToInt32(values[0]);
                int i = 2;
                byte[,] image = new byte[height, width];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var int32 = values[i++];
                        image[y, x] = Convert.ToByte(int32);
                    }
                }

                return image;
            }
        }
    }
}
