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
            tabPageEval = new TabPage();
            rtbLogEval = new RichTextBox();
            dgvEval = new DataGridView();
            lbLimitEval = new Label();
            numLimitEval = new NumericUpDown();
            lbAnythingApiKey = new Label();
            txtAnythingApiKey = new TextBox();
            lbAnythingIp = new Label();
            txtAnythingIp = new TextBox();
            lbDuongDanBanDichEval = new Label();
            btnMoBanDichEval = new Button();
            btnEval = new Button();
            tabPageDichTuConTrong.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPageEval.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLimitEval).BeginInit();
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
            tabPageDichTuConTrong.Controls.Add(tabPageEval);
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
            tabPage3.Size = new Size(1152, 552);
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
            // tabPageEval
            // 
            tabPageEval.Controls.Add(btnEval);
            tabPageEval.Controls.Add(btnMoBanDichEval);
            tabPageEval.Controls.Add(lbDuongDanBanDichEval);
            tabPageEval.Controls.Add(txtAnythingIp);
            tabPageEval.Controls.Add(lbAnythingIp);
            tabPageEval.Controls.Add(txtAnythingApiKey);
            tabPageEval.Controls.Add(rtbLogEval);
            tabPageEval.Controls.Add(lbAnythingApiKey);
            tabPageEval.Controls.Add(numLimitEval);
            tabPageEval.Controls.Add(lbLimitEval);
            tabPageEval.Controls.Add(dgvEval);
            tabPageEval.Location = new Point(4, 24);
            tabPageEval.Name = "tabPageEval";
            tabPageEval.Padding = new Padding(3);
            tabPageEval.Size = new Size(1152, 552);
            tabPageEval.TabIndex = 3;
            tabPageEval.Text = "Đánh giá & Dịch lại";
            tabPageEval.UseVisualStyleBackColor = true;
            // 
            // rtbLogEval
            // 
            rtbLogEval.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            rtbLogEval.BackColor = Color.Black;
            rtbLogEval.ForeColor = Color.Lime;
            rtbLogEval.Location = new Point(781, 85);
            rtbLogEval.Name = "rtbLogEval";
            rtbLogEval.ReadOnly = true;
            rtbLogEval.Size = new Size(355, 449);
            rtbLogEval.TabIndex = 8;
            rtbLogEval.Text = "";
            // 
            // dgvEval
            // 
            dgvEval.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvEval.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEval.Location = new Point(18, 85);
            dgvEval.Name = "dgvEval";
            dgvEval.Size = new Size(745, 449);
            dgvEval.TabIndex = 7;
            // 
            // lbAnythingIp
            // 
            lbAnythingIp.AutoSize = true;
            lbAnythingIp.Location = new Point(18, 55);
            lbAnythingIp.Name = "lbAnythingIp";
            lbAnythingIp.Size = new Size(71, 15);
            lbAnythingIp.TabIndex = 12;
            lbAnythingIp.Text = "Anything IP";
            // 
            // txtAnythingIp
            // 
            txtAnythingIp.Location = new Point(95, 52);
            txtAnythingIp.Name = "txtAnythingIp";
            txtAnythingIp.Size = new Size(180, 23);
            txtAnythingIp.TabIndex = 13;
            txtAnythingIp.Text = "http://localhost:3001";
            // 
            // lbAnythingApiKey
            // 
            lbAnythingApiKey.AutoSize = true;
            lbAnythingApiKey.Location = new Point(290, 55);
            lbAnythingApiKey.Name = "lbAnythingApiKey";
            lbAnythingApiKey.Size = new Size(99, 15);
            lbAnythingApiKey.TabIndex = 4;
            lbAnythingApiKey.Text = "Anything API Key";
            // 
            // txtAnythingApiKey
            // 
            txtAnythingApiKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAnythingApiKey.Location = new Point(395, 52);
            txtAnythingApiKey.Name = "txtAnythingApiKey";
            txtAnythingApiKey.Size = new Size(205, 23);
            txtAnythingApiKey.TabIndex = 3;
            // 
            // lbLimitEval
            // 
            lbLimitEval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbLimitEval.AutoSize = true;
            lbLimitEval.Location = new Point(620, 55);
            lbLimitEval.Name = "lbLimitEval";
            lbLimitEval.Size = new Size(65, 15);
            lbLimitEval.TabIndex = 6;
            lbLimitEval.Text = "Limit rows:";
            // 
            // numLimitEval
            // 
            numLimitEval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numLimitEval.Location = new Point(690, 52);
            numLimitEval.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numLimitEval.Name = "numLimitEval";
            numLimitEval.Size = new Size(60, 23);
            numLimitEval.TabIndex = 5;
            numLimitEval.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lbDuongDanBanDichEval
            // 
            lbDuongDanBanDichEval.AutoSize = true;
            lbDuongDanBanDichEval.Location = new Point(268, 22);
            lbDuongDanBanDichEval.Name = "lbDuongDanBanDichEval";
            lbDuongDanBanDichEval.Size = new Size(85, 15);
            lbDuongDanBanDichEval.TabIndex = 11;
            lbDuongDanBanDichEval.Text = "Đường dẫn file";
            // 
            // btnMoBanDichEval
            // 
            btnMoBanDichEval.Location = new Point(18, 18);
            btnMoBanDichEval.Margin = new Padding(3, 2, 3, 2);
            btnMoBanDichEval.Name = "btnMoBanDichEval";
            btnMoBanDichEval.Size = new Size(216, 22);
            btnMoBanDichEval.TabIndex = 10;
            btnMoBanDichEval.Text = "Mở file Đánh giá";
            btnMoBanDichEval.UseVisualStyleBackColor = true;
            btnMoBanDichEval.Click += btnMoBanDichEval_Click;
            // 
            // btnEval
            // 
            btnEval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEval.Location = new Point(781, 18);
            btnEval.Margin = new Padding(3, 2, 3, 2);
            btnEval.Name = "btnEval";
            btnEval.Size = new Size(196, 22);
            btnEval.TabIndex = 0;
            btnEval.Text = "Đánh giá & Dịch";
            btnEval.UseVisualStyleBackColor = true;
            btnEval.Click += btnEval_Click;
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
            tabPageEval.ResumeLayout(false);
            tabPageEval.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEval).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLimitEval).EndInit();
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
        private TabPage tabPageEval;
        private Button btnEval;
        private Button btnMoBanDichEval;
        private Label lbDuongDanBanDichEval;
        private TextBox txtAnythingApiKey;
        private RichTextBox rtbLogEval;
        private Label lbAnythingApiKey;
        private NumericUpDown numLimitEval;
        private Label lbLimitEval;
        private DataGridView dgvEval;
        private Label lbAnythingIp;
        private TextBox txtAnythingIp;
    }
}