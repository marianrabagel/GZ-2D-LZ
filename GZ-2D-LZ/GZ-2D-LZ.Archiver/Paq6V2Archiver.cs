﻿using System;
using System.Diagnostics;
using System.IO;
using GZ_2D_LZ.Archiver.Contracts;

namespace GZ_2D_LZ.Archiver
{
    public class Paq6V2Archiver : IArchiver
    {
        public string Compress(string inputFolderPath, string outputName = null, int compresionRate = 3)
        {
            if (string.IsNullOrEmpty(inputFolderPath))
            {
                throw new ArgumentNullException(nameof(inputFolderPath));
            }

            if (string.IsNullOrEmpty(outputName))
            {
                outputName = inputFolderPath + Constants.Paq6Extension;
            }

            if (File.GetAttributes(inputFolderPath) == FileAttributes.Directory)
            {
                outputName = outputName.Replace("\\" + Constants.Paq6Extension, Constants.Paq6Extension);
                var strings = Directory.GetFiles(inputFolderPath);
                inputFolderPath = "";

                foreach (var file in strings)
                {
                    inputFolderPath += file + " ";
                }
            }

            string arguments = "-" + compresionRate + " " + outputName + " " + inputFolderPath;
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

        public string Decompress(string archivePath)
        {
            if (string.IsNullOrEmpty(archivePath))
            {
                throw new ArgumentNullException(nameof(archivePath));
            }

            string arguments = archivePath;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Constants.Paq6ExeFileLocation;
            startInfo.Arguments = arguments;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }

            return archivePath.Replace(Constants.Paq6Extension, "");
        }
    }
}
