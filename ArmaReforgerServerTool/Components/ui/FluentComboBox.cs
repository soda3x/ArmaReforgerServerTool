using System.ComponentModel;
using System.Drawing.Drawing2D;

public class FluentComboBox : Control
{
  private int m_borderRadius = 8;
  private Color m_borderColor = Color.FromArgb(120, 120, 120);
  private Color m_fieldBackColor = SystemColors.Window;
  private Color m_textColour;

  // Popup components
  private ToolStripDropDown m_popup;
  private ToolStripControlHost m_host;
  private ListBox m_popupList;

  // State
  private bool m_isHovered = false;

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
    m_textColour = this.ForeColor;

    InitializePopup();
  }

  private void InitializePopup()
  {
    m_popupList = new ListBox();

    m_popupList.BindingContext = new BindingContext();

    m_popupList.BorderStyle = BorderStyle.None;
    m_popupList.IntegralHeight = false;
    m_popupList.DrawMode = DrawMode.OwnerDrawFixed;
    m_popupList.ItemHeight = 32;
    m_popupList.FormattingEnabled = true;

    m_popupList.DrawItem += (s, e) =>
    {
      if (e.Index < 0)
        return;
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

      using (SolidBrush bgBrush = new SolidBrush(m_fieldBackColor))
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
      string text = m_popupList.GetItemText(m_popupList.Items[e.Index]);

      Rectangle textRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height);
      TextRenderer.DrawText(g, text, this.Font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    };

    m_popupList.SelectedIndexChanged += (s, e) =>
    {
      this.Invalidate();
      SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
    };

    m_popupList.SelectedValueChanged += (s, e) =>
    {
      SelectedValueChanged?.Invoke(this, EventArgs.Empty);
    };

    m_popupList.MouseClick += (s, e) => m_popup.Close();

    m_host = new ToolStripControlHost(m_popupList);
    m_host.Margin = Padding.Empty;
    m_host.Padding = Padding.Empty;
    m_host.AutoSize = false;

    m_popup = new ToolStripDropDown();
    m_popup.Padding = new Padding(1);
    m_popup.Items.Add(m_host);
    m_popup.DropShadowEnabled = true;
  }

  [Category("Data")]
  [AttributeProvider(typeof(IListSource))]
   
  public object DataSource
  {
    get => m_popupList.DataSource;
    set { m_popupList.DataSource = value; this.Invalidate(); }
  }

  [Category("Data")]
   
  public string DisplayMember
  {
    get => m_popupList.DisplayMember;
    set { m_popupList.DisplayMember = value; this.Invalidate(); }
  }

  [Category("Data")]
   
  public string ValueMember
  {
    get => m_popupList.ValueMember;
    set { m_popupList.ValueMember = value; }
  }

  [Category("Data")]
  [Browsable(false)]
   
  public object SelectedValue
  {
    get => m_popupList.SelectedValue;
    set { m_popupList.SelectedValue = value; this.Invalidate(); }
  }

  [Category("Data")]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  public ListBox.ObjectCollection Items => m_popupList.Items;

  [Category("Behavior")]
   
  public int SelectedIndex
  {
    get => m_popupList.SelectedIndex;
    set
    {
      if (m_popupList.SelectedIndex != value)
      {
        m_popupList.SelectedIndex = value;
        this.Invalidate();
      }
    }
  }

  [Category("Behavior")]
   
  public object SelectedItem
  {
    get => m_popupList.SelectedItem;
    set
    {
      if (m_popupList.SelectedItem != value)
      {
        m_popupList.SelectedItem = value;
        this.Invalidate();
      }
    }
  }

  public string Text
  {
    get => m_popupList.SelectedItem.ToString();
  }

  [Category("Appearance")]
   
  public Color FieldBackColor
  {
    get => m_fieldBackColor;
    set { m_fieldBackColor = value; this.Invalidate(); }
  }

  protected override void OnMouseEnter(EventArgs e) { m_isHovered = true; this.Invalidate(); base.OnMouseEnter(e); }
  protected override void OnMouseLeave(EventArgs e) { m_isHovered = false; this.Invalidate(); base.OnMouseLeave(e); }

  protected override void OnMouseDown(MouseEventArgs e)
  {
    base.OnMouseDown(e);

    if (e.Button == MouseButtons.Left && m_popupList.Items.Count > 0)
    {
      m_popupList.BackColor = m_fieldBackColor;
      m_popup.BackColor = m_borderColor;

      m_popupList.Width = this.Width - 2;
      m_popupList.Height = Math.Min(m_popupList.Items.Count * m_popupList.ItemHeight, 200);
      m_host.Size = m_popupList.Size;

      m_popup.Show(this, new Point(0, this.Height + 2));
    }
  }

  protected override void OnEnabledChanged(EventArgs e)
  {
    base.OnEnabledChanged(e);
    this.Invalidate();
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    base.OnPaint(e);
    Graphics g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;

    using (GraphicsPath path = GetRoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), m_borderRadius))
    {
      Color currentBackColor = m_isHovered ? Color.FromArgb(Math.Max(0, m_fieldBackColor.R - 10), Math.Max(0, m_fieldBackColor.G - 10), Math.Max(0, m_fieldBackColor.B - 10)) : m_fieldBackColor;

      if (!this.Enabled)
      {
        currentBackColor = Color.FromArgb(Math.Max(0, m_fieldBackColor.R - 15),
                                          Math.Max(0, m_fieldBackColor.G - 15),
                                          Math.Max(0, m_fieldBackColor.B - 15));

        m_borderColor = Color.FromArgb(100, 150, 150, 150); // Semi-transparent gray border
        this.ForeColor = Color.FromArgb(150, 150, 150);     // Washed out text
      } else
      {
        this.ForeColor = m_textColour;
      }

      using (SolidBrush brush = new SolidBrush(currentBackColor))
        g.FillPath(brush, path);

      using (Pen pen = new Pen(m_borderColor, 1.5f))
        g.DrawPath(pen, path);
    }

    string displayText = "";
    if (m_popupList.SelectedIndex >= 0 && m_popupList.SelectedIndex < m_popupList.Items.Count)
    {
      displayText = m_popupList.GetItemText(m_popupList.Items[m_popupList.SelectedIndex]);
    }

    Rectangle textBounds = new Rectangle(10, 0, this.Width - 40, this.Height);
    TextRenderer.DrawText(g, displayText, this.Font, textBounds, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

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
