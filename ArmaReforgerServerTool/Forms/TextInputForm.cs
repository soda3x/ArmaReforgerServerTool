namespace ReforgerServerApp
{
    public partial class TextInputForm : Form
    {

        public TextInputForm(string windowTitle)
        {
            InitializeComponent();
            this.Text = $"Arma Reforger Dedicated Server Tool - {windowTitle}";
            textInputField.Text = ConfigurationManager.GetInstance().GetServerConfiguration().MissionHeaderAsJsonString();
        }

        private void OkBtnClicked(object sender, EventArgs e)
        {
            ConfigurationManager.GetInstance().GetServerConfiguration().SetMissionHeaderFromJson(textInputField.Text);
            Close();
        }
    }
}
