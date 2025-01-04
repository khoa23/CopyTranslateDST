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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            rtbBanDichTrong = new RichTextBox();
            btnMoBanDich = new Button();
            lbDuongDanBanDich = new Label();
            btnCutTranslate = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // btnOldTrans
            // 
            btnOldTrans.Location = new Point(31, 38);
            btnOldTrans.Name = "btnOldTrans";
            btnOldTrans.Size = new Size(247, 29);
            btnOldTrans.TabIndex = 0;
            btnOldTrans.Text = "Mở bản dịch cũ/Old translation";
            btnOldTrans.UseVisualStyleBackColor = true;
            btnOldTrans.Click += btnOldTrans_Click;
            // 
            // lbOldTrans
            // 
            lbOldTrans.AutoSize = true;
            lbOldTrans.Location = new Point(299, 47);
            lbOldTrans.Name = "lbOldTrans";
            lbOldTrans.Size = new Size(164, 20);
            lbOldTrans.TabIndex = 1;
            lbOldTrans.Text = "Đường dẫn bản dịch cũ";
            // 
            // btnNewTrans
            // 
            btnNewTrans.Location = new Point(31, 114);
            btnNewTrans.Name = "btnNewTrans";
            btnNewTrans.Size = new Size(247, 29);
            btnNewTrans.TabIndex = 2;
            btnNewTrans.Text = "Mở bản dịch mới/New translation";
            btnNewTrans.UseVisualStyleBackColor = true;
            btnNewTrans.Click += btnNewTrans_Click;
            // 
            // lbNewTrans
            // 
            lbNewTrans.AutoSize = true;
            lbNewTrans.Location = new Point(299, 114);
            lbNewTrans.Name = "lbNewTrans";
            lbNewTrans.Size = new Size(175, 20);
            lbNewTrans.TabIndex = 3;
            lbNewTrans.Text = "Đường dẫn bản dịch mới";
            // 
            // btnExecuteCopy
            // 
            btnExecuteCopy.Location = new Point(96, 246);
            btnExecuteCopy.Name = "btnExecuteCopy";
            btnExecuteCopy.Size = new Size(314, 65);
            btnExecuteCopy.TabIndex = 0;
            btnExecuteCopy.Text = "Compare And Copy";
            btnExecuteCopy.Click += btnExecuteCopy_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(33, 25);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1161, 506);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnOldTrans);
            tabPage1.Controls.Add(btnExecuteCopy);
            tabPage1.Controls.Add(lbOldTrans);
            tabPage1.Controls.Add(lbNewTrans);
            tabPage1.Controls.Add(btnNewTrans);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1153, 473);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Copy Translate";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnCutTranslate);
            tabPage2.Controls.Add(rtbBanDichTrong);
            tabPage2.Controls.Add(btnMoBanDich);
            tabPage2.Controls.Add(lbDuongDanBanDich);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1153, 473);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Bản dịch trống";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // rtbBanDichTrong
            // 
            rtbBanDichTrong.Location = new Point(21, 73);
            rtbBanDichTrong.Name = "rtbBanDichTrong";
            rtbBanDichTrong.Size = new Size(1105, 380);
            rtbBanDichTrong.TabIndex = 4;
            rtbBanDichTrong.Text = "";
            // 
            // btnMoBanDich
            // 
            btnMoBanDich.Location = new Point(21, 26);
            btnMoBanDich.Name = "btnMoBanDich";
            btnMoBanDich.Size = new Size(247, 29);
            btnMoBanDich.TabIndex = 2;
            btnMoBanDich.Text = "Mở bản dịch";
            btnMoBanDich.UseVisualStyleBackColor = true;
            btnMoBanDich.Click += btnMoBanDich_ClickAsync;
            // 
            // lbDuongDanBanDich
            // 
            lbDuongDanBanDich.AutoSize = true;
            lbDuongDanBanDich.Location = new Point(306, 30);
            lbDuongDanBanDich.Name = "lbDuongDanBanDich";
            lbDuongDanBanDich.Size = new Size(145, 20);
            lbDuongDanBanDich.TabIndex = 3;
            lbDuongDanBanDich.Text = "Đường dẫn bản dịch";
            // 
            // btnCutTranslate
            // 
            btnCutTranslate.Location = new Point(893, 26);
            btnCutTranslate.Name = "btnCutTranslate";
            btnCutTranslate.Size = new Size(224, 29);
            btnCutTranslate.TabIndex = 5;
            btnCutTranslate.Text = "Cắt đoạn chưa dịch xuống cuối";
            btnCutTranslate.UseVisualStyleBackColor = true;
            btnCutTranslate.Click += btnCutTranslate_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1245, 543);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Copy Translate Don't Starve Together";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnOldTrans;
        private Label lbOldTrans;
        private Button btnNewTrans;
        private Label lbNewTrans;
        private Button btnExecuteCopy;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button btnMoBanDich;
        private Label lbDuongDanBanDich;
        private RichTextBox rtbBanDichTrong;
        private Button btnCutTranslate;
    }
}