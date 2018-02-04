namespace GZ_2D_LZ
{
    public class Constants
    {
        public static int SearchWidth = 4;
        public static int SearchHeight = 4;
        public static int Threshold = 25;
        public static double MaxMse = 1.8;
        public static int MinMatchSize = 10; // 16 why on the paper this is bigger than search width * height?
        public static int NoMatchBlockWidth = 5;
    }
}
