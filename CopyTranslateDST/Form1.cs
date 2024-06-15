using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CopyTranslateDST
{
    public partial class Form1 : Form
    {
        private string fileOldTransPath;
        private string fileNewTransPath;

        public Form1()
        {
            InitializeComponent();
        }

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

        private void btnExecuteCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileOldTransPath) || string.IsNullOrEmpty(fileNewTransPath))
            {
                MessageBox.Show("Vui lòng chọn file dịch cũ và mới trước khi thực thi sao chép.");
                return;
            }

            try
            {
                // Đọc và tạo map bản dịch từ file PO cũ
                Dictionary<string, string> translationMap = CreateTranslationMap(fileOldTransPath);

                // Sao chép bản dịch từ file PO cũ sang file PO mới
                CopyTranslations(fileNewTransPath, translationMap);

                MessageBox.Show("Sao chép bản dịch thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private Dictionary<string, string> CreateTranslationMap(string filePath)
        {
            Dictionary<string, string> translationMap = new Dictionary<string, string>();

            // Đọc từng dòng trong file PO cũ
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            string msgctxt = null;
            string msgid = null;
            string msgstr = null;

            foreach (string line in lines)
            {
                if (line.StartsWith("#. "))
                {
                    // Xử lý dòng msgctxt hoặc msgid
                    msgctxt = GetValueFromLine(line);
                }
                else if (line.StartsWith("msgctxt"))
                {
                    msgctxt = GetValueFromLine(line);
                }
                else if (line.StartsWith("msgid"))
                {
                    msgid = GetValueFromLine(line);
                }
                else if (line.StartsWith("msgstr"))
                {
                    msgstr = GetValueFromLine(line);

                    // Nếu đã có msgctxt và msgid, thêm vào map bản dịch
                    if (!string.IsNullOrEmpty(msgctxt) && !string.IsNullOrEmpty(msgid))
                    {
                        translationMap[msgctxt + "|" + msgid] = msgstr;
                        msgctxt = null;
                        msgid = null;
                        msgstr = null;
                    }
                }
            }

            return translationMap;
        }

        private void CopyTranslations(string filePath, Dictionary<string, string> translationMap)
        {
            // Đọc từng dòng trong file PO mới
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            List<string> outputLines = new List<string>();
            string msgctxt = null;
            string msgid = null;

            foreach (string originalLine in lines)
            {
                string line = originalLine; // Tạo một bản sao của originalLine để có thể sửa đổi

                if (line.StartsWith("#. "))
                {
                    // Xử lý dòng msgctxt hoặc msgid
                    msgctxt = GetValueFromLine(line);
                }
                else if (line.StartsWith("msgctxt"))
                {
                    msgctxt = GetValueFromLine(line);
                }
                else if (line.StartsWith("msgid"))
                {
                    msgid = GetValueFromLine(line);
                }
                else if (line.StartsWith("msgstr"))
                {
                    // Kiểm tra nếu có bản dịch tương ứng trong translationMap và chưa có bản dịch trong PO mới
                    if (!string.IsNullOrEmpty(msgctxt) && !string.IsNullOrEmpty(msgid))
                    {
                        string key = msgctxt + "|" + msgid;
                        if (translationMap.ContainsKey(key))
                        {
                            string translatedMsgstr = translationMap[key];
                            if (string.IsNullOrEmpty(GetValueFromLine(line)))
                            {
                                line = $"msgstr \"{translatedMsgstr}\"";
                            }
                        }
                    }
                    msgctxt = null;
                    msgid = null;
                }

                outputLines.Add(line); // Thêm dòng đã sửa đổi hoặc không sửa vào list outputLines
            }

            // Ghi lại file PO mới
            File.WriteAllLines(filePath, outputLines, Encoding.UTF8);
        }



        private string GetValueFromLine(string line)
        {
            int startIndex = line.IndexOf('"');
            int endIndex = line.LastIndexOf('"');

            // Kiểm tra nếu không tìm thấy cặp dấu "
            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            {
                return ""; // Hoặc có thể trả về null, tùy thuộc vào yêu cầu của bạn
            }

            // Lấy giá trị nằm giữa các dấu "
            return line.Substring(startIndex + 1, endIndex - startIndex - 1);
        }
    }
}
