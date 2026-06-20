using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Longbow.Components.ui
{
  using System;
  using System.Drawing;
  using System.Drawing.Drawing2D;
  using System.Windows.Forms;

  public class FluentTextBox : UserControl
  {
    private TextBox textBox;
    private int borderRadius = 8;
    private Color borderColor = Color.FromArgb(120, 120, 120);
    private Color focusedBorderColor = Color.FromArgb(0, 120, 212); // Windows 11 Blue
    private Color fieldBackColor = SystemColors.Window;

    private bool isHovered = false;
    private bool isFocused = false;

    public FluentTextBox()
    {
      // 1. Setup transparency and double buffering
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
      this.BackColor = Color.Transparent;
      this.Padding = new Padding(10, 7, 10, 7);
      this.Size = new Size(250, 32);
      this.Cursor = Cursors.IBeam;

      // 2. Setup the internal borderless textbox
      textBox = new TextBox();
      textBox.BorderStyle = BorderStyle.None;
      textBox.Dock = DockStyle.Fill;
      textBox.BackColor = fieldBackColor;
      textBox.ForeColor = this.ForeColor;

      // Sync .NET 9 Dark Mode text colors
      this.ForeColorChanged += (s, e) => textBox.ForeColor = this.ForeColor;

      // 3. Interactive Hover States
      // We must hook the mouse events of BOTH the wrapper and the internal textbox
      textBox.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
      textBox.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };
      this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
      this.MouseLeave += (s, e) => { isHovered = false; this.Invalidate(); };

      // 4. Interactive Focus States (WinUI 3 Blue Border)
      textBox.Enter += (s, e) => { isFocused = true; this.Invalidate(); };
      textBox.Leave += (s, e) => { isFocused = false; this.Invalidate(); };

      this.Controls.Add(textBox);
    }

    // --- Expose essential TextBox properties to the Designer ---

    [Category("Data")]
    public override string Text
    {
      get => textBox.Text;
      set => textBox.Text = value;
    }

    [Category("Appearance")]
    public Color FieldBackColor
    {
      get => fieldBackColor;
      set
      {
        fieldBackColor = value;
        textBox.BackColor = value;
        this.Invalidate();
      }
    }

    [Category("Behavior")]
    public bool UseSystemPasswordChar
    {
      get => textBox.UseSystemPasswordChar;
      set => textBox.UseSystemPasswordChar = value;
    }

    [Category("Behavior")]
    public bool Multiline
    {
      get => textBox.Multiline;
      set => textBox.Multiline = value;
    }

    public void AppendText(string text)
    {
      textBox.AppendText(text);
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
          currentBackColor = Color.FromArgb(Math.Max(0, fieldBackColor.R - 10),
                                            Math.Max(0, fieldBackColor.G - 10),
                                            Math.Max(0, fieldBackColor.B - 10));
        }

        // Sync inner textbox to match the hover color so there's no ugly square behind the text
        textBox.BackColor = currentBackColor;

        // Fill background
        using (SolidBrush brush = new SolidBrush(currentBackColor))
        {
          g.FillPath(brush, path);
        }

        // 2. Calculate Focus Border
        Color currentBorderColor = isFocused ? focusedBorderColor : borderColor;
        float borderThickness = isFocused ? 2f : 1.5f;

        using (Pen pen = new Pen(currentBorderColor, borderThickness))
        {
          g.DrawPath(pen, path);

          // WinUI 3 textboxes feature a distinct thicker accent line at the bottom when focused.
          // This draws a thicker stroke along the bottom edge to mimic that exact design language.
          if (isFocused)
          {
            using (Pen thickPen = new Pen(focusedBorderColor, 3f))
            {
              g.DrawLine(thickPen, borderRadius, this.Height - 2, this.Width - borderRadius, this.Height - 2);
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
