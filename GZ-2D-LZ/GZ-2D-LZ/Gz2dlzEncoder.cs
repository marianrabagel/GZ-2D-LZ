using System;
using System.IO;
using BitOperations;
using BitOperations.Contracts;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Writers;

namespace G2_2D_LZ
{
    public class Gz2DlzEncoder
    {
        private readonly byte[,] _originalImage;
        private readonly int _height;
        private readonly int _width;
        public string ArchivePath;
        
        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimension[,] MatchDimension { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public int[,] GeometricTransformation { get; private set; } 
        public byte[,] WorkImage { get; set; } //fractal debugging

        private readonly IGz2DlzEncoderFacade _gz2DlzEncoderFacade;

        public Gz2DlzEncoder(IGz2DlzEncoderFacade gz2DlzEncoderFacade)
        {
            _gz2DlzEncoderFacade = gz2DlzEncoderFacade;
            _originalImage = _gz2DlzEncoderFacade.ImageReader.GetImageFromFile(_gz2DlzEncoderFacade.InputFilePath);

            _height = _originalImage.GetLength(0);
            _width = _originalImage.GetLength(1);

            InstatiateTables();
            CopyOriginalImageWorkImage();
        }
        
        public void Encode(int? specificGeometricTransform)
        {
            _gz2DlzEncoderFacade.AbstractPredictor.SetOriginalMatrix(WorkImage);
            _gz2DlzEncoderFacade.AbstractPredictor.InitializePredictionError((int) _height, (int)_width);
            PredictFirstRow();
            EncodeWorkImage(specificGeometricTransform);
            Directory.CreateDirectory(_gz2DlzEncoderFacade.InputFilePath + Constants.Folder);
            WriteResultingMatricesToIndividualFiles(specificGeometricTransform);
            ArchivePath = _gz2DlzEncoderFacade.Archiver.Compress(_gz2DlzEncoderFacade.InputFilePath + Constants.Folder, null, 9);
        }

        public void WriteMatrixToFileAsText()
        {
            TxtWriter txtWriter = new TxtWriter(_gz2DlzEncoderFacade.InputFilePath + Constants.IntermediaryFileExtension);

            txtWriter.Write(WorkImage.GetLength(1) + Constants.Separator.ToString());
            txtWriter.Write(WorkImage.GetLength(0) + Constants.Separator.ToString());
            txtWriter.Write(Environment.NewLine);

            txtWriter.WriteMatchFlagToFile(IsMatchFound);
            txtWriter.WriteMatchLocationToFile(MatchLocation);
            txtWriter.WriteMatchDimensionsToFile(MatchDimension);
            txtWriter.WriteMatrixToFile(Residual);
            txtWriter.WriteMatrixToFile(_gz2DlzEncoderFacade.AbstractPredictor.PredictionError);
            txtWriter.WriteMatrixToFile(GeometricTransformation);
        }

        public void WriteResultingMatricesToIndividualFiles(int? specificGeometricTransform)
        {
            //todo move this into its own class
            SaveIsMatchFoundToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(IsMatchFound), IsMatchFound);
            SaveMatchLocationToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(MatchLocation), MatchLocation);
            SaveMatchDimensionsToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(MatchDimension), MatchDimension);
            ConvertToPositiveAndSaveMatrixToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(Residual), Residual);
            ConvertToPositiveAndSaveMatrixToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(_gz2DlzEncoderFacade.AbstractPredictor.PredictionError), _gz2DlzEncoderFacade.AbstractPredictor.PredictionError);

            if (specificGeometricTransform != (int) Helpers.Constants.GeometricTransformation.NoGeometricTransformation)
            {
                SaveIntMatrixToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(GeometricTransformation),
                    GeometricTransformation);
            }
        }

        private void SaveIntMatrixToFile(string filePath, string matrixName, int[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        uint bits = Convert.ToUInt32(matrix[y, x]);
                        bitWriter.WriteNBits(bits, Constants.NumberOfBitsForPredictionError);
                    }
                }
            }
        }

        private void ConvertToPositiveAndSaveMatrixToFile(string filePath, string matrixName, int[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        uint bits = Convert.ToUInt32(matrix[y, x] + 255);
                        bitWriter.WriteNBits(bits, Constants.NumberOfBitsForPredictionError);
                    }
                }
            }
        }

        private void SaveMatchDimensionsToFile(string filePath, string matrixName, Dimension[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var dimension = matrix[y, x] ?? new Dimension(0,0);

                        bitWriter.WriteNBits(Convert.ToUInt32(dimension.Width), Constants.NumberOfBitsForSize);
                        bitWriter.WriteNBits(Convert.ToUInt32(dimension.Height), Constants.NumberOfBitsForSize);
                    }
                }
            }
        }

        private void SaveMatchLocationToFile(string filePath, string matrixName, PixelLocation[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var pixelLocation = matrix[y, x] ?? new PixelLocation(0, 0);

                        bitWriter.WriteNBits(Convert.ToUInt32(pixelLocation.X), Constants.NumberOfBitsForX);
                        bitWriter.WriteNBits(Convert.ToUInt32(pixelLocation.Y), Constants.NumberOfBitsForX);
                    }
                }
            }
        }

        private void SaveIsMatchFoundToFile(string filePath, string matrixName, bool[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var bit = matrix[y, x] ? 0X01 : 0X00;

                        bitWriter.WriteBit(bit);
                    }
                }
            }
        }

        private string GetOutputFileName(string filePath, string matrixName)
        {
            string fileName = Path.GetFileName(filePath);

            return filePath + $"{Constants.Folder}\\" + fileName + "." + matrixName + Constants.IntermediaryFileExtension;
        }

        private void WriteWithdAndHeightToFile(IBitWriter bitWriter)
        {
            bitWriter.WriteNBits(Convert.ToUInt32(_width), Constants.NumberOfBitsForSize);
            bitWriter.WriteNBits(Convert.ToUInt32(_height), Constants.NumberOfBitsForSize);
        }
        
        private void EncodeWorkImage(int? specificGeometricTransform)
        {

            int countIdentiy = 0;
            int countVertical = 0;
            int countHorizontal = 0;
            int count180 = 0;
            int countPrediction = 0;

            for (int y = 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (!IsPixelEncoded[y, x])
                    {
                        PixelLocation rootPoint;
                        var encoderPoint = new PixelLocation(x, y);
                        BestMatch bestMatch;
                        int geometricTransformation;

                        if (specificGeometricTransform == null)
                        {
                            geometricTransformation = (int) Constants.GeometricTransformation.Identity;
                            var rootPointForIdentity = new PixelLocation(GetRootX(encoderPoint.X, geometricTransformation), GetRootY(encoderPoint.Y, geometricTransformation));
                            var bestMatchIdentity = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPointForIdentity, geometricTransformation);

                            geometricTransformation = (int) Constants.GeometricTransformation.VerticalMirror;
                            var rootPointForVerticalMirror = new PixelLocation(GetRootX(encoderPoint.X, geometricTransformation), GetRootY(encoderPoint.Y, geometricTransformation));
                            var bestMatchVerticalMirror = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPointForVerticalMirror, geometricTransformation);

                            geometricTransformation = (int) Constants.GeometricTransformation.HorizontalMirror;
                            var rootPointForHorizontalMirror = new PixelLocation(GetRootX(encoderPoint.X, geometricTransformation), GetRootY(encoderPoint.Y, geometricTransformation));
                            var bestMatchHorizontalMirror = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPointForVerticalMirror, geometricTransformation);

                            geometricTransformation = (int)Constants.GeometricTransformation.FirstDiagonalMirror;
                            var rootPointForFirstDiagonallMirror = new PixelLocation(GetRootX(encoderPoint.X, geometricTransformation), GetRootY(encoderPoint.Y, geometricTransformation));
                            var bestMatchForFIrstDiagonalMirror = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPointForFirstDiagonallMirror, geometricTransformation);

                            if (bestMatchIdentity.Size >= bestMatchVerticalMirror.Size)
                            {
                                bestMatch = bestMatchIdentity;
                                geometricTransformation = (int) Constants.GeometricTransformation.Identity;
                                rootPoint = rootPointForIdentity;
                            }
                            else
                            {
                                bestMatch = bestMatchVerticalMirror;
                                geometricTransformation = (int) Constants.GeometricTransformation.VerticalMirror;
                                rootPoint = rootPointForVerticalMirror;
                            }

                            if (bestMatchHorizontalMirror.Size > bestMatch.Size)
                            {
                                bestMatch = bestMatchHorizontalMirror;
                                geometricTransformation = (int) Constants.GeometricTransformation.HorizontalMirror;
                                rootPoint = rootPointForHorizontalMirror;
                            }
                            if (bestMatchForFIrstDiagonalMirror.Size > bestMatch.Size)
                            {
                                bestMatch = bestMatchForFIrstDiagonalMirror;
                                geometricTransformation = (int)Constants.GeometricTransformation.FirstDiagonalMirror;
                                rootPoint = rootPointForFirstDiagonallMirror;
                            }
                        }
                        else
                        {
                            geometricTransformation = (int) specificGeometricTransform;
                            rootPoint = new PixelLocation(GetRootX(x, geometricTransformation), GetRootY(y, geometricTransformation));
                            bestMatch = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint, geometricTransformation);
                        }
                        if (bestMatch.Size > Constants.MinMatchSize)
                        {
                            IsMatchFound[y, x] = true;
                            MatchDimension[y, x] = new Dimension(bestMatch.Width, bestMatch.Height);
                            MatchLocation[y, x] = rootPoint;
                            GeometricTransformation[y, x] = geometricTransformation;
                            SetResidualAndIsPixelEncoded(x, y, geometricTransformation);
                            
                            if (geometricTransformation == (int) Constants.GeometricTransformation.Identity
                                || geometricTransformation == (int)Constants.GeometricTransformation.NoGeometricTransformation)
                            {
                                countIdentiy++;
                            }
                            if (geometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror)
                            {
                                countVertical++;
                            }
                            if (geometricTransformation == (int)Constants.GeometricTransformation.HorizontalMirror)
                            {
                                countHorizontal++;
                            }
                            if (geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
                            {
                                count180++;
                            }
                        }
                        else
                        {
                            IsMatchFound[y, x] = false;
                            PredictNoMatchBlock(x, y);
                            x += Constants.NoMatchBlockWidth - 1;
                            countPrediction++;
                        }
                    }
                }
            }

            using (StreamWriter wr = new StreamWriter(_gz2DlzEncoderFacade.InputFilePath + "_counts.txt"))
            {
                wr.WriteLine(_gz2DlzEncoderFacade.InputFilePath);
                wr.WriteLine(nameof(countIdentiy) + ": " + countIdentiy);
                wr.WriteLine(nameof(countVertical)+ ": " + countVertical);
                wr.WriteLine(nameof(countHorizontal) + ": " + countHorizontal);
                wr.WriteLine(nameof(count180) + ": " + count180);
                wr.WriteLine(nameof(countPrediction) + ": " + countPrediction);
            }
        }

        public BestMatch LocateTheBestAproximateMatchForGivenRootPixel(PixelLocation encoderPoint, PixelLocation rootPoint, int geometricTransformation)
        {
            BestMatch bestMatch = new BestMatch();
            int rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = _width - encoderPoint.X; //-1

            do
            {
                int colOffset = CalculateTheWidthOfTheMatch(encoderPoint, rowOffset, rootPoint, widthOfTheMatchInThePreviousRow, geometricTransformation);
                widthOfTheMatchInThePreviousRow = colOffset;
                rowOffset++;

                var matchSize = widthOfTheMatchInThePreviousRow * rowOffset;
                var blockDimensions = new Dimension(widthOfTheMatchInThePreviousRow, rowOffset);
                var matchMse = GetMse(encoderPoint, rootPoint, blockDimensions, geometricTransformation);

                if (matchSize >= bestMatch.Size && matchMse <= Constants.MaxMse)
                {
                    bestMatch.Height = rowOffset;
                    bestMatch.Width = widthOfTheMatchInThePreviousRow;
                    bestMatch.Size = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow != 0 && encoderPoint.Y + rowOffset != _height );//-1

            return bestMatch;
        }

        private int CalculateTheWidthOfTheMatch(PixelLocation encoderPoint, int rowOffset,
            PixelLocation rootPoint, int widthOfTheMatchInThePreviousRow, int geometricTransformation)
        {
            int colOffset = 0;
            var nextRootY = GetNextRootY(rootPoint.Y, rowOffset, geometricTransformation);

            if (nextRootY < 0)
            {
                return colOffset;
            }
            
            var nextRootPoint = new PixelLocation(rootPoint.X, nextRootY);

            if (IsPixelEncoded[nextRootPoint.Y, nextRootPoint.X]
                || nextRootPoint.Y >= encoderPoint.Y && nextRootPoint.X >= encoderPoint.X)
            {
                do
                {
                    var nextToBeEncoded = new PixelLocation(encoderPoint.X + colOffset, encoderPoint.Y + rowOffset);

                    if (IsHigherThanWidthOrHeight(nextToBeEncoded.X, nextToBeEncoded.Y) 
                        || IsPixelEncoded[nextToBeEncoded.Y, nextToBeEncoded.X])
                    {
                        break;
                    }

                    var pixelToBeEncoded = WorkImage[nextToBeEncoded.Y, nextToBeEncoded.X];
                    var nextRootX = GetNextRootX(nextRootPoint.X, colOffset, geometricTransformation);

                    if (nextRootX < 0)
                    {
                        break;
                    }
/*
                    if (!IsPixelEncoded[nextRootPoint.Y, nextRootX])
                    {
                        break;
                    }*/

                    var possibleMatchPixel = WorkImage[nextRootPoint.Y, nextRootX];

                    if (Math.Abs(pixelToBeEncoded - possibleMatchPixel) <= Constants.Threshold)
                    {
                        colOffset++;
                    }
                    else
                    {
                        break;
                    }

                } while (colOffset != widthOfTheMatchInThePreviousRow);
            }

            return colOffset;
        }

        private int GetNextRootY(int y, int rowOffset, int geometricTransformation)
        {
            if (geometricTransformation == (int) Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return y - rowOffset;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.Identity
                || geometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.NoGeometricTransformation
                )
            {
                return y + rowOffset;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetNextRootY));
        }

        private int GetNextRootX(int x, int colOffset, int geometricTransformation)
        {
            if (geometricTransformation == (int) Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return x - colOffset;
            }
            if (geometricTransformation == (int) Constants.GeometricTransformation.Identity
                || geometricTransformation == (int) Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == (int) Constants.GeometricTransformation.NoGeometricTransformation)
            {
                return x + colOffset;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetNextRootX));
        }

        private bool IsHigherThanWidthOrHeight(int x, int y)
        {
            return x == _width || y == _height ;
        }

        private void PredictNoMatchBlock(int x, int y)
        {
            for (int yy = 0; yy < Constants.NoMatchBlockHeight; yy++)
            {
                for (int xx = 0; xx < Constants.NoMatchBlockWidth; xx++)
                {
                    if (y + yy < WorkImage.GetLength(0) && x + xx < WorkImage.GetLength(1))
                    {
                        IsPixelEncoded[y + yy, x + xx] = true;
                        _gz2DlzEncoderFacade.AbstractPredictor.PredictionError[y + yy, x + xx] = WorkImage[y + yy, x + xx] - _gz2DlzEncoderFacade.AbstractPredictor.GetPredictionValue(x + xx, y + yy);
                    }
                }
            }
        }

        private void PredictFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = true;
                _gz2DlzEncoderFacade.AbstractPredictor.PredictionError[0, i] = WorkImage[0, i] - _gz2DlzEncoderFacade.AbstractPredictor.GetPredictionValue(i, 0);
            }
        }

        private void SetResidualAndIsPixelEncoded(int encoderX, int encoderY, int geometricTransformation)
        {
            var matchDimension = MatchDimension[encoderY, encoderX];
            var matchLocation = MatchLocation[encoderY, encoderX];

            for (int yy = 0; yy < matchDimension.Height; yy++)
            {
                for (int xx = 0; xx < matchDimension.Width; xx++)
                {
                    var nextX = GetNextRootX(matchLocation.X, xx, geometricTransformation);
                    var nextY = GetNextRootY(matchLocation.Y, yy, geometricTransformation);
                    
                    Residual[encoderY + yy, encoderX + xx] = WorkImage[encoderY + yy, encoderX + xx] -
                                                     WorkImage[nextY, nextX];
                    IsPixelEncoded[encoderY + yy, encoderX + xx] = true;
                }
            }
        }

        private static int GetRootY(int y, int geometricTransformation)
        {
            if (geometricTransformation == (int) Constants.GeometricTransformation.HorizontalMirror)
            {
                return y - 1;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return y - 1;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int) Constants.GeometricTransformation.Identity
                || geometricTransformation == (int) Constants.GeometricTransformation.NoGeometricTransformation)
            {

                var rootY = y - Constants.SearchHeight;

                if (rootY < 0)
                {
                    rootY = 0;
                }

                return rootY;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetRootY));
        }

        private static int GetRootX(int x, int geometricTransformation)
        {
            if (geometricTransformation == (int) Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return x;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == (int) Constants.GeometricTransformation.Identity
                || geometricTransformation == (int) Constants.GeometricTransformation.NoGeometricTransformation)
            {
                var rootX = x - Constants.SearchWidth;

                if (rootX < 0)
                {
                    rootX = 0;
                }

                return rootX;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetRootX));
        }

        private double GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, Dimension blockDimension, int geometricTransformation)
        {
            int sum = 0;

            for (int yy = 0; yy < blockDimension.Height; yy++)
            {
                for (int xx = 0; xx < blockDimension.Width; xx++)
                {
                    var matchedPointX = GetNextRootX(matchedPoint.X, xx, geometricTransformation);
                    var matchedPointY = GetNextRootY(matchedPoint.Y, yy, geometricTransformation);
                    sum += Convert.ToInt32(Math.Pow(WorkImage[encoderPoint.Y + yy, encoderPoint.X + xx] -
                                                    WorkImage[matchedPointY, matchedPointX], 2));
                }
            }

            return sum / (double) blockDimension.Height * blockDimension.Width;
        }

        private void InstatiateTables()
        {
            IsMatchFound = new bool[_height, _width];
            IsPixelEncoded = new bool[_height, _width];
            MatchLocation = new PixelLocation[_height, _width];
            MatchDimension = new Dimension[_height, _width];
            Residual = new int[_height, _width];
            WorkImage = new byte[_height, _width];
            GeometricTransformation = new int[_height, _width];
        }

        private void CopyOriginalImageWorkImage()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    WorkImage[y, x] = _originalImage[y, x];
                }
            }
        }
    }
}
