using FontAwesome.Sharp;
using System.ComponentModel;

namespace Longbow.Forms
{
  partial class Dashboard
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
      ComponentResourceManager resources = new ComponentResourceManager(typeof(Dashboard));
      panelBottom = new Panel();
      btnNewServer = new IconButton();
      flowPanelCards = new FlowLayoutPanel();
      lblEmptyState = new Label();
      btnLaunchRecon = new IconButton();
      panelBottom.SuspendLayout();
      SuspendLayout();
      // 
      // panelBottom
      // 
      panelBottom.BackColor = Color.Transparent;
      panelBottom.Controls.Add(btnLaunchRecon);
      panelBottom.Controls.Add(btnNewServer);
      panelBottom.Dock = DockStyle.Bottom;
      panelBottom.Location = new Point(0, 451);
      panelBottom.Name = "panelBottom";
      panelBottom.Size = new Size(834, 60);
      panelBottom.TabIndex = 1;
      // 
      // btnNewServer
      // 
      btnNewServer.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      btnNewServer.IconChar = IconChar.Add;
      btnNewServer.IconColor = Color.Black;
      btnNewServer.IconFont = IconFont.Auto;
      btnNewServer.IconSize = 20;
      btnNewServer.ImageAlign = ContentAlignment.MiddleLeft;
      btnNewServer.Location = new Point(15, 15);
      btnNewServer.Margin = new Padding(0);
      btnNewServer.Name = "btnNewServer";
      btnNewServer.Padding = new Padding(5);
      btnNewServer.Size = new Size(125, 32);
      btnNewServer.TabIndex = 0;
      btnNewServer.Text = "Add Server";
      btnNewServer.TextAlign = ContentAlignment.MiddleRight;
      btnNewServer.UseVisualStyleBackColor = true;
      btnNewServer.Click += OnNewServerPressed;
      // 
      // flowPanelCards
      // 
      flowPanelCards.AutoScroll = true;
      flowPanelCards.Dock = DockStyle.Fill;
      flowPanelCards.Location = new Point(0, 0);
      flowPanelCards.Name = "flowPanelCards";
      flowPanelCards.Padding = new Padding(10);
      flowPanelCards.Size = new Size(834, 451);
      flowPanelCards.TabIndex = 0;
      // 
      // lblEmptyState
      // 
      lblEmptyState.Anchor = AnchorStyles.None;
      lblEmptyState.Font = new Font("Segoe UI", 12F);
      lblEmptyState.ForeColor = Color.FromArgb(  140,   140,   140);
      lblEmptyState.Location = new Point(0, 0);
      lblEmptyState.Name = "lblEmptyState";
      lblEmptyState.Size = new Size(350, 80);
      lblEmptyState.TabIndex = 0;
      lblEmptyState.Text = "You don't have any servers yet.\nPress '+' to create one.";
      lblEmptyState.TextAlign = ContentAlignment.MiddleCenter;
      CenterEmptyLabel();
      // 
      // btnLaunchRecon
      // 
      btnLaunchRecon.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
      btnLaunchRecon.IconChar = IconChar.Binoculars;
      btnLaunchRecon.IconColor = Color.Black;
      btnLaunchRecon.IconFont = IconFont.Auto;
      btnLaunchRecon.IconSize = 20;
      btnLaunchRecon.ImageAlign = ContentAlignment.MiddleLeft;
      btnLaunchRecon.Location = new Point(154, 15);
      btnLaunchRecon.Margin = new Padding(0);
      btnLaunchRecon.Name = "btnLaunchRecon";
      btnLaunchRecon.Padding = new Padding(5);
      btnLaunchRecon.Size = new Size(125, 32);
      btnLaunchRecon.TabIndex = 1;
      btnLaunchRecon.Text = "Launch ReCON";
      btnLaunchRecon.TextAlign = ContentAlignment.MiddleRight;
      btnLaunchRecon.UseVisualStyleBackColor = true;
      ToolTip launchReconTooltip = new ToolTip();
      launchReconTooltip.SetToolTip(btnLaunchRecon, "Launch ReCON Client and monitor an Arma Server");
      // 
      // Dashboard
      // 
      ClientSize = new Size(834, 511);
      Controls.Add(lblEmptyState);
      Controls.Add(flowPanelCards);
      Controls.Add(panelBottom);
      Icon = (System.Drawing.Icon) resources.GetObject("$this.Icon");
      Name = "Dashboard";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "Longbow - Arma Dedicated Server Tool";
      panelBottom.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion
    private FlowLayoutPanel flowPanelCards;
    private IconButton btnNewServer;
    private Label lblEmptyState;
    private Panel panelBottom;
    private IconButton btnLaunchRecon;
  }
}
