using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Longbow.Components.ui
{
  public class FluentTimePicker : Control
  {
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private int m_borderRadius = 8;
    private Color m_borderColour = Color.FromArgb(120, 120, 120);
    private Color m_focusedBorderColour = Color.FromArgb(0, 120, 212); // Windows 11 Blue
    private Color m_fieldBackColour = SystemColors.Window;
    private bool m_isHovered = false;
    private bool m_isFocused = false;
    private DateTime m_value = DateTime.Now; // Default to now
    private DateTime m_minDate = DateTime.Now;
    private DateTime m_maxDate = DateTime.Now;
    private string m_timeFormat = "hh\\:mm"; // 24-hour format
    private ToolStripDropDown m_popup;
    private TableLayoutPanel m_popupContainer;
    private ListBox m_lstHours;
    private ListBox m_lstMinutes;

    public event EventHandler ValueChanged;

    public FluentTimePicker()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.Selectable, true);

      this.BackColor = Color.Transparent;
      this.Size = new Size(150, 32);
      this.Cursor = Cursors.Hand;

      InitializePopup();
    }

    private void InitializePopup()
    {
      m_popupContainer = new TableLayoutPanel();
      m_popupContainer.ColumnCount = 2;
      m_popupContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      m_popupContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      m_popupContainer.RowCount = 1;
      m_popupContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      m_popupContainer.Size = new Size(160, 220); // Height of the dropdown flyout
      m_popupContainer.Margin = Padding.Empty;
      m_popupContainer.Padding = new Padding(2);

      m_lstHours = CreatePopupList();
      for (int i = 0; i <= 23; i++)
        m_lstHours.Items.Add(i.ToString("D2"));

      m_lstMinutes = CreatePopupList();
      for (int i = 0; i <= 59; i++)
        m_lstMinutes.Items.Add(i.ToString("D2"));

      m_lstHours.MouseEnter += (s, e) => m_lstHours.Focus();
      m_lstMinutes.MouseEnter += (s, e) => m_lstMinutes.Focus();

      m_popupContainer.Controls.Add(m_lstHours, 0, 0);
      m_popupContainer.Controls.Add(m_lstMinutes, 1, 0);

      // Update the Time value when the user clicks an item in the popup
      EventHandler listSelectionChanged = (s, e) =>
        {
          if (m_lstHours.SelectedIndex >= 0 && m_lstMinutes.SelectedIndex >= 0)
          {
            // Preserve the date, overwrite the time
            this.Value = new DateTime(m_value.Year, m_value.Month, m_value.Day, m_lstHours.SelectedIndex, m_lstMinutes.SelectedIndex, 0);
          }
        };
      m_lstHours.SelectedIndexChanged += listSelectionChanged;
      m_lstMinutes.SelectedIndexChanged += listSelectionChanged;

      ToolStripControlHost host = new ToolStripControlHost(m_popupContainer);
      host.Margin = Padding.Empty;
      host.Padding = Padding.Empty;
      host.AutoSize = false;
      host.Size = m_popupContainer.Size;

      m_popup = new ToolStripDropDown();
      m_popup.Padding = new Padding(1);
      m_popup.Items.Add(host);
      m_popup.DropShadowEnabled = true;
    }

    private ListBox CreatePopupList()
    {
      ListBox list = new ListBox();
      list.BorderStyle = BorderStyle.None;
      list.Dock = DockStyle.Fill;
      list.IntegralHeight = false;
      list.DrawMode = DrawMode.OwnerDrawFixed;
      list.ItemHeight = 32;

      list.DrawItem += (s, e) =>
      {
        if (e.Index < 0)
          return;
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        using (SolidBrush bgBrush = new SolidBrush(m_fieldBackColour))
          g.FillRectangle(bgBrush, e.Bounds);

        if (isSelected)
        {
          Rectangle pillRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height - 4);
          using (GraphicsPath path = GetRoundedRect(pillRect, 4))
          using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 120, 212)))
            g.FillPath(brush, path);
        }

        Color textColor = isSelected ? Color.White : this.ForeColor;
        string text = ((ListBox)s).Items[e.Index].ToString();

        Rectangle textRect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
        TextRenderer.DrawText(g, text, this.Font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
      };

      return list;
    }

    [Category("Data")]
    public DateTime Value
    {
      get => m_value;
      set
      {
        if (m_value != value)
        {
          m_value = value;
          this.Invalidate();
          ValueChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    public DateTime MinDate
    {
      get => m_minDate;
      set => m_minDate = value;
    }

    public DateTime MaxDate
    {
      get => m_maxDate;
      set => m_maxDate = value;
    }

    [Category("Appearance")]
    public string CustomFormat
    {
      get => m_timeFormat;
      set { m_timeFormat = value; this.Invalidate(); }
    }

    [Category("Appearance")]
    public Color FieldBackColor
    {
      get => m_fieldBackColour;
      set { m_fieldBackColour = value; this.Invalidate(); }
    }

    protected override void OnMouseEnter(EventArgs e) { m_isHovered = true; this.Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { m_isHovered = false; this.Invalidate(); base.OnMouseLeave(e); }
    protected override void OnEnter(EventArgs e) { m_isFocused = true; this.Invalidate(); base.OnEnter(e); }
    protected override void OnLeave(EventArgs e) { m_isFocused = false; this.Invalidate(); base.OnLeave(e); }
    protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); this.Invalidate(); }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      base.OnMouseClick(e);
      if (e.Button == MouseButtons.Left && this.Enabled)
      {
        this.Focus();

        m_popupContainer.BackColor = m_fieldBackColour;
        m_lstHours.BackColor = m_fieldBackColour;
        m_lstMinutes.BackColor = m_fieldBackColour;
        m_popup.BackColor = m_borderColour;

        m_lstHours.SelectedIndex = m_value.Hour;
        m_lstMinutes.SelectedIndex = m_value.Minute;

        m_lstHours.TopIndex = Math.Max(0, m_value.Hour - 2);
        m_lstMinutes.TopIndex = Math.Max(0, m_value.Minute - 2);

        m_popup.Show(this, new Point(0, this.Height + 2));
      }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      if (m_popup != null && m_popup.Visible)
      {
        // Determine which ListBox the mouse is actually over
        Point mousePos = Cursor.Position;

        // Find which list the mouse is over
        ListBox target = null;
        if (m_lstHours.Bounds.Contains(m_popupContainer.PointToClient(m_popup.PointToClient(mousePos))))
          target = m_lstHours;
        else if (m_lstMinutes.Bounds.Contains(m_popupContainer.PointToClient(m_popup.PointToClient(mousePos))))
          target = m_lstMinutes;

        if (target != null)
        {
          // The Win32 message WM_MOUSEWHEEL is 0x020A, use SendMessage to force the ListBox to process the scroll
          const int WM_MOUSEWHEEL = 0x020A;
          // Create the WParam containing the wheel delta (e.Delta)
          IntPtr wParam = (IntPtr)((e.Delta << 16) | (0x0000FFFF & (int)0));
          // Send the message directly to the ListBox handle
          SendMessage(target.Handle, WM_MOUSEWHEEL, wParam, IntPtr.Zero);
        }
        // Stop the form from attempting to scroll
        ((HandledMouseEventArgs) e).Handled = true;
      }
      else
      {
        base.OnMouseWheel(e);
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      Color currentBackColor = m_fieldBackColour;
      Color currentBorderColor = m_isFocused ? m_focusedBorderColour : m_borderColour;
      Color currentTextColor = this.ForeColor;

      if (!this.Enabled)
      {
        currentBackColor = Color.FromArgb(Math.Max(0, m_fieldBackColour.R - 15), Math.Max(0, m_fieldBackColour.G - 15), Math.Max(0, m_fieldBackColour.B - 15));
        currentBorderColor = Color.FromArgb(100, 150, 150, 150);
        currentTextColor = Color.FromArgb(150, 150, 150);
      }
      else if (m_isHovered && !m_isFocused)
      {
        currentBackColor = Color.FromArgb(Math.Max(0, m_fieldBackColour.R - 10), Math.Max(0, m_fieldBackColour.G - 10), Math.Max(0, m_fieldBackColour.B - 10));
      }

      float borderThickness = m_isFocused ? 2f : 1.5f;

      using (GraphicsPath path = GetRoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), m_borderRadius))
      {
        using (SolidBrush brush = new SolidBrush(currentBackColor))
          g.FillPath(brush, path);

        using (Pen pen = new Pen(currentBorderColor, borderThickness))
        {
          g.DrawPath(pen, path);
          if (m_isFocused)
          {
            using (Pen thickPen = new Pen(m_focusedBorderColour, 3f))
              g.DrawLine(thickPen, m_borderRadius, this.Height - 2, this.Width - m_borderRadius, this.Height - 2);
          }
        }
      }

      string displayText = m_value.ToString(m_timeFormat);
      Rectangle textBounds = new Rectangle(10, 0, this.Width - 40, this.Height);
      TextRenderer.DrawText(g, displayText, this.Font, textBounds, currentTextColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

      using (Font iconFont = new Font("Segoe MDL2 Assets", 10f, FontStyle.Regular))
      {
        Rectangle iconBounds = new Rectangle(this.Width - 30, 0, 30, this.Height);
        TextRenderer.DrawText(g, "\uE121", iconFont, iconBounds, currentTextColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
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
  }
}
