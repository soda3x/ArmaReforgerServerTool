using System.Drawing.Drawing2D;

namespace Longbow.Components.ui
{
  public class FluentChart : Control
  {
    private Queue<float> m_dataPoints = new Queue<float>();
    private int m_maxPoints = 50; // How many data points to show at once
    private Color m_lineColor = Color.FromArgb(0, 120, 212); // Fluent Blue
    private Point? m_hoverPoint = null; // Stores where the mouse is
    private float? m_hoverValue = null; // Stores the value at that point
    private string m_units = "%"; // Default to percent

    public FluentChart()
    {
      this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    public void AddDataPoint(float value)
    {
      m_dataPoints.Enqueue(value);
      if (m_dataPoints.Count > m_maxPoints)
        m_dataPoints.Dequeue();
      this.Invalidate();
    }

    public void Clear()
    {
      m_dataPoints.Clear();
      this.Invalidate();
    }

    public string Units
    {
      get => m_units;
      set => m_units = value;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (m_dataPoints.Count < 2)
        return;

      Graphics g = e.Graphics;
      g.SmoothingMode = SmoothingMode.AntiAlias;

      float xStep = (float)this.Width / (m_maxPoints - 1);
      List<PointF> points = new List<PointF>();

      for (int i = 0; i < m_dataPoints.Count; i++)
      {
        float x = i * xStep;
        // Normalize data (assuming 0-100 range)
        float y = this.Height - ((m_dataPoints.ElementAt(i) / 100f) * this.Height);
        points.Add(new PointF(x, y));
      }

      using (Pen pen = new Pen(m_lineColor, 2.5f))
      {
        g.DrawLines(pen, points.ToArray());
      }

      // Draw a subtle fill beneath the line
      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddLines(points.ToArray());
        path.AddLine(points.Last().X, this.Height, points.First().X, this.Height);
        path.CloseFigure();
        using (Brush brush = new SolidBrush(Color.FromArgb(30, m_lineColor)))
          g.FillPath(brush, path);
      }

      if (m_hoverPoint.HasValue && m_hoverValue.HasValue)
      {
        // Crosshair
        using (Pen p = new Pen(Color.FromArgb(100, ForeColor), 1f) { DashStyle = DashStyle.Dash })
        {
          g.DrawLine(p, m_hoverPoint.Value.X, 0, m_hoverPoint.Value.X, this.Height);
          g.DrawEllipse(new Pen(m_lineColor, 2f), m_hoverPoint.Value.X - 4, m_hoverPoint.Value.Y - 4, 8, 8);
        }

        // Tooltip
        string text = $"{m_hoverValue.Value:F1} {m_units}";
        SizeF textSize = g.MeasureString(text, this.Font);
        RectangleF rect = new RectangleF(m_hoverPoint.Value.X + 10, m_hoverPoint.Value.Y - 20, textSize.Width + 10, textSize.Height + 5);

        using (SolidBrush bg = new SolidBrush(Color.FromArgb(240, 32, 32, 32))) // Fluent dark theme style
        using (Pen border = new Pen(Color.FromArgb(100, 100, 100)))
        {
          g.FillRectangle(bg, rect);
          g.DrawRectangle(border, Rectangle.Round(rect));
        }
        TextRenderer.DrawText(g, text, this.Font, Rectangle.Round(rect), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
      }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (m_dataPoints.Count < 2)
        return;

      // Map mouse X to the nearest data point index
      float xStep = (float)this.Width / (m_maxPoints - 1);
      int index = (int)Math.Round(e.X / xStep);

      if (index >= 0 && index < m_dataPoints.Count)
      {
        m_hoverPoint = new Point(e.X, (int) (this.Height - ((m_dataPoints.ElementAt(index) / 100f) * this.Height)));
        m_hoverValue = m_dataPoints.ElementAt(index);
        this.Invalidate();
      }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      m_hoverPoint = null;
      m_hoverValue = null;
      this.Invalidate();
    }
  }
}
