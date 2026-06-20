using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class FluentToggleSwitch : Control
{
  private bool _checked = false;
  private Color _onBackColor = Color.FromArgb(0, 120, 212); // Windows 11 Blue Accent
  private Color _offBackColor = Color.FromArgb(100, 100, 100); // Dark gray for off state
  private Color _thumbColor = Color.White;

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
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public bool Checked
  {
    get { return _checked; }
    set
    {
      if (_checked != value)
      {
        _checked = value;
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

    // 1. Calculate the track (the pill shape)
    int trackHeight = this.Height - 4;
    int trackWidth = this.Width - 4;
    int radius = trackHeight / 2;

    using (GraphicsPath trackPath = new GraphicsPath())
    {
      trackPath.AddArc(2, 2, radius * 2, radius * 2, 90, 180);
      trackPath.AddArc(trackWidth - (radius * 2) + 2, 2, radius * 2, radius * 2, 270, 180);
      trackPath.CloseFigure();

      // 2. Determine colors based on state and dark mode
      // If the app is in light mode, the "off" track should be lighter
      Color currentOffColor = (this.ForeColor.R < 128) ? Color.FromArgb(200, 200, 200) : _offBackColor;
      Color trackColor = _checked ? _onBackColor : currentOffColor;

      using (SolidBrush trackBrush = new SolidBrush(trackColor))
      {
        g.FillPath(trackBrush, trackPath);
      }
    }

    // 3. Calculate the thumb (the moving circle)
    int thumbSize = trackHeight - 8;
    int thumbY = 6;
    int thumbX = _checked ? this.Width - thumbSize - 6 : 6;

    using (SolidBrush thumbBrush = new SolidBrush(_thumbColor))
    {
      g.FillEllipse(thumbBrush, thumbX, thumbY, thumbSize, thumbSize);
    }
  }
}
