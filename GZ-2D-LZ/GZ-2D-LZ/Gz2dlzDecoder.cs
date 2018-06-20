using System;
using System.IO;
using System.Windows.Forms.VisualStyles;
using BitOperations;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Readers;

namespace G2_2D_LZ
{
    public class Gz2DlzDecoder
    {
        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimension[,] MatchDimension { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public int[,] GeometricTransformation { get; private set; }
        public byte[,] WorkImage { get; private set; }

        private int _height;
        private int _width;

        private readonly IGz2DlzDecoderFacade _gz2DlzDecoderFacade;

        public Gz2DlzDecoder(IGz2DlzDecoderFacade gz2DlzDecoderFacade)
        {
            _gz2DlzDecoderFacade = gz2DlzDecoderFacade;
        }

        public void Decode(int? specificGeometricTransform)
        {
            var decompressArchivePath = DecompressArchive();
            var inputFilePath = _gz2DlzDecoderFacade.InputFilePath;
            DecodeMatchingTablesAndSetWidthAndHeight(inputFilePath, specificGeometricTransform);
            WorkImage = new byte[_height, _width];
            SetIsPixelEncodedToTrue();
            _gz2DlzDecoderFacade.AbstractPredictor.SetOriginalMatrix(WorkImage);
            DecodeFirstRow();
            DecodeAllImageExceptFirstRow(specificGeometricTransform);
            WriteImageToDisk(decompressArchivePath);
        }

        private void WriteImageToDisk(string decompressArchivePath)
        {
            /*int width = Convert.ToInt32(_width);
            int height = Convert.ToInt32(_height);
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var color = WorkImage[y, x];
                        bitmap.SetPixel(x,y, Color.FromArgb(color, color, color)) ;
                    }
                }
                var filename = decompressArchivePath + "\\" + Path.GetFileNameWithoutExtension(decompressArchivePath);
                bitmap.Save(filename);
            }*/
        }

        private string DecompressArchive()
        {
            CreateAFolderWithTheARchiveNameWithouExtension();
            return _gz2DlzDecoderFacade.Archiver.Decompress(_gz2DlzDecoderFacade.InputFilePath);
        }

        private void CreateAFolderWithTheARchiveNameWithouExtension()
        {
            var extension = Path.GetExtension(_gz2DlzDecoderFacade.InputFilePath);
            var filePathWithourExtension =
                _gz2DlzDecoderFacade.InputFilePath.Substring(0, _gz2DlzDecoderFacade.InputFilePath.Length - extension.Length);
            Directory.CreateDirectory(filePathWithourExtension);
        }

        private void DecodeAllImageExceptFirstRow(int? specificGeometricTransform)
        {
            for (int y = 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (IsPixelEncoded[y, x])
                    {
                        var hasMatch = IsMatchFound[y, x];

                        if (hasMatch)
                        {
                            ReproduceImage(y, x, specificGeometricTransform);
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

        private void DecodeMatchingTablesAndSetWidthAndHeight(string inputFilePath, int? specificGeometricTransform)
        {
            IsMatchFound = LoadIsMatchFoundFromFileAndSetWidthAndHeight(GetInputFileName(inputFilePath, nameof(IsMatchFound)));
            MatchLocation = LoadMatchLocationFromFileAndSetWidthAndHeight(GetInputFileName(inputFilePath, nameof(MatchLocation)));
            MatchDimension = LoadMatchDimensionFromFileAndSetWidthAndHeight(GetInputFileName(inputFilePath, nameof(MatchDimension)));
            Residual = LoadResidualFromFileAndSetWidthAndHeight(GetInputFileName(inputFilePath, nameof(Residual)));

            if (specificGeometricTransform != (int)Constants.GeometricTransformation.NoGeometricTransformation)
            {
                GeometricTransformation = LoadGeometricTransformationFromFileAndSetWidthAndHeight(GetInputFileName(inputFilePath,
                        nameof(GeometricTransformation)));
            }

            _gz2DlzDecoderFacade.AbstractPredictor.PredictionError = LoadResidualFromFileAndSetWidthAndHeight(GetInputFileName(inputFilePath, nameof(_gz2DlzDecoderFacade.AbstractPredictor.PredictionError)));
        }

        private int[,] LoadGeometricTransformationFromFileAndSetWidthAndHeight(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));
                _height = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));

                var matrix = new int[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var value = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                        matrix[y, x] = Convert.ToInt32(value);
                    }
                }

                return matrix;
            }
        }

        private int[,] LoadResidualFromFileAndSetWidthAndHeight(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));
                _height = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));

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

        private Dimension[,] LoadMatchDimensionFromFileAndSetWidthAndHeight(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));
                _height = Convert.ToInt32( bitReader.ReadNBits(Constants.NumberOfBitsForSize));

                var matrix = new Dimension[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var xValue = bitReader.ReadNBits(Constants.NumberOfBitsForSize);
                        var yValue = bitReader.ReadNBits(Constants.NumberOfBitsForSize);

                        matrix[y, x] = new Dimension(Convert.ToInt32(xValue), Convert.ToInt32(yValue));
                    }
                }

                return matrix;
            }
        }

        private PixelLocation[,] LoadMatchLocationFromFileAndSetWidthAndHeight(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));
                _height = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));

                var matrix = new PixelLocation[_height, _width];

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        var xValue = bitReader.ReadNBits(Constants.NumberOfBitsForX);
                        var yValue = bitReader.ReadNBits(Constants.NumberOfBitsForX);

                        matrix[y, x] = new PixelLocation(Convert.ToInt32(xValue), Convert.ToInt32(yValue));
                    }
                }

                return matrix;
            }
        }

        private bool[,] LoadIsMatchFoundFromFileAndSetWidthAndHeight(string outputFileName)
        {
            using (BitReader bitReader = new BitReader(outputFileName))
            {
                _width = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));
                _height = Convert.ToInt32(bitReader.ReadNBits(Constants.NumberOfBitsForSize));

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

        private string GetInputFileName(string archiveFilePath, string matrixName)
        {
            var folderPath = GetFolderPath(archiveFilePath);
            var extension = Path.GetExtension(folderPath);
            var fileName = Path.GetFileName(archiveFilePath.Substring(0, folderPath.Length - extension.Length));

            //archiveFilePath = archiveFilePath.Replace(Constants.IntermediaryFileExtension, "");
            //string fileName = Path.GetFileName(archiveFilePath);
            //folderPath = archiveFilePath + $"{Constants.Folder}\\";

            return folderPath + "\\" + fileName + "." + matrixName + Constants.IntermediaryFileExtension;
        }

        private string GetFolderPath(string inputFilePath)
        {
            var extension = Path.GetExtension(inputFilePath);
            return inputFilePath.Substring(0, inputFilePath.Length - extension.Length);
        }

        public void LoadMatrixFromTxtFile()
        {
            using (StreamReader reader = new StreamReader(_gz2DlzDecoderFacade.InputFilePath))
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
                _gz2DlzDecoderFacade.AbstractPredictor.PredictionError = txtReader.ReadPredicionErrorFromTxtFile(values);
            }
        }

        public void SaveOriginalImageAsTxtFile()
        {
            using (StreamWriter streamWriter = new StreamWriter(_gz2DlzDecoderFacade.InputFilePath + ".decoded.txt"))
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

        private void ReproduceImage(int y, int x, int? specificGeometricTransform)
        {
            var matchLocation = MatchLocation[y, x];
            var matchDimension = MatchDimension[y, x];
            
            for (int yOffset = 0; yOffset < matchDimension.Height; yOffset++)
            {
                for (int xOffset = 0; xOffset < matchDimension.Width; xOffset++)
                {
                    var geometricTransformation = GetGeometricTransformation(y, x, specificGeometricTransform);
                    var nextPixel = new PixelLocation(x + xOffset, y + yOffset);
                    var nextRootY = NextRoot.GetNextRootY(matchLocation.Y, yOffset, geometricTransformation);
                    var nextRootX = NextRoot.GetNextRootX(matchLocation.X, xOffset, geometricTransformation);
                    WorkImage[nextPixel.Y, nextPixel.X] = (byte) (WorkImage[nextRootY, nextRootX] + Residual[nextPixel.Y, nextPixel.X]);
                    IsPixelEncoded[nextPixel.Y, nextPixel.X] = false;
                }
            }
        }

        private Constants.GeometricTransformation GetGeometricTransformation(int y, int x, int? specificGeometricTransform)
        {
            Constants.GeometricTransformation geometricTransformation;
            var isWithGeometricTransformation = specificGeometricTransform != (int) Constants.GeometricTransformation.NoGeometricTransformation;

            if (isWithGeometricTransformation)
            {
                geometricTransformation = GetGeometricTransformationFromMatrix(y, x);
            }
            else
            {
                geometricTransformation = Constants.GeometricTransformation.NoGeometricTransformation;
            }

            return geometricTransformation;
        }

        private Constants.GeometricTransformation GetGeometricTransformationFromMatrix(int y, int x)
        {
            if (GeometricTransformation[y, x] == (int) Constants.GeometricTransformation.Identity)
            {
                return  Constants.GeometricTransformation.Identity;
            }
            if (GeometricTransformation[y, x] == (int) Constants.GeometricTransformation.VerticalMirror)
            {
                return Constants.GeometricTransformation.VerticalMirror;
            }
            if (GeometricTransformation[y, x] == (int) Constants.GeometricTransformation.HorizontalMirror)
            {
                return Constants.GeometricTransformation.HorizontalMirror;
            }
            if (GeometricTransformation[y, x] == (int) Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return Constants.GeometricTransformation.FirstDiagonalMirror;
            }

            throw new InvalidOperationException("geometric transforamtion not found in matrix " +
                                                nameof(GetGeometricTransformationFromMatrix));
        }

        private void ReconstructWithPredictonNoMatchBlock(int y, int x)
        {
            for (int yy = 0; yy < Constants.NoMatchBlockHeight; yy++)
            {
                for (int xx = 0; xx < Constants.NoMatchBlockWidth; xx++)
                {
                    if (y + yy < _height && x + xx < _width)
                    {
                        WorkImage[y + yy, x + xx] = (byte) (_gz2DlzDecoderFacade.AbstractPredictor.GetPredictionValue(x + xx, y + yy) + _gz2DlzDecoderFacade.AbstractPredictor.PredictionError[y + yy, x + xx]);
                    }
                }
            }
        }

        private void DecodeFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = false;
                WorkImage[0, i] = (byte) (_gz2DlzDecoderFacade.AbstractPredictor.GetPredictionValue(i, 0) + _gz2DlzDecoderFacade.AbstractPredictor.PredictionError[0, i]);
            }
        }
    }
}
