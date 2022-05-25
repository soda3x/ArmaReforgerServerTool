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
        private readonly Main m_mainReference;
        public AddModDialog(Main mainReference)
        {
            InitializeComponent();
            m_mainReference = mainReference;
        }

        private void CancelBtnPressed(object sender, EventArgs e)
        {
            Close();
        }

        private void AddBtnPressed(object sender, EventArgs e)
        {
            Mod mod = new(modId.Text, modName.Text, modVersion.Text);
            m_mainReference.GetAvailableModsList().Items.Add(mod);
            m_mainReference.WriteModsDatabase();
            Close();
        }
    }
}
