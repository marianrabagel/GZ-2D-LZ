namespace G2_2D_LZ
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loadImageBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.OriginalImage = new System.Windows.Forms.Panel();
            this.EncodeBtn = new System.Windows.Forms.Button();
            this.DecodeBtn = new System.Windows.Forms.Button();
            this.LoadArchiveBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadImageBtn
            // 
            this.loadImageBtn.Location = new System.Drawing.Point(13, 13);
            this.loadImageBtn.Name = "loadImageBtn";
            this.loadImageBtn.Size = new System.Drawing.Size(75, 23);
            this.loadImageBtn.TabIndex = 0;
            this.loadImageBtn.Text = "Load image";
            this.loadImageBtn.UseVisualStyleBackColor = true;
            this.loadImageBtn.Click += new System.EventHandler(this.loadImageBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // OriginalImage
            // 
            this.OriginalImage.Location = new System.Drawing.Point(117, 13);
            this.OriginalImage.Name = "OriginalImage";
            this.OriginalImage.Size = new System.Drawing.Size(391, 338);
            this.OriginalImage.TabIndex = 1;
            // 
            // EncodeBtn
            // 
            this.EncodeBtn.Location = new System.Drawing.Point(13, 42);
            this.EncodeBtn.Name = "EncodeBtn";
            this.EncodeBtn.Size = new System.Drawing.Size(75, 23);
            this.EncodeBtn.TabIndex = 2;
            this.EncodeBtn.Text = "Encode";
            this.EncodeBtn.UseVisualStyleBackColor = true;
            this.EncodeBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // DecodeBtn
            // 
            this.DecodeBtn.Location = new System.Drawing.Point(12, 112);
            this.DecodeBtn.Name = "DecodeBtn";
            this.DecodeBtn.Size = new System.Drawing.Size(99, 23);
            this.DecodeBtn.TabIndex = 3;
            this.DecodeBtn.Text = "Decode";
            this.DecodeBtn.UseVisualStyleBackColor = true;
            this.DecodeBtn.Click += new System.EventHandler(this.DecodeBtn_Click);
            // 
            // LoadArchiveBtn
            // 
            this.LoadArchiveBtn.Location = new System.Drawing.Point(13, 83);
            this.LoadArchiveBtn.Name = "LoadArchiveBtn";
            this.LoadArchiveBtn.Size = new System.Drawing.Size(98, 23);
            this.LoadArchiveBtn.TabIndex = 4;
            this.LoadArchiveBtn.Text = "Load archive";
            this.LoadArchiveBtn.UseVisualStyleBackColor = true;
            this.LoadArchiveBtn.Click += new System.EventHandler(this.LoadArchiveBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 453);
            this.Controls.Add(this.LoadArchiveBtn);
            this.Controls.Add(this.DecodeBtn);
            this.Controls.Add(this.EncodeBtn);
            this.Controls.Add(this.OriginalImage);
            this.Controls.Add(this.loadImageBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button loadImageBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel OriginalImage;
        private System.Windows.Forms.Button EncodeBtn;
        private System.Windows.Forms.Button DecodeBtn;
        private System.Windows.Forms.Button LoadArchiveBtn;
    }
}

