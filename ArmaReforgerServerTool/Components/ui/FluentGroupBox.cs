using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Longbow.Components.ui
{
  public class FluentGroupBox : GroupBox
  {
    private int borderRadius = 8;
    private Color borderColor = Color.FromArgb(120, 120, 120);

    public FluentGroupBox()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);

      this.BackColor = Color.Transparent;
    }

    [Category("Appearance")]
     
    public Color BorderColor
    {
      get => borderColor;
      set { borderColor = value; this.Invalidate(); }
    }

    [Category("Appearance")]
     
    public int BorderRadius
    {
      get => borderRadius;
      set { borderRadius = value; this.Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      Size textSize = TextRenderer.MeasureText(this.Text, this.Font);
      Rectangle textBounds = new Rectangle(12, 0, textSize.Width, textSize.Height);

      int borderTop = textSize.Height / 2;
      Rectangle borderRect = new Rectangle(0, borderTop, this.Width - 1, this.Height - borderTop - 1);

      using (GraphicsPath path = GetRoundedRect(borderRect, borderRadius))
      {
        if (!string.IsNullOrEmpty(this.Text))
        {
          Rectangle clipRect = new Rectangle(textBounds.X - 4, 0, textBounds.Width + 8, textBounds.Height);

          g.SetClip(clipRect, CombineMode.Exclude);
        }

        using (Pen pen = new Pen(borderColor, 1.5f))
        {
          g.DrawPath(pen, path);
        }

        g.ResetClip();
      }

      if (!string.IsNullOrEmpty(this.Text))
      {
        TextRenderer.DrawText(g, this.Text, this.Font, textBounds, this.ForeColor, TextFormatFlags.Left | TextFormatFlags.Top);
      }
    }

    // Helper for drawing rounded rectangles
    private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
    {
      int d = radius * 2;
      GraphicsPath path = new GraphicsPath();

      if (bounds.Width < d || bounds.Height < d)
      {
        path.AddRectangle(bounds);
        return path;
      }

      path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
      path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
      path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
      path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
      path.CloseFigure();
      return path;
    }
  }
}
