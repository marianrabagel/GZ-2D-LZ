namespace G2_2D_LZ.Helpers
{
    public class Constants
    {
        public static int SearchWidth = 4;
        public static int SearchHeight = 4;
        public static int Threshold = 25;
        public static double MaxMse = 1.8;
        public static int MinMatchSize = 10; // 16 why on the paper this is bigger than search width * height?
        public static int NoMatchBlockWidth = 5;
        public static int NoMatchBlockHeight = 5;
    }
}
