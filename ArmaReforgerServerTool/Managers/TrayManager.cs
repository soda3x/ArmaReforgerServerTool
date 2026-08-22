using Longbow.Forms;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

namespace Longbow.Managers
{
  public class TrayManager : ApplicationContext
  {
    private NotifyIcon m_trayIcon;
    private ContextMenuStrip m_trayMenu;

    public TrayManager()
    {
      // Setup context menu
      m_trayMenu = new ContextMenuStrip();

      ToolStripMenuItem header = new ToolStripMenuItem("Longbow - Arma Dedicated Server Tool");
      header.Enabled = false;
      header.Font = new Font(m_trayMenu.Font, FontStyle.Bold);

      m_trayMenu.Items.Add(header);
      m_trayMenu.Items.Add(new ToolStripSeparator());
      m_trayMenu.Items.Add("Open Dashboard", null, (s, e) => ShowDashboard());
      m_trayMenu.Items.Add("Launch ReCON", null, (s, e) => LaunchRecon());
      m_trayMenu.Items.Add(new ToolStripSeparator());
      m_trayMenu.Items.Add("Exit Longbow (stops all running servers)", null, (s, e) => ExitApp());

      // Setup tray icon
      m_trayIcon = new NotifyIcon()
      {
        Icon = SystemIcons.Application, // TODO: Replace with app icon
        ContextMenuStrip = m_trayMenu,
        Visible = true,
        Text = "Longbow - Arma Dedicated Server Tool"
      };

      m_trayIcon.DoubleClick += (s, e) => ShowDashboard();

      ShowDashboard();
    }

    private void ShowDashboard()
    {
      Dashboard.GetInstance().Show();
      Dashboard.GetInstance().BringToFront();
    }

    private void LaunchRecon()
    {
      // TODO:
    }

    private void ExitApp()
    {
      m_trayIcon.Visible = false;
      Application.Exit();
    }
  }
}
