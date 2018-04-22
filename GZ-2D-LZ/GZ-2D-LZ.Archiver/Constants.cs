using System;

namespace GZ_2D_LZ.Archiver
{
    public class Constants
    {
        public static string Paq6Extension = ".paq6";

        private static readonly string BasePath = "\\Paq\\paq6\\";
        public static string Paq6ExeFileLocation = Environment.CurrentDirectory + $"{BasePath}paq6v2.exe";
    }
}
