using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReforgerServerApp
{
    public partial class AdminManager : Form
    {
        private ServerConfiguration serverConfig;
        public AdminManager(ServerConfiguration sc, string windowTitle)
        {
            InitializeComponent();
            this.Text = $"Arma Reforger Dedicated Server Tool - {windowTitle}";
            serverConfig = sc;
            string[] splitAdmins = serverConfig.Admins.Split(',');
            if (splitAdmins.Length > 0)
            {
                foreach (string admin in splitAdmins)
                {
                    if (admin.Trim() != string.Empty)
                    {
                        adminListView.Items.Add(new ListViewItem(admin));
                    }
                }
            }
        }

        private void CloseBtnClicked(object sender, EventArgs e)
        {
            if (adminListView.Items.Count == 0)
            {
                serverConfig.Admins = string.Empty;
            }
            string newAdmins = string.Empty;
            foreach (ListViewItem lvi in adminListView.Items)
            {
                if (lvi.Text.Trim() != string.Empty)
                {
                    newAdmins = String.Join(',', newAdmins, lvi.Text.Trim());
                }
            }
            if (newAdmins != string.Empty)
            {
                serverConfig.Admins = newAdmins[1..];
            }
            Close();
        }

        private void AddBtnClicked(object sender, EventArgs e)
        {
            if (adminTB.Text != string.Empty)
            {
                adminListView.Items.Add(new ListViewItem(adminTB.Text));
                adminTB.Text = string.Empty;
            }
        }

        private void RemoveBtnClicked(object sender, EventArgs e)
        {
            if (adminListView.SelectedItems.Count > 0 && adminListView.SelectedItems[0] != null)
            {
                adminListView.Items.RemoveAt(adminListView.SelectedItems[0].Index);
            }
        }

        private void linkLblSteamIdIo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = "https://steamid.io/",
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }

        private void linkLblSteamIdFinder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = "https://www.steamidfinder.com/",
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }
    }
}
