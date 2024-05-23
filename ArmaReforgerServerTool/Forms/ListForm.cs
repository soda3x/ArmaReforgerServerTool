/******************************************************************************
 * File Name:    ListForm.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This is a generic form for managing lists graphically
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
    public partial class ListForm : Form
    {
        public ListForm(string windowTitle)
        {
            InitializeComponent();
            this.Text = $"Arma Reforger Dedicated Server Tool - {windowTitle}";
            string[] admins = ConfigurationManager.GetInstance().GetServerConfiguration().root.game.admins;
            if (admins.Length > 0)
            {
                foreach (string admin in admins)
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
            string[] newAdmins = new string[adminListView.Items.Count];

            for (int i = 0; i < adminListView.Items.Count; i++)
            {
                newAdmins[i] = adminListView.Items[i].Text;
            }

            ConfigurationManager.GetInstance().GetServerConfiguration().root.game.admins = newAdmins;
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
