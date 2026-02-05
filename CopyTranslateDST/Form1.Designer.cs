namespace CopyTranslateDST
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnOldTrans = new Button();
            lbOldTrans = new Label();
            btnNewTrans = new Button();
            lbNewTrans = new Label();
            btnExecuteCopy = new Button();
            tabPageDichTuConTrong = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            btnCutTranslate = new Button();
            rtbBanDichTrong = new RichTextBox();
            btnMoBanDich = new Button();
            lbDuongDanBanDich = new Label();
            btnGoogleTranslate = new Button();
            btnLuuBanDich = new Button();
            numMaxTrans = new NumericUpDown();
            lbMaxTrans = new Label();
            tabPage3 = new TabPage();
            btnChonFilePoDichTuConTrong = new Button();
            lbDuongDanFilePoDichTuConTrong = new Label();
            rtbLog = new RichTextBox();
            tabPageDichTuConTrong.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // btnOldTrans
            // 
            btnOldTrans.Location = new Point(27, 28);
            btnOldTrans.Margin = new Padding(3, 2, 3, 2);
            btnOldTrans.Name = "btnOldTrans";
            btnOldTrans.Size = new Size(216, 22);
            btnOldTrans.TabIndex = 0;
            btnOldTrans.Text = "Mở bản dịch cũ/Old translation";
            btnOldTrans.UseVisualStyleBackColor = true;
            btnOldTrans.Click += btnOldTrans_Click;
            // 
            // lbOldTrans
            // 
            lbOldTrans.AutoSize = true;
            lbOldTrans.Location = new Point(262, 35);
            lbOldTrans.Name = "lbOldTrans";
            lbOldTrans.Size = new Size(131, 15);
            lbOldTrans.TabIndex = 1;
            lbOldTrans.Text = "Đường dẫn bản dịch cũ";
            // 
            // btnNewTrans
            // 
            btnNewTrans.Location = new Point(27, 86);
            btnNewTrans.Margin = new Padding(3, 2, 3, 2);
            btnNewTrans.Name = "btnNewTrans";
            btnNewTrans.Size = new Size(216, 22);
            btnNewTrans.TabIndex = 2;
            btnNewTrans.Text = "Mở bản dịch mới/New translation";
            btnNewTrans.UseVisualStyleBackColor = true;
            btnNewTrans.Click += btnNewTrans_Click;
            // 
            // lbNewTrans
            // 
            lbNewTrans.AutoSize = true;
            lbNewTrans.Location = new Point(262, 86);
            lbNewTrans.Name = "lbNewTrans";
            lbNewTrans.Size = new Size(139, 15);
            lbNewTrans.TabIndex = 3;
            lbNewTrans.Text = "Đường dẫn bản dịch mới";
            // 
            // btnExecuteCopy
            // 
            btnExecuteCopy.Location = new Point(84, 184);
            btnExecuteCopy.Margin = new Padding(3, 2, 3, 2);
            btnExecuteCopy.Name = "btnExecuteCopy";
            btnExecuteCopy.Size = new Size(275, 49);
            btnExecuteCopy.TabIndex = 0;
            btnExecuteCopy.Text = "Compare And Copy";
            btnExecuteCopy.Click += btnExecuteCopy_Click;
            // 
            // tabPageDichTuConTrong
            // 
            tabPageDichTuConTrong.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabPageDichTuConTrong.Controls.Add(tabPage1);
            tabPageDichTuConTrong.Controls.Add(tabPage2);
            tabPageDichTuConTrong.Controls.Add(tabPage3);
            tabPageDichTuConTrong.Location = new Point(12, 12);
            tabPageDichTuConTrong.Margin = new Padding(3, 2, 3, 2);
            tabPageDichTuConTrong.Name = "tabPageDichTuConTrong";
            tabPageDichTuConTrong.SelectedIndex = 0;
            tabPageDichTuConTrong.Size = new Size(1160, 580);
            tabPageDichTuConTrong.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnOldTrans);
            tabPage1.Controls.Add(btnExecuteCopy);
            tabPage1.Controls.Add(lbOldTrans);
            tabPage1.Controls.Add(lbNewTrans);
            tabPage1.Controls.Add(btnNewTrans);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Margin = new Padding(3, 2, 3, 2);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3, 2, 3, 2);
            tabPage1.Size = new Size(1008, 352);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Copy Translate";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(rtbLog);
            tabPage2.Controls.Add(lbMaxTrans);
            tabPage2.Controls.Add(numMaxTrans);
            tabPage2.Controls.Add(btnLuuBanDich);
            tabPage2.Controls.Add(btnGoogleTranslate);
            tabPage2.Controls.Add(btnCutTranslate);
            tabPage2.Controls.Add(rtbBanDichTrong);
            tabPage2.Controls.Add(btnMoBanDich);
            tabPage2.Controls.Add(lbDuongDanBanDich);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Margin = new Padding(3, 2, 3, 2);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3, 2, 3, 2);
            tabPage2.Size = new Size(1152, 552);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Bản dịch trống";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // rtbLog
            // 
            rtbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            rtbLog.BackColor = Color.Black;
            rtbLog.ForeColor = Color.Lime;
            rtbLog.Location = new Point(781, 52);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(355, 482);
            rtbLog.TabIndex = 10;
            rtbLog.Text = "";
            // 
            // numMaxTrans
            // 
            numMaxTrans.Location = new Point(620, 52);
            numMaxTrans.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numMaxTrans.Name = "numMaxTrans";
            numMaxTrans.Size = new Size(80, 23);
            numMaxTrans.TabIndex = 8;
            numMaxTrans.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // lbMaxTrans
            // 
            lbMaxTrans.AutoSize = true;
            lbMaxTrans.Location = new Point(540, 55);
            lbMaxTrans.Name = "lbMaxTrans";
            lbMaxTrans.Size = new Size(74, 15);
            lbMaxTrans.TabIndex = 9;
            lbMaxTrans.Text = "Số câu tối đa:";
            // 
            // btnLuuBanDich
            // 
            btnLuuBanDich.Location = new Point(380, 20);
            btnLuuBanDich.Margin = new Padding(3, 2, 3, 2);
            btnLuuBanDich.Name = "btnLuuBanDich";
            btnLuuBanDich.Size = new Size(150, 22);
            btnLuuBanDich.TabIndex = 7;
            btnLuuBanDich.Text = "Lưu bản dịch vào file";
            btnLuuBanDich.UseVisualStyleBackColor = true;
            btnLuuBanDich.Click += btnLuuBanDich_Click;
            // 
            // btnGoogleTranslate
            // 
            btnGoogleTranslate.Location = new Point(550, 20);
            btnGoogleTranslate.Margin = new Padding(3, 2, 3, 2);
            btnGoogleTranslate.Name = "btnGoogleTranslate";
            btnGoogleTranslate.Size = new Size(196, 22);
            btnGoogleTranslate.TabIndex = 6;
            btnGoogleTranslate.Text = "Dịch tự động (Google)";
            btnGoogleTranslate.UseVisualStyleBackColor = true;
            btnGoogleTranslate.Click += btnGoogleTranslate_Click;
            // 
            // btnCutTranslate
            // 
            btnCutTranslate.Location = new Point(781, 20);
            btnCutTranslate.Margin = new Padding(3, 2, 3, 2);
            btnCutTranslate.Name = "btnCutTranslate";
            btnCutTranslate.Size = new Size(196, 22);
            btnCutTranslate.TabIndex = 5;
            btnCutTranslate.Text = "Cắt đoạn chưa dịch xuống cuối";
            btnCutTranslate.UseVisualStyleBackColor = true;
            btnCutTranslate.Click += btnCutTranslate_Click;
            // 
            // rtbBanDichTrong
            // 
            rtbBanDichTrong.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbBanDichTrong.Location = new Point(18, 85);
            rtbBanDichTrong.Margin = new Padding(3, 2, 3, 2);
            rtbBanDichTrong.Name = "rtbBanDichTrong";
            rtbBanDichTrong.Size = new Size(745, 449);
            rtbBanDichTrong.TabIndex = 4;
            rtbBanDichTrong.Text = "";
            // 
            // btnMoBanDich
            // 
            btnMoBanDich.Location = new Point(18, 20);
            btnMoBanDich.Margin = new Padding(3, 2, 3, 2);
            btnMoBanDich.Name = "btnMoBanDich";
            btnMoBanDich.Size = new Size(216, 22);
            btnMoBanDich.TabIndex = 2;
            btnMoBanDich.Text = "Mở bản dịch";
            btnMoBanDich.UseVisualStyleBackColor = true;
            btnMoBanDich.Click += btnMoBanDich_ClickAsync;
            // 
            // lbDuongDanBanDich
            // 
            lbDuongDanBanDich.AutoSize = true;
            lbDuongDanBanDich.Location = new Point(268, 22);
            lbDuongDanBanDich.Name = "lbDuongDanBanDich";
            lbDuongDanBanDich.Size = new Size(115, 15);
            lbDuongDanBanDich.TabIndex = 3;
            lbDuongDanBanDich.Text = "Đường dẫn bản dịch";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(lbDuongDanFilePoDichTuConTrong);
            tabPage3.Controls.Add(btnChonFilePoDichTuConTrong);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1008, 352);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Dịch các từ còn trống";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnChonFilePoDichTuConTrong
            // 
            btnChonFilePoDichTuConTrong.Location = new Point(44, 34);
            btnChonFilePoDichTuConTrong.Margin = new Padding(3, 2, 3, 2);
            btnChonFilePoDichTuConTrong.Name = "btnChonFilePoDichTuConTrong";
            btnChonFilePoDichTuConTrong.Size = new Size(216, 22);
            btnChonFilePoDichTuConTrong.TabIndex = 3;
            btnChonFilePoDichTuConTrong.Text = "Mở bản dịch";
            btnChonFilePoDichTuConTrong.UseVisualStyleBackColor = true;
            // 
            // lbDuongDanFilePoDichTuConTrong
            // 
            lbDuongDanFilePoDichTuConTrong.AutoSize = true;
            lbDuongDanFilePoDichTuConTrong.Location = new Point(319, 38);
            lbDuongDanFilePoDichTuConTrong.Name = "lbDuongDanFilePoDichTuConTrong";
            lbDuongDanFilePoDichTuConTrong.Size = new Size(115, 15);
            lbDuongDanFilePoDichTuConTrong.TabIndex = 4;
            lbDuongDanFilePoDichTuConTrong.Text = "Đường dẫn bản dịch";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 611);
            Controls.Add(tabPageDichTuConTrong);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(800, 450);
            Name = "Form1";
            Text = "Copy Translate Don't Starve Together";
            tabPageDichTuConTrong.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnOldTrans;
        private Label lbOldTrans;
        private Button btnNewTrans;
        private Label lbNewTrans;
        private Button btnExecuteCopy;
        private TabControl tabPageDichTuConTrong;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button btnMoBanDich;
        private Label lbDuongDanBanDich;
        private RichTextBox rtbBanDichTrong;
        private Button btnCutTranslate;
        private TabPage tabPage3;
        private Label lbDuongDanFilePoDichTuConTrong;
        private Button btnChonFilePoDichTuConTrong;
        private Button btnGoogleTranslate;
        private Button btnLuuBanDich;
        private NumericUpDown numMaxTrans;
        private Label lbMaxTrans;
        private RichTextBox rtbLog;
    }
}