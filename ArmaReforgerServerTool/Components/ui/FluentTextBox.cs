using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Longbow.Components.ui
{
  public class FluentTextBox : UserControl
  {
    private TextBox m_textBox;
    private int m_borderRadius = 8;
    private Color m_borderColor = Color.FromArgb(120, 120, 120);
    private Color m_focusedBorderColor = Color.FromArgb(0, 120, 212); // Windows 11 Blue
    private Color m_fieldBackColor = SystemColors.Window;

    private bool m_isHovered = false;
    private bool m_isFocused = false;

    public FluentTextBox()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
      this.BackColor = Color.Transparent;
      this.Padding = new Padding(10, 7, 10, 7);
      this.Size = new Size(250, 32);
      this.Cursor = Cursors.IBeam;

      m_textBox = new TextBox();
      m_textBox.BorderStyle = BorderStyle.None;
      m_textBox.Dock = DockStyle.Fill;
      m_textBox.BackColor = m_fieldBackColor;
      m_textBox.ForeColor = this.ForeColor;

      this.ForeColorChanged += (s, e) => m_textBox.ForeColor = this.ForeColor;

      m_textBox.MouseEnter += (s, e) => { m_isHovered = true; this.Invalidate(); };
      m_textBox.MouseLeave += (s, e) => { m_isHovered = false; this.Invalidate(); };
      this.MouseEnter += (s, e) => { m_isHovered = true; this.Invalidate(); };
      this.MouseLeave += (s, e) => { m_isHovered = false; this.Invalidate(); };

      m_textBox.Enter += (s, e) => { m_isFocused = true; this.Invalidate(); };
      m_textBox.Leave += (s, e) => { m_isFocused = false; this.Invalidate(); };

      this.Controls.Add(m_textBox);
    }

    [Category("Data")]
    public override string Text
    {
      get => m_textBox.Text;
      set => m_textBox.Text = value;
    }

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color FieldBackColor
    {
      get => m_fieldBackColor;
      set
      {
        m_fieldBackColor = value;
        m_textBox.BackColor = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UseSystemPasswordChar
    {
      get => m_textBox.UseSystemPasswordChar;
      set => m_textBox.UseSystemPasswordChar = value;
    }

    [Category("Behavior")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Multiline
    {
      get => m_textBox.Multiline;
      set => m_textBox.Multiline = value;
    }

    public void AppendText(string text)
    {
      m_textBox.AppendText(text);
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
          currentBackColor = Color.FromArgb(Math.Max(0, m_fieldBackColor.R - 10),
                                            Math.Max(0, m_fieldBackColor.G - 10),
                                            Math.Max(0, m_fieldBackColor.B - 10));
        }

        m_textBox.BackColor = currentBackColor;

        using (SolidBrush brush = new SolidBrush(currentBackColor))
        {
          g.FillPath(brush, path);
        }

        Color currentBorderColor = m_isFocused ? m_focusedBorderColor : m_borderColor;
        float borderThickness = m_isFocused ? 2f : 1.5f;

        using (Pen pen = new Pen(currentBorderColor, borderThickness))
        {
          g.DrawPath(pen, path);

          if (m_isFocused)
          {
            using (Pen thickPen = new Pen(m_focusedBorderColor, 3f))
            {
              g.DrawLine(thickPen, m_borderRadius, this.Height - 2, this.Width - m_borderRadius, this.Height - 2);
            }
          }
        }
      }
    }

    // Helper for drawing rounded rectangles
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
  }
}
