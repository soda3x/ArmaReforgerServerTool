/******************************************************************************
 * File Name:    AboutBox.cs
 * Project:      Longbow
 * Description:  This is the AboutBox Form
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using System.Reflection;
using System.Text;

namespace ReforgerServerApp
{
  public partial class AboutBox : Form
  {
    public AboutBox()
    {
      InitializeComponent();
      StringBuilder sb = new();
      sb.AppendLine("Longbow - Arma Reforger Dedicated Server Tool by soda3x");
      sb.AppendLine($"Version {Assembly.GetExecutingAssembly().GetName().Version}");
      sb.AppendLine("\r\n\"No Backend Scenario Loader\" mod (v1.0.1) provided by ceo_of_bacon");
      sb.AppendLine("See full list of contributors on GitHub");
      aboutText.Text = sb.ToString();
    }

    private void ReportBtnPressed(object sender, EventArgs e)
    {
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = @"https://github.com/soda3x/ArmaReforgerServerTool/issues/", UseShellExecute = true });
    }

    private void OKBtnPressed(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
