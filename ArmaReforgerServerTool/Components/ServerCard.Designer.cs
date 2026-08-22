using FontAwesome.Sharp;

namespace Longbow.Components
{
  partial class ServerCard
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>


    private void InitializeComponent()
    {
      lblServerName = new Label();
      lblStatus = new Label();
      lblMetrics = new Label();
      btnGoToServer = new IconButton();
      btnRemoveServer = new IconButton();
      armaGameLbl = new Label();
      launchReconBtn = new IconButton();
      SuspendLayout();
      ToolTip tooltip = new ToolTip();
      // 
      // lblServerName
      // 
      lblServerName.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
      lblServerName.ForeColor = Color.FromArgb(240, 240, 240);
      lblServerName.Location = new Point(14, 14);
      lblServerName.Name = "lblServerName";
      lblServerName.Size = new Size(150, 24);
      lblServerName.TabIndex = 0;
      lblServerName.Text = "Server Instance";
      // 
      // lblStatus
      // 
      lblStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
      lblStatus.ForeColor = Color.FromArgb(57, 197, 107);
      lblStatus.Location = new Point(168, 42);
      lblStatus.Name = "lblStatus";
      lblStatus.Size = new Size(96, 20);
      lblStatus.TabIndex = 1;
      lblStatus.Text = "● Running";
      lblStatus.TextAlign = ContentAlignment.TopRight;
      // 
      // lblMetrics
      // 
      lblMetrics.Font = new Font("Segoe UI", 9F);
      lblMetrics.ForeColor = Color.FromArgb(160, 160, 160);
      lblMetrics.Location = new Point(14, 42);
      lblMetrics.Name = "lblMetrics";
      lblMetrics.Size = new Size(250, 20);
      lblMetrics.TabIndex = 2;
      lblMetrics.Text = "CPU: 0%  •  RAM: 0 MB";
      // 
      // btnGoToServer
      // 
      btnGoToServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnGoToServer.IconChar = IconChar.ArrowRight;
      btnGoToServer.IconColor = Color.Black;
      btnGoToServer.IconFont = IconFont.Auto;
      btnGoToServer.IconSize = 20;
      btnGoToServer.Location = new Point(232, 68);
      btnGoToServer.Name = "btnGoToServer";
      btnGoToServer.Size = new Size(32, 32);
      btnGoToServer.TabIndex = 3;
      btnGoToServer.UseVisualStyleBackColor = true;
      btnGoToServer.Click += OnGoToServerPressed;
      tooltip.SetToolTip(btnGoToServer, "Go to Server");
      // 
      // btnRemoveServer
      // 
      btnRemoveServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      btnRemoveServer.IconChar = IconChar.Trash;
      btnRemoveServer.IconColor = Color.Black;
      btnRemoveServer.IconFont = IconFont.Auto;
      btnRemoveServer.IconSize = 20;
      btnRemoveServer.Location = new Point(14, 70);
      btnRemoveServer.Name = "btnRemoveServer";
      btnRemoveServer.Size = new Size(32, 32);
      btnRemoveServer.TabIndex = 4;
      btnRemoveServer.UseVisualStyleBackColor = true;
      btnRemoveServer.Click += OnRemoveServerPressed;
      tooltip.SetToolTip(btnRemoveServer, "Remove Server");
      // 
      // armaGameLbl
      // 
      armaGameLbl.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
      armaGameLbl.ForeColor = SystemColors.ButtonShadow;
      armaGameLbl.Location = new Point(170, 14);
      armaGameLbl.Name = "armaGameLbl";
      armaGameLbl.Size = new Size(96, 20);
      armaGameLbl.TabIndex = 5;
      armaGameLbl.Text = "Arma Reforger";
      armaGameLbl.TextAlign = ContentAlignment.MiddleRight;
      tooltip.SetToolTip(armaGameLbl, ";)");

      // 
      // launchReconBtn
      // 
      launchReconBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      launchReconBtn.IconChar = IconChar.Binoculars;
      launchReconBtn.IconColor = Color.Black;
      launchReconBtn.IconFont = IconFont.Auto;
      launchReconBtn.IconSize = 20;
      launchReconBtn.Location = new Point(185, 68);
      launchReconBtn.Name = "launchReconBtn";
      launchReconBtn.Size = new Size(32, 32);
      launchReconBtn.TabIndex = 6;
      launchReconBtn.UseVisualStyleBackColor = true;
      tooltip.SetToolTip(launchReconBtn, "Launch ReCON Client and monitor this Server");
      // 
      // ServerCard
      // 
      BackColor = Color.FromArgb(37, 37, 38);
      Controls.Add(launchReconBtn);
      Controls.Add(armaGameLbl);
      Controls.Add(btnRemoveServer);
      Controls.Add(lblServerName);
      Controls.Add(lblStatus);
      Controls.Add(lblMetrics);
      Controls.Add(btnGoToServer);
      Margin = new Padding(10);
      Name = "ServerCard";
      Padding = new Padding(16);
      Size = new Size(280, 115);
      ResumeLayout(false);
    }

    #endregion
    private Label lblServerName;
    private Label lblStatus;
    private Label lblMetrics;
    private IconButton btnGoToServer;
    private IconButton btnRemoveServer;
    private Label armaGameLbl;
    private IconButton launchReconBtn;
  }
}
