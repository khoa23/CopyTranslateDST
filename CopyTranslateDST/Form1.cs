namespace CopyTranslateDST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string fileOldTransPath;
        string fileNewTransPath;
        private void btnOldTrans_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Title = "Chọn file dịch cũ";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.FileName;
                lbOldTrans.Text = sSelectedPath;
                fileOldTransPath = sSelectedPath;
            }
        }

        private void btnNewTrans_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Title = "Chọn file dịch mới";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.FileName;
                lbNewTrans.Text = sSelectedPath;
                fileNewTransPath = sSelectedPath;
            }
        }
    }
}