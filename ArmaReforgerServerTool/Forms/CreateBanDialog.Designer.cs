namespace Longbow.Forms
{
  partial class CreateBanDialog
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
      quickBan1HrBtn = new FontAwesome.Sharp.IconButton();
      creatingBanForLabel = new Label();
      quickBan24HoursBtn = new FontAwesome.Sharp.IconButton();
      banBtn = new FontAwesome.Sharp.IconButton();
      label1 = new Label();
      banDurationSecondsTB = new Longbow.Components.ui.FluentTextBox();
      quickBanForever = new FontAwesome.Sharp.IconButton();
      banReasonTB = new Longbow.Components.ui.FluentTextBox();
      SuspendLayout();
      // 
      // quickBan1HrBtn
      // 
      quickBan1HrBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      quickBan1HrBtn.IconChar = FontAwesome.Sharp.IconChar.Gavel;
      quickBan1HrBtn.IconColor = Color.Black;
      quickBan1HrBtn.IconFont = FontAwesome.Sharp.IconFont.Solid;
      quickBan1HrBtn.IconSize = 24;
      quickBan1HrBtn.ImageAlign = ContentAlignment.MiddleRight;
      quickBan1HrBtn.Location = new Point(12, 167);
      quickBan1HrBtn.Name = "quickBan1HrBtn";
      quickBan1HrBtn.Size = new Size(125, 32);
      quickBan1HrBtn.TabIndex = 0;
      quickBan1HrBtn.Text = "Ban for 1 Hour";
      quickBan1HrBtn.TextAlign = ContentAlignment.MiddleLeft;
      quickBan1HrBtn.UseVisualStyleBackColor = true;
      quickBan1HrBtn.Click += OnBan1HrBtnPressed;
      // 
      // creatingBanForLabel
      // 
      creatingBanForLabel.Dock = DockStyle.Top;
      creatingBanForLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
      creatingBanForLabel.Location = new Point(0, 0);
      creatingBanForLabel.Name = "creatingBanForLabel";
      creatingBanForLabel.Size = new Size(484, 32);
      creatingBanForLabel.TabIndex = 12;
      creatingBanForLabel.Text = "Banning user foo...";
      creatingBanForLabel.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // quickBan24HoursBtn
      // 
      quickBan24HoursBtn.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      quickBan24HoursBtn.IconChar = FontAwesome.Sharp.IconChar.Gavel;
      quickBan24HoursBtn.IconColor = Color.Black;
      quickBan24HoursBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      quickBan24HoursBtn.IconSize = 24;
      quickBan24HoursBtn.ImageAlign = ContentAlignment.MiddleRight;
      quickBan24HoursBtn.Location = new Point(179, 167);
      quickBan24HoursBtn.Name = "quickBan24HoursBtn";
      quickBan24HoursBtn.Size = new Size(125, 32);
      quickBan24HoursBtn.TabIndex = 14;
      quickBan24HoursBtn.Text = "Ban for 24 Hours";
      quickBan24HoursBtn.TextAlign = ContentAlignment.MiddleLeft;
      quickBan24HoursBtn.UseVisualStyleBackColor = true;
      quickBan24HoursBtn.Click += OnBan24HrBtnPressed;
      // 
      // banBtn
      // 
      banBtn.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
      banBtn.IconChar = FontAwesome.Sharp.IconChar.Gavel;
      banBtn.IconColor = Color.Black;
      banBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
      banBtn.IconSize = 24;
      banBtn.ImageAlign = ContentAlignment.MiddleRight;
      banBtn.Location = new Point(380, 85);
      banBtn.Name = "banBtn";
      banBtn.Size = new Size(92, 32);
      banBtn.TabIndex = 15;
      banBtn.Text = "Ban";
      banBtn.TextAlign = ContentAlignment.MiddleLeft;
      banBtn.UseVisualStyleBackColor = true;
      banBtn.Click += OnBanBtnPressed;
      // 
      // label1
      // 
      label1.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
      label1.Location = new Point(12, 128);
      label1.Name = "label1";
      label1.Size = new Size(460, 32);
      label1.TabIndex = 16;
      label1.Text = "Quick Bans";
      label1.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // banDurationSecondsTB
      // 
      banDurationSecondsTB.BackColor = Color.Transparent;
      banDurationSecondsTB.FieldBackColor = SystemColors.Window;
      banDurationSecondsTB.Location = new Point(12, 85);
      banDurationSecondsTB.Multiline = false;
      banDurationSecondsTB.Name = "banDurationSecondsTB";
      banDurationSecondsTB.Padding = new Padding(10, 7, 10, 7);
      banDurationSecondsTB.PlaceholderText = "Ban Duration in seconds, or choose a Quick Ban";
      banDurationSecondsTB.ReadOnly = false;
      banDurationSecondsTB.ScrollBars = ScrollBars.None;
      banDurationSecondsTB.Size = new Size(362, 32);
      banDurationSecondsTB.TabIndex = 17;
      banDurationSecondsTB.UseSystemPasswordChar = false;
      // 
      // quickBanForever
      // 
      quickBanForever.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
      quickBanForever.IconChar = FontAwesome.Sharp.IconChar.Gavel;
      quickBanForever.IconColor = Color.Black;
      quickBanForever.IconFont = FontAwesome.Sharp.IconFont.Solid;
      quickBanForever.IconSize = 24;
      quickBanForever.ImageAlign = ContentAlignment.MiddleRight;
      quickBanForever.Location = new Point(347, 167);
      quickBanForever.Name = "quickBanForever";
      quickBanForever.Size = new Size(125, 32);
      quickBanForever.TabIndex = 18;
      quickBanForever.Text = "Ban Forever";
      quickBanForever.TextAlign = ContentAlignment.MiddleLeft;
      quickBanForever.UseVisualStyleBackColor = true;
      quickBanForever.Click += OnBanForeverBtnPressed;
      // 
      // banReasonTB
      // 
      banReasonTB.BackColor = Color.Transparent;
      banReasonTB.FieldBackColor = SystemColors.Window;
      banReasonTB.Location = new Point(12, 40);
      banReasonTB.Multiline = false;
      banReasonTB.Name = "banReasonTB";
      banReasonTB.Padding = new Padding(10, 7, 10, 7);
      banReasonTB.PlaceholderText = "Optional, enter a reason for the ban...";
      banReasonTB.ReadOnly = false;
      banReasonTB.ScrollBars = ScrollBars.None;
      banReasonTB.Size = new Size(460, 32);
      banReasonTB.TabIndex = 19;
      banReasonTB.UseSystemPasswordChar = false;
      // 
      // CreateBanDialog
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(484, 211);
      Controls.Add(banReasonTB);
      Controls.Add(quickBanForever);
      Controls.Add(banDurationSecondsTB);
      Controls.Add(label1);
      Controls.Add(banBtn);
      Controls.Add(quickBan24HoursBtn);
      Controls.Add(creatingBanForLabel);
      Controls.Add(quickBan1HrBtn);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      MaximumSize = new Size(500, 250);
      MinimumSize = new Size(500, 250);
      Name = "CreateBanDialog";
      ShowIcon = false;
      Text = "Longbow - Ban Creator";
      ResumeLayout(false);
    }

    #endregion

    private FontAwesome.Sharp.IconButton quickBan1HrBtn;
    private Label creatingBanForLabel;
    private FontAwesome.Sharp.IconButton quickBan24HoursBtn;
    private FontAwesome.Sharp.IconButton banBtn;
    private Label label1;
    private Components.ui.FluentTextBox banDurationSecondsTB;
    private FontAwesome.Sharp.IconButton quickBanForever;
    private Components.ui.FluentTextBox banReasonTB;
  }
}