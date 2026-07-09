using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyTranslateDST
{
    public partial class Form1
    {
        private TabPage tabPageGemini;
        private RichTextBox rtbLogGemini;
        private DataGridView dgvGemini;
        private Label lbLimitGemini;
        private NumericUpDown numLimitGemini;
        private Label lbGeminiApiKey;
        private TextBox txtGeminiApiKey;
        private Label lbGeminiModel;
        private ComboBox cbGeminiModel;
        private Label lbDuongDanBanDichGemini;
        private Button btnMoBanDichGemini;
        private Button btnGeminiTranslate;
        private Label lbContextFilterGemini;
        private ComboBox cbContextFilterGemini;

        private string _geminiFilePath;
        private List<PoEntry> _allGeminiEntries = new List<PoEntry>();

        public void InitializeGeminiTab()
        {
            tabPageGemini = new TabPage();
            rtbLogGemini = new RichTextBox();
            dgvGemini = new DataGridView();
            lbLimitGemini = new Label();
            numLimitGemini = new NumericUpDown();
            lbGeminiApiKey = new Label();
            txtGeminiApiKey = new TextBox();
            lbGeminiModel = new Label();
            cbGeminiModel = new ComboBox();
            lbDuongDanBanDichGemini = new Label();
            btnMoBanDichGemini = new Button();
            btnGeminiTranslate = new Button();

            // setup properties
            tabPageGemini.Text = "Đánh giá & Dịch (Gemini)";
            tabPageGemini.Location = new Point(4, 24);
            tabPageGemini.Padding = new Padding(3);
            tabPageGemini.Size = new Size(1152, 552);
            tabPageGemini.UseVisualStyleBackColor = true;

            btnMoBanDichGemini.Text = "Mở file PO";
            btnMoBanDichGemini.Location = new Point(18, 18);
            btnMoBanDichGemini.Size = new Size(180, 25);
            btnMoBanDichGemini.Click += BtnMoBanDichGemini_Click;

            lbDuongDanBanDichGemini.Text = "Đường dẫn file";
            lbDuongDanBanDichGemini.Location = new Point(210, 23);
            lbDuongDanBanDichGemini.AutoSize = true;

            lbGeminiModel.Text = "Model:";
            lbGeminiModel.Location = new Point(18, 55);
            lbGeminiModel.AutoSize = true;

            cbGeminiModel.Location = new Point(70, 52);
            cbGeminiModel.Size = new Size(150, 23);
            cbGeminiModel.Items.AddRange(new object[] { "gemini-2.5-flash", "gemini-2.0-flash", "gemini-3-flash", "gemini-2.5-flash-lite" });
            cbGeminiModel.SelectedIndex = 0;

            lbGeminiApiKey.Text = "Gemini API Key:";
            lbGeminiApiKey.Location = new Point(230, 55);
            lbGeminiApiKey.AutoSize = true;

            txtGeminiApiKey.Location = new Point(330, 52);
            txtGeminiApiKey.Size = new Size(200, 23);

            lbLimitGemini.Text = "Limit rows:";
            lbLimitGemini.Location = new Point(550, 55);
            lbLimitGemini.AutoSize = true;

            numLimitGemini.Location = new Point(620, 52);
            numLimitGemini.Size = new Size(60, 23);
            numLimitGemini.Maximum = 10000;
            numLimitGemini.Value = 100;

            btnGeminiTranslate.Text = "Đánh giá && Dịch";
            btnGeminiTranslate.Location = new Point(700, 50);
            btnGeminiTranslate.Size = new Size(140, 25);
            btnGeminiTranslate.Click += BtnGeminiTranslate_Click;

            dgvGemini.Location = new Point(18, 85);
            dgvGemini.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvGemini.Size = new Size(745, 449);
            dgvGemini.AllowUserToAddRows = false;
            dgvGemini.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            rtbLogGemini.Location = new Point(781, 85);
            rtbLogGemini.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            rtbLogGemini.Size = new Size(355, 449);
            rtbLogGemini.BackColor = Color.Black;
            rtbLogGemini.ForeColor = Color.Lime;
            rtbLogGemini.ReadOnly = true;

            lbContextFilterGemini = new Label();
            lbContextFilterGemini.Text = "Lọc theo:";
            lbContextFilterGemini.AutoSize = true;
            lbContextFilterGemini.Location = new Point(850, 55);

            cbContextFilterGemini = new ComboBox();
            cbContextFilterGemini.DropDownStyle = ComboBoxStyle.DropDown;
            cbContextFilterGemini.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbContextFilterGemini.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbContextFilterGemini.Location = new Point(910, 52);
            cbContextFilterGemini.Size = new Size(220, 23);
            cbContextFilterGemini.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbContextFilterGemini.SelectedIndexChanged += CbContextFilterGemini_SelectedIndexChanged;
            cbContextFilterGemini.TextUpdate += CbContextFilterGemini_TextUpdate;

            tabPageGemini.Controls.Add(btnMoBanDichGemini);
            tabPageGemini.Controls.Add(lbDuongDanBanDichGemini);
            tabPageGemini.Controls.Add(lbGeminiModel);
            tabPageGemini.Controls.Add(cbGeminiModel);
            tabPageGemini.Controls.Add(lbGeminiApiKey);
            tabPageGemini.Controls.Add(txtGeminiApiKey);
            tabPageGemini.Controls.Add(lbLimitGemini);
            tabPageGemini.Controls.Add(numLimitGemini);
            tabPageGemini.Controls.Add(btnGeminiTranslate);
            tabPageGemini.Controls.Add(lbContextFilterGemini);
            tabPageGemini.Controls.Add(cbContextFilterGemini);
            tabPageGemini.Controls.Add(dgvGemini);
            tabPageGemini.Controls.Add(rtbLogGemini);

            SetupDgvGemini();

            this.tabPageDichTuConTrong.Controls.Add(tabPageGemini);
        }

        private void SetupDgvGemini()
        {
            dgvGemini.Columns.Clear();
            dgvGemini.Columns.Add("Context", "Ngữ cảnh");
            dgvGemini.Columns.Add("English", "Tiếng Anh (gốc)");
            dgvGemini.Columns.Add("Vietnamese", "Tiếng Việt (hiện tại)");
            dgvGemini.Columns.Add("GeminiTrans", "Dịch lại (Gemini)");
            dgvGemini.Columns.Add("EvalScore", "Điểm");
            dgvGemini.Columns.Add("EvalComment", "Nhận xét");

            var btnAccept = new DataGridViewButtonColumn();
            btnAccept.HeaderText = "Chấp nhận";
            btnAccept.Text = "OK";
            btnAccept.UseColumnTextForButtonValue = true;
            btnAccept.Name = "btnAcceptGemini";
            dgvGemini.Columns.Add(btnAccept);

            var btnReject = new DataGridViewButtonColumn();
            btnReject.HeaderText = "Bỏ qua";
            btnReject.Text = "Skip";
            btnReject.UseColumnTextForButtonValue = true;
            btnReject.Name = "btnRejectGemini";
            dgvGemini.Columns.Add(btnReject);

            dgvGemini.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGemini.Columns["Context"].FillWeight = 25;
            dgvGemini.Columns["English"].FillWeight = 40;
            dgvGemini.Columns["Vietnamese"].FillWeight = 40;
            dgvGemini.Columns["GeminiTrans"].FillWeight = 40;
            dgvGemini.Columns["EvalScore"].FillWeight = 15;
            dgvGemini.Columns["EvalComment"].FillWeight = 45;
            dgvGemini.Columns["btnAcceptGemini"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvGemini.Columns["btnAcceptGemini"].Width = 80;
            dgvGemini.Columns["btnRejectGemini"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvGemini.Columns["btnRejectGemini"].Width = 70;

            dgvGemini.CellContentClick += DgvGemini_CellContentClick;
        }

        private void WriteGeminiLog(string message)
        {
            if (rtbLogGemini.InvokeRequired)
            {
                rtbLogGemini.Invoke(new Action(() => WriteGeminiLog(message)));
                return;
            }
            rtbLogGemini.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            rtbLogGemini.SelectionStart = rtbLogGemini.Text.Length;
            rtbLogGemini.ScrollToCaret();
        }

        private async void BtnMoBanDichGemini_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog fbd = new OpenFileDialog
            {
                Title = "Chọn file PO để đánh giá & dịch với Gemini",
                Filter = "PO Files (*.po)|*.po|All Files (*.*)|*.*"
            };

            if (fbd.ShowDialog() != DialogResult.OK) return;

            _geminiFilePath = fbd.FileName;
            lbDuongDanBanDichGemini.Text = _geminiFilePath;

            try
            {
                btnMoBanDichGemini.Enabled = false;
                btnMoBanDichGemini.Text = "Đang xử lý...";
                dgvGemini.Rows.Clear();
                rtbLogGemini.Clear();
                WriteGeminiLog($"Đang tải dữ liệu từ: {_geminiFilePath}");

                _allGeminiEntries = await Task.Run(() => ProcessPoFileForAllEntries(_geminiFilePath));

                var prefixes = _allGeminiEntries
                    .Select(entry => entry.Prefix)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList();

                cbContextFilterGemini.SelectedIndexChanged -= CbContextFilterGemini_SelectedIndexChanged;
                cbContextFilterGemini.Items.Clear();
                cbContextFilterGemini.Items.Add("Tất cả");
                foreach (var p in prefixes)
                {
                    cbContextFilterGemini.Items.Add(p);
                }
                cbContextFilterGemini.SelectedIndex = 0;
                cbContextFilterGemini.SelectedIndexChanged += CbContextFilterGemini_SelectedIndexChanged;

                PopulateDgvGemini(_allGeminiEntries);
                WriteGeminiLog($"Đã tải {_allGeminiEntries.Count} mục.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnMoBanDichGemini.Enabled = true;
                btnMoBanDichGemini.Text = "Mở file PO";
            }
        }

        private void CbContextFilterGemini_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_allGeminiEntries == null || cbContextFilterGemini.SelectedItem == null) return;

            string selected = cbContextFilterGemini.SelectedItem.ToString() ?? "";
            if (selected == "Tất cả")
            {
                PopulateDgvGemini(_allGeminiEntries);
            }
            else
            {
                var filtered = _allGeminiEntries.Where(entry => entry.Prefix == selected).ToList();
                PopulateDgvGemini(filtered);
            }
        }

        private void CbContextFilterGemini_TextUpdate(object? sender, EventArgs e)
        {
            if (_allGeminiEntries == null) return;

            string filterText = cbContextFilterGemini.Text.Trim();
            if (string.IsNullOrEmpty(filterText) || filterText == "Tất cả")
            {
                PopulateDgvGemini(_allGeminiEntries);
            }
            else
            {
                var filtered = _allGeminiEntries.Where(entry =>
                    (entry.Prefix != null && entry.Prefix.Contains(filterText, StringComparison.OrdinalIgnoreCase)) ||
                    (entry.Context != null && entry.Context.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                ).ToList();
                PopulateDgvGemini(filtered);
            }
        }

        private void PopulateDgvGemini(List<PoEntry> entries)
        {
            dgvGemini.SuspendLayout();
            dgvGemini.Rows.Clear();

            var rows = new DataGridViewRow[entries.Count];
            for (int i = 0; i < entries.Count; i++)
            {
                var row = new DataGridViewRow();
                // Context, English, Vietnamese, GeminiTrans, EvalScore, EvalComment
                row.CreateCells(dgvGemini, entries[i].Context, entries[i].Id, entries[i].CurrentStr, "", "", "");
                rows[i] = row;
            }
            dgvGemini.Rows.AddRange(rows);
            dgvGemini.ResumeLayout();
        }

        private async void BtnGeminiTranslate_Click(object? sender, EventArgs e)
        {
            if (dgvGemini.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng mở file PO trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string apiKey = txtGeminiApiKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Vui lòng nhập API key Gemini.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string model = cbGeminiModel.SelectedItem?.ToString() ?? "gemini-2.5-flash";

            try
            {
                btnGeminiTranslate.Enabled = false;
                btnGeminiTranslate.Text = "Đang đánh giá...";
                int limit = (int)numLimitGemini.Value;
                int processed = 0;

                WriteGeminiLog($"Bắt đầu đánh giá & dịch. Limit: {limit}, Model: {model}");

                foreach (DataGridViewRow row in dgvGemini.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (processed >= limit) break;

                    string context = row.Cells["Context"].Value?.ToString() ?? "";
                    string original = row.Cells["English"].Value?.ToString() ?? "";
                    string currentTrans = row.Cells["Vietnamese"].Value?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(original)) continue;

                    row.Cells["EvalScore"].Value = "...";

                    var result = await EvaluateWithGeminiApiAsync(model, apiKey, context, original, currentTrans);

                    if (result.Success)
                    {
                        row.Cells["EvalScore"].Value = result.Score;
                        row.Cells["EvalComment"].Value = result.Comment;
                        row.Cells["GeminiTrans"].Value = result.Suggested;

                        processed++;
                        WriteGeminiLog($"Đã đánh giá {processed}: {original} => Điểm: {result.Score}");
                    }
                    else
                    {
                        row.Cells["EvalScore"].Value = "Err";
                        WriteGeminiLog($"Lỗi đánh giá '{original}': {result.Error}");
                    }

                    // Delay to avoid hitting rate limits
                    await Task.Delay(500);
                }

                WriteGeminiLog($"Hoàn tất đánh giá {processed} mục.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGeminiTranslate.Enabled = true;
                btnGeminiTranslate.Text = "Đánh giá && Dịch";
            }
        }

        private async Task<(bool Success, string Score, string Comment, string Suggested, string Error)> EvaluateWithGeminiApiAsync(
            string model, string apiKey, string context, string original, string currentTrans)
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
                        return (true, "10", "Từ khóa giữ nguyên không dịch.", matchedWord, "");
                    }
                }

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
                    $"1. Cho điểm chất lượng bản dịch hiện tại (msgstr) dựa trên ngữ cảnh được cung cấp từ 1-10 (chỉ số nguyên).\n" +
                    $"2. Nhận xét ngắn gọn về bản dịch hiện tại (1-2 câu).\n" +
                    $"3. Đề xuất bản dịch tốt hơn cho phần msgid này nếu điểm < 8 (chú ý giữ đúng giọng điệu và ngữ cảnh nhân vật trong game), nếu không cần thì để trống.\n" +
                    noTranslateInstructions +
                    $"\nTrả lời ĐÚNG định dạng JSON sau, không thêm bất kỳ giải thích nào khác ngoài JSON:\n" +
                    $"{{\"score\":8,\"comment\":\"Nhận xét\",\"suggested\":\"Bản dịch đề xuất\"}}";

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = prompt } }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.3,
                        responseMimeType = "application/json"
                    }
                };

                string jsonBody = JsonSerializer.Serialize(payload);
                string maskedApiKey = apiKey.Length <= 8 ? "***" : $"{apiKey.Substring(0, 4)}...{apiKey.Substring(apiKey.Length - 4)}";
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
                string maskedUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={maskedApiKey}";

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);

                WriteGeminiLog("===== Gemini API REQUEST =====");
                WriteGeminiLog($"POST {maskedUrl}");
                WriteGeminiLog($"Body: {jsonBody}");

                var response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                string raw = await response.Content.ReadAsStringAsync();

                WriteGeminiLog("===== Gemini API RESPONSE =====");
                WriteGeminiLog($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                WriteGeminiLog($"Body: {raw}");

                if (!response.IsSuccessStatusCode)
                {
                    WriteGeminiLog($"ERROR: Gemini API trả về HTTP {(int)response.StatusCode}.");
                    return (false, "", "", "", $"HTTP {(int)response.StatusCode}: {raw}");
                }

                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var textResponse = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? "";

                    textResponse = textResponse.Trim();
                    if (textResponse.StartsWith("```json")) textResponse = textResponse.Substring(7);
                    if (textResponse.EndsWith("```")) textResponse = textResponse.Substring(0, textResponse.Length - 3);
                    textResponse = textResponse.Trim();

                    WriteGeminiLog($"textResponse: {textResponse}");

                    // Try to extract JSON from response
                    int start = textResponse.IndexOf('{');
                    int end = textResponse.LastIndexOf('}');
                    if (start >= 0 && end > start)
                    {
                        string jsonPart = textResponse.Substring(start, end - start + 1);

                        using var inner = JsonDocument.Parse(jsonPart);
                        string score = inner.RootElement.TryGetProperty("score", out var s) ? s.ToString() : "?";
                        string comment = inner.RootElement.TryGetProperty("comment", out var c) ? c.GetString() ?? "" : "";
                        string suggested = inner.RootElement.TryGetProperty("suggested", out var sg) ? sg.GetString() ?? "" : "";
                        WriteGeminiLog($"Parsed => score={score}, suggested={suggested}");
                        return (true, score, comment, suggested, "");
                    }

                    return (false, "", "", "", "Không tìm thấy JSON hợp lệ trong response");
                }
                catch (Exception ex)
                {
                    return (false, "", "", "", "Lỗi parse JSON: " + ex.Message + "\nRaw: " + raw);
                }
            }
            catch (Exception ex)
            {
                return (false, "", "", "", ex.Message);
            }
        }

        private async void DgvGemini_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvGemini.Columns[e.ColumnIndex].Name;
            var row = dgvGemini.Rows[e.RowIndex];

            if (colName == "btnAcceptGemini")
            {
                // Lấy bản dịch Gemini đề xuất để lưu vào file PO
                string geminiTrans = row.Cells["GeminiTrans"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(geminiTrans))
                {
                    WriteGeminiLog("⚠ Chưa có bản dịch đề xuất từ Gemini để áp dụng.");
                    return;
                }

                string context = row.Cells["Context"].Value?.ToString() ?? "";
                string english = row.Cells["English"].Value?.ToString() ?? "";

                if (!string.IsNullOrEmpty(_geminiFilePath) && System.IO.File.Exists(_geminiFilePath))
                {
                    bool savedSuccessfully = false;
                    try
                    {
                        await _saveSemaphore.WaitAsync();
                        await Task.Run(() =>
                        {
                            var map = new Dictionary<string, string>
                            {
                                { context + "|" + english, geminiTrans }
                            };
                            savedSuccessfully = UpdatePoFileWithMap(_geminiFilePath, map);
                        });
                    }
                    catch (Exception ex)
                    {
                        WriteGeminiLog($"❌ Lỗi nghiêm trọng khi lưu: {ex.Message}");
                        return;
                    }
                    finally
                    {
                        _saveSemaphore.Release();
                    }

                    if (!savedSuccessfully)
                    {
                        WriteGeminiLog($"❌ LỖI: Không tìm thấy dòng khớp trong file PO để lưu! (Context: '{context}', English: '{english}')");
                        return;
                    }

                    row.Cells["Vietnamese"].Value = geminiTrans;
                    row.Cells["GeminiTrans"].Style.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.BackColor = Color.Honeydew;
                    WriteGeminiLog($"✅ Đã chấp nhận & lưu: {english} -> {geminiTrans}");

                    var entry = _allGeminiEntries.FirstOrDefault(ent => ent.Context == context && ent.Id == english);
                    if (entry != null)
                    {
                        entry.CurrentStr = geminiTrans;
                    }
                }
            }
            else if (colName == "btnRejectGemini")
            {
                // Bỏ qua — đánh dấu dòng này là bỏ qua
                row.Cells["GeminiTrans"].Value = "";
                row.DefaultCellStyle.BackColor = Color.LightGray;
                string english = row.Cells["English"].Value?.ToString() ?? "";
                WriteGeminiLog($"⏭ Bỏ qua: {english}");
            }
        }
    }
}
