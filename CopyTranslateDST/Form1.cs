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

        private async void btnMoBanDich_ClickAsync(object sender, EventArgs e)
        {
            // Tạo một hộp thoại mở file
            OpenFileDialog fbd = new OpenFileDialog
            {
                Title = "Chọn file dịch",
                Filter = "PO Files (*.po)|*.po|All Files (*.*)|*.*" // Điều chỉnh filter tùy theo loại file
            };

            // Hiển thị hộp thoại và kiểm tra nếu người dùng chọn OK
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string sSelectedPath = fbd.FileName;
                lbDuongDanBanDich.Text = sSelectedPath;

                try
                {
                    // Cập nhật UI để thông báo đang xử lý
                    btnMoBanDich.Enabled = false;
                    btnMoBanDich.Text = "Đang xử lý...";
                    rtbBanDichTrong.Clear();

                    // Thực hiện xử lý file trên luồng nền
                    var entriesWithEmptyMsgStr = await Task.Run(() => ProcessPoFile(sSelectedPath));

                    // Hiển thị kết quả lên RichTextBox trên luồng UI
                    if (entriesWithEmptyMsgStr.Count > 0)
                    {
                        StringBuilder displayText = new StringBuilder();
                        foreach (var entry in entriesWithEmptyMsgStr)
                        {
                            displayText.AppendLine(entry);
                            displayText.AppendLine(); // Thêm dòng trống giữa các mục
                        }
                        rtbBanDichTrong.Text = displayText.ToString();
                    }
                    else
                    {
                        rtbBanDichTrong.Text = "Không tìm thấy mục nào có msgstr \"\" hoàn toàn.";
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    MessageBox.Show("Đã xảy ra lỗi khi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Khôi phục trạng thái ban đầu của nút
                    btnMoBanDich.Enabled = true;
                    btnMoBanDich.Text = "Mở File Dịch";
                }
            }
        }

        private List<string> ProcessPoFile(string filePath)
        {
            var entriesWithEmptyMsgStr = new List<string>();
            var currentEntryLines = new List<string>();
            bool msgstrFound = false;
            bool msgstrEmpty = false;
            bool hasTranslation = false;

            // Đọc tất cả các dòng trong file
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    // Khi gặp dòng trống, kiểm tra nếu mục hiện tại hợp lệ
                    if (msgstrFound && msgstrEmpty && !hasTranslation)
                    {
                        entriesWithEmptyMsgStr.Add(string.Join(Environment.NewLine, currentEntryLines).Trim());
                    }
                    // Reset cho mục tiếp theo
                    currentEntryLines.Clear();
                    msgstrFound = false;
                    msgstrEmpty = false;
                    hasTranslation = false;
                }
                else
                {
                    currentEntryLines.Add(line);

                    if (line.Trim().StartsWith("msgstr"))
                    {
                        msgstrFound = true;
                        // Kiểm tra xem msgstr có phải là msgstr "" không
                        if (line.Trim() == "msgstr \"\"")
                        {
                            msgstrEmpty = true;
                        }
                        else
                        {
                            msgstrEmpty = false;
                        }
                    }
                    else if (msgstrFound && line.Trim().StartsWith("\""))
                    {
                        // Nếu sau msgstr "" có dòng bắt đầu bằng ", tức là đã có dịch
                        if (msgstrEmpty)
                        {
                            hasTranslation = true;
                        }
                    }
                }
            }

            // Kiểm tra mục cuối cùng nếu không có dòng trống sau nó
            if (msgstrFound && msgstrEmpty && !hasTranslation && currentEntryLines.Count > 0)
            {
                entriesWithEmptyMsgStr.Add(string.Join(Environment.NewLine, currentEntryLines).Trim());
            }

            return entriesWithEmptyMsgStr;
        }

        private async void btnCutTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbBanDichTrong.Text))
            {
                MessageBox.Show("Không có đoạn dịch nào để cắt và lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy đường dẫn file đang xử lý từ lbDuongDanBanDich
                string filePath = lbDuongDanBanDich.Text;

                // Đọc toàn bộ nội dung file gốc
                var allLines = await Task.Run(() => File.ReadAllLines(filePath, Encoding.UTF8).ToList());

                // Tách các mục cần cắt từ RichTextBox
                var entriesToCut = rtbBanDichTrong.Text.Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.None)
                                                        .Where(entry => !string.IsNullOrWhiteSpace(entry))
                                                        .ToList();

                // Xóa các mục có msgstr "" khỏi nội dung gốc
                foreach (var entry in entriesToCut)
                {
                    var entryLines = entry.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    for (int i = 0; i < allLines.Count; i++)
                    {
                        // Tìm khớp đoạn entry và xóa khỏi file gốc
                        if (allLines.Skip(i).Take(entryLines.Length).SequenceEqual(entryLines))
                        {
                            allLines.RemoveRange(i, entryLines.Length);
                            break;
                        }
                    }
                }

                // Thêm các mục đã cắt xuống cuối file
                allLines.Add(Environment.NewLine + "# Các mục có msgstr \"\" đã cắt:");
                allLines.AddRange(entriesToCut);

                // Ghi nội dung mới vào file
                await Task.Run(() => File.WriteAllLines(filePath, allLines, Encoding.UTF8));

                MessageBox.Show("Đã cắt và lưu các mục msgstr \"\" xuống cuối file thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm sạch RichTextBox
                rtbBanDichTrong.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cắt và lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
