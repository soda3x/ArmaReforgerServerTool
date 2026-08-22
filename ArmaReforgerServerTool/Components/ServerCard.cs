using Longbow.Forms;
using ReforgerServerApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;

namespace Longbow.Components
{
  public partial class ServerCard : UserControl
  {
    private Main m_serverForm;

    private Dashboard m_parent;

    private int cornerRadius = 8;

    public ServerCard(string serverName, Main serverForm, Dashboard parent)
    {
      InitializeComponent();
      m_parent = parent;
      m_serverForm = serverForm;
      lblServerName.Text = serverName;

      SetStatus(false);

      // Hook up hover effects for depth interaction
      this.MouseEnter += Card_MouseEnter;
      this.MouseLeave += Card_MouseLeave;

      // Ensure child controls don't block the card's hover events
      foreach (Control c in this.Controls)
      {
        c.MouseEnter += Card_MouseEnter;
        c.MouseLeave += Card_MouseLeave;
      }
    }

    private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
    {
      GraphicsPath path = new GraphicsPath();
      int diameter = radius * 2;

      path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
      path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
      path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
      path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
      path.CloseFigure();

      return path;
    }

    // Apply the rounded clipping region whenever the control resizes
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      using (GraphicsPath path = GetRoundedRectanglePath(this.ClientRectangle, cornerRadius))
      {
        this.Region = new Region(path);
      }
    }

    // Custom Paint to draw smooth anti-aliased borders around the curved edges
    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

      // Draw a smooth rounded border outline to give it depth
      Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
      using (GraphicsPath path = GetRoundedRectanglePath(rect, cornerRadius))
      using (Pen borderPen = new Pen(Color.FromArgb(65, 65, 68), 1))
      {
        e.Graphics.DrawPath(borderPen, path);
      }
    }

    private void Card_MouseEnter(object sender, EventArgs e)
    {
      this.BackColor = Color.FromArgb(43, 43, 45);
      this.Invalidate();
    }

    private void Card_MouseLeave(object sender, EventArgs e)
    {
      if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
      {
        this.BackColor = Color.FromArgb(37, 37, 38);
        this.Invalidate();
      }
    }

    public void SetStatus(bool isRunning, string statusText = null)
    {
      if (InvokeRequired)
      {
        Invoke(new Action(() => SetStatus(isRunning, statusText)));
        return;
      }

      if (isRunning)
      {
        lblStatus.Text = statusText ?? "● Running";
        lblStatus.ForeColor = Color.FromArgb(57, 197, 107);
      }
      else
      {
        lblStatus.Text = statusText ?? "○ Stopped";
        lblStatus.ForeColor = Color.FromArgb(241, 76, 76);
      }
    }

    public void UpdateMetrics(string cpuUsage, string ramUsage)
    {
      if (InvokeRequired)
      {
        Invoke(new Action(() => UpdateMetrics(cpuUsage, ramUsage)));
        return;
      }

      lblMetrics.Text = $"CPU: {cpuUsage}  •  RAM: {ramUsage}";
    }

    private void OnGoToServerPressed(object sender, EventArgs e)
    {
      if (m_serverForm != null)
      {
        m_serverForm.Show();
        m_serverForm.WindowState = FormWindowState.Normal;
        m_serverForm.BringToFront();
        m_serverForm.Focus();
      }
    }

    private void OnRemoveServerPressed(object sender, EventArgs e)
    {
      if (m_serverForm != null)
      {
        lblMetrics.Invoke(() =>
        {
          m_serverForm.HandleClose();
          m_serverForm.Hide();
          m_parent.RemoveServerCard(this);
          m_serverForm.Dispose();
        });
        
      }
    }
  }
}
