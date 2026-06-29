namespace Longbow.Components.ui
{
  using System;
  using System.ComponentModel;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Windows.Forms;

  public class FluentNumericUpDown : UserControl, ISupportInitialize
  {
    private TextBox m_textBox;
    private int m_borderRadius = 8;
    private Color m_borderColor = Color.FromArgb(120, 120, 120);
    private Color m_focusedBorderColor = Color.FromArgb(0, 120, 212); // Windows 11 Blue
    private Color m_fieldBackColor = SystemColors.Window;

    // Numeric State
    private decimal m_value = 0;
    private decimal m_minimum = 0;
    private decimal m_maximum = 100;
    private decimal m_increment = 1;
    private int m_decimalPlaces = 0;

    // Interaction State
    private bool m_isHovered = false;
    private bool m_isFocused = false;
    private bool m_isUpHovered = false;
    private bool m_isDownHovered = false;

    private int m_spinnerWidth = 30; // Width of the up/down button area

    public event EventHandler ValueChanged;

    public FluentNumericUpDown()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
      this.BackColor = Color.Transparent;

      // Extra padding on the right to make room for our custom spinner buttons
      this.Padding = new Padding(10, 7, m_spinnerWidth + 5, 7);
      this.Size = new Size(150, 32);

      // Setup the internal borderless textbox
      m_textBox = new TextBox();
      m_textBox.BorderStyle = BorderStyle.None;
      m_textBox.Dock = DockStyle.Fill;
      m_textBox.BackColor = m_fieldBackColor;
      m_textBox.ForeColor = this.ForeColor;
      m_textBox.Text = m_value.ToString();

      this.ForeColorChanged += (s, e) => m_textBox.ForeColor = this.ForeColor;

      // Interaction Hooks
      m_textBox.MouseEnter += (s, e) => { m_isHovered = true; this.Invalidate(); };
      m_textBox.MouseLeave += (s, e) => { m_isHovered = false; this.Invalidate(); };
      this.MouseEnter += (s, e) => { m_isHovered = true; this.Invalidate(); };
      this.MouseLeave += (s, e) => { m_isHovered = false; m_isUpHovered = false; m_isDownHovered = false; this.Invalidate(); };

      m_textBox.Enter += (s, e) => { m_isFocused = true; this.Invalidate(); };
      m_textBox.Leave += (s, e) =>
      {
        m_isFocused = false;
        ValidateAndApplyText(); // Parse the text when the user clicks away
        this.Invalidate();
      };

      // Text validation on Enter key
      m_textBox.KeyDown += (s, e) =>
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

      this.Controls.Add(m_textBox);
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Value
    {
      get => m_value;
      set
      {
        decimal clampedValue = Math.Max(m_minimum, Math.Min(m_maximum, value));
        if (m_value != clampedValue)
        {
          m_value = Math.Round(clampedValue, m_decimalPlaces);
          m_textBox.Text = m_value.ToString($"F{m_decimalPlaces}");
          ValueChanged?.Invoke(this, EventArgs.Empty);
          this.Invalidate();
        }
      }
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Minimum { get => m_minimum; set { m_minimum = value; ValidateAndApplyText(); } }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Maximum { get => m_maximum; set { m_maximum = value; ValidateAndApplyText(); } }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Increment { get => m_increment; set => m_increment = value; }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int DecimalPlaces
    {
      get => m_decimalPlaces;
      set
      {
        m_decimalPlaces = Math.Max(0, value);
        m_textBox.Text = m_value.ToString($"F{m_decimalPlaces}");
      }
    }

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color FieldBackColor
    {
      get => m_fieldBackColor;
      set { m_fieldBackColor = value; m_textBox.BackColor = value; this.Invalidate(); }
    }

    private void ValidateAndApplyText()
    {
      if (decimal.TryParse(m_textBox.Text, out decimal parsedValue))
      {
        Value = parsedValue; // The setter handles clamping and formatting
      }
      else
      {
        // Revert to last known good value if they typed garbage
        m_textBox.Text = m_value.ToString($"F{m_decimalPlaces}");
      }
    }

    private Rectangle GetUpRect() => new Rectangle(this.Width - m_spinnerWidth - 2, 2, m_spinnerWidth, this.Height / 2 - 2);
    private Rectangle GetDownRect() => new Rectangle(this.Width - m_spinnerWidth - 2, this.Height / 2, m_spinnerWidth, this.Height / 2 - 2);

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      bool oldUp = m_isUpHovered;
      bool oldDown = m_isDownHovered;

      m_isUpHovered = GetUpRect().Contains(e.Location);
      m_isDownHovered = GetDownRect().Contains(e.Location);

      if (oldUp != m_isUpHovered || oldDown != m_isDownHovered)
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
        m_textBox.Focus();
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      using (GraphicsPath path = GetRoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), m_borderRadius))
      {
        Color currentBackColor = m_fieldBackColor;
        if (m_isHovered && !m_isFocused)
        {
          currentBackColor = Color.FromArgb(Math.Max(0, m_fieldBackColor.R - 10), Math.Max(0, m_fieldBackColor.G - 10), Math.Max(0, m_fieldBackColor.B - 10));
        }
        m_textBox.BackColor = currentBackColor;

        using (SolidBrush brush = new SolidBrush(currentBackColor))
          g.FillPath(brush, path);

        Color currentBorderColor = m_isFocused ? m_focusedBorderColor : m_borderColor;
        float borderThickness = m_isFocused ? 2f : 1.5f;

        using (Pen pen = new Pen(currentBorderColor, borderThickness))
        {
          g.DrawPath(pen, path);
          if (m_isFocused)
          {
            using (Pen thickPen = new Pen(m_focusedBorderColor, 3f))
              g.DrawLine(thickPen, m_borderRadius, this.Height - 2, this.Width - m_borderRadius, this.Height - 2);
          }
        }
      }

      using (Font iconFont = new Font("Segoe MDL2 Assets", 7f, FontStyle.Bold))
      {
        Rectangle upRect = GetUpRect();
        Rectangle downRect = GetDownRect();

        Color upColor = m_isUpHovered ? this.ForeColor : Color.FromArgb(150, this.ForeColor);
        Color downColor = m_isDownHovered ? this.ForeColor : Color.FromArgb(150, this.ForeColor);

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
      // No op
    }

    public void EndInit()
    {
      // No op
    }
  }
}
