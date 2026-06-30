using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace Longbow.Components.ui
{
  public class FluentStatus : Control
  {
    public enum StatusState
    {
      Idle,
      Running,
      Success,
      Error
    }

    private StatusState _currentState = StatusState.Idle;
    private string _statusText = "Ready";

    // Animation properties
    private Timer spinnerTimer;
    private float rotationAngle = 0f;

    // Fluent Colors
    private Color accentColor = Color.FromArgb(0, 120, 212); // Win 11 Blue
    private Color successColor = Color.FromArgb(16, 124, 16); // Win 11 Green
    private Color errorColor = Color.FromArgb(232, 17, 35); // Win 11 Red

    public FluentStatus()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);

      this.BackColor = Color.Transparent;
      this.Size = new Size(250, 32);

      // Setup the animation timer (runs at ~60fps)
      spinnerTimer = new Timer();
      spinnerTimer.Interval = 16;
      spinnerTimer.Tick += (s, e) =>
      {
        rotationAngle += 12f; // Speed of rotation
        if (rotationAngle >= 360f)
          rotationAngle -= 360f;
        this.Invalidate();
      };
    }

    // --- Developer API (Thread-Safe) ---

    public void StartChecking(string initialText = "Checking for updates...")
    {
      if (this.InvokeRequired)
      { this.Invoke(new Action(() => StartChecking(initialText))); return; }

      _statusText = initialText;
      _currentState = StatusState.Running;
      spinnerTimer.Start();
      this.Invalidate();
    }

    public void UpdateStatus(string currentTaskText)
    {
      if (this.InvokeRequired)
      { this.Invoke(new Action(() => UpdateStatus(currentTaskText))); return; }

      _statusText = currentTaskText;
      this.Invalidate();
    }

    public void SetSuccess(string successText = "All systems operational.")
    {
      if (this.InvokeRequired)
      { this.Invoke(new Action(() => SetSuccess(successText))); return; }

      _statusText = successText;
      _currentState = StatusState.Success;
      spinnerTimer.Stop();
      this.Invalidate();
    }

    public void SetError(string errorText = "An error occurred.")
    {
      if (this.InvokeRequired)
      { this.Invoke(new Action(() => SetError(errorText))); return; }

      _statusText = errorText;
      _currentState = StatusState.Error;
      spinnerTimer.Stop();
      this.Invalidate();
    }

    // Allow resetting back to an idle state
    public void ResetControl(string idleText = "Ready")
    {
      if (this.InvokeRequired)
      { this.Invoke(new Action(() => ResetControl(idleText))); return; }

      _statusText = idleText;
      _currentState = StatusState.Idle;
      spinnerTimer.Stop();
      this.Invalidate();
    }

    // --- Custom Painting ---

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      // 1. Define the layout areas
      int iconSize = 20;
      Rectangle iconRect = new Rectangle(0, (this.Height - iconSize) / 2, iconSize, iconSize);
      Rectangle textRect = new Rectangle(iconSize + 10, 0, this.Width - iconSize - 10, this.Height);

      // 2. Draw the Icon based on the current state
      using (Font iconFont = new Font("Segoe MDL2 Assets", 12f, FontStyle.Regular))
      {
        switch (_currentState)
        {
          case StatusState.Running:
          // Draw the animated Fluent Progress Ring
          using (Pen ringPen = new Pen(accentColor, 2.5f))
          {
            // Draw a 270-degree arc so there is a clear "gap" spinning around
            g.DrawArc(ringPen, iconRect, rotationAngle, 270f);
          }
          break;

          case StatusState.Success:
          // Draw a solid green circle with a white tick cutout
          using (SolidBrush bgBrush = new SolidBrush(successColor))
          {
            g.FillEllipse(bgBrush, iconRect);
          }
          TextRenderer.DrawText(g, "\uE73E", iconFont, iconRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
          break;

          case StatusState.Error:
          // Draw a red circle with a white X
          using (SolidBrush bgBrush = new SolidBrush(errorColor))
          {
            g.FillEllipse(bgBrush, iconRect);
          }
          TextRenderer.DrawText(g, "\uE711", iconFont, iconRect, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
          break;

          case StatusState.Idle:
          // Draw a subtle gray circle to indicate waiting
          Color idleColor = this.ForeColor.R > 128 ? Color.FromArgb(70, 70, 70) : Color.FromArgb(200, 200, 200);
          using (Pen idlePen = new Pen(idleColor, 2f))
          {
            g.DrawEllipse(idlePen, iconRect);
          }
          break;
        }
      }

      // 3. Draw the Status Text
      TextRenderer.DrawText(g, _statusText, this.Font, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    }
  }
}
