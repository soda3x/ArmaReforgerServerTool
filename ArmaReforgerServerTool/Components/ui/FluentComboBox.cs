using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class FluentComboBox : Control
{
  private int borderRadius = 8;
  private Color borderColor = Color.FromArgb(120, 120, 120);
  private Color fieldBackColor = SystemColors.Window;

  // Popup components
  private ToolStripDropDown popup;
  private ToolStripControlHost host;
  private ListBox popupList;

  // State
  private bool isHovered = false;

  public event EventHandler SelectedIndexChanged;
  public event EventHandler SelectedValueChanged;

  public FluentComboBox()
  {
    this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                  ControlStyles.UserPaint |
                  ControlStyles.AllPaintingInWmPaint |
                  ControlStyles.OptimizedDoubleBuffer, true);
    this.BackColor = Color.Transparent;
    this.Size = new Size(200, 32);
    this.Cursor = Cursors.Hand;

    InitializePopup();
  }

  private void InitializePopup()
  {
    popupList = new ListBox();

    // 1. THE MAGIC FIX: Force the ListBox to process data bindings even while hidden
    popupList.BindingContext = new BindingContext();

    popupList.BorderStyle = BorderStyle.None;
    popupList.IntegralHeight = false;
    popupList.DrawMode = DrawMode.OwnerDrawFixed;
    popupList.ItemHeight = 32;
    popupList.FormattingEnabled = true;

    popupList.DrawItem += (s, e) =>
    {
      if (e.Index < 0)
        return;
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

      using (SolidBrush bgBrush = new SolidBrush(fieldBackColor))
      {
        g.FillRectangle(bgBrush, e.Bounds);
      }

      if (isSelected)
      {
        Rectangle pillRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height - 4);
        Color accentColor = Color.FromArgb(0, 120, 212);

        using (GraphicsPath path = GetRoundedRect(pillRect, 4))
        using (SolidBrush brush = new SolidBrush(accentColor))
        {
          g.FillPath(brush, path);
        }
      }

      Color textColor = isSelected ? Color.White : this.ForeColor;
      string text = popupList.GetItemText(popupList.Items[e.Index]);

      Rectangle textRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height);
      TextRenderer.DrawText(g, text, this.Font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    };

    popupList.SelectedIndexChanged += (s, e) =>
    {
      this.Invalidate();
      SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
    };

    popupList.SelectedValueChanged += (s, e) =>
    {
      SelectedValueChanged?.Invoke(this, EventArgs.Empty);
    };

    popupList.MouseClick += (s, e) => popup.Close();

    host = new ToolStripControlHost(popupList);
    host.Margin = Padding.Empty;
    host.Padding = Padding.Empty;
    host.AutoSize = false; // 2. Turn off auto-size to prevent clipping glitches

    popup = new ToolStripDropDown();
    popup.Padding = new Padding(1);
    popup.Items.Add(host);
    popup.DropShadowEnabled = true;
  }

  // --- Data Binding Properties ---

  [Category("Data")]
  [AttributeProvider(typeof(IListSource))]
  public object DataSource
  {
    get => popupList.DataSource;
    set { popupList.DataSource = value; this.Invalidate(); }
  }

  [Category("Data")]
  public string DisplayMember
  {
    get => popupList.DisplayMember;
    set { popupList.DisplayMember = value; this.Invalidate(); }
  }

  [Category("Data")]
  public string ValueMember
  {
    get => popupList.ValueMember;
    set { popupList.ValueMember = value; }
  }

  [Category("Data")]
  [Browsable(false)]
  public object SelectedValue
  {
    get => popupList.SelectedValue;
    set { popupList.SelectedValue = value; this.Invalidate(); }
  }

  [Category("Data")]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  public ListBox.ObjectCollection Items => popupList.Items;

  [Category("Behavior")]
  public int SelectedIndex
  {
    get => popupList.SelectedIndex;
    set
    {
      if (popupList.SelectedIndex != value)
      {
        popupList.SelectedIndex = value;
        this.Invalidate();
      }
    }
  }

  [Category("Behavior")]
  public object SelectedItem
  {
    get => popupList.SelectedItem;
    set
    {
      if (popupList.SelectedItem != value)
      {
        popupList.SelectedItem = value;
        this.Invalidate();
      }
    }
  }

  // --- Appearance & Interactions ---

  [Category("Appearance")]
  public Color FieldBackColor
  {
    get => fieldBackColor;
    set { fieldBackColor = value; this.Invalidate(); }
  }

  protected override void OnMouseEnter(EventArgs e) { isHovered = true; this.Invalidate(); base.OnMouseEnter(e); }
  protected override void OnMouseLeave(EventArgs e) { isHovered = false; this.Invalidate(); base.OnMouseLeave(e); }

  protected override void OnMouseDown(MouseEventArgs e)
  {
    base.OnMouseDown(e);

    // Now that BindingContext is forced, Items.Count will correctly report your DataSource size
    if (e.Button == MouseButtons.Left && popupList.Items.Count > 0)
    {
      popupList.BackColor = fieldBackColor;
      popup.BackColor = borderColor;

      // 3. Explicitly size both the list AND the wrapper host
      popupList.Width = this.Width - 2;
      popupList.Height = Math.Min(popupList.Items.Count * popupList.ItemHeight, 200);
      host.Size = popupList.Size;

      popup.Show(this, new Point(0, this.Height + 2));
    }
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    base.OnPaint(e);
    Graphics g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;

    // 1. Draw rounded base
    using (GraphicsPath path = GetRoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), borderRadius))
    {
      Color currentBackColor = isHovered ? Color.FromArgb(Math.Max(0, fieldBackColor.R - 10), Math.Max(0, fieldBackColor.G - 10), Math.Max(0, fieldBackColor.B - 10)) : fieldBackColor;

      using (SolidBrush brush = new SolidBrush(currentBackColor))
        g.FillPath(brush, path);

      using (Pen pen = new Pen(borderColor, 1.5f))
        g.DrawPath(pen, path);
    }

    // 2. Draw the selected text respecting DisplayMember
    string displayText = "";
    if (popupList.SelectedIndex >= 0 && popupList.SelectedIndex < popupList.Items.Count)
    {
      displayText = popupList.GetItemText(popupList.Items[popupList.SelectedIndex]);
    }

    Rectangle textBounds = new Rectangle(10, 0, this.Width - 40, this.Height);
    TextRenderer.DrawText(g, displayText, this.Font, textBounds, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

    // 3. Draw the Chevron
    using (Font iconFont = new Font("Segoe MDL2 Assets", 10f, FontStyle.Regular))
    {
      Rectangle iconBounds = new Rectangle(this.Width - 30, 0, 30, this.Height);
      TextRenderer.DrawText(g, "\uE70D", iconFont, iconBounds, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
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
