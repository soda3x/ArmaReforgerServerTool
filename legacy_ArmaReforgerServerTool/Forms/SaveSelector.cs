/******************************************************************************
 * File Name:    SaveSelector.cs
 * Project:      Longbow
 * Description:  This is the Save Selector Form
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp;
using ReforgerServerApp.Managers;
using ReforgerServerApp.Utils;

namespace Longbow.Forms
{
  public partial class SaveSelector : Form
  {
    Dictionary<string, string> m_savedGames;
    public SaveSelector()
    {
      InitializeComponent();
      m_savedGames = new();
      RefreshSavedGamesList();
      UpdateSaveLabel();
    }

    /// <summary>
    /// Utility method for reloading the saved games list
    /// </summary>
    private void RefreshSavedGamesList()
    {
      m_savedGames = FileIOManager.GetInstance().GetSavedGames();
      savedGamesList.Items.Clear();
      foreach (string save in m_savedGames.Keys)
      {
        savedGamesList.Items.Add(save);
      }
    }

    /// <summary>
    /// Toggle Button Enabled states based on logic required
    /// </summary>
    /// <param name="selectedItem"></param>
    private void ToggleButtonStates(object? selectedItem)
    {
      renameSaveBtn.Enabled = selectedItem != null;
      loadSelectedSaveBtn.Enabled = selectedItem != null;
      renameSaveBtn.Enabled = selectedItem != null;
      deleteSaveBtn.Enabled = selectedItem != null && !selectedItem.Equals(string.Empty);
      clearSaveBtn.Enabled = ConfigurationManager.GetInstance().usingSave;

      if (selectedItem != null && selectedItem is string saveStr)
      {
        keepPermanentlyBtn.Enabled = saveStr.Contains(".auto");
      }
    }

    /// <summary>
    /// Update the label stating which save is loaded
    /// </summary>
    private void UpdateSaveLabel()
    {
      if (ConfigurationManager.GetInstance().usingSave)
      {
        if (ConfigurationManager.GetInstance().save.Equals(".LatestSave"))
        {
          loadedSaveLabel.Text = $"Using latest save.";
          return;
        }
        loadedSaveLabel.Text = $"Loaded save: {ConfigurationManager.GetInstance().save}";
        return;
      }
      loadedSaveLabel.Text = "No save loaded. Your server will not use a save.";
    }

    private void SavedGamesListSelectedIndexChanged(object sender, EventArgs e)
    {
      ToggleButtonStates(savedGamesList.SelectedItem);
    }

    private void KeepBtnPressed(object sender, EventArgs e)
    {
      string origNameKey = (string) savedGamesList.SelectedItem!;
      string origName = m_savedGames[origNameKey];
      string newName = origName.Replace(".auto", "");
      FileIOManager.GetInstance().RenameFile(origName, newName);
      ToggleButtonStates(savedGamesList.SelectedItem);
      RefreshSavedGamesList();
    }

    private void DeleteBtnPressed(object sender, EventArgs e)
    {
      string selectedSave = (string) savedGamesList.SelectedItem!;
      bool confirm = Utilities.DisplayConfirmationMessage($"You are about to delete your saved game '{selectedSave}', are you sure you would like to do this?", true);
      if (confirm)
      {
        FileIOManager.DeleteFile(m_savedGames[selectedSave]);
        ToggleButtonStates(savedGamesList.SelectedItem);
        RefreshSavedGamesList();
      }
    }

    private void RenameBtnPressed(object sender, EventArgs e)
    {
      string origName = (string) savedGamesList.SelectedItem!;
      RenameForm rf = new(origName);
      DialogResult result = rf.ShowDialog();
      if (result == DialogResult.OK)
      {
        string origPath = m_savedGames[origName];
        string newPath = $"{FileIOManager.GetInstance().GetSavesPath()}\\{rf.GetResultingRename()}.json";
        FileIOManager.GetInstance().RenameFile(origPath, newPath);
      }
      RefreshSavedGamesList();
    }

    private void LoadBtnPressed(object sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().save = (string) savedGamesList.SelectedItem!;
      ConfigurationManager.GetInstance().usingSave = true;
      UpdateSaveLabel();
      ToggleButtonStates(savedGamesList.SelectedItem);
    }

    private void LatestBtnPressed(object sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().save = ".LatestSave";
      ConfigurationManager.GetInstance().usingSave = true;
      UpdateSaveLabel();
      ToggleButtonStates(savedGamesList.SelectedItem);
    }

    private void ClearSaveBtnPressed(object sender, EventArgs e)
    {
      ConfigurationManager.GetInstance().usingSave = false;
      UpdateSaveLabel();
      ToggleButtonStates(savedGamesList.SelectedItem);
    }
  }
}
