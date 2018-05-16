namespace G2_2D_LZ.Helpers
{
    public class Constants
    {
        public static int SearchWidth = 4;
        public static int SearchHeight = 4;
        public static int Threshold = 27;
        public static int MinMatchSize = 17; // 16 why on the paper this is bigger than search width * height?
        public static double MaxMse = 2.5;
        public static int NoMatchBlockWidth = 5;
        public static int NoMatchBlockHeight = 5;

        public static int NumberOfBitsForSize = 16;
        public static int NumberOfBitsForX = 16;
        public static int NumberOfBitsForPredictionError = 16;

        public static string IntermediaryFileExtension = ".mat";
        public static string Folder = ".matrices";
        
        public static char Separator = ' ';
    }
}
