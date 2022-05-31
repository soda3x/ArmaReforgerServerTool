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
    public partial class AddModDialog : Form
    {
        private readonly ReforgerServerApp m_reforgerApp;
        public AddModDialog(ReforgerServerApp reforgerApp)
        {
            InitializeComponent();
            m_reforgerApp = reforgerApp;
        }

        private void CancelBtnPressed(object sender, EventArgs e)
        {
            Close();
        }

        private void AddBtnPressed(object sender, EventArgs e)
        {
            Mod mod = new(modId.Text, modName.Text);
            m_reforgerApp.GetAvailableModsList().Items.Add(mod);
            ReforgerServerApp.AlphabetiseModListBox(m_reforgerApp.GetAvailableModsList());
            ReforgerServerApp.AlphabetiseModListBox(m_reforgerApp.GetEnabledModsList());
            m_reforgerApp.WriteModsDatabase();
            Close();
        }
    }
}
