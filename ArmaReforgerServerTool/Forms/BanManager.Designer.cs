using Longbow.Components.ui;

namespace Longbow.Forms
{
  partial class BanManager
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
      DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
      DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
      banList = new FluentTableView();
      unbanButton = new FontAwesome.Sharp.IconButton();
      cancelButton = new FontAwesome.Sharp.IconButton();
      ((System.ComponentModel.ISupportInitialize) banList).BeginInit();
      SuspendLayout();
      // 
      // banList
      // 
      banList.AllowUserToAddRows = false;
      banList.AllowUserToDeleteRows = false;
      banList.AllowUserToResizeRows = false;
      banList.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      banList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      banList.BackgroundColor = Color.White;
      banList.BorderStyle = BorderStyle.None;
      banList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
      banList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
      dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = Color.White;
      dataGridViewCellStyle1.Font = new Font("Segoe UI Semibold", 10F);
      dataGridViewCellStyle1.ForeColor = Color.FromArgb(  60,   60,   60);
      dataGridViewCellStyle1.Padding = new Padding(10, 8, 10, 8);
      dataGridViewCellStyle1.SelectionBackColor = Color.White;
      dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(  60,   60,   60);
      banList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      banList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle2.BackColor = Color.White;
      dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.5F);
      dataGridViewCellStyle2.ForeColor = Color.Black;
      dataGridViewCellStyle2.Padding = new Padding(10, 4, 10, 4);
      dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(  240,   245,   255);
      dataGridViewCellStyle2.SelectionForeColor = Color.Black;
      dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
      banList.DefaultCellStyle = dataGridViewCellStyle2;
      banList.EnableHeadersVisualStyles = false;
      banList.GridColor = Color.FromArgb(  235,   235,   235);
      banList.Location = new Point(12, 12);
      banList.MultiSelect = false;
      banList.Name = "banList";
      banList.ReadOnly = true;
      banList.RowHeadersVisible = false;
      dataGridViewCellStyle3.BackColor = Color.Transparent;
      banList.RowsDefaultCellStyle = dataGridViewCellStyle3;
      banList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      banList.Size = new Size(776, 388);
      banList.TabIndex = 0;
      // 
      // unbanButton
      // 
      unbanButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      unbanButton.IconChar = FontAwesome.Sharp.IconChar.None;
      unbanButton.IconColor = Color.Black;
      unbanButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
      unbanButton.Location = new Point(713, 406);
      unbanButton.Name = "unbanButton";
      unbanButton.Size = new Size(75, 32);
      unbanButton.TabIndex = 1;
      unbanButton.Text = "Unban";
      unbanButton.UseVisualStyleBackColor = true;
      // 
      // cancelButton
      // 
      cancelButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      cancelButton.IconChar = FontAwesome.Sharp.IconChar.None;
      cancelButton.IconColor = Color.Black;
      cancelButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
      cancelButton.Location = new Point(12, 406);
      cancelButton.Name = "cancelButton";
      cancelButton.Size = new Size(75, 32);
      cancelButton.TabIndex = 2;
      cancelButton.Text = "Cancel";
      cancelButton.UseVisualStyleBackColor = true;
      // 
      // BanManager
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 450);
      Controls.Add(cancelButton);
      Controls.Add(unbanButton);
      Controls.Add(banList);
      MinimumSize = new Size(640, 480);
      Name = "BanManager";
      ShowIcon = false;
      Text = "Longbow - Ban Manager";
      Load += BanManagerLoad;
      ((System.ComponentModel.ISupportInitialize) banList).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private FluentTableView banList;
    private FontAwesome.Sharp.IconButton unbanButton;
    private FontAwesome.Sharp.IconButton cancelButton;
  }
}
