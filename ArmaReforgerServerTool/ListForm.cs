using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReforgerServerApp
{
    public partial class ListForm : Form
    {
        private ServerConfiguration serverConfig;
        public ListForm(ServerConfiguration sc, string windowTitle)
        {
            InitializeComponent();
            this.Text = $"Arma Reforger Dedicated Server Tool - {windowTitle}";
            serverConfig = sc;
            if (serverConfig.Admins.Contains(","))
            {
                foreach (string admin in serverConfig.Admins.Split(","))
                {
                    adminListView.Items.Add(new ListViewItem(admin));
                }
            }
        }

        private void CloseBtnClicked(object sender, EventArgs e)
        {
            if (adminListView.Items.Count > 0)
            {
                foreach (ListViewItem lvi in adminListView.Items)
                {
                    String.Join(",", serverConfig.Admins, lvi);
                }
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
    }
}
