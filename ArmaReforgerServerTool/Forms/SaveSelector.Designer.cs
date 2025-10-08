namespace Longbow.Forms
{
  partial class SaveSelector
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveSelector));
      savedGamesList = new ListBox();
      loadSelectedSaveBtn = new FontAwesome.Sharp.IconButton();
      keepPermanentlyBtn = new FontAwesome.Sharp.IconButton();
      deleteSaveBtn = new FontAwesome.Sharp.IconButton();
      tableLayoutPanel1 = new TableLayoutPanel();
      renameSaveBtn = new FontAwesome.Sharp.IconButton();
      useLatestSaveBtn = new FontAwesome.Sharp.IconButton();
      clearSaveBtn = new FontAwesome.Sharp.IconButton();
      loadedSaveLabel = new Label();
      groupBox1 = new GroupBox();
      tableLayoutPanel1.SuspendLayout();
      groupBox1.SuspendLayout();
      SuspendLayout();
      // 
      // savedGamesList
      // 
      savedGamesList.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      savedGamesList.FormattingEnabled = true;
      savedGamesList.ItemHeight = 15;
      savedGamesList.Location = new Point(12, 12);
      savedGamesList.Name = "savedGamesList";
      savedGamesList.Size = new Size(600, 349);
      savedGamesList.TabIndex = 0;
      savedGamesList.SelectedIndexChanged += SavedGamesListSelectedIndexChanged;
      // 
      // loadSelectedSaveBtn
      // 
      loadSelectedSaveBtn.Dock = DockStyle.Fill;
      loadSelectedSaveBtn.Enabled = false;
      loadSelectedSaveBtn.IconChar = FontAwesome.Sharp.IconChar.FileUpload;
      loadSelectedSaveBtn.IconColor = Color.Black;
      loadSelectedSaveBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      loadSelectedSaveBtn.IconSize = 16;
      loadSelectedSaveBtn.ImageAlign = ContentAlignment.MiddleLeft;
      loadSelectedSaveBtn.Location = new Point(0, 0);
      loadSelectedSaveBtn.Margin = new Padding(0);
      loadSelectedSaveBtn.Name = "loadSelectedSaveBtn";
      loadSelectedSaveBtn.Size = new Size(97, 23);
      loadSelectedSaveBtn.TabIndex = 1;
      loadSelectedSaveBtn.Text = "Select";
      loadSelectedSaveBtn.TextAlign = ContentAlignment.MiddleRight;
      loadSelectedSaveBtn.UseVisualStyleBackColor = true;
      loadSelectedSaveBtn.Click += LoadBtnPressed;
      // 
      // keepPermanentlyBtn
      // 
      keepPermanentlyBtn.Dock = DockStyle.Fill;
      keepPermanentlyBtn.Enabled = false;
      keepPermanentlyBtn.IconChar = FontAwesome.Sharp.IconChar.BookBookmark;
      keepPermanentlyBtn.IconColor = Color.Black;
      keepPermanentlyBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      keepPermanentlyBtn.IconSize = 16;
      keepPermanentlyBtn.ImageAlign = ContentAlignment.MiddleLeft;
      keepPermanentlyBtn.Location = new Point(291, 0);
      keepPermanentlyBtn.Margin = new Padding(0);
      keepPermanentlyBtn.Name = "keepPermanentlyBtn";
      keepPermanentlyBtn.Size = new Size(97, 23);
      keepPermanentlyBtn.TabIndex = 2;
      keepPermanentlyBtn.Text = "Keep";
      keepPermanentlyBtn.TextAlign = ContentAlignment.MiddleRight;
      keepPermanentlyBtn.UseVisualStyleBackColor = true;
      keepPermanentlyBtn.Click += KeepBtnPressed;
      // 
      // deleteSaveBtn
      // 
      deleteSaveBtn.Dock = DockStyle.Fill;
      deleteSaveBtn.Enabled = false;
      deleteSaveBtn.IconChar = FontAwesome.Sharp.IconChar.Trash;
      deleteSaveBtn.IconColor = Color.Black;
      deleteSaveBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      deleteSaveBtn.IconSize = 16;
      deleteSaveBtn.ImageAlign = ContentAlignment.MiddleLeft;
      deleteSaveBtn.Location = new Point(485, 0);
      deleteSaveBtn.Margin = new Padding(0);
      deleteSaveBtn.Name = "deleteSaveBtn";
      deleteSaveBtn.Size = new Size(103, 23);
      deleteSaveBtn.TabIndex = 3;
      deleteSaveBtn.Text = "Delete";
      deleteSaveBtn.TextAlign = ContentAlignment.MiddleRight;
      deleteSaveBtn.UseVisualStyleBackColor = true;
      deleteSaveBtn.Click += DeleteBtnPressed;
      // 
      // tableLayoutPanel1
      // 
      tableLayoutPanel1.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      tableLayoutPanel1.ColumnCount = 6;
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
      tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
      tableLayoutPanel1.Controls.Add(loadSelectedSaveBtn, 0, 0);
      tableLayoutPanel1.Controls.Add(keepPermanentlyBtn, 3, 0);
      tableLayoutPanel1.Controls.Add(renameSaveBtn, 4, 0);
      tableLayoutPanel1.Controls.Add(deleteSaveBtn, 5, 0);
      tableLayoutPanel1.Controls.Add(useLatestSaveBtn, 1, 0);
      tableLayoutPanel1.Controls.Add(clearSaveBtn, 2, 0);
      tableLayoutPanel1.Location = new Point(6, 39);
      tableLayoutPanel1.Name = "tableLayoutPanel1";
      tableLayoutPanel1.RowCount = 1;
      tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      tableLayoutPanel1.Size = new Size(588, 23);
      tableLayoutPanel1.TabIndex = 4;
      // 
      // renameSaveBtn
      // 
      renameSaveBtn.Dock = DockStyle.Fill;
      renameSaveBtn.Enabled = false;
      renameSaveBtn.IconChar = FontAwesome.Sharp.IconChar.ICursor;
      renameSaveBtn.IconColor = Color.Black;
      renameSaveBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      renameSaveBtn.IconSize = 16;
      renameSaveBtn.ImageAlign = ContentAlignment.MiddleLeft;
      renameSaveBtn.Location = new Point(388, 0);
      renameSaveBtn.Margin = new Padding(0);
      renameSaveBtn.Name = "renameSaveBtn";
      renameSaveBtn.Size = new Size(97, 23);
      renameSaveBtn.TabIndex = 4;
      renameSaveBtn.Text = "Rename";
      renameSaveBtn.TextAlign = ContentAlignment.MiddleRight;
      renameSaveBtn.UseVisualStyleBackColor = true;
      renameSaveBtn.Click += RenameBtnPressed;
      // 
      // useLatestSaveBtn
      // 
      useLatestSaveBtn.Dock = DockStyle.Fill;
      useLatestSaveBtn.IconChar = FontAwesome.Sharp.IconChar.ClockRotateLeft;
      useLatestSaveBtn.IconColor = Color.Black;
      useLatestSaveBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      useLatestSaveBtn.IconSize = 16;
      useLatestSaveBtn.ImageAlign = ContentAlignment.MiddleLeft;
      useLatestSaveBtn.Location = new Point(97, 0);
      useLatestSaveBtn.Margin = new Padding(0);
      useLatestSaveBtn.Name = "useLatestSaveBtn";
      useLatestSaveBtn.Size = new Size(97, 23);
      useLatestSaveBtn.TabIndex = 5;
      useLatestSaveBtn.Text = "Latest";
      useLatestSaveBtn.TextAlign = ContentAlignment.MiddleRight;
      useLatestSaveBtn.UseVisualStyleBackColor = true;
      useLatestSaveBtn.Click += LatestBtnPressed;
      // 
      // clearSaveBtn
      // 
      clearSaveBtn.Dock = DockStyle.Fill;
      clearSaveBtn.IconChar = FontAwesome.Sharp.IconChar.Eject;
      clearSaveBtn.IconColor = Color.Black;
      clearSaveBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      clearSaveBtn.IconSize = 16;
      clearSaveBtn.ImageAlign = ContentAlignment.MiddleLeft;
      clearSaveBtn.Location = new Point(194, 0);
      clearSaveBtn.Margin = new Padding(0);
      clearSaveBtn.Name = "clearSaveBtn";
      clearSaveBtn.Size = new Size(97, 23);
      clearSaveBtn.TabIndex = 6;
      clearSaveBtn.Text = "Eject";
      clearSaveBtn.TextAlign = ContentAlignment.MiddleRight;
      clearSaveBtn.UseVisualStyleBackColor = true;
      clearSaveBtn.Click += ClearSaveBtnPressed;
      // 
      // loadedSaveLabel
      // 
      loadedSaveLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      loadedSaveLabel.Location = new Point(6, 19);
      loadedSaveLabel.Name = "loadedSaveLabel";
      loadedSaveLabel.Size = new Size(588, 15);
      loadedSaveLabel.TabIndex = 5;
      loadedSaveLabel.Text = "No save loaded. Your server will not use a save.";
      loadedSaveLabel.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // groupBox1
      // 
      groupBox1.Anchor = AnchorStyles.Bottom;
      groupBox1.Controls.Add(loadedSaveLabel);
      groupBox1.Controls.Add(tableLayoutPanel1);
      groupBox1.Location = new Point(12, 367);
      groupBox1.Name = "groupBox1";
      groupBox1.Size = new Size(600, 68);
      groupBox1.TabIndex = 6;
      groupBox1.TabStop = false;
      // 
      // SaveSelector
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(624, 441);
      Controls.Add(groupBox1);
      Controls.Add(savedGamesList);
      Icon = (Icon) resources.GetObject("$this.Icon");
      MinimumSize = new Size(640, 480);
      Name = "SaveSelector";
      ShowIcon = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Longbow - Save Manager";
      tableLayoutPanel1.ResumeLayout(false);
      groupBox1.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private ListBox savedGamesList;
    private FontAwesome.Sharp.IconButton loadSelectedSaveBtn;
    private FontAwesome.Sharp.IconButton keepPermanentlyBtn;
    private FontAwesome.Sharp.IconButton deleteSaveBtn;
    private TableLayoutPanel tableLayoutPanel1;
    private FontAwesome.Sharp.IconButton renameSaveBtn;
    private Label loadedSaveLabel;
    private FontAwesome.Sharp.IconButton useLatestSaveBtn;
    private FontAwesome.Sharp.IconButton clearSaveBtn;
    private GroupBox groupBox1;
  }
}
