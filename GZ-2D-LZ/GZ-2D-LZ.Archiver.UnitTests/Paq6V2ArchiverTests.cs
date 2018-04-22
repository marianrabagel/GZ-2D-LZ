using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using GZ_2D_LZ.Archiver.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZ_2D_LZ.Archiver.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class Paq6V2ArchiverTests
    {
        private static readonly string basePath = "\\TestData\\";
        public string TestBmpPath = Environment.CurrentDirectory + $"{basePath}test.bmp";

        private IArchiver _archiver;

        [TestInitialize]
        public void Setup()
        {
            _archiver = new Paq6V2Archiver();
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
        public void Compress()
        {
            var archive = _archiver.Compress(TestBmpPath);

            Assert.IsTrue(File.Exists(archive));
        }

    }
}
