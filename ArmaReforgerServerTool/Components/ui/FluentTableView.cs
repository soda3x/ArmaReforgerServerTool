using System;
using System.Collections.Generic;
using System.Text;

namespace Longbow.Components.ui
{
  public class FluentTableView : DataGridView
  {
    public FluentTableView()
    {
      this.BackgroundColor = SystemColors.Window; // Automatically dark grey/black in Dark Mode, white in Light Mode
      this.BorderStyle = BorderStyle.None;
      this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
      this.GridColor = SystemColors.ControlDark;
      DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
      headerStyle.BackColor = SystemColors.Control;
      headerStyle.ForeColor = SystemColors.ControlText;
      headerStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Regular, GraphicsUnit.Point);
      headerStyle.SelectionBackColor = SystemColors.Control;
      headerStyle.SelectionForeColor = SystemColors.ControlText;
      headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      headerStyle.Padding = new Padding(10, 8, 10, 8);
      this.ColumnHeadersDefaultCellStyle = headerStyle;

      DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
      cellStyle.BackColor = SystemColors.Window;
      cellStyle.ForeColor = SystemColors.WindowText;
      cellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
      cellStyle.SelectionBackColor = SystemColors.Highlight;       // Native system accent color
      cellStyle.SelectionForeColor = SystemColors.HighlightText;   // Always readable against the accent
      cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      cellStyle.Padding = new Padding(10, 4, 10, 4);
      this.DefaultCellStyle = cellStyle;

      this.EnableHeadersVisualStyles = false;
      this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
      this.RowHeadersVisible = false; // Hides the ugly left-hand arrow column

      this.AllowUserToAddRows = false;
      this.AllowUserToDeleteRows = false;
      this.AllowUserToResizeRows = false;
      this.ReadOnly = true; // Set to false if you need in-line editing
      this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.MultiSelect = false;
      this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

      this.ColumnHeadersHeight = 40;
      this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

      this.RowTemplate.Height = 35;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      if (this.Columns.Count > 0)
      {
        int y = this.ColumnHeadersHeight - 1;
        // Use ControlDarkDark for a border that works in both light and dark modes
        using (Pen pen = new Pen(SystemColors.ControlDarkDark, 1))
        {
          e.Graphics.DrawLine(pen, 0, y, this.Width, y);
        }
      }
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
      base.OnBackColorChanged(e);

      // When the ThemeManager changes the table's background, push it to the cells
      this.BackgroundColor = this.BackColor;
      this.DefaultCellStyle.BackColor = this.BackColor;
      this.DefaultCellStyle.SelectionBackColor = Color.FromArgb(40, 130, 220); // A safe Fluent blue

      // Make the header slightly offset from the background so it stands out
      // If it's dark mode (BackColor.R < 100), make header slightly lighter. Otherwise, slightly darker.
      if (this.BackColor.R < 100)
        this.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(this.BackColor.R + 15, this.BackColor.G + 15, this.BackColor.B + 15);
      else
        this.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(this.BackColor.R - 10, this.BackColor.G - 10, this.BackColor.B - 10);
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
      base.OnForeColorChanged(e);

      // Push the text color down to the cells and headers
      this.DefaultCellStyle.ForeColor = this.ForeColor;
      this.ColumnHeadersDefaultCellStyle.ForeColor = this.ForeColor;
      this.DefaultCellStyle.SelectionForeColor = Color.White;
    }
  }
}
