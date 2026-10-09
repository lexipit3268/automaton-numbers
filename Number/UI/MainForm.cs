using Number.Core.Models;
using Number.Core.Services;

namespace Number.UI;

public class MainForm : Form
{
    private readonly IAutomatonService _engine = new DfaNumberEngine();

    // ── Controls ──────────────────────────────────────────────
    private TextBox          _inputBox     = null!;
    private Button           _checkBtn     = null!;
    private Button           _randomBtn    = null!;
    private Button           _clearBtn     = null!;
    private Panel            _resultPanel  = null!;
    private Label            _resultIcon   = null!;
    private Label            _resultType   = null!;
    private Label            _resultMsg    = null!;
    private Label            _pathLabel    = null!;
    private FlowLayoutPanel  _pathFlow     = null!;
    private bool             _lastIsValid;

    // ── Design Tokens ─────────────────────────────────────────
    static readonly Color BgPage    = Color.FromArgb(246, 247, 249);
    static readonly Color BgCard    = Color.White;
    static readonly Color Border    = Color.FromArgb(229, 231, 235);
    static readonly Color Primary   = Color.FromArgb(24,  24,  24);
    static readonly Color Secondary = Color.FromArgb(243, 244, 246);
    static readonly Color TextMain  = Color.FromArgb(17,  24,  39);
    static readonly Color TextMuted = Color.FromArgb(107, 114, 128);
    static readonly Color Success   = Color.FromArgb(22,  163,  74);
    static readonly Color Danger    = Color.FromArgb(220,  38,  38);
    static readonly Color SuccessBg = Color.FromArgb(240, 253, 244);
    static readonly Color DangerBg  = Color.FromArgb(254, 242, 242);

    public MainForm()
    {
        InitForm();
        BuildUI();
    }

    // ── Form ──────────────────────────────────────────────────
    void InitForm()
    {
        Text            = "Number Checker — DFA";
        ClientSize      = new Size(560, 520);
        MinimumSize     = new Size(480, 480);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = BgPage;
        Font            = new Font("Segoe UI", 10f);
    }

    // ── Build ─────────────────────────────────────────────────
    void BuildUI()
    {
        // Card chính, căn giữa với margin trang 24px
        var card = new Panel
        {
            BackColor = BgCard,
            Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            Location  = new Point(24, 24),
            Size      = new Size(ClientSize.Width - 48, ClientSize.Height - 48)
        };
        card.Paint += (_, e) => e.Graphics.DrawRectangle(new Pen(Border), 0, 0, card.Width - 1, card.Height - 1);
        Controls.Add(card);

        int x = 28, w = card.Width - 56;
        int y = 24;

        // ── Tiêu đề ──────────────────────────────────────────
        var title = MakeLabel("Kiểm tra giá trị số", new Font("Segoe UI", 14f, FontStyle.Bold), TextMain);
        title.SetBounds(x, y, w, 28);
        card.Controls.Add(title);
        y += 32;

        var subtitle = MakeLabel("Nhập hoặc sinh ngẫu nhiên một chuỗi để DFA phân tích", new Font("Segoe UI", 9f), TextMuted);
        subtitle.SetBounds(x, y, w, 18);
        card.Controls.Add(subtitle);
        y += 30;

        // ── Label ô nhập ──────────────────────────────────────
        var inputLabel = MakeLabel("Chuỗi đầu vào", new Font("Segoe UI", 9.5f, FontStyle.Bold), TextMain);
        inputLabel.SetBounds(x, y, w, 18);
        card.Controls.Add(inputLabel);
        y += 24;

        // ── Ô nhập ───────────────────────────────────────────
        _inputBox = new TextBox
        {
            Font            = new Font("Consolas", 11f),
            BorderStyle     = BorderStyle.FixedSingle,
            BackColor       = BgCard,
            ForeColor       = TextMain,
            PlaceholderText = "VD: 123  hoặc  -45.67  hoặc  +.5",
            Anchor          = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        _inputBox.SetBounds(x, y, w, 32);
        _inputBox.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { RunCheck(); e.SuppressKeyPress = true; } };
        card.Controls.Add(_inputBox);
        y += 44;

        // ── Hàng nút ─────────────────────────────────────────
        _checkBtn  = MakeBtn("Kiểm tra",        Primary,   Color.White, Primary);
        _randomBtn = MakeBtn("Sinh ngẫu nhiên",  Secondary, TextMain,   Border);
        _clearBtn  = MakeBtn("Xoá",              BgCard,    TextMuted,  Border);

        _checkBtn.Location  = new Point(x, y);
        _randomBtn.Location = new Point(x + _checkBtn.Width + 8, y);
        _clearBtn.Location  = new Point(x + _checkBtn.Width + _randomBtn.Width + 16, y);

        _checkBtn.Click  += (_, _) => RunCheck();
        _randomBtn.Click += (_, _) => RunRandom();
        _clearBtn.Click  += (_, _) => ClearAll();

        card.Controls.AddRange(new Control[] { _checkBtn, _randomBtn, _clearBtn });
        y += 48;

        // ── Panel kết quả ─────────────────────────────────────
        _resultPanel = new Panel
        {
            Visible   = false,
            BackColor = SuccessBg,
            Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        _resultPanel.SetBounds(x, y, w, 90);
        _resultPanel.Paint += (_, e) =>
        {
            var c = _lastIsValid ? Success : Danger;
            using var pen = new Pen(c);
            e.Graphics.DrawRectangle(pen, 0, 0, _resultPanel.Width - 1, _resultPanel.Height - 1);
        };
        card.Controls.Add(_resultPanel);

        // Bên trong result panel
        _resultIcon = MakeLabel("✓", new Font("Segoe UI", 18f), Success);
        _resultIcon.SetBounds(16, 16, 32, 32);

        _resultType = MakeLabel("", new Font("Segoe UI", 16f, FontStyle.Bold), Success);
        _resultType.SetBounds(52, 12, _resultPanel.Width - 68, 30);
        _resultType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _resultMsg = MakeLabel("", new Font("Segoe UI", 9f), TextMuted);
        _resultMsg.SetBounds(52, 48, _resultPanel.Width - 68, 32);
        _resultMsg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _resultPanel.Controls.AddRange(new Control[] { _resultIcon, _resultType, _resultMsg });
        y += 102;

        // ── Đường phân cách ───────────────────────────────────
        var sep = new Panel { BackColor = Border, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        sep.SetBounds(x, y, w, 1);
        card.Controls.Add(sep);
        y += 16;

        // ── Nhãn lộ trình ─────────────────────────────────────
        _pathLabel = MakeLabel("Lộ trình trạng thái DFA", new Font("Segoe UI", 9.5f, FontStyle.Bold), TextMain);
        _pathLabel.SetBounds(x, y, w, 18);
        _pathLabel.Visible = false;
        card.Controls.Add(_pathLabel);
        y += 24;

        // ── Flow lộ trình (có scroll ngang) ───────────────────
        _pathFlow = new FlowLayoutPanel
        {
            FlowDirection  = FlowDirection.LeftToRight,
            WrapContents   = false,
            AutoScroll     = true,
            Visible        = false,
            BackColor      = Color.Transparent,
            Anchor         = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        _pathFlow.SetBounds(x, y, w, 40);
        card.Controls.Add(_pathFlow);
    }

    // ── Logic ─────────────────────────────────────────────────
    void RunCheck()
    {
        string raw = _inputBox.Text.Trim();
        if (string.IsNullOrEmpty(raw)) { _inputBox.Focus(); return; }
        var result = _engine.Analyze(raw);
        ShowResult(result);
        ShowPath(result.ExecutionPath);
    }

    void RunRandom()
    {
        string rnd = _engine.GenerateRandom();
        _inputBox.Text = rnd;
        var result = _engine.Analyze(rnd);
        ShowResult(result);
        ShowPath(result.ExecutionPath);
    }

    void ClearAll()
    {
        _inputBox.Clear();
        _resultPanel.Visible = false;
        _pathFlow.Visible    = false;
        _pathLabel.Visible   = false;
        _pathFlow.Controls.Clear();
        _inputBox.Focus();
    }

    // ── Hiển thị kết quả ──────────────────────────────────────
    void ShowResult(AutomatonResult r)
    {
        _lastIsValid = r.IsValid;

        if (r.IsValid)
        {
            _resultPanel.BackColor = SuccessBg;
            _resultIcon.Text       = "✓";
            _resultIcon.ForeColor  = Success;
            _resultType.Text       = r.NumberType;
            _resultType.ForeColor  = Success;
            _resultMsg.Text        = $"Chuỗi \"{r.Input}\" là một số hợp lệ.";
            _resultMsg.ForeColor   = TextMuted;
        }
        else
        {
            _resultPanel.BackColor = DangerBg;
            _resultIcon.Text       = "✗";
            _resultIcon.ForeColor  = Danger;
            _resultType.Text       = "Không hợp lệ";
            _resultType.ForeColor  = Danger;
            _resultMsg.Text        = $"Chuỗi \"{r.Input}\" không phải là số.";
            _resultMsg.ForeColor   = TextMuted;
        }
        _resultPanel.Visible = true;
        _resultPanel.Invalidate();
    }

    // ── Lộ trình trạng thái ───────────────────────────────────
    void ShowPath(List<State> path)
    {
        _pathFlow.SuspendLayout();
        _pathFlow.Controls.Clear();

        for (int i = 0; i < path.Count; i++)
        {
            bool isLast  = i == path.Count - 1;
            bool isErr   = path[i] == State.q_err;
            bool isAccept = isLast && path[i] is State.q2 or State.q5;

            var chip = new Label
            {
                Text      = path[i].ToString(),
                Font      = new Font("Consolas", 9.5f),
                ForeColor = isErr ? Danger : isAccept ? Success : TextMain,
                BackColor = isErr ? DangerBg : isAccept ? SuccessBg : Secondary,
                AutoSize  = false,
                Size      = new Size(52, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin    = new Padding(0, 4, 0, 0)
            };
            _pathFlow.Controls.Add(chip);

            if (!isLast)
            {
                var arrow = new Label
                {
                    Text      = "→",
                    Font      = new Font("Segoe UI", 9f),
                    ForeColor = TextMuted,
                    AutoSize  = false,
                    Size      = new Size(20, 28),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin    = new Padding(0, 4, 0, 0)
                };
                _pathFlow.Controls.Add(arrow);
            }
        }

        _pathFlow.ResumeLayout();
        _pathLabel.Visible = true;
        _pathFlow.Visible  = true;
    }

    // ── Helpers ───────────────────────────────────────────────
    static Label MakeLabel(string text, Font font, Color fg) => new()
    {
        Text      = text,
        Font      = font,
        ForeColor = fg,
        AutoSize  = false
    };

    static Button MakeBtn(string text, Color bg, Color fg, Color border)
    {
        var btn = new Button
        {
            Text      = text,
            Font      = new Font("Segoe UI", 9f),
            BackColor = bg,
            ForeColor = fg,
            FlatStyle = FlatStyle.Flat,
            Size      = new Size(TextRenderer.MeasureText(text, new Font("Segoe UI", 9f)).Width + 24, 36),
            Cursor    = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        btn.FlatAppearance.BorderColor = border;
        btn.FlatAppearance.BorderSize  = 1;
        return btn;
    }
}
