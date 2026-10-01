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
        private static readonly System.Threading.SemaphoreSlim _saveSemaphore = new System.Threading.SemaphoreSlim(1, 1);

        public class PoEntry
        {
            public string Context { get; set; }
            public string Id { get; set; }
            public string CurrentStr { get; set; }
            public string Prefix { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            SetupDgvEval();
            InitializeGeminiTab();
            LoadAndSaveApiKeys();
        }

        private void LoadAndSaveApiKeys()
        {
            try
            {
                // Load & auto-save Anything API Key (Đánh giá & Dịch lại tab)
                string anythingApiKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "anything_api_key.txt");
                if (File.Exists(anythingApiKeyPath))
                {
                    txtAnythingApiKey.Text = File.ReadAllText(anythingApiKeyPath, Encoding.UTF8).Trim();
                }

                txtAnythingApiKey.TextChanged += (sender, e) =>
                {
                    try
                    {
                        File.WriteAllText(anythingApiKeyPath, txtAnythingApiKey.Text.Trim(), Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error saving Anything API Key: " + ex.Message);
                    }
                };

                // Load & auto-save Gemini API Key (Đánh giá & Dịch (Gemini) tab)
                string geminiApiKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gemini_api_key.txt");
                if (File.Exists(geminiApiKeyPath))
                {
                    txtGeminiApiKey.Text = File.ReadAllText(geminiApiKeyPath, Encoding.UTF8).Trim();
                }

                txtGeminiApiKey.TextChanged += (sender, e) =>
                {
                    try
                    {
                        File.WriteAllText(geminiApiKeyPath, txtGeminiApiKey.Text.Trim(), Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error saving Gemini API Key: " + ex.Message);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo API Key: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                WriteLog("Bắt đầu tiến trình dịch tự động (Đa luồng: 3)...");
                WriteLog($"Tổng độ dài văn bản gốc: {text.Length} ký tự.");

                // Sử dụng Regex để split chuẩn hơn, tránh lỗi xuống dòng khác nhau giữa các hệ thống
                string[] entries = System.Text.RegularExpressions.Regex.Split(text, @"(\r\n){2,}|(\n){2,}");
                // Lọc bỏ các phần tử rỗng sau khi split bằng Regex
                var filteredEntries = entries.Where(e => !string.IsNullOrWhiteSpace(e) && (e.Contains("msgid") || e.Contains("msgstr"))).ToList();
                
                WriteLog($"Tìm thấy {filteredEntries.Count} đoạn văn bản PO để xử lý.");
                
                int totalCount = filteredEntries.Count;
                int successCountForSave = 0;
                int totalSuccessCount = 0;
                int maxToTranslate = (int)numMaxTrans.Value;
                object saveLock = new object();

                WriteLog($"Giới hạn dịch tối đa được thiết lập: {maxToTranslate} câu.");

                using (HttpClient client = new HttpClient())
                {
                    // Thiết lập Timeout dài hơn một chút cho đa luồng
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var indexedEntries = filteredEntries.Select((entry, index) => new { entry, index }).ToList();
                    var cts = new CancellationTokenSource();
                    var parallelOptions = new ParallelOptions 
                    { 
                        MaxDegreeOfParallelism = 3,
                        CancellationToken = cts.Token
                    };

                    try
                    {
                        await Parallel.ForEachAsync(indexedEntries, parallelOptions, async (item, token) =>
                        {
                            // Kiểm tra giới hạn câu dịch
                            if (Interlocked.CompareExchange(ref totalSuccessCount, 0, 0) >= maxToTranslate)
                            {
                                cts.Cancel();
                                return;
                            }

                            string entry = item.entry;
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
                                string translatedText = "";

                                while (!token.IsCancellationRequested)
                                {
                                    // Kiểm tra giới hạn câu dịch
                                    if (Interlocked.CompareExchange(ref totalSuccessCount, 0, 0) >= maxToTranslate)
                                    {
                                        cts.Cancel();
                                        return;
                                    }

                                    WriteLog($"[{item.index + 1}/{totalCount}] Đang dịch: {finalMsgId}");
                                    translatedText = await TranslateTextAsync(client, finalMsgId, token);

                                    // Nếu nhận được lỗi, không ghi vào mà delay 10s rồi gửi lại request
                                    if (!string.IsNullOrEmpty(translatedText) && translatedText.StartsWith("Error in translation", StringComparison.OrdinalIgnoreCase))
                                    {
                                        WriteLog($"[{item.index + 1}/{totalCount}] Nhận được '{translatedText}'. Không ghi vào, tạm dừng 10s rồi gửi lại request...");
                                        await Task.Delay(10000, token);
                                        continue;
                                    }

                                    // Dịch thành công
                                    break;
                                }

                                if (token.IsCancellationRequested || string.IsNullOrEmpty(translatedText) || translatedText.StartsWith("Error in translation", StringComparison.OrdinalIgnoreCase))
                                {
                                    return;
                                }

                                WriteLog($"   -> [{item.index + 1}] Kết quả: {translatedText}");

                                lines[msgstrIndex] = $"msgstr \"{translatedText}\"";
                                filteredEntries[item.index] = string.Join(Environment.NewLine, lines);

                                // Tăng đếm tổng số câu đã dịch thành công
                                int currentTotal = Interlocked.Increment(ref totalSuccessCount);
                                
                                // Xử lý lưu dự phòng mỗi 100 câu
                                int currentSave = Interlocked.Increment(ref successCountForSave);
                                if (currentSave >= 100)
                                {
                                    lock (saveLock)
                                    {
                                        if (successCountForSave >= 100)
                                        {
                                            successCountForSave = 0;
                                            WriteLog($"Đã đạt mốc {currentTotal} câu. Đang tự động lưu dự phòng...");
                                            
                                            // Cập nhật UI
                                            string currentContent = string.Join(Environment.NewLine + Environment.NewLine, filteredEntries);
                                            this.Invoke((MethodInvoker)delegate { rtbBanDichTrong.Text = currentContent; });

                                            // Lưu file
                                            string filePath = lbDuongDanBanDich.Text;
                                            SaveRichTextBoxToFile(filePath);
                                            WriteLog($"---> Đã tự động lưu thành công tại mốc {currentTotal} câu.");
                                        }
                                    }
                                }
                            }
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        WriteLog($"Đã chạm giới hạn tối đa ({maxToTranslate} câu) hoặc tiến trình bị hủy.");
                    }
                }

                // Cập nhật kết quả cuối cùng lên giao diện
                rtbBanDichTrong.Text = string.Join(Environment.NewLine + Environment.NewLine, filteredEntries);

                var result = MessageBox.Show("Dịch tự động đa luồng hoàn tất! Bạn có muốn lưu kết quả này vào file ngay không?", "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private async Task<string> TranslateTextAsync(HttpClient client, string text, CancellationToken cancellationToken = default)
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
                var response = await client.GetStringAsync(url, cancellationToken);
                
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
            catch (OperationCanceledException)
            {
                throw;
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
        private bool HasPunctuationMismatch(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return false;
            
            string t1 = s1.TrimEnd();
            string t2 = s2.TrimEnd();
            
            if (t1.Length == 0 || t2.Length == 0) return false;

            char last1 = t1[t1.Length - 1];
            char last2 = t2[t2.Length - 1];

            char[] puncts = { '.', '!', '?', ':', ';', ',', '\u2026' };
            
            bool isP1 = puncts.Contains(last1);
            bool isP2 = puncts.Contains(last2);

            if (isP1 != isP2) return true;
            if (isP1 && isP2 && last1 != last2) return true;

            return false;
        }

        private List<PoEntry> ProcessPoFileForAllEntries(string filePath)
        {
            var entries = new List<PoEntry>();
            string currentCtxt = "";
            var idBuilder = new StringBuilder();
            var strBuilder = new StringBuilder();
            
            bool collectingId = false;
            bool collectingStr = false;

            foreach (var rawLine in File.ReadLines(filePath, Encoding.UTF8))
            {
                string line = rawLine.TrimStart();
                if (line.StartsWith("#. "))
                {
                    currentCtxt = GetValueFromLine(rawLine);
                }
                else if (line.StartsWith("msgctxt "))
                {
                    currentCtxt = GetValueFromLine(rawLine);
                    collectingId = false;
                    collectingStr = false;
                }
                else if (line.StartsWith("msgid "))
                {
                    idBuilder.Clear();
                    idBuilder.Append(GetValueFromLine(rawLine));
                    collectingId = true;
                    collectingStr = false;
                }
                else if (line.StartsWith("msgstr "))
                {
                    strBuilder.Clear();
                    strBuilder.Append(GetValueFromLine(rawLine));
                    collectingId = false;
                    collectingStr = true;
                }
                else if (line.StartsWith("\""))
                {
                    if (collectingId) idBuilder.Append(GetValueFromLine(rawLine));
                    else if (collectingStr) strBuilder.Append(GetValueFromLine(rawLine));
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    if (idBuilder.Length > 0)
                    {
                        if (string.IsNullOrEmpty(currentCtxt) || (!currentCtxt.StartsWith("STRINGS.NAMES") && !currentCtxt.StartsWith("\"STRINGS.NAMES")))
                        {
                            entries.Add(new PoEntry 
                            { 
                                Context = currentCtxt, 
                                Id = idBuilder.ToString(), 
                                CurrentStr = strBuilder.ToString(),
                                Prefix = GetPrefix(currentCtxt)
                            });
                        }
                    }
                    currentCtxt = "";
                    idBuilder.Clear();
                    strBuilder.Clear();
                    collectingId = false;
                    collectingStr = false;
                }
            }

            if (idBuilder.Length > 0)
            {
                if (string.IsNullOrEmpty(currentCtxt) || (!currentCtxt.StartsWith("STRINGS.NAMES") && !currentCtxt.StartsWith("\"STRINGS.NAMES")))
                {
                    entries.Add(new PoEntry 
                    { 
                        Context = currentCtxt, 
                        Id = idBuilder.ToString(), 
                        CurrentStr = strBuilder.ToString(),
                        Prefix = GetPrefix(currentCtxt)
                    });
                }
            }

            return entries;
        }

        private bool UpdatePoFileWithMap(string filePath, Dictionary<string, string> updates)
        {
            string fullPath = Path.GetFullPath(filePath);
            WriteEvalLog($"[LƯU FILE] Bắt đầu cập nhật file PO. Đường dẫn đầy đủ: {fullPath}");
            if (!File.Exists(fullPath))
            {
                WriteEvalLog($"[LƯU FILE] ❌ LỖI: File không tồn tại tại đường dẫn: {fullPath}");
                return false;
            }

            DateTime beforeWriteTime = File.GetLastWriteTime(fullPath);
            WriteEvalLog($"[LƯU FILE] Thời gian sửa đổi file trước khi ghi: {beforeWriteTime:yyyy-MM-dd HH:mm:ss.fff}");

            foreach (var kvp in updates)
            {
                WriteEvalLog($"[LƯU FILE] Cần cập nhật key: '{kvp.Key}' -> '{kvp.Value}'");
            }

            string[] poLines = File.ReadAllLines(fullPath, Encoding.UTF8);
            List<string> outputLines = new List<string>();
            string currentCtxt = "";
            string currentId = "";
            bool collectingMsgId = false;
            bool foundAny = false;

            for (int i = 0; i < poLines.Length; i++)
            {
                string line = poLines[i];
                string trimmed = line.Trim();

                if (trimmed.StartsWith("#. "))
                {
                    currentCtxt = GetValueFromLine(line);
                    outputLines.Add(line);
                }
                else if (trimmed.StartsWith("msgctxt "))
                {
                    currentCtxt = GetValueFromLine(line);
                    outputLines.Add(line);
                }
                else if (trimmed.StartsWith("msgid "))
                {
                    currentId = GetValueFromLine(line);
                    collectingMsgId = true;
                    outputLines.Add(line);
                }
                else if (collectingMsgId && trimmed.StartsWith("\""))
                {
                    currentId += GetValueFromLine(line);
                    outputLines.Add(line);
                }
                else if (trimmed.StartsWith("msgstr"))
                {
                    collectingMsgId = false;
                    string key = (currentCtxt ?? "") + "|" + currentId;
                    if (updates.ContainsKey(key))
                    {
                        foundAny = true;
                        string cleanVal = updates[key].Replace("\"", "\\\"");
                        string oldLine = line;
                        string newLine = $"msgstr \"{cleanVal}\"";
                        WriteEvalLog($"[LƯU FILE] tìm thấy dòng khớp key '{key}'.");
                        WriteEvalLog($"[LƯU FILE] Dòng cũ: {oldLine}");
                        WriteEvalLog($"[LƯU FILE] Dòng mới: {newLine}");
                        outputLines.Add(newLine);
                        
                        int next = i + 1;
                        while (next < poLines.Length && poLines[next].Trim().StartsWith("\""))
                        {
                            WriteEvalLog($"[LƯU FILE] Bỏ qua dòng phụ msgstr cũ: {poLines[next]}");
                            i = next;
                            next++;
                        }
                    }
                    else
                    {
                        outputLines.Add(line);
                    }
                    currentCtxt = "";
                    currentId = "";
                }
                else
                {
                    outputLines.Add(line);
                }
            }

            if (foundAny)
            {
                try
                {
                    File.WriteAllLines(fullPath, outputLines, Encoding.UTF8);
                    // Đọc lại thuộc tính file để chắc chắn
                    DateTime afterWriteTime = File.GetLastWriteTime(fullPath);
                    long fileLength = new FileInfo(fullPath).Length;
                    WriteEvalLog($"[LƯU FILE] ✔ Đã ghi file thành công. Số dòng: {outputLines.Count}. Kích thước file: {fileLength} bytes. Thời gian sửa đổi mới: {afterWriteTime:yyyy-MM-dd HH:mm:ss.fff}");
                }
                catch (Exception writeEx)
                {
                    WriteEvalLog($"[LƯU FILE] ❌ LỖI khi ghi xuống đĩa: {writeEx.Message}");
                    throw;
                }
            }
            else
            {
                WriteEvalLog($"[LƯU FILE] ⚠ Không tìm thấy key nào để cập nhật trong file PO.");
            }
            return foundAny;
        }


        // ============================================================
        // Tab: Đánh giá & Dịch lại
        // ============================================================

        private string _evalFilePath = "";
        private CancellationTokenSource? _evalCts;
        private ComboBox? cbContextFilter;
        private Label? lbContextFilter;
        private Label? lbNoTranslate;
        private TextBox? txtNoTranslate;
        private List<PoEntry>? _allEvalEntries;
        private Button? btnExportEval;
        private Button? btnLoadCsv;
        private string _evalCsvPath = "";
        private DateTime _lastEvalDateTime = DateTime.MinValue;

        private void LoadNoTranslateWords()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notranslate.txt");
                if (File.Exists(path))
                {
                    txtNoTranslate.Text = File.ReadAllText(path, Encoding.UTF8);
                }
                else
                {
                    // Default values for Don't Starve Together
                    txtNoTranslate.Text = "Wilson, Willow, Wolfgang, Wendy, WX-78, Wickerbottom, Woodie, Wes, Maxwell, Wigfrid, Webber, Winona, Wortox, Wormwood, Wurt, Walter, Wanda, abigail, Deerclops";
                    File.WriteAllText(path, txtNoTranslate.Text, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading no-translate words: " + ex.Message);
            }
        }

        private void TxtNoTranslate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notranslate.txt");
                File.WriteAllText(path, txtNoTranslate.Text, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error saving no-translate words: " + ex.Message);
            }
        }

        private string GetPrefix(string context)
        {
            if (string.IsNullOrEmpty(context)) return "Khác";

            // Remove starting/ending quotes if present
            string cleanContext = context.Trim('"');

            string[] parts = cleanContext.Split('.');
            if (parts.Length >= 2)
            {
                return parts[0] + "." + parts[1];
            }
            return cleanContext;
        }

        private void SetupDgvEval()
        {
            dgvEval.Columns.Clear();
            dgvEval.Columns.Add("Context", "Ngữ cảnh");
            dgvEval.Columns.Add("English", "Tiếng Anh (gốc)");
            dgvEval.Columns.Add("Vietnamese", "Tiếng Việt (hiện tại)");
            dgvEval.Columns.Add("SuggestedTrans", "Dịch lại (AI)");
            dgvEval.Columns.Add("EvalScore", "Điểm đánh giá");
            dgvEval.Columns.Add("EvalComment", "Nhận xét");

            var btnAcceptEval = new DataGridViewButtonColumn();
            btnAcceptEval.HeaderText = "Chấp nhận";
            btnAcceptEval.Text = "OK";
            btnAcceptEval.UseColumnTextForButtonValue = true;
            btnAcceptEval.Name = "btnAcceptEval";
            dgvEval.Columns.Add(btnAcceptEval);

            var btnRejectEval = new DataGridViewButtonColumn();
            btnRejectEval.HeaderText = "Bỏ qua";
            btnRejectEval.Text = "Skip";
            btnRejectEval.UseColumnTextForButtonValue = true;
            btnRejectEval.Name = "btnRejectEval";
            dgvEval.Columns.Add(btnRejectEval);

            dgvEval.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEval.Columns["Context"].FillWeight = 30;
            dgvEval.Columns["English"].FillWeight = 45;
            dgvEval.Columns["Vietnamese"].FillWeight = 45;
            dgvEval.Columns["SuggestedTrans"].FillWeight = 45;
            dgvEval.Columns["EvalScore"].FillWeight = 18;
            dgvEval.Columns["EvalComment"].FillWeight = 55;
            dgvEval.Columns["btnAcceptEval"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvEval.Columns["btnAcceptEval"].Width = 80;
            dgvEval.Columns["btnRejectEval"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvEval.Columns["btnRejectEval"].Width = 70;

            dgvEval.CellContentClick += dgvEval_CellContentClick;
            dgvEval.CellEndEdit += dgvEval_CellEndEdit;

            // Make other columns read-only so the user can only edit the Vietnamese translation column
            dgvEval.Columns["Context"].ReadOnly = true;
            dgvEval.Columns["English"].ReadOnly = true;
            dgvEval.Columns["SuggestedTrans"].ReadOnly = true;
            dgvEval.Columns["EvalScore"].ReadOnly = true;
            dgvEval.Columns["EvalComment"].ReadOnly = true;

            // Add dynamic filter ComboBox
            if (cbContextFilter == null)
            {
                lbContextFilter = new Label();
                lbContextFilter.Text = "Lọc theo:";
                lbContextFilter.AutoSize = true;
                lbContextFilter.Location = new Point(dgvEval.Left, dgvEval.Top - 25);
                tabPageEval.Controls.Add(lbContextFilter);

                cbContextFilter = new ComboBox();
                cbContextFilter.DropDownStyle = ComboBoxStyle.DropDown;
                cbContextFilter.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbContextFilter.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbContextFilter.Location = new Point(dgvEval.Left + 60, dgvEval.Top - 28);
                cbContextFilter.Size = new Size(350, 23);
                cbContextFilter.SelectedIndexChanged += CbContextFilter_SelectedIndexChanged;
                cbContextFilter.TextUpdate += CbContextFilter_TextUpdate;
                tabPageEval.Controls.Add(cbContextFilter);
                
                // Move DataGridView and RichTextBox down by 35px to make room
                int offset = 35;
                dgvEval.Top += offset;
                dgvEval.Height -= offset;
                rtbLogEval.Top += offset;
                rtbLogEval.Height -= offset;
                
                lbContextFilter.Top += offset;
                cbContextFilter.Top += offset;

                lbNoTranslate = new Label();
                lbNoTranslate.Text = "Từ không dịch:";
                lbNoTranslate.AutoSize = true;
                lbNoTranslate.Location = new Point(dgvEval.Left + 430, dgvEval.Top - 25);
                tabPageEval.Controls.Add(lbNoTranslate);

                txtNoTranslate = new TextBox();
                txtNoTranslate.Location = new Point(dgvEval.Left + 520, dgvEval.Top - 28);
                txtNoTranslate.Size = new Size(dgvEval.Width - 520, 23);
                txtNoTranslate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                LoadNoTranslateWords();
                txtNoTranslate.TextChanged += TxtNoTranslate_TextChanged;
                tabPageEval.Controls.Add(txtNoTranslate);

                // Nút Xuất CSV — đặt phía trên bên phải DataGridView (cùng hàng với toolbar)
                btnExportEval = new Button();
                btnExportEval.Text = "Xuất CSV";
                btnExportEval.AutoSize = true;
                btnExportEval.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnExportEval.Location = new Point(rtbLogEval.Left, dgvEval.Top - 28);
                btnExportEval.Size = new Size(100, 23);
                btnExportEval.Click += BtnExportEval_Click;
                tabPageEval.Controls.Add(btnExportEval);

                // Nút "Tải CSV kết quả" — load kết quả đánh giá cũ từ file CSV
                btnLoadCsv = new Button();
                btnLoadCsv.Text = "Tải CSV kết quả";
                btnLoadCsv.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnLoadCsv.Location = new Point(rtbLogEval.Left + 105, dgvEval.Top - 28);
                btnLoadCsv.Size = new Size(130, 23);
                btnLoadCsv.Click += BtnLoadCsv_Click;
                tabPageEval.Controls.Add(btnLoadCsv);
            }
        }

        private void CbContextFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_allEvalEntries == null || cbContextFilter.SelectedItem == null) return;
            
            string selected = cbContextFilter.SelectedItem.ToString();
            if (selected == "Tất cả")
            {
                PopulateDgvEval(_allEvalEntries);
            }
            else
            {
                var filtered = _allEvalEntries.Where(entry => entry.Prefix == selected).ToList();
                PopulateDgvEval(filtered);
            }
        }

        private void CbContextFilter_TextUpdate(object? sender, EventArgs e)
        {
            if (_allEvalEntries == null) return;

            string filterText = cbContextFilter.Text.Trim();
            if (string.IsNullOrEmpty(filterText) || filterText == "Tất cả")
            {
                PopulateDgvEval(_allEvalEntries);
            }
            else
            {
                var filtered = _allEvalEntries.Where(entry =>
                    (entry.Prefix != null && entry.Prefix.Contains(filterText, StringComparison.OrdinalIgnoreCase)) ||
                    (entry.Context != null && entry.Context.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                ).ToList();
                PopulateDgvEval(filtered);
            }
        }

        private void PopulateDgvEval(List<PoEntry> entries)
        {
            dgvEval.SuspendLayout();
            dgvEval.Rows.Clear();
            
            var rows = new DataGridViewRow[entries.Count];
            for (int i = 0; i < entries.Count; i++)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvEval, entries[i].Context, entries[i].Id, entries[i].CurrentStr, "", "", "");
                rows[i] = row;
            }
            dgvEval.Rows.AddRange(rows);
            dgvEval.ResumeLayout();
        }

        private async void btnMoBanDichEval_Click(object sender, EventArgs e)
        {
            using OpenFileDialog fbd = new OpenFileDialog
            {
                Title = "Chọn file dịch để đánh giá",
                Filter = "PO Files (*.po)|*.po|All Files (*.*)|*.*"
            };

            if (fbd.ShowDialog() != DialogResult.OK) return;

            _evalFilePath = fbd.FileName;
            lbDuongDanBanDichEval.Text = _evalFilePath;

            try
            {
                btnMoBanDichEval.Enabled = false;
                btnMoBanDichEval.Text = "Đang xử lý...";
                dgvEval.Rows.Clear();
                rtbLogEval.Clear();
                WriteEvalLog($"Đang tải dữ liệu từ: {_evalFilePath}");

                _allEvalEntries = await Task.Run(() => ProcessPoFileForAllEntries(_evalFilePath));
                
                // Populate cbContextFilter
                var prefixes = _allEvalEntries
                    .Select(entry => entry.Prefix)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();
                    
                cbContextFilter.SelectedIndexChanged -= CbContextFilter_SelectedIndexChanged;
                cbContextFilter.Items.Clear();
                cbContextFilter.Items.Add("Tất cả");
                foreach (var p in prefixes)
                {
                    cbContextFilter.Items.Add(p);
                }
                cbContextFilter.SelectedIndex = 0;
                cbContextFilter.SelectedIndexChanged += CbContextFilter_SelectedIndexChanged;

                PopulateDgvEval(_allEvalEntries);
                WriteEvalLog($"Đã tải {_allEvalEntries.Count} mục.");

                // Auto-detect file CSV companion (<pofilename>_eval.csv) cùng thư mục
                string poDir = Path.GetDirectoryName(_evalFilePath) ?? "";
                string poName = Path.GetFileNameWithoutExtension(_evalFilePath);
                string companionCsv = Path.Combine(poDir, poName + "_eval.csv");
                if (File.Exists(companionCsv))
                {
                    var dlg = MessageBox.Show(
                        $"Tìm thấy file kết quả đánh giá cũ:\n{companionCsv}\n\nBạn có muốn tải lại kết quả đã đánh giá không? (Các hàng đã có kết quả sẽ không bị đánh giá lại.)",
                        "Tải kết quả cũ",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (dlg == DialogResult.Yes)
                    {
                        int loaded = await Task.Run(() => LoadEvalFromCsv(companionCsv));
                        WriteEvalLog($"✔ Đã tải {loaded} kết quả từ CSV. Các hàng này sẽ được bỏ qua khi chạy đánh giá.");
                        _evalCsvPath = companionCsv;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnMoBanDichEval.Enabled = true;
                btnMoBanDichEval.Text = "Mở file Đánh giá";
            }
        }

        private void btnCancelEval_Click(object sender, EventArgs e)
        {
            if (_evalCts != null)
            {
                _evalCts.Cancel();
                WriteEvalLog("Đang yêu cầu hủy đánh giá...");
                btnCancelEval.Enabled = false;
            }
        }

        private async void btnEval_Click(object sender, EventArgs e)
        {
            if (dgvEval.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng mở file PO trước khi đánh giá.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string apiKey = txtAnythingApiKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Vui lòng nhập API key AnythingLLM.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            string apiUrl = txtAnythingIp.Text.Trim();
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                apiUrl = "http://localhost:3001";
            }
            apiUrl = apiUrl.TrimEnd('/');

            try
            {
                btnEval.Enabled = false;
                btnEval.Text = "Đang đánh giá...";
                btnCancelEval.Enabled = true;
                _evalCts = new CancellationTokenSource();
                var token = _evalCts.Token;

                int limit = (int)numLimitEval.Value;
                int processed = 0;

                int skipped = 0;
                foreach (DataGridViewRow row in dgvEval.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (processed >= limit) break;

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    string context = row.Cells["Context"].Value?.ToString() ?? "";
                    string original = row.Cells["English"].Value?.ToString() ?? "";
                    string currentTrans = row.Cells["Vietnamese"].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(original)) continue;

                    // Skip các hàng đã có kết quả đánh giá hợp lệ (từ CSV load hoặc lần chạy trước)
                    string existingScore = row.Cells["EvalScore"].Value?.ToString() ?? "";
                    bool alreadyEvaluated = !string.IsNullOrWhiteSpace(existingScore)
                        && existingScore != "..."
                        && existingScore != "Lỗi"
                        && existingScore != "Đã hủy";
                    if (alreadyEvaluated)
                    {
                        skipped++;
                        continue;
                    }

                    row.Cells["EvalScore"].Value = "...";
                    try
                    {
                        var result = await EvaluateTranslationAsync(apiUrl, apiKey, context, original, currentTrans, token);
                        row.Cells["EvalScore"].Value = result.Score;
                        row.Cells["EvalComment"].Value = result.Comment;

                        // Fallback: nếu đề xuất trống nhưng bản dịch hiện tại có giá trị, lấy bản dịch hiện tại
                        string finalSuggested = result.Suggested;
                        if (string.IsNullOrWhiteSpace(finalSuggested) && !string.IsNullOrWhiteSpace(currentTrans))
                        {
                            finalSuggested = currentTrans;
                        }
                        row.Cells["SuggestedTrans"].Value = finalSuggested;
                    }
                    catch (OperationCanceledException)
                    {
                        row.Cells["EvalScore"].Value = "Đã hủy";
                        WriteEvalLog("Đã hủy đánh giá.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        row.Cells["EvalScore"].Value = "Lỗi";
                        WriteEvalLog($"Lỗi khi đánh giá '{original}': {ex.Message}");
                    }

                    processed++;
                    WriteEvalLog($"Đã đánh giá {processed}: {original}");
                }
                if (skipped > 0)
                    WriteEvalLog($"⏩ Đã bỏ qua {skipped} hàng đã có kết quả đánh giá.");

                if (token.IsCancellationRequested)
                {
                    WriteEvalLog($"Đã dừng đánh giá. Đã xử lý {processed} mục.");
                }
                else
                {
                    _lastEvalDateTime = DateTime.Now;
                    WriteEvalLog($"Hoàn tất đánh giá {processed} mục.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnEval.Enabled = true;
                btnEval.Text = "Đánh giá & Dịch";
                btnCancelEval.Enabled = false;
                _evalCts?.Dispose();
                _evalCts = null;
            }
        }

        private async Task<(string Score, string Comment, string Suggested)> EvaluateTranslationAsync(string apiUrl, string apiKey, string context, string original, string currentTrans, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the original word is exactly in the no-translate list
                if (txtNoTranslate != null && !string.IsNullOrWhiteSpace(txtNoTranslate.Text))
                {
                    var noTranslateList = txtNoTranslate.Text.Split(',')
                        .Select(w => w.Trim())
                        .Where(w => !string.IsNullOrEmpty(w))
                        .ToList();

                    if (noTranslateList.Any(w => string.Equals(original.Trim(), w, StringComparison.OrdinalIgnoreCase)))
                    {
                        string matchedWord = noTranslateList.First(w => string.Equals(original.Trim(), w, StringComparison.OrdinalIgnoreCase));
                        return ("10", "Từ khóa giữ nguyên không dịch.", matchedWord);
                    }
                }

                string baseUrl = apiUrl;
                string noTranslateInstructions = "";
                if (txtNoTranslate != null && !string.IsNullOrWhiteSpace(txtNoTranslate.Text))
                {
                    noTranslateInstructions = $"\nLưu ý quan trọng: Các từ/cụm từ sau đây bắt buộc giữ nguyên không được phép dịch sang tiếng Việt: {txtNoTranslate.Text.Trim()}.\n";
                }

                string prompt =
                    $"Bạn là chuyên gia dịch thuật game Don't Starve sang tiếng Việt.\n" +
                    $"Dưới đây là thông tin đoạn dịch từ file PO để cung cấp ngữ cảnh đầy đủ (bao gồm tên nhân vật, mô tả, hành động nằm trong msgctxt):\n\n" +
                    $"#. {context}\n" +
                    $"msgctxt \"{context}\"\n" +
                    $"msgid \"{original}\"\n" +
                    $"msgstr \"{currentTrans}\"\n\n" +
                    $"Hãy thực hiện các yêu cầu sau:\n" +
                    $"1. Cho điểm chất lượng bản dịch hiện tại (msgstr) dựa trên ngữ cảnh được cung cấp từ 1-10 (chỉ số nguyên). Nếu msgstr rỗng (chưa dịch), hãy cho 1 điểm.\n" +
                    $"2. Nhận xét ngắn gọn về bản dịch hiện tại (1-2 câu). Nếu chưa dịch, nhận xét là 'Chưa dịch'.\n" +
                    $"3. Đề xuất bản dịch tiếng Việt tốt nhất cho phần msgid này vào ô 'suggested':\n" +
                    $"   - Nếu bản dịch hiện tại đã tốt (điểm >= 8), hãy điền lại chính bản dịch hiện tại đó vào ô 'suggested' (tuyệt đối không được để trống).\n" +
                    $"   - Nếu bản dịch hiện tại chưa tốt hoặc đang trống (msgstr \"\"), bắt buộc phải đề xuất bản dịch tiếng Việt mới/tốt nhất vào ô 'suggested' (tuyệt đối không được để trống và tuyệt đối không để nguyên tiếng Anh, trừ khi đó là từ thuộc danh sách giữ nguyên không dịch).\n" +
                    noTranslateInstructions +
                    $"\nTrả lời ĐÚNG định dạng JSON sau, không thêm bất kỳ giải thích nào khác ngoài JSON:\n" +
                    $"{{\"score\":8,\"comment\":\"Nhận xét\",\"suggested\":\"Bản dịch đề xuất\"}}";

                var payload = new
                {
                    message = prompt,
                    mode = "chat"
                };

                string jsonBody = JsonSerializer.Serialize(payload);
                string maskedApiKey = apiKey.Length <= 8 ? "***" : $"{apiKey.Substring(0, 4)}...{apiKey.Substring(apiKey.Length - 4)}";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.Timeout = TimeSpan.FromSeconds(60);

                string? workspaceSlug = await ResolveAnythingWorkspaceSlugAsync(client, baseUrl, cancellationToken);
                if (string.IsNullOrWhiteSpace(workspaceSlug))
                {
                    WriteEvalLog("ERROR: Không lấy được workspace slug hợp lệ từ AnythingLLM.");
                    return ("Err", "Không lấy được workspace slug", "");
                }

                string url = $"{baseUrl}/api/v1/workspace/{Uri.EscapeDataString(workspaceSlug)}/chat";

                WriteEvalLog("===== AnythingLLM REQUEST =====");
                WriteEvalLog($"POST {url}");
                WriteEvalLog($"Workspace slug: {workspaceSlug}");
                WriteEvalLog($"Authorization: Bearer {maskedApiKey}");
                WriteEvalLog($"Body: {jsonBody}");

                var response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"), cancellationToken);
                string raw = await response.Content.ReadAsStringAsync();

                WriteEvalLog("===== AnythingLLM RESPONSE =====");
                WriteEvalLog($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                WriteEvalLog($"Body: {raw}");

                if (!response.IsSuccessStatusCode)
                {
                    WriteEvalLog($"ERROR: AnythingLLM trả về HTTP {(int)response.StatusCode}. Kiểm tra API key, workspace slug, hoặc server AnythingLLM.");
                    return ("Err", $"HTTP {(int)response.StatusCode}: {raw}", "");
                }

                string textResponse = "";
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    if (doc.RootElement.TryGetProperty("textResponse", out var tr))
                    {
                        textResponse = tr.GetString() ?? "";
                    }
                    else
                    {
                        WriteEvalLog("ERROR: Response JSON không có field 'textResponse'.");
                    }
                }
                catch (JsonException jsonEx)
                {
                    WriteEvalLog($"ERROR: Không parse được response JSON: {jsonEx.Message}");
                    return ("Err", jsonEx.Message, "");
                }

                if (string.IsNullOrWhiteSpace(textResponse))
                {
                    WriteEvalLog("ERROR: textResponse rỗng, nên không có bản dịch/đánh giá để hiển thị.");
                    return ("Err", "textResponse rỗng", "");
                }

                WriteEvalLog($"textResponse: {textResponse}");

                var parsed = ParseEvaluationResponse(textResponse);
                if (parsed.Score == "?" && string.IsNullOrEmpty(parsed.Comment) && string.IsNullOrEmpty(parsed.Suggested))
                {
                    WriteEvalLog("ERROR: Không thể parse kết quả đánh giá từ textResponse.");
                    return ("Err", textResponse, "");
                }
                return parsed;
            }
            catch (Exception ex)
            {
                WriteEvalLog($"ERROR: Lỗi khi gọi AnythingLLM: {ex.GetType().Name}: {ex.Message}");
                return ("Err", ex.Message, "");
            }
        }

        private (string Score, string Comment, string Suggested) ParseEvaluationResponse(string textResponse)
        {
            string score = "?";
            string comment = "";
            string suggested = "";

            if (string.IsNullOrWhiteSpace(textResponse))
            {
                return (score, comment, suggested);
            }

            textResponse = textResponse.Trim();

            // Loại bỏ các bọc markdown ```json ... ``` hoặc ``` ... ``` nếu có
            if (textResponse.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
            {
                textResponse = textResponse.Substring(7);
            }
            else if (textResponse.StartsWith("```"))
            {
                textResponse = textResponse.Substring(3);
            }

            if (textResponse.EndsWith("```"))
            {
                textResponse = textResponse.Substring(0, textResponse.Length - 3);
            }
            textResponse = textResponse.Trim();

            // 1. Tìm cặp dấu ngoặc nhọn hoặc ngoặc vuông để lấy chuỗi JSON
            int start = textResponse.IndexOf('{');
            int end = textResponse.LastIndexOf('}');

            // Nếu không tìm thấy {}, kiểm tra xem có [] không (phòng trường hợp dùng ngoặc vuông thay vì ngoặc nhọn)
            if (start < 0 || end < 0)
            {
                int startSq = textResponse.IndexOf('[');
                int endSq = textResponse.LastIndexOf(']');
                if (startSq >= 0 && endSq > startSq)
                {
                    // Nếu là mảng chứa object [{...}]
                    int startInside = textResponse.IndexOf('{', startSq);
                    int endInside = textResponse.LastIndexOf('}', endSq);
                    if (startInside >= 0 && endInside > startInside)
                    {
                        start = startInside;
                        end = endInside;
                    }
                    else
                    {
                        // Nếu là dạng [ "score": 7, ... ] thì thay thế ngoặc vuông bằng ngoặc nhọn để parse thử
                        string rawContent = textResponse.Substring(startSq + 1, endSq - startSq - 1).Trim();
                        textResponse = "{" + rawContent + "}";
                        start = 0;
                        end = textResponse.Length - 1;
                    }
                }
            }

            if (start >= 0 && end > start)
            {
                string jsonPart = textResponse.Substring(start, end - start + 1);
                try
                {
                    // Sửa một số lỗi phổ biến của LLM trước khi parse
                    string fixedJson = jsonPart.Replace("\"suggested:\":", "\"suggested\":")
                                               .Replace("\"suggested:\"", "\",\"suggested\":\"");
                    using var doc = JsonDocument.Parse(fixedJson);
                    
                    if (doc.RootElement.TryGetProperty("score", out var s))
                        score = s.ToString();
                    
                    if (doc.RootElement.TryGetProperty("comment", out var c))
                        comment = c.ValueKind == JsonValueKind.String ? c.GetString() ?? "" : c.ToString();
                    
                    if (doc.RootElement.TryGetProperty("suggested", out var sg))
                        suggested = sg.ValueKind == JsonValueKind.String ? sg.GetString() ?? "" : sg.ToString();

                    return (score, comment, suggested);
                }
                catch (JsonException)
                {
                    // Nếu lỗi parse JSON, tiếp tục chạy Regex trên jsonPart
                    return ParseUsingRegex(jsonPart);
                }
            }

            // 2. Nếu không tìm thấy dấu ngoặc nào, dùng Regex trực tiếp trên toàn bộ textResponse
            return ParseUsingRegex(textResponse);
        }

        private (string Score, string Comment, string Suggested) ParseUsingRegex(string input)
        {
            string score = "?";
            string comment = "";
            string suggested = "";

            // Trích xuất score
            var matchScore = System.Text.RegularExpressions.Regex.Match(input, @"\""score\""\s*:\s*(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!matchScore.Success)
            {
                matchScore = System.Text.RegularExpressions.Regex.Match(input, @"\bscore\b\s*:\s*(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            if (matchScore.Success)
            {
                score = matchScore.Groups[1].Value;
            }

            // Trích xuất comment
            var matchComment = System.Text.RegularExpressions.Regex.Match(input, @"\""comment\""\s*:\s*\""([\s\S]*?)\""\s*(?:,|\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!matchComment.Success)
            {
                matchComment = System.Text.RegularExpressions.Regex.Match(input, @"\bcomment\b\s*:\s*\""([\s\S]*?)\""\s*(?:,|\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            if (!matchComment.Success)
            {
                matchComment = System.Text.RegularExpressions.Regex.Match(input, @"\bcomment\b\s*:\s*([\s\S]*?)(?:\r?\n|,?\s*\""suggested|\r?\n?\s*suggested|\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            if (matchComment.Success)
            {
                comment = matchComment.Groups[1].Value.Trim().TrimStart('"', ':').TrimEnd('"', ',', ' ').Trim();
            }

            // Trích xuất suggested
            var matchSuggested = System.Text.RegularExpressions.Regex.Match(input, @"\""suggested\""\s*:\s*\""([\s\S]*?)\""\s*(?:,|\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!matchSuggested.Success)
            {
                matchSuggested = System.Text.RegularExpressions.Regex.Match(input, @"\bsuggested\b\s*:\s*\""([\s\S]*?)\""\s*(?:,|\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            if (!matchSuggested.Success)
            {
                matchSuggested = System.Text.RegularExpressions.Regex.Match(input, @"\bsuggested\b\s*:\s*([\s\S]*?)(?:\r?\n|\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            if (matchSuggested.Success)
            {
                suggested = matchSuggested.Groups[1].Value.Trim().TrimStart('"', ':').TrimEnd('"', ',', ' ').Trim();
            }

            return (score, comment, suggested);
        }

        private async Task<string?> ResolveAnythingWorkspaceSlugAsync(HttpClient client, string baseUrl, CancellationToken cancellationToken)
        {
            string url = $"{baseUrl}/api/v1/workspaces";
            WriteEvalLog("===== AnythingLLM WORKSPACES =====");
            WriteEvalLog($"GET {url}");

            try
            {
                var response = await client.GetAsync(url, cancellationToken);
                string raw = await response.Content.ReadAsStringAsync();
                WriteEvalLog($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                WriteEvalLog($"Body: {raw}");

                if (!response.IsSuccessStatusCode)
                {
                    WriteEvalLog($"ERROR: Không lấy được danh sách workspace. HTTP {(int)response.StatusCode}.");
                    return null;
                }

                using var doc = JsonDocument.Parse(raw);
                if (!doc.RootElement.TryGetProperty("workspaces", out var workspaces) || workspaces.ValueKind != JsonValueKind.Array)
                {
                    WriteEvalLog("ERROR: Response /workspaces không có mảng 'workspaces'.");
                    return null;
                }

                string? firstSlug = null;
                foreach (var workspace in workspaces.EnumerateArray())
                {
                    string slug = workspace.TryGetProperty("slug", out var slugProp) ? slugProp.GetString() ?? "" : "";
                    string name = workspace.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
                    if (string.IsNullOrWhiteSpace(slug)) continue;

                    WriteEvalLog($"Workspace found: name='{name}', slug='{slug}'");
                    firstSlug ??= slug;

                    if (string.Equals(slug, "default", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(name, "default", StringComparison.OrdinalIgnoreCase))
                    {
                        WriteEvalLog($"Using workspace slug: {slug}");
                        return slug;
                    }
                }

                if (!string.IsNullOrWhiteSpace(firstSlug))
                {
                    WriteEvalLog($"Không thấy workspace tên default, dùng workspace đầu tiên: {firstSlug}");
                    return firstSlug;
                }

                WriteEvalLog("ERROR: Danh sách workspace rỗng hoặc không có slug.");
                return null;
            }
            catch (Exception ex)
            {
                WriteEvalLog($"ERROR: Lỗi khi gọi /api/v1/workspaces: {ex.GetType().Name}: {ex.Message}");
                return null;
            }
        }

        private async void dgvEval_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvEval.Columns[e.ColumnIndex].Name;
            var row = dgvEval.Rows[e.RowIndex];

            if (colName == "btnAcceptEval")
            {
                string suggested  = row.Cells["SuggestedTrans"].Value?.ToString() ?? "";
                string vietnamese = row.Cells["Vietnamese"].Value?.ToString() ?? "";
                string toSave     = string.IsNullOrWhiteSpace(suggested) ? vietnamese : suggested;

                string context = row.Cells["Context"].Value?.ToString() ?? "";
                string english = row.Cells["English"].Value?.ToString() ?? "";

                if (!string.IsNullOrEmpty(_evalFilePath) && System.IO.File.Exists(_evalFilePath))
                {
                    bool savedSuccessfully = false;
                    try
                    {
                        await _saveSemaphore.WaitAsync();
                        await Task.Run(() =>
                        {
                            var map = new Dictionary<string, string>
                            {
                                { context + "|" + english, toSave }
                            };
                            savedSuccessfully = UpdatePoFileWithMap(_evalFilePath, map);
                        });
                    }
                    catch (Exception ex)
                    {
                        WriteEvalLog($"❌ Lỗi nghiêm trọng khi lưu: {ex.Message}");
                        return;
                    }
                    finally
                    {
                        _saveSemaphore.Release();
                    }

                    if (!savedSuccessfully)
                    {
                        WriteEvalLog($"❌ LỖI: Không tìm thấy dòng khớp trong file PO để lưu! (Context: '{context}', English: '{english}')");
                        return;
                    }

                    row.Cells["Vietnamese"].Value = toSave;
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.PaleGreen;
                    
                    if (_allEvalEntries != null)
                    {
                        var entry = _allEvalEntries.FirstOrDefault(e => e.Context == context && e.Id == english);
                        if (entry != null)
                        {
                            entry.CurrentStr = toSave;
                        }
                    }

                    WriteEvalLog($"✔ Chấp nhận: \"{english}\" → \"{toSave}\"");
                }
            }
            else if (colName == "btnRejectEval")
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                WriteEvalLog($"✘ Bỏ qua: \"{row.Cells["English"].Value}\"");
            }
        }

        private async void dgvEval_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvEval.Columns[e.ColumnIndex].Name;
            if (colName == "Vietnamese")
            {
                var row = dgvEval.Rows[e.RowIndex];
                string context = row.Cells["Context"].Value?.ToString() ?? "";
                string english = row.Cells["English"].Value?.ToString() ?? "";
                string newValue = row.Cells["Vietnamese"].Value?.ToString() ?? "";

                if (!string.IsNullOrEmpty(_evalFilePath) && System.IO.File.Exists(_evalFilePath))
                {
                    bool savedSuccessfully = false;
                    try
                    {
                        await _saveSemaphore.WaitAsync();
                        await Task.Run(() =>
                        {
                            var map = new Dictionary<string, string>
                            {
                                { context + "|" + english, newValue }
                            };
                            savedSuccessfully = UpdatePoFileWithMap(_evalFilePath, map);
                        });
                    }
                    catch (Exception ex)
                    {
                        WriteEvalLog($"❌ Lỗi nghiêm trọng khi lưu chỉnh sửa thủ công: {ex.Message}");
                        return;
                    }
                    finally
                    {
                        _saveSemaphore.Release();
                    }

                    if (!savedSuccessfully)
                    {
                        WriteEvalLog($"❌ LỖI: Không tìm thấy dòng khớp trong file PO để lưu! (Context: '{context}', English: '{english}')");
                        return;
                    }

                    row.DefaultCellStyle.BackColor = System.Drawing.Color.PaleGreen;

                    if (_allEvalEntries != null)
                    {
                        var entry = _allEvalEntries.FirstOrDefault(ent => ent.Context == context && ent.Id == english);
                        if (entry != null)
                        {
                            entry.CurrentStr = newValue;
                        }
                    }

                    WriteEvalLog($"✔ Đã lưu chỉnh sửa thủ công: \"{english}\" → \"{newValue}\"");
                }
            }
        }

        private void WriteEvalLog(string message)
        {
            if (rtbLogEval.InvokeRequired)
            {
                rtbLogEval.Invoke(new Action<string>(WriteEvalLog), message);
                return;
            }

            string time = DateTime.Now.ToString("HH:mm:ss");
            rtbLogEval.AppendText($"[{time}] {message}{Environment.NewLine}");
            rtbLogEval.SelectionStart = rtbLogEval.Text.Length;
            rtbLogEval.ScrollToCaret();

            System.Diagnostics.Debug.WriteLine($"[EVAL LOG][{time}] {message}");
        }

        private void BtnExportEval_Click(object? sender, EventArgs e)
        {
            if (dgvEval.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất. Vui lòng mở file PO và chạy đánh giá trước.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chỉ xuất các hàng đã được đánh giá (EvalScore không rỗng, không phải "...")
            var evaluatedRows = dgvEval.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r =>
                {
                    string score = r.Cells["EvalScore"].Value?.ToString() ?? "";
                    return !string.IsNullOrWhiteSpace(score) && score != "...";
                })
                .ToList();

            if (evaluatedRows.Count == 0)
            {
                MessageBox.Show("Chưa có hàng nào được đánh giá. Hãy chạy 'Đánh giá & Dịch' trước khi xuất.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tên file companion: <pofilename>_eval.csv, cùng thư mục với file PO
            string defaultCsvName = string.IsNullOrEmpty(_evalFilePath)
                ? $"eval_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                : Path.GetFileNameWithoutExtension(_evalFilePath) + "_eval.csv";
            string defaultCsvDir = string.IsNullOrEmpty(_evalFilePath)
                ? ""
                : Path.GetDirectoryName(_evalFilePath) ?? "";

            using var sfd = new SaveFileDialog
            {
                Title = "Lưu kết quả đánh giá",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = defaultCsvName,
                InitialDirectory = defaultCsvDir,
                DefaultExt = "csv"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                string evalDateStr = _lastEvalDateTime != DateTime.MinValue
                    ? _lastEvalDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var sb = new StringBuilder();

                // Header — dùng dấu phẩy, bao trong ngoặc kép
                sb.AppendLine("\"Ngày đánh giá\",\"Ngữ cảnh\",\"Tiếng Anh\",\"Tiếng Việt (hiện tại)\",\"Bản dịch đề xuất (AI)\",\"Điểm\",\"Nhận xét\"");

                foreach (var row in evaluatedRows)
                {
                    string context  = EscapeCsvField(row.Cells["Context"].Value?.ToString() ?? "");
                    string english  = EscapeCsvField(row.Cells["English"].Value?.ToString() ?? "");
                    string viet     = EscapeCsvField(row.Cells["Vietnamese"].Value?.ToString() ?? "");
                    string suggested = EscapeCsvField(row.Cells["SuggestedTrans"].Value?.ToString() ?? "");
                    string score    = EscapeCsvField(row.Cells["EvalScore"].Value?.ToString() ?? "");
                    string comment  = EscapeCsvField(row.Cells["EvalComment"].Value?.ToString() ?? "");

                    sb.AppendLine($"{EscapeCsvField(evalDateStr)},{context},{english},{viet},{suggested},{score},{comment}");
                }

                // Ghi với UTF-8 BOM để Excel mở đúng tiếng Việt
                File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                _evalCsvPath = sfd.FileName;
                WriteEvalLog($"✔ Đã xuất {evaluatedRows.Count} hàng ra file: {sfd.FileName}");
                MessageBox.Show($"Đã xuất {evaluatedRows.Count} hàng thành công!\n\nFile: {sfd.FileName}",
                    "Xuất CSV hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load kết quả đánh giá từ file CSV vào DataGridView.
        /// Key match: (Ngữ cảnh + Tiếng Anh). Trả về số hàng được fill.
        /// Phương thức này được gọi từ thread pool nên không được trực tiếp truy cập UI.
        /// Các thao tác UI được thực hiện qua Invoke.
        /// </summary>
        private int LoadEvalFromCsv(string csvPath)
        {
            if (!File.Exists(csvPath)) return 0;

            // Đọc CSV: cột 0=Ngày, 1=Ngữ cảnh, 2=Tiếng Anh, 3=Tiếng Việt,
            //             4=Đề xuất, 5=Điểm, 6=Nhận xét
            var lines = File.ReadAllLines(csvPath, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            // Build lookup: "context|english" -> (suggested, score, comment)
            var lookup = new Dictionary<string, (string suggested, string score, string comment)>(StringComparer.Ordinal);
            for (int i = 1; i < lines.Length; i++) // Bỏ qua header
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var fields = ParseCsvLine(line);
                if (fields.Count < 7) continue;

                string ctx  = fields[1];
                string eng  = fields[2];
                string sug  = fields[4];
                string scr  = fields[5];
                string cmt  = fields[6];

                // Bỏ qua hàng lỗi hoặc chưa đánh giá
                if (string.IsNullOrWhiteSpace(scr) || scr == "Lỗi" || scr == "Đã hủy") continue;

                string key = ctx + "|" + eng;
                lookup[key] = (sug, scr, cmt);
            }

            if (lookup.Count == 0) return 0;

            // Fill vào DataGridView trên UI thread
            int filled = 0;
            this.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow row in dgvEval.Rows)
                {
                    if (row.IsNewRow) continue;
                    string ctx = row.Cells["Context"].Value?.ToString() ?? "";
                    string eng = row.Cells["English"].Value?.ToString() ?? "";
                    string key = ctx + "|" + eng;

                    if (lookup.TryGetValue(key, out var result))
                    {
                        row.Cells["EvalScore"].Value    = result.score;
                        row.Cells["EvalComment"].Value  = result.comment;
                        row.Cells["SuggestedTrans"].Value = result.suggested;
                        // Tô màu vàng nhạt để phân biệt hàng load từ CSV
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                        filled++;
                    }
                }
            });
            return filled;
        }

        /// <summary>
        /// Parse một dòng CSV có thể chứa dấu phẩy và ngoặc kép bên trong field.
        /// </summary>
        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            int i = 0;
            while (i < line.Length)
            {
                if (line[i] == '"')
                {
                    i++; // bỏ qua dấu " mở
                    var sb = new StringBuilder();
                    while (i < line.Length)
                    {
                        if (line[i] == '"')
                        {
                            if (i + 1 < line.Length && line[i + 1] == '"')
                            {
                                sb.Append('"');
                                i += 2;
                            }
                            else
                            {
                                i++; // bỏ qua dấu " đóng
                                break;
                            }
                        }
                        else
                        {
                            sb.Append(line[i++]);
                        }
                    }
                    fields.Add(sb.ToString());
                    if (i < line.Length && line[i] == ',') i++; // bỏ qua dấu phẩy
                }
                else
                {
                    int comma = line.IndexOf(',', i);
                    if (comma == -1)
                    {
                        fields.Add(line.Substring(i));
                        break;
                    }
                    fields.Add(line.Substring(i, comma - i));
                    i = comma + 1;
                }
            }
            return fields;
        }

        private async void BtnLoadCsv_Click(object? sender, EventArgs e)
        {
            if (dgvEval.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng mở file PO trước khi tải kết quả.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string startDir = string.IsNullOrEmpty(_evalFilePath)
                ? ""
                : Path.GetDirectoryName(_evalFilePath) ?? "";

            using var ofd = new OpenFileDialog
            {
                Title = "Chọn file kết quả đánh giá (CSV)",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                InitialDirectory = startDir
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                btnLoadCsv!.Enabled = false;
                btnLoadCsv.Text = "Đang tải...";

                int loaded = await Task.Run(() => LoadEvalFromCsv(ofd.FileName));
                _evalCsvPath = ofd.FileName;
                WriteEvalLog($"✔ Đã tải {loaded} kết quả từ: {ofd.FileName}");
                WriteEvalLog($"⏩ Các hàng đã tải (nền vàng) sẽ được bỏ qua khi chạy đánh giá.");

                MessageBox.Show($"Đã tải {loaded} kết quả vào bảng.\nCác hàng này sẽ được bỏ qua khi chạy đánh giá.",
                    "Tải CSV hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải CSV: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLoadCsv!.Enabled = true;
                btnLoadCsv.Text = "Tải CSV kết quả";
            }
        }

        /// <summary>
        /// Escape một giá trị để dùng trong CSV: bọc trong dấu ngoặc kép,
        /// escape các dấu ngoặc kép bên trong bằng cách nhân đôi.
        /// </summary>
        private static string EscapeCsvField(string value)
        {
            if (value == null) return "\"\"";
            return '"' + value.Replace("\"", "\"\"") + '"';
        }
    }
}
