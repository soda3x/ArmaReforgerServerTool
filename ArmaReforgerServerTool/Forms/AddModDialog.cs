/******************************************************************************
 * File Name:    AddModDialog.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This is the Add Mod Dialog Form
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp.Managers;

namespace ReforgerServerApp
{
  public partial class AddModDialog : Form
  {
    private readonly Main m_reforgerApp;
    private readonly bool m_isEditMode = false;
    private readonly int m_idx = -1;
    public AddModDialog(Main reforgerApp)
    {
      InitializeComponent();
      this.Text = "Arma Reforger Dedicated Server Tool - Add Mod";
      m_reforgerApp = reforgerApp;
      CreateTooltips();
    }

    public AddModDialog(Main reforgerApp, Mod m, int idx)
    {
      InitializeComponent();
      this.Text = "Arma Reforger Dedicated Server Tool - Edit Mod";
      m_reforgerApp = reforgerApp;
      modId.Text = m.GetModID();
      modName.Text = m.GetModName();
      modVers.Text = m.GetModVersion().Equals("latest") ? "" : m.GetModVersion();
      required.Checked = m.IsModRequired();
      addBtn.Text = "Edit";
      m_isEditMode = true;
      m_idx = idx;
      CreateTooltips();
    }

    private void CreateTooltips()
    {
      ToolTip requiredTooltip = new();
      requiredTooltip.SetToolTip(requiredLabel, "Setting this will make it so that when this mod cannot be downloaded from the Workshop, the server will not start.");
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
      if (!string.IsNullOrWhiteSpace(modId.Text.Trim()) && !string.IsNullOrWhiteSpace(modName.Text.Trim()))
      {
        Mod mod;

        if (!string.IsNullOrWhiteSpace(modVers.Text.Trim()))
        {
          mod = new(modId.Text.Trim(), modName.Text.Trim(), modVers.Text.Trim(), required.Checked);
        }
        else
        {
          mod = new(modId.Text.Trim(), modName.Text.Trim(), required.Checked);
        }

        if (m_isEditMode)
        {
          ConfigurationManager.GetInstance().GetAvailableMods()[m_idx] = mod;
        }
        else
        {
          ConfigurationManager.GetInstance().GetAvailableMods().Add(mod);
        }

        FileIOManager.GetInstance().WriteModsDatabase();
        ConfigurationManager.GetInstance().AlphabetiseModLists();
        Close();
      }
    }
  }
}
