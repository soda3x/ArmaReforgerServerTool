/******************************************************************************
 * File Name:    RenameForm.cs
 * Project:      Longbow
 * Description:  This file contains the RenameForm class which is a dialog
 *               that provides the ability to rename text in a listbox
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Longbow.Managers;
using WinForms.Fluent;

namespace Longbow.Forms
{
  public partial class RenameForm : Form
  {
    string m_resultingRename;
    public RenameForm(string text)
    {
      InitializeComponent();
      ThemeManager.GetInstance().ConfigureTheme(this);
      renameTB.Text = text;
    }

    private void CancelBtnPressed(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
    }

    private void RenameBtnPressed(object sender, EventArgs e)
    {
      m_resultingRename = renameTB.Text;
      this.DialogResult = DialogResult.OK;
    }

    public string GetResultingRename()
    {
      return m_resultingRename;
    }
  }
}
