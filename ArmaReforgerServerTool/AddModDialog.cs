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
            this.Text = "Arma Reforger Dedicated Server Tool - Add Mod";
            m_reforgerApp = reforgerApp;
        }

        /// <summary>
        /// Handler for the Close Button.
        /// Closes the Add Mod Dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtnPressed(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handler for the Add Mod Button.
        /// Adds the mod to the Available Mods ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtnPressed(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(modId.Text) && !String.IsNullOrWhiteSpace(modName.Text))
            {
                Mod mod;

                if (!String.IsNullOrWhiteSpace(modVers.Text))
                {
                    mod = new(modId.Text, modName.Text, modVers.Text);
                }
                else
                {
                    mod = new(modId.Text, modName.Text);
                }

                m_reforgerApp.GetAvailableModsList().Items.Add(mod);
                ReforgerServerApp.AlphabetiseModListBox(m_reforgerApp.GetAvailableModsList());
                ReforgerServerApp.AlphabetiseModListBox(m_reforgerApp.GetEnabledModsList());
                m_reforgerApp.WriteModsDatabase();
                Close();
            }
        }
    }
}
