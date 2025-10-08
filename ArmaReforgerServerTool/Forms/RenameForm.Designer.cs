namespace Longbow.Forms
{
  partial class RenameForm
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
      renameTB = new TextBox();
      renameBtn = new FontAwesome.Sharp.IconButton();
      cancelBtn = new FontAwesome.Sharp.IconButton();
      tableLayoutPanel1 = new TableLayoutPanel();
      tableLayoutPanel1.SuspendLayout();
      SuspendLayout();
      // 
      // renameTB
      // 
      renameTB.Location = new Point(12, 12);
      renameTB.Name = "renameTB";
      renameTB.Size = new Size(260, 23);
      renameTB.TabIndex = 0;
      // 
      // renameBtn
      // 
      renameBtn.Dock = DockStyle.Fill;
      renameBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      renameBtn.IconColor = Color.Black;
      renameBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      renameBtn.Location = new Point(100, 0);
      renameBtn.Margin = new Padding(0);
      renameBtn.Name = "renameBtn";
      renameBtn.Size = new Size(100, 23);
      renameBtn.TabIndex = 1;
      renameBtn.Text = "Rename";
      renameBtn.UseVisualStyleBackColor = true;
      renameBtn.Click += RenameBtnPressed;
      // 
      // cancelBtn
      // 
      cancelBtn.Dock = DockStyle.Fill;
      cancelBtn.IconChar = FontAwesome.Sharp.IconChar.None;
      cancelBtn.IconColor = Color.Black;
      cancelBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      cancelBtn.Location = new Point(0, 0);
      cancelBtn.Margin = new Padding(0);
      cancelBtn.Name = "cancelBtn";
      cancelBtn.Size = new Size(100, 23);
      cancelBtn.TabIndex = 2;
      cancelBtn.Text = "Cancel";
      cancelBtn.UseVisualStyleBackColor = true;
      cancelBtn.Click += CancelBtnPressed;
      // 
      // tableLayoutPanel1
      // 
      tableLayoutPanel1.ColumnCount = 2;
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
      tableLayoutPanel1.Controls.Add(cancelBtn, 0, 0);
      tableLayoutPanel1.Controls.Add(renameBtn, 1, 0);
      tableLayoutPanel1.Location = new Point(72, 41);
      tableLayoutPanel1.Name = "tableLayoutPanel1";
      tableLayoutPanel1.RowCount = 1;
      tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
      tableLayoutPanel1.Size = new Size(200, 23);
      tableLayoutPanel1.TabIndex = 3;
      // 
      // RenameForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(284, 73);
      ControlBox = false;
      Controls.Add(tableLayoutPanel1);
      Controls.Add(renameTB);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Name = "RenameForm";
      ShowIcon = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - Rename Save";
      tableLayoutPanel1.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox renameTB;
    private FontAwesome.Sharp.IconButton renameBtn;
    private FontAwesome.Sharp.IconButton cancelBtn;
    private TableLayoutPanel tableLayoutPanel1;
  }
}