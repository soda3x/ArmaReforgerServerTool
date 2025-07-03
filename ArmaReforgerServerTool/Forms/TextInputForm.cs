/******************************************************************************
 * File Name:    TextInputForm.cs
 * Project:      Longbow
 * Description:  This is a generic form for entering lots of text, either
 *               formatted or not
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  public partial class TextInputForm : Form
  {

    public TextInputForm(string windowTitle)
    {
      InitializeComponent();
      this.Text = $"Longbow - {windowTitle}";
      textInputField.Text = ConfigurationManager.GetInstance().GetServerConfiguration().MissionHeaderAsJsonString();
    }

    private void OkBtnClicked(object sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().GetServerConfiguration().SetMissionHeaderFromJson(textInputField.Text);
      Close();
    }
  }
}
