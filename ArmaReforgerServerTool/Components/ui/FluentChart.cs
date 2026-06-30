using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Longbow.Components.ui
{
  
  public class FluentChart : Chart
  {
    // Modern Windows 11 / Fluent color palette for the data series
    private readonly Color[] fluentPalette = new Color[]
    {
        Color.FromArgb(0, 120, 212),   // Win 11 Blue
        Color.FromArgb(0, 204, 106),   // Teal/Green
        Color.FromArgb(139, 60, 212),  // Purple
        Color.FromArgb(255, 140, 0),   // Orange
        Color.FromArgb(232, 17, 35),   // Red
        Color.FromArgb(0, 183, 195)    // Light Blue
    };

    public FluentChart()
    {
      // Smooth out the drawing of the lines and text
      this.AntiAliasing = AntiAliasingStyles.All;
      this.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

      // Remove the ugly default 3D border around the entire control
      this.BorderlineDashStyle = ChartDashStyle.NotSet;

      // Hook into the .NET 9 theme changes
      this.ForeColorChanged += (s, e) => ApplyFluentTheme();
      this.BackColorChanged += (s, e) => ApplyFluentTheme();
    }

    // You can call this manually if you add new ChartAreas or Series dynamically via code later
    public void ApplyFluentTheme()
    {
      this.SuspendLayout();

      // 1. Determine if Dark Mode is active based on the text color
      bool isDarkMode = this.ForeColor.R > 128;

      Color chartBackColor = isDarkMode ? Color.Transparent : Color.Transparent;
      Color areaBackColor = isDarkMode ? Color.FromArgb(32, 32, 32) : Color.White; // The actual plot area
      Color gridColor = isDarkMode ? Color.FromArgb(70, 70, 70) : Color.FromArgb(230, 230, 230);
      Color axisLineColor = isDarkMode ? Color.FromArgb(100, 100, 100) : Color.FromArgb(200, 200, 200);
      Color textColor = this.ForeColor;

      this.BackColor = chartBackColor;

      // 2. Format Chart Areas (The actual graph background and gridlines)
      foreach (ChartArea area in this.ChartAreas)
      {
        area.BackColor = areaBackColor;
        area.BorderColor = Color.Transparent; // Remove border around the graph

        // Disable 3D completely
        area.Area3DStyle.Enable3D = false;

        // X Axis Styling
        area.AxisX.LabelStyle.ForeColor = textColor;
        area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9f);
        area.AxisX.LineColor = axisLineColor;
        area.AxisX.MajorGrid.LineColor = gridColor;
        area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash; // Modern dotted grids
        area.AxisX.MajorTickMark.LineColor = axisLineColor;
        area.AxisX.MinorGrid.Enabled = false;

        // Y Axis Styling
        area.AxisY.LabelStyle.ForeColor = textColor;
        area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9f);
        area.AxisY.LineColor = Color.Transparent; // Hide Y axis spine for a cleaner look
        area.AxisY.MajorGrid.LineColor = gridColor;
        area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
        area.AxisY.MajorTickMark.LineColor = axisLineColor;
        area.AxisY.MinorGrid.Enabled = false;
      }

      // 3. Format Legends
      foreach (Legend legend in this.Legends)
      {
        legend.BackColor = Color.Transparent; // Blend into the form
        legend.ForeColor = textColor;
        legend.Font = new Font("Segoe UI", 9f);
        legend.BorderColor = Color.Transparent;

        // Fluent design puts legends at the top or bottom, spaced out
        legend.Alignment = StringAlignment.Center;
        legend.Docking = Docking.Top;
      }

      // 4. Format Titles
      foreach (Title title in this.Titles)
      {
        title.ForeColor = textColor;
        title.Font = new Font("Segoe UI Semibold", 12f);
      }

      // 5. Format Series (The data lines/bars)
      int colorIndex = 0;
      foreach (Series series in this.Series)
      {
        // Apply custom Fluent palette colors
        series.Color = fluentPalette[colorIndex % fluentPalette.Length];
        colorIndex++;

        // Thicker lines and flat bars
        series.BorderWidth = 3;

        // Strip any legacy gradients
        series.BackGradientStyle = GradientStyle.None;
        series.BackHatchStyle = ChartHatchStyle.None;

        // If it's a line chart, use smooth, rounded points
        if (series.ChartType == SeriesChartType.Line || series.ChartType == SeriesChartType.Spline)
        {
          series.MarkerStyle = MarkerStyle.Circle;
          series.MarkerSize = 8;
          series.MarkerBorderColor = areaBackColor; // Creates a neat cutout effect
          series.MarkerBorderWidth = 1;
        }
      }

      this.ResumeLayout();
    }

    // Automatically apply the theme when the control is first created and handles are bound
    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      ApplyFluentTheme();
    }
  }
}
