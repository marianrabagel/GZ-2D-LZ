using System;

namespace G2_2D_LZ.Helpers
{
    public class Constants
    {
        public static int SearchWidth = 4;
        public static int SearchHeight = 4;
        public static int Threshold = 27;
        public static int MinMatchSize = 16; 
        public static double MaxMse = 1.8;
        public static int NoMatchBlockWidth = 5;
        public static int NoMatchBlockHeight = 5;

        public static int NumberOfBitsForSize = 16;
        public static int NumberOfBitsForX = 16;
        public static int NumberOfBitsForPredictionError = 16;

        public static string IntermediaryFileExtension = ".mat";
        public static string Folder = ".matrices";
        
        public static char Separator = ' ';
        public enum GeometricTransformation
        {
            Identity = 0,
            VerticalMirror,
            HorizontalMirror,
            FirstDiagonalMirror,
            NoGeometricTransformation
        }
    }
}
