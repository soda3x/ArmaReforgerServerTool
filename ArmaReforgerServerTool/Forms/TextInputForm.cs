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

    public TextInputForm(string windowTitle, String prefilledText)
    {
      InitializeComponent();
      this.Text = $"Longbow - {windowTitle}";
      textInputField.Text = prefilledText;
    }

    private void OkBtnClicked(object sender, EventArgs e)
    {
      Close();
    }

    /// <summary>
    /// Get the text contents of the input form
    /// </summary>
    /// <returns></returns>
    public String GetText()
    {
      return textInputField.Text;
    }
  }
}
