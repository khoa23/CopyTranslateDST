using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Net.Http;
using System.Net;

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
                WriteLog($"Đang tạo map bản dịch từ: {fileOldTransPath}");
                Dictionary<string, string> translationMap = CreateTranslationMap(fileOldTransPath);
                WriteLog($"Tìm thấy {translationMap.Count} mục bản dịch.");

                // Sao chép bản dịch từ file PO cũ sang file PO mới
                WriteLog($"Đang sao chép bản dịch sang: {fileNewTransPath}");
                CopyTranslations(fileNewTransPath, translationMap);

                MessageBox.Show("Sao chép bản dịch thành công!");
                WriteLog("Sao chép bản dịch hoàn tất.");
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
            if (string.IsNullOrEmpty(line)) return "";
            
            int startIndex = line.IndexOf('"');
            int endIndex = line.LastIndexOf('"');

            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            {
                return "";
            }

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
                    rtbLog.Clear();
                    WriteLog($"Đang mở file: {sSelectedPath}");

                    // Thực hiện xử lý file trên luồng nền
                    var entriesWithEmptyMsgStr = await Task.Run(() => ProcessPoFile(sSelectedPath));

                    // Hiển thị kết quả lên RichTextBox trên luồng UI
                    if (entriesWithEmptyMsgStr.Count > 0)
                    {
                        WriteLog($"Tìm thấy {entriesWithEmptyMsgStr.Count} mục chưa dịch.");
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

                // Tạo danh sách mới để lưu các dòng đã lọc (bỏ các mục chưa dịch)
                var filteredLines = new List<string>();
                var entriesRemoved = new List<string>();
                var currentEntry = new List<string>();
                bool isEntryToCut = false;
                bool isMsgstrPresent = false;

                for (int i = 0; i < allLines.Count; i++)
                {
                    var line = allLines[i];
                    currentEntry.Add(line);

                    // Kiểm tra sự tồn tại của msgstr và nếu msgstr thực sự rỗng
                    if (line.StartsWith("msgstr"))
                    {
                        isMsgstrPresent = true;
                        if (line.Trim() == "msgstr \"\"")
                        {
                            isEntryToCut = true;
                        }
                        else
                        {
                            isEntryToCut = false;
                        }
                    }

                    // Nếu gặp dòng trống => kết thúc một mục và kiểm tra có cần cắt không
                    if (string.IsNullOrWhiteSpace(line) && currentEntry.Count > 0)
                    {
                        // Xác định xem có cần cắt mục không (chỉ cắt nếu msgstr rỗng hoàn toàn)
                        if (isEntryToCut && isMsgstrPresent)
                        {
                            entriesRemoved.AddRange(currentEntry);
                        }
                        else
                        {
                            filteredLines.AddRange(currentEntry);
                        }

                        currentEntry.Clear();
                        isMsgstrPresent = false;
                    }
                }

                // Xử lý entry cuối cùng nếu còn tồn tại
                if (currentEntry.Count > 0)
                {
                    if (isEntryToCut && isMsgstrPresent)
                    {
                        entriesRemoved.AddRange(currentEntry);
                    }
                    else
                    {
                        filteredLines.AddRange(currentEntry);
                    }
                }

                // Thêm một dòng trống giữa các entry còn lại
                filteredLines = filteredLines.Where((line, index) => !(string.IsNullOrWhiteSpace(line) && (index == 0 || string.IsNullOrWhiteSpace(filteredLines[index - 1])))).ToList();

                // Thêm các mục chưa dịch xuống cuối file nếu có và loại bỏ khoảng trắng dư thừa
                if (entriesRemoved.Count > 0)
                {
                    filteredLines.Add(""); // Chỉ thêm đúng một dòng trống
                    filteredLines.Add("\"Language-Team: Khoa.ga\\n\"");
                    filteredLines.AddRange(entriesRemoved);
                }

                // Loại bỏ dòng trống dư thừa trong entriesRemoved
                entriesRemoved = entriesRemoved.Where((line, index) => !(string.IsNullOrWhiteSpace(line) && (index == 0 || string.IsNullOrWhiteSpace(entriesRemoved[index - 1])))).ToList();

                // Ghi nội dung mới vào file
                await Task.Run(() => File.WriteAllLines(filePath, filteredLines, Encoding.UTF8));

                MessageBox.Show("Đã cắt và lưu các mục msgstr \"\" xuống cuối file thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Làm sạch RichTextBox
                rtbBanDichTrong.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cắt và lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGoogleTranslate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbBanDichTrong.Text))
            {
                MessageBox.Show("Không có nội dung để dịch.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnGoogleTranslate.Enabled = false;
                btnGoogleTranslate.Text = "Đang dịch...";

                string text = rtbBanDichTrong.Text;
                WriteLog("Bắt đầu tiến trình dịch tự động...");
                WriteLog($"Tổng độ dài văn bản gốc: {text.Length} ký tự.");

                // Sử dụng Regex để split chuẩn hơn, tránh lỗi xuống dòng khác nhau giữa các hệ thống
                string[] entries = System.Text.RegularExpressions.Regex.Split(text, @"(\r\n){2,}|(\n){2,}");
                // Lọc bỏ các phần tử rỗng sau khi split bằng Regex
                var filteredEntries = entries.Where(e => !string.IsNullOrWhiteSpace(e) && (e.Contains("msgid") || e.Contains("msgstr"))).ToList();
                
                WriteLog($"Tìm thấy {filteredEntries.Count} đoạn văn bản PO để xử lý.");
                List<string> translatedEntries = new List<string>();

                using (HttpClient client = new HttpClient())
                {
                    int totalCount = filteredEntries.Count;
                    int successCountForSave = 0;
                    int totalSuccessCount = 0;
                    int maxToTranslate = (int)numMaxTrans.Value;

                    WriteLog($"Giới hạn dịch tối đa được thiết lập: {maxToTranslate} câu.");

                    for (int i = 0; i < totalCount; i++)
                    {
                        if (totalSuccessCount >= maxToTranslate)
                        {
                            WriteLog($"Đã chạm giới hạn tối đa ({maxToTranslate} câu). Dừng tiến trình.");
                            break;
                        }

                        string entry = filteredEntries[i];
                        string[] lines = entry.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        StringBuilder fullMsgId = new StringBuilder();
                        int msgstrIndex = -1;
                        bool collectingMsgId = false;

                        for (int j = 0; j < lines.Length; j++)
                        {
                            string trimmedLine = lines[j].Trim();
                            if (trimmedLine.StartsWith("msgid "))
                            {
                                string val = GetValueFromLine(lines[j]);
                                fullMsgId.Append(val);
                                collectingMsgId = true;
                            }
                            else if (trimmedLine.StartsWith("msgstr"))
                            {
                                msgstrIndex = j; 
                                collectingMsgId = false;
                            }
                            else if (collectingMsgId && trimmedLine.StartsWith("\""))
                            {
                                fullMsgId.Append(GetValueFromLine(lines[j]));
                            }
                        }

                        string finalMsgId = fullMsgId.ToString();
                        if (!string.IsNullOrEmpty(finalMsgId) && msgstrIndex != -1)
                        {
                            WriteLog($"[{i + 1}/{totalCount}] Đang dịch: {finalMsgId}");
                            string translatedText = await TranslateTextAsync(client, finalMsgId);
                            WriteLog($"   -> Kết quả: {translatedText}");
                            
                            lines[msgstrIndex] = $"msgstr \"{translatedText}\"";
                            filteredEntries[i] = string.Join(Environment.NewLine, lines); // Cập nhật lại danh sách gốc
                            
                            successCountForSave++;
                            totalSuccessCount++;
                        }

                        // Cứ mỗi 100 câu dịch thành công thì lưu một lần
                        if (successCountForSave >= 100)
                        {
                            WriteLog("Đã đạt mốc 100 câu mới. Đang tự động lưu dự phòng...");
                            
                            // Cập nhật UI để người dùng thấy tiến độ
                            string currentContent = string.Join(Environment.NewLine + Environment.NewLine, filteredEntries);
                            this.Invoke((MethodInvoker)delegate { rtbBanDichTrong.Text = currentContent; });

                            // Thực hiện lưu vào file
                            string filePath = lbDuongDanBanDich.Text;
                            await Task.Run(() => SaveRichTextBoxToFile(filePath));
                            
                            WriteLog($"---> Đã tự động lưu thành công tại mốc {totalSuccessCount} câu.");
                            successCountForSave = 0; 
                        }
                    }
                }

                // Cập nhật kết quả cuối cùng lên giao diện
                rtbBanDichTrong.Text = string.Join(Environment.NewLine + Environment.NewLine, filteredEntries);

                var result = MessageBox.Show("Dịch tự động hoàn tất! Bạn có muốn lưu kết quả này vào file ngay không?", "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string filePath = lbDuongDanBanDich.Text;
                    await Task.Run(() => SaveRichTextBoxToFile(filePath));
                    MessageBox.Show("Đã lưu vào file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi dịch: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGoogleTranslate.Enabled = true;
                btnGoogleTranslate.Text = "Dịch tự động (Google)";
            }
        }

        private async void btnLuuBanDich_Click(object sender, EventArgs e)
        {
            string filePath = lbDuongDanBanDich.Text;
            if (string.IsNullOrWhiteSpace(rtbBanDichTrong.Text) || !File.Exists(filePath))
            {
                MessageBox.Show("Vui lòng mở bản dịch và thực hiện dịch trước khi lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnLuuBanDich.Enabled = false;
                btnLuuBanDich.Text = "Đang lưu...";

                await Task.Run(() => SaveRichTextBoxToFile(filePath));

                MessageBox.Show("Đã lưu bản dịch vào file thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLuuBanDich.Enabled = true;
                btnLuuBanDich.Text = "Lưu bản dịch vào file";
            }
        }

        private void SaveRichTextBoxToFile(string filePath)
        {
            var updates = new Dictionary<string, string>();
            string text = "";
            this.Invoke((MethodInvoker)delegate { text = rtbBanDichTrong.Text; });

            // Split theo 2 dòng trống để tách các entry
            string[] entries = System.Text.RegularExpressions.Regex.Split(text, @"(\r\n){2,}|(\n){2,}");
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry)) continue;

                string[] lines = entry.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string msgctxt = "";
                string msgid = "";
                string msgstr = "";

                foreach (var line in lines)
                {
                    string t = line.Trim();
                    if (t.StartsWith("msgctxt ")) msgctxt = GetValueFromLine(line);
                    else if (t.StartsWith("msgid ")) msgid = GetValueFromLine(line);
                    else if (t.StartsWith("msgstr ")) msgstr = GetValueFromLine(line);
                    // Hỗ trợ cộng dồn nếu msgid/msgstr nhiều dòng trong RichTextBox
                    else if (t.StartsWith("\"") && msgid != "" && msgstr == "") msgid += GetValueFromLine(line);
                    else if (t.StartsWith("\"") && msgstr != "") msgstr += GetValueFromLine(line);
                }

                if (!string.IsNullOrEmpty(msgid))
                {
                    string key = (msgctxt ?? "") + "|" + msgid;
                    updates[key] = msgstr;
                }
            }

            string[] allLines = File.ReadAllLines(filePath, Encoding.UTF8);
            List<string> outputLines = new List<string>();
            
            string currentEntryCtxt = "";
            string currentEntryId = "";
            bool inMsgstr = false;

            for (int i = 0; i < allLines.Length; i++)
            {
                string line = allLines[i];
                string trimmed = line.Trim();

                if (trimmed.StartsWith("msgctxt "))
                {
                    currentEntryCtxt = GetValueFromLine(line);
                    outputLines.Add(line);
                }
                else if (trimmed.StartsWith("msgid "))
                {
                    currentEntryId = GetValueFromLine(line);
                    outputLines.Add(line);
                    // Đọc tiếp nếu msgid nhiều dòng
                    int nextIdx = i + 1;
                    while (nextIdx < allLines.Length && allLines[nextIdx].Trim().StartsWith("\""))
                    {
                        currentEntryId += GetValueFromLine(allLines[nextIdx]);
                        outputLines.Add(allLines[nextIdx]);
                        i = nextIdx;
                        nextIdx++;
                    }
                }
                else if (trimmed.StartsWith("msgstr"))
                {
                    string key = currentEntryCtxt + "|" + currentEntryId;
                    if (updates.ContainsKey(key))
                    {
                        outputLines.Add($"msgstr \"{updates[key]}\"");
                        // Bỏ qua nội dung cũ của msgstr
                        int nextIdx = i + 1;
                        while (nextIdx < allLines.Length && allLines[nextIdx].Trim().StartsWith("\""))
                        {
                            i = nextIdx;
                            nextIdx++;
                        }
                    }
                    else
                    {
                        outputLines.Add(line);
                    }
                    // Reset cho entry tiếp theo
                    currentEntryCtxt = "";
                    currentEntryId = "";
                }
                else
                {
                    // Giữ nguyên dòng trống, ghi chú #
                    outputLines.Add(line);
                }
            }

            File.WriteAllLines(filePath, outputLines, Encoding.UTF8);
        }

        private async Task<string> TranslateTextAsync(HttpClient client, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            try
            {
                // 1. Bảo vệ các từ trong dấu ngoặc nhọn { } (biến trong game)
                var placeholders = new List<string>();
                string protectedText = System.Text.RegularExpressions.Regex.Replace(text, @"\{[^}]+\}", m =>
                {
                    string placeholder = $"__VAR_{placeholders.Count}__";
                    placeholders.Add(m.Value);
                    return placeholder;
                });

                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=vi&dt=t&q={WebUtility.UrlEncode(protectedText)}";
                var response = await client.GetStringAsync(url);
                
                string translatedResult = "";
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    var root = doc.RootElement;
                    var translations = root[0];
                    StringBuilder sb = new StringBuilder();
                    foreach (var translation in translations.EnumerateArray())
                    {
                        sb.Append(translation[0].GetString());
                    }
                    translatedResult = sb.ToString();
                }

                // 2. Khôi phục các biến { } về nguyên bản
                for (int i = 0; i < placeholders.Count; i++)
                {
                    string placeholder = $"__VAR_{i}__";
                    // Google Translate đôi khi thêm khoảng trắng xung quanh "__ VAR _ 0 __"
                    // Chúng ta sẽ xử lý thay thế chuỗi chính xác trước
                    translatedResult = translatedResult.Replace(placeholder, placeholders[i]);
                    
                    // Xử lý trường hợp Google Translate tự ý thêm khoảng trắng
                    string greedyPlaceholder = $"__ VAR_{i} __";
                    translatedResult = translatedResult.Replace(greedyPlaceholder, placeholders[i]);
                }

                return translatedResult;
            }
            catch (Exception ex)
            {
                WriteLog($"[ERROR] Lỗi dịch thuật: {ex.Message}");
                return "Error in translation: " + ex.Message;
            }
        }

        private void WriteLog(string message)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(WriteLog), message);
                return;
            }

            string time = DateTime.Now.ToString("HH:mm:ss");
            rtbLog.AppendText($"[{time}] {message}{Environment.NewLine}");
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
            
            // Cũng in ra Debug để tiện debug
            System.Diagnostics.Debug.WriteLine($"[{time}] {message}");
        }
    }
}
