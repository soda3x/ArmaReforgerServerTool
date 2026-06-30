using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Longbow.Components.ui
{
  public class FluentToggleSwitch : Control
  {
    private bool m_checked = false;
    private Color m_onBackColor = Color.FromArgb(0, 120, 212); // Windows 11 Blue Accent
    private Color m_offBackColor = Color.FromArgb(100, 100, 100); // Dark gray for off state
    private Color m_thumbColor = Color.White;

    public event EventHandler CheckedChanged;

    public FluentToggleSwitch()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);

      this.BackColor = Color.Transparent;
      this.Size = new Size(50, 24); // Standard Fluent switch size
      this.Cursor = Cursors.Hand;
    }

    [Category("Behavior")]
     
    public bool Checked
    {
      get { return m_checked; }
      set
      {
        if (m_checked != value)
        {
          m_checked = value;
          this.Invalidate(); // Redraw the control when state changes
          CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    // Toggle the state when the user clicks the control
    protected override void OnMouseClick(MouseEventArgs e)
    {
      base.OnMouseClick(e);
      if (e.Button == MouseButtons.Left)
      {
        this.Checked = !this.Checked;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      int trackHeight = this.Height - 4;
      int trackWidth = this.Width - 4;
      int radius = trackHeight / 2;

      using (GraphicsPath trackPath = new GraphicsPath())
      {
        trackPath.AddArc(2, 2, radius * 2, radius * 2, 90, 180);
        trackPath.AddArc(trackWidth - (radius * 2) + 2, 2, radius * 2, radius * 2, 270, 180);
        trackPath.CloseFigure();

        Color currentOffColor = (this.ForeColor.R < 128) ? Color.FromArgb(200, 200, 200) : m_offBackColor;
        Color trackColor = m_checked ? m_onBackColor : currentOffColor;

        using (SolidBrush trackBrush = new SolidBrush(trackColor))
        {
          g.FillPath(trackBrush, trackPath);
        }
      }

      int thumbSize = trackHeight - 8;
      int thumbY = 6;
      int thumbX = m_checked ? this.Width - thumbSize - 6 : 6;

      using (SolidBrush thumbBrush = new SolidBrush(m_thumbColor))
      {
        g.FillEllipse(thumbBrush, thumbX, thumbY, thumbSize, thumbSize);
      }
    }
  }
}
