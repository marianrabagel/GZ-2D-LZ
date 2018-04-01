using System.IO;
using G2_2D_LZ.Helpers;

namespace G2_2D_LZ.Writers
{
    //todo maybe create an Interface?
    public class TxtWriter
    {
        private readonly string _inputFileName;

        public TxtWriter(string inputFileName)
        {
            _inputFileName = inputFileName;

            using (StreamWriter streamWriter = new StreamWriter(_inputFileName, false))
            {
                streamWriter.Write("");
            }
        }

        public void WriteMatchFlagToFile(bool[,] matrix)
        {
            var height = matrix.GetLength(0);
            var width = matrix.GetLength(1);

            using (StreamWriter streamWriter = new StreamWriter(_inputFileName, true))
            {
                for (int y = 0; y < height; y++)
                {

                    for (int x = 0; x < width; x++)
                    {
                        var value = matrix[y, x] ? 1 : 0;
                        streamWriter.Write(value + Constants.Separator.ToString());
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        public void WriteMatchLocationToFile(PixelLocation[,] matrix)
        {
            using (StreamWriter streamWriter = new StreamWriter(_inputFileName, true))
            {
                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var value = matrix[y, x] ?? new PixelLocation(0, 0);
                        streamWriter.Write(value.X + Constants.Separator.ToString());
                        streamWriter.Write(value.Y + Constants.Separator.ToString());
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        public void WriteMatchDimensionsToFile(Dimension[,] matrix)
        {
            using (StreamWriter streamWriter = new StreamWriter(_inputFileName, true))
            {
                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var value = matrix[y, x] ?? new Dimension(0, 0);

                        streamWriter.Write(value.Width + Constants.Separator.ToString());
                        streamWriter.Write(value.Height + Constants.Separator.ToString());
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        public void WriteMatrixToFile(int[,] matrix)
        {
            using (StreamWriter streamWriter = new StreamWriter(_inputFileName, true))
            {
                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var format = matrix[y, x] + Constants.Separator.ToString();
                        streamWriter.Write(format);
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        public void Write(string text)
        {
            using (StreamWriter streamWriter = new StreamWriter(_inputFileName, true))
            {
                streamWriter.Write(text);
            }
        }
    }
}
