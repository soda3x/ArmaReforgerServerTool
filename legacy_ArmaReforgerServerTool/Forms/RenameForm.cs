/******************************************************************************
 * File Name:    RenameForm.cs
 * Project:      Longbow
 * Description:  This file contains the RenameForm class which is a dialog
 *               that provides the ability to rename text in a listbox
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace Longbow.Forms
{
  public partial class RenameForm : Form
  {
    string m_resultingRename;
    public RenameForm(string text)
    {
      InitializeComponent();
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
