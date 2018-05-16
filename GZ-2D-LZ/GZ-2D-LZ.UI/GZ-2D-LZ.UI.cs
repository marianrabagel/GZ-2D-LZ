using System;
using System.IO;
using System.Windows.Forms;
using G2_2D_LZ;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Facades;
using G2_2D_LZ.Predictors;
using G2_2D_LZ.Readers;
using GZ_2D_LZ.Archiver;

namespace GZ_2D_LZ.UI
{
    public partial class Form1 : Form
    {
        private string inputFile;
        private string archiveFilePath;


        public Form1()
        {
            InitializeComponent();
        }

        private void loadImageBtn_Click(object sender, System.EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                inputFile = openFileDialog1.FileName;

                /*if (Path.GetExtension(inputFile) != ".bmp")
                {
                    MessageBox.Show("Selectati un fisier bmp");
                    return;
                }

                Bitmap bitmap = new Bitmap(inputFile);
                OriginalImage.BackgroundImage = bitmap;*/
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            IGz2DlzEncoderFacade _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
            _gz2DlzEncoderFacade.InputFilePath = inputFile;
            _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
            _gz2DlzEncoderFacade.ImageReader = new BmpImageReader();
            _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
            var _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
            _encoder.Encode();
        }

        private void LoadArchiveBtn_Click(object sender, System.EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                archiveFilePath = openFileDialog1.FileName;
            }
        }

        private void DecodeBtn_Click(object sender, System.EventArgs e)
        {
            var gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade = new Gz2DlzDecoderFacade();
            gz2DlzDecoderFacade.InputFilePath = archiveFilePath;
            gz2DlzDecoderFacade.AbstractPredictor = new ABasedPredictor();
            gz2DlzDecoderFacade.Archiver = new Paq6V2Archiver();
            var _decoder = new Gz2DlzDecoder(gz2DlzDecoderFacade);
            _decoder.Decode();
        }

        private void LoadFolderBtn_Click(object sender, System.EventArgs e)
        {
            var dialogResult = folderBrowserDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                var folerPath = folderBrowserDialog1.SelectedPath;
                var allFilesInFolder = Directory.GetFiles(folerPath);
                var content = "name | uncompressed size | compressed size | compression ratio | bits per pixel" + Environment.NewLine;

                foreach (var filePath in allFilesInFolder)
                {
                    string name = Path.GetFileName(filePath);
                    var uncompressedSize = new FileInfo(filePath).Length;

                    IGz2DlzEncoderFacade _gz2DlzEncoderFacade = new Gz2DlzEncoderFacade();
                    _gz2DlzEncoderFacade.InputFilePath = filePath;
                    _gz2DlzEncoderFacade.AbstractPredictor = new ABasedPredictor();
                    _gz2DlzEncoderFacade.ImageReader = new BmpImageReader();
                    _gz2DlzEncoderFacade.Archiver = new Paq6V2Archiver();
                    var _encoder = new Gz2DlzEncoder(_gz2DlzEncoderFacade);
                    _encoder.Encode();
                    var archivePath = _encoder.ArchivePath;
                    var compressedSize = new FileInfo(archivePath).Length;

                    content += $"{name} | {uncompressedSize} | {compressedSize} | " +
                               $" {uncompressedSize/(decimal)compressedSize} " +
                               $" | {8*(compressedSize / (decimal)(512 * 512))}  " +
                               $" {Environment.NewLine}";
                }

                using (var streamWriter = new StreamWriter(folerPath + "\\results.txt"))
                {
                    streamWriter.Write(content);
                }
            }
        }
    }
}
