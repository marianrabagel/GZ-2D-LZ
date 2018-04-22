using System;
using System.Diagnostics;
using GZ_2D_LZ.Archiver.Contracts;

namespace GZ_2D_LZ.Archiver
{
    public class Paq6V2Archiver : IArchiver
    {
        public string Compress(string inputFolderPath, string outputName = null)
        {
            if (string.IsNullOrEmpty(inputFolderPath))
            {
                throw new ArgumentNullException(nameof(inputFolderPath));
            }

            if (string.IsNullOrEmpty(outputName))
            {
                outputName = inputFolderPath + Constants.Paq6Extension;
            }

            string arguments = "-5 " + outputName + " " + inputFolderPath;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Constants.Paq6ExeFileLocation;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            return outputName;
        }

        public void Decompress(string archivePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
