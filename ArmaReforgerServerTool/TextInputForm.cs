using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ReforgerServerApp
{
    public partial class TextInputForm : Form
    {
        private ServerConfiguration serverConfig;

        public TextInputForm(ServerConfiguration sc, string windowTitle)
        {
            InitializeComponent();
            this.Text = $"Arma Reforger Dedicated Server Tool - {windowTitle}";
            textInputField.Text = sc.ConvertMissionHeaderLineEndingsToJson();
            serverConfig = sc;
        }

        private void OkBtnClicked(object sender, EventArgs e)
        {
            serverConfig.MissionHeader = textInputField.Text;
            Close();
        }
    }
}
