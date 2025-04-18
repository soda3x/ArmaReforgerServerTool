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
        private List<string> m_items;
        public ListForm(string windowTitle, string[] items)
        {
            InitializeComponent();
            this.Text = $"Arma Reforger Dedicated Server Tool - {windowTitle}";
            m_items = new List<string>(items);
            itemListView.DataSource = m_items;
        }

        private void CloseBtnClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void AddBtnClicked(object sender, EventArgs e)
        {
            if (itemTB.Text != string.Empty)
            {
                m_items.Add(itemTB.Text);
                itemTB.Text = string.Empty;
            }

            itemListView.RefreshItems();
        }

        private void RemoveBtnClicked(object sender, EventArgs e)
        {
            if (itemListView.SelectedItems.Count > 0 && itemListView.SelectedItems[0] != null)
            {
                m_items.Remove(itemListView.Text);
            }

            itemListView.RefreshItems();
        }

        public string[] GetItems() { return m_items.ToArray(); }
    }
}
