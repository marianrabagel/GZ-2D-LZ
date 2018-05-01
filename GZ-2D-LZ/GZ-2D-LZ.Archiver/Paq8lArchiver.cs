using System;
using System.Diagnostics;
using System.IO;
using GZ_2D_LZ.Archiver.Contracts;

namespace GZ_2D_LZ.Archiver
{
    public class Paq8lArchiver : IArchiver
    {
        public string Compress(string inputFolderPath, string outputName = null, int compresionRate = 5)
        {
            if (string.IsNullOrEmpty(inputFolderPath))
            {
                throw new ArgumentNullException(nameof(inputFolderPath));
            }

            if (string.IsNullOrEmpty(outputName))
            {
                outputName = inputFolderPath;
            }

            if (File.GetAttributes(inputFolderPath) == FileAttributes.Directory)
            {
                outputName = outputName.Substring(0, outputName.Length - 1);
            }

            string arguments = "-" + compresionRate + " " + outputName + " " + inputFolderPath;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Constants.Paq8lExeFileLocation;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            return outputName + Constants.Paq8lExtension;
        }

        public string Decompress(string archivePath)
        {
            if (string.IsNullOrEmpty(archivePath))
            {
                throw new ArgumentNullException(nameof(archivePath));
            }

            string arguments = "-d " + archivePath;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Constants.Paq8lExeFileLocation;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            return archivePath.Replace(Constants.Paq8lExtension, "");
        }
    }
}
