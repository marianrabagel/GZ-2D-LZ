using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G2_2D_LZ.Helpers;

namespace G2_2D_LZ.Readers
{
    public class TxtReader
    {
        private Dimension _dimension;
        public TxtReader(Dimension dimension)
        {
            _dimension = dimension;
        }

        public bool[,] GetMatchFlagFromString(string[] values)
        {
            int i = 2;
            bool[,] matrix = new bool[_dimension.Height, _dimension.Width];

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    var value = Convert.ToInt32(values[i++]);
                    matrix[y, x] = value != 0;
                }
            }

            return matrix;
        }

        public PixelLocation[,] GetMatchLocationFromString(string[] values)
        {
            var i = 2 + _dimension.Height * _dimension.Width;
            PixelLocation[,] matrix = new PixelLocation[_dimension.Height, _dimension.Width];

            for (int y = 0; y < _dimension.Height; y++)
            {
                for (int x = 0; x < _dimension.Width; x++)
                {
                    var valueX = Convert.ToUInt32(values[i++]);
                    var valueY = Convert.ToUInt32(values[i++]);

                    matrix[y, x] = new PixelLocation(valueX, valueY);
                }
            }

            return matrix;
        }

        public Dimension[,] GetMatchDimensionsFromString(string[] values)
        {
            var i = 2 + 3 * _dimension.Height * _dimension.Width;
            Dimension[,] matrix = new Dimension[_dimension.Height, _dimension.Width];

            for (int y = 0; y < _dimension.Height; y++)
            {
                for (int x = 0; x < _dimension.Width; x++)
                {
                    var valueWidth = Convert.ToInt32(values[i++]);
                    var valueHeight = Convert.ToInt32(values[i++]);

                    
                    matrix[y, x] = new Dimension(valueWidth, valueHeight);
                }
            }

            return matrix;
        }

        public int[,] ReadResidualFromTxtFile(string[] values)
        {
            var i = 2 + 5 * _dimension.Height * _dimension.Width;
            int[,] matrix = new int[_dimension.Height, _dimension.Width];

            for (int y = 0; y < _dimension.Height; y++)
            {
                for (int x = 0; x < _dimension.Width; x++)
                {
                    var value = Convert.ToInt32(values[i++]);
                    
                    matrix[y, x] = value;
                }
            }

            return matrix;
        }

        public int[,] ReadPredicionErrorFromTxtFile(string[] values)
        {
            var i = 2 + 6 * _dimension.Height * _dimension.Width;
            int[,] matrix = new int[_dimension.Height, _dimension.Width];

            for (int y = 0; y < _dimension.Height; y++)
            {
                for (int x = 0; x < _dimension.Width; x++)
                {
                    var value = Convert.ToInt32(values[i++]);

                    matrix[y, x] = value;
                }
            }

            return matrix;
        }

    }
}
