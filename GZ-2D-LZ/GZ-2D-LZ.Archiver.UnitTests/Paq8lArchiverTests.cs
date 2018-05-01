using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using GZ_2D_LZ.Archiver.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZ.Archiver.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class Paq8lArchiverTests
    {
        private static readonly string basePath = "\\TestData\\";
        public string TestBmpPath = Environment.CurrentDirectory + $"{basePath}test.bmp";

        private IArchiver _archiver;

        [TestInitialize]
        public void Setup()
        {
            _archiver = new Paq8lArchiver();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressWhenInputFolderPathIsNullThrowsArgumentNullException()
        {
            _archiver.Compress(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompressWhenInputFolderPathIsEmptyThrowsArgumentNullException()
        {
            _archiver.Compress(string.Empty);
        }

        [TestMethod]
        public void CompressASingleFileProducesAnArchiveWithTheSameNameAndPaq6Extension()
        {
            var archiveName = _archiver.Compress(TestBmpPath);

            Assert.IsTrue(File.Exists(archiveName));
        }

        [TestMethod]
        public void CompressADirectoryArchivesAllTheFilesInTheDirectory()
        {
            var archiveName = _archiver.Compress(Environment.CurrentDirectory + basePath);

            Assert.IsTrue(File.Exists(archiveName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DecompressWhenTheParameterIsNullThrowsArgumentNullException()
        {
            _archiver.Decompress(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DecompressWhenTheParameterIsEmptyThrowsArgumentNullException()
        {
            _archiver.Decompress(string.Empty);
        }

        [TestMethod]
        public void CompressAndDecompressASingleFileResultsTheSameFile()
        {
            var archiveName = _archiver.Compress(TestBmpPath);
            File.Delete(TestBmpPath);

            var fileName = _archiver.Decompress(archiveName);

            Assert.IsTrue(File.Exists(fileName));
        }

        [TestMethod]
        public void CompressAndDecompressADirectoryArchiveResultsTheSameFiles()
        {
            var inputFolderPath = Environment.CurrentDirectory + basePath;
            var files = Directory.GetFiles(inputFolderPath);
            var archiveName = _archiver.Compress(inputFolderPath);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            var outputFileName = _archiver.Decompress(archiveName);

            Assert.AreEqual(files.Length, Directory.GetFiles(outputFileName).Length);
            Assert.AreEqual(files.Length, files.Intersect(Directory.GetFiles(outputFileName)).Count());
        }
    }
}
