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
            this.btnOldTrans = new System.Windows.Forms.Button();
            this.lbOldTrans = new System.Windows.Forms.Label();
            this.btnNewTrans = new System.Windows.Forms.Button();
            this.lbNewTrans = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOldTrans
            // 
            this.btnOldTrans.Location = new System.Drawing.Point(35, 28);
            this.btnOldTrans.Name = "btnOldTrans";
            this.btnOldTrans.Size = new System.Drawing.Size(164, 29);
            this.btnOldTrans.TabIndex = 0;
            this.btnOldTrans.Text = "Mở bản dịch cũ";
            this.btnOldTrans.UseVisualStyleBackColor = true;
            this.btnOldTrans.Click += new System.EventHandler(this.btnOldTrans_Click);
            // 
            // lbOldTrans
            // 
            this.lbOldTrans.AutoSize = true;
            this.lbOldTrans.Location = new System.Drawing.Point(232, 37);
            this.lbOldTrans.Name = "lbOldTrans";
            this.lbOldTrans.Size = new System.Drawing.Size(164, 20);
            this.lbOldTrans.TabIndex = 1;
            this.lbOldTrans.Text = "Đường dẫn bản dịch cũ";
            // 
            // btnNewTrans
            // 
            this.btnNewTrans.Location = new System.Drawing.Point(35, 104);
            this.btnNewTrans.Name = "btnNewTrans";
            this.btnNewTrans.Size = new System.Drawing.Size(164, 29);
            this.btnNewTrans.TabIndex = 2;
            this.btnNewTrans.Text = "Mở bản dịch mới";
            this.btnNewTrans.UseVisualStyleBackColor = true;
            this.btnNewTrans.Click += new System.EventHandler(this.btnNewTrans_Click);
            // 
            // lbNewTrans
            // 
            this.lbNewTrans.AutoSize = true;
            this.lbNewTrans.Location = new System.Drawing.Point(232, 113);
            this.lbNewTrans.Name = "lbNewTrans";
            this.lbNewTrans.Size = new System.Drawing.Size(175, 20);
            this.lbNewTrans.TabIndex = 3;
            this.lbNewTrans.Text = "Đường dẫn bản dịch mới";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 543);
            this.Controls.Add(this.lbNewTrans);
            this.Controls.Add(this.btnNewTrans);
            this.Controls.Add(this.lbOldTrans);
            this.Controls.Add(this.btnOldTrans);
            this.Name = "Form1";
            this.Text = "Copy Translate Don\'t Starve Together";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnOldTrans;
        private Label lbOldTrans;
        private Button btnNewTrans;
        private Label lbNewTrans;
    }
}