using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Longbow.Components.ui
{
  public class FluentListBox : UserControl
  {
    private BoundListBox m_listBox;
    private int m_borderRadius = 8;
    private Color m_borderColor = Color.FromArgb(120, 120, 120);
    private Color m_fieldBackColor = SystemColors.Window;

    public event EventHandler SelectedIndexChanged;
    public event EventHandler SelectedValueChanged;

    public FluentListBox()
    {
      this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
      this.BackColor = Color.Transparent;
      this.Padding = new Padding(3);
      this.Size = new Size(200, 150);

      m_listBox = new BoundListBox();
      m_listBox.BorderStyle = BorderStyle.None;
      m_listBox.Dock = DockStyle.Fill;
      m_listBox.BackColor = m_fieldBackColor;
      m_listBox.ForeColor = this.ForeColor;
      m_listBox.IntegralHeight = false; // Allows the control to size smoothly, not jump by item height

      // Critical for Fluent styling
      m_listBox.DrawMode = DrawMode.OwnerDrawFixed;
      m_listBox.ItemHeight = 32;

      this.ForeColorChanged += (s, e) => m_listBox.ForeColor = this.ForeColor;

      m_listBox.DrawItem += ListBox_DrawItem;

      // Bubble up standard events
      m_listBox.SelectedIndexChanged += (s, e) => SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
      m_listBox.SelectedValueChanged += (s, e) => SelectedValueChanged?.Invoke(this, EventArgs.Empty);

      this.Controls.Add(m_listBox);
    }

    private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;

      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

      // ONLY paint the background for this specific item to prevent wiping out other items
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
      string text = m_listBox.GetItemText(m_listBox.Items[e.Index]);

      Rectangle textRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 24, e.Bounds.Height);
      TextRenderer.DrawText(g, text, this.Font, textRect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      using (GraphicsPath path = GetRoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), m_borderRadius))
      {
        // Fill background of the wrapper
        using (SolidBrush brush = new SolidBrush(m_fieldBackColor))
        {
          g.FillPath(brush, path);
        }

        // Draw the border
        using (Pen pen = new Pen(m_borderColor, 1.5f))
        {
          g.DrawPath(pen, path);
        }
      }
    }

    [Category("Data")]
    [AttributeProvider(typeof(IListSource))]
     
    public object DataSource
    {
      get => m_listBox.DataSource;
      set { m_listBox.DataSource = value; this.Invalidate(); }
    }

    [Category("Data")]
     
    public string DisplayMember
    {
      get => m_listBox.DisplayMember;
      set { m_listBox.DisplayMember = value; this.Invalidate(); }
    }

    [Category("Data")]
     
    public string ValueMember
    {
      get => m_listBox.ValueMember;
      set { m_listBox.ValueMember = value; }
    }

    [Category("Data")]
    [Browsable(false)]
     
    public object SelectedValue
    {
      get => m_listBox.SelectedValue;
      set { m_listBox.SelectedValue = value; this.Invalidate(); }
    }

    [Category("Data")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ListBox.ObjectCollection Items => m_listBox.Items;

    [Category("Behavior")]
     
    public int SelectedIndex
    {
      get => m_listBox.SelectedIndex;
      set => m_listBox.SelectedIndex = value;
    }

    [Category("Behavior")]
     
    public object SelectedItem
    {
      get => m_listBox.SelectedItem;
      set => m_listBox.SelectedItem = value;
    }

    public ListBox.SelectedObjectCollection SelectedItems
    {
      get => m_listBox.SelectedItems;
    }

    [Category("Behavior")]
     
    public SelectionMode SelectionMode
    {
      get => m_listBox.SelectionMode;
      set => m_listBox.SelectionMode = value;
    }

    [Category("Appearance")]
     
    public Color FieldBackColor
    {
      get => m_fieldBackColor;
      set
      {
        m_fieldBackColor = value;
        m_listBox.BackColor = value;
        this.Invalidate();
      }
    }

     
    public Boolean FormattingEnabled
    {
      get => m_listBox.FormattingEnabled;
      set => m_listBox.FormattingEnabled = value;
    }

     
    public int ItemHeight
    {
      get => m_listBox.ItemHeight;
      set => m_listBox.ItemHeight = value;
    }

    public void RefreshItems()
    {
      m_listBox.RefreshItems();
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
