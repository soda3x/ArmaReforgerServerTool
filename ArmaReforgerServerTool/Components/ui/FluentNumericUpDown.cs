namespace Longbow.Components.ui
{
  using System;
  using System.ComponentModel;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Windows.Forms;

  public class FluentNumericUpDown : UserControl, ISupportInitialize
  {
    private TextBox textBox;
    private int borderRadius = 8;
    private Color borderColor = Color.FromArgb(120, 120, 120);
    private Color focusedBorderColor = Color.FromArgb(0, 120, 212); // Windows 11 Blue
    private Color fieldBackColor = SystemColors.Window;

    // Numeric State
    private decimal _value = 0;
    private decimal _minimum = 0;
    private decimal _maximum = 100;
    private decimal _increment = 1;
    private int _decimalPlaces = 0;

    // Interaction State
    private bool isHovered = false;
    private bool isFocused = false;
    private bool isUpHovered = false;
    private bool isDownHovered = false;

    private int spinnerWidth = 30; // Width of the up/down button area

    public event EventHandler ValueChanged;

    public FluentNumericUpDown()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
      this.BackColor = Color.Transparent;

      // Extra padding on the right to make room for our custom spinner buttons
      this.Padding = new Padding(10, 7, spinnerWidth + 5, 7);
      this.Size = new Size(150, 32);

      // Setup the internal borderless textbox
      textBox = new TextBox();
      textBox.BorderStyle = BorderStyle.None;
      textBox.Dock = DockStyle.Fill;
      textBox.BackColor = fieldBackColor;
      textBox.ForeColor = this.ForeColor;
      textBox.Text = _value.ToString();

      this.ForeColorChanged += (s, e) => textBox.ForeColor = this.ForeColor;

      // Interaction Hooks
      textBox.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
      textBox.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };
      this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
      this.MouseLeave += (s, e) => { isHovered = false; isUpHovered = false; isDownHovered = false; this.Invalidate(); };

      textBox.Enter += (s, e) => { isFocused = true; this.Invalidate(); };
      textBox.Leave += (s, e) =>
      {
        isFocused = false;
        ValidateAndApplyText(); // Parse the text when the user clicks away
        this.Invalidate();
      };

      // Text validation on Enter key
      textBox.KeyDown += (s, e) =>
      {
        if (e.KeyCode == Keys.Enter)
        {
          ValidateAndApplyText();
          e.Handled = true;
          e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Up)
        { Value += Increment; e.Handled = true; }
        else if (e.KeyCode == Keys.Down)
        { Value -= Increment; e.Handled = true; }
      };

      this.Controls.Add(textBox);
    }

    // --- Numeric Properties ---

    [Category("Data")]
    public decimal Value
    {
      get => _value;
      set
      {
        decimal clampedValue = Math.Max(_minimum, Math.Min(_maximum, value));
        if (_value != clampedValue)
        {
          _value = Math.Round(clampedValue, _decimalPlaces);
          textBox.Text = _value.ToString($"F{_decimalPlaces}");
          ValueChanged?.Invoke(this, EventArgs.Empty);
          this.Invalidate();
        }
      }
    }

    [Category("Data")]
    public decimal Minimum { get => _minimum; set { _minimum = value; ValidateAndApplyText(); } }

    [Category("Data")]
    public decimal Maximum { get => _maximum; set { _maximum = value; ValidateAndApplyText(); } }

    [Category("Data")]
    public decimal Increment { get => _increment; set => _increment = value; }

    [Category("Data")]
    public int DecimalPlaces
    {
      get => _decimalPlaces;
      set
      {
        _decimalPlaces = Math.Max(0, value);
        textBox.Text = _value.ToString($"F{_decimalPlaces}");
      }
    }

    [Category("Appearance")]
    public Color FieldBackColor
    {
      get => fieldBackColor;
      set { fieldBackColor = value; textBox.BackColor = value; this.Invalidate(); }
    }

    // --- Logic ---

    private void ValidateAndApplyText()
    {
      if (decimal.TryParse(textBox.Text, out decimal parsedValue))
      {
        Value = parsedValue; // The setter handles clamping and formatting
      }
      else
      {
        // Revert to last known good value if they typed garbage
        textBox.Text = _value.ToString($"F{_decimalPlaces}");
      }
    }

    // --- Custom Spinner Buttons (Mouse Handling) ---

    private Rectangle GetUpRect() => new Rectangle(this.Width - spinnerWidth - 2, 2, spinnerWidth, this.Height / 2 - 2);
    private Rectangle GetDownRect() => new Rectangle(this.Width - spinnerWidth - 2, this.Height / 2, spinnerWidth, this.Height / 2 - 2);

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      bool oldUp = isUpHovered;
      bool oldDown = isDownHovered;

      isUpHovered = GetUpRect().Contains(e.Location);
      isDownHovered = GetDownRect().Contains(e.Location);

      if (oldUp != isUpHovered || oldDown != isDownHovered)
        this.Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (e.Button == MouseButtons.Left)
      {
        if (GetUpRect().Contains(e.Location))
          Value += Increment;
        if (GetDownRect().Contains(e.Location))
          Value -= Increment;

        // Give focus back to the textbox so the user can continue typing
        textBox.Focus();
      }
    }

    // --- Custom Painting ---

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      using (GraphicsPath path = GetRoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), borderRadius))
      {
        // 1. Calculate Hover Color
        Color currentBackColor = fieldBackColor;
        if (isHovered && !isFocused)
        {
          currentBackColor = Color.FromArgb(Math.Max(0, fieldBackColor.R - 10), Math.Max(0, fieldBackColor.G - 10), Math.Max(0, fieldBackColor.B - 10));
        }
        textBox.BackColor = currentBackColor;

        using (SolidBrush brush = new SolidBrush(currentBackColor))
          g.FillPath(brush, path);

        // 2. Draw Focus Border
        Color currentBorderColor = isFocused ? focusedBorderColor : borderColor;
        float borderThickness = isFocused ? 2f : 1.5f;

        using (Pen pen = new Pen(currentBorderColor, borderThickness))
        {
          g.DrawPath(pen, path);
          if (isFocused)
          {
            using (Pen thickPen = new Pen(focusedBorderColor, 3f))
              g.DrawLine(thickPen, borderRadius, this.Height - 2, this.Width - borderRadius, this.Height - 2);
          }
        }
      }

      // 3. Draw the Spinner Chevrons
      using (Font iconFont = new Font("Segoe MDL2 Assets", 7f, FontStyle.Bold))
      {
        Rectangle upRect = GetUpRect();
        Rectangle downRect = GetDownRect();

        // Slightly dim the chevron if the user isn't hovering directly over it
        Color upColor = isUpHovered ? this.ForeColor : Color.FromArgb(150, this.ForeColor);
        Color downColor = isDownHovered ? this.ForeColor : Color.FromArgb(150, this.ForeColor);

        TextRenderer.DrawText(g, "\uE70E", iconFont, upRect, upColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        TextRenderer.DrawText(g, "\uE70D", iconFont, downRect, downColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
      }
    }

    private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
    {
      int d = radius * 2;
      GraphicsPath path = new GraphicsPath();
      path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
      path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
      path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
      path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
      path.CloseFigure();
      return path;
    }

    public void BeginInit()
    {

    }

    public void EndInit()
    {

    }
  }
}
