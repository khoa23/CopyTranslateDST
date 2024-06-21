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
            SuspendLayout();
            // 
            // btnOldTrans
            // 
            btnOldTrans.Location = new Point(35, 28);
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
            lbOldTrans.Location = new Point(303, 37);
            lbOldTrans.Name = "lbOldTrans";
            lbOldTrans.Size = new Size(164, 20);
            lbOldTrans.TabIndex = 1;
            lbOldTrans.Text = "Đường dẫn bản dịch cũ";
            // 
            // btnNewTrans
            // 
            btnNewTrans.Location = new Point(35, 104);
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
            lbNewTrans.Location = new Point(303, 104);
            lbNewTrans.Name = "lbNewTrans";
            lbNewTrans.Size = new Size(175, 20);
            lbNewTrans.TabIndex = 3;
            lbNewTrans.Text = "Đường dẫn bản dịch mới";
            // 
            // btnExecuteCopy
            // 
            btnExecuteCopy.Location = new Point(430, 297);
            btnExecuteCopy.Name = "btnExecuteCopy";
            btnExecuteCopy.Size = new Size(314, 65);
            btnExecuteCopy.TabIndex = 0;
            btnExecuteCopy.Text = "Compare And Copy";
            btnExecuteCopy.Click += btnExecuteCopy_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1245, 543);
            Controls.Add(btnExecuteCopy);
            Controls.Add(lbNewTrans);
            Controls.Add(btnNewTrans);
            Controls.Add(lbOldTrans);
            Controls.Add(btnOldTrans);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Copy Translate Don't Starve Together";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOldTrans;
        private Label lbOldTrans;
        private Button btnNewTrans;
        private Label lbNewTrans;
        private Button btnExecuteCopy;
    }
}