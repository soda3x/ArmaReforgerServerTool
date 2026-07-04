using Longbow.Managers;
using Longbow.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Longbow.Forms
{
  public partial class BanManager : Form
  {
    private BindingList<RconBan> m_bans;
    private ReconClient m_reconClient;
    public BanManager(ReconClient reconClient)
    {
      InitializeComponent();

      m_bans = [];
      m_reconClient = reconClient;

      banList.AutoGenerateColumns = false;
      banList.Columns.Clear();

      DataGridViewTextBoxColumn uidCol = new();
      uidCol.DataPropertyName = "PlayerUid";
      uidCol.HeaderText = "ID";
      uidCol.Name = "colUid";
      uidCol.FillWeight = 150;
      banList.Columns.Add(uidCol);

      DataGridViewTextBoxColumn nameCol = new();
      nameCol.DataPropertyName = "PlayerName";
      nameCol.HeaderText = "Player Name";
      nameCol.Name = "colName";
      nameCol.FillWeight = 100;
      banList.Columns.Add(nameCol);

      banList.DataSource = m_bans;
      
      ThemeManager.GetInstance().ConfigureTheme(this);
    }

    private async Task FetchAllBansAsync()
    {
      if (m_reconClient == null || !m_reconClient.IsConnected)
      {
        return;
      }

      List<RconBan> allParsedBans = new List<RconBan>();
      int currentPage = 1;
      int totalPages = 1; // Default to 1, we will update this when we read the first line
      bool hasMorePages = true;

      while (hasMorePages)
      {
        string cmd = currentPage == 1 ? "ban list" : $"ban list {currentPage}";
        string response = await m_reconClient.SendCommandAsync(cmd);

        if (string.IsNullOrEmpty(response))
        {
          break;
        }

        string[] lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
          string trimmed = line.Trim();
          if (string.IsNullOrEmpty(trimmed))
            continue;

          // 1. Extract the total pages from the header line
          if (trimmed.StartsWith("Total bans:"))
          {
            try
            {
              // Looks for "Page: 1/3", splits by "Page: ", then by "/" to get the "3"
              string[] pageParts = trimmed.Split(new string[] { "Page:" }, StringSplitOptions.None);
              if (pageParts.Length > 1)
              {
                string[] numbers = pageParts[1].Trim().Split('/');
                if (numbers.Length == 2 && int.TryParse(numbers[1], out int parsedTotal))
                {
                  totalPages = parsedTotal;
                }
              }
            }
            catch { /* Ignore parse errors and assume 1 page */ }
            continue; // Skip to next line
          }

          // 2. Skip the column header text
          if (trimmed.Contains("Identity Id | Banned name"))
            continue;

          // 3. Process the actual ban data lines
          if (trimmed.StartsWith("-"))
          {
            try
            {
              allParsedBans.Add(new RconBan(trimmed));
            }
            catch (Exception ex)
            {
              Debug.WriteLine($"Failed to parse ban line: {ex.Message}");
            }
          }
        }

        // Check if we need to request another page
        if (currentPage >= totalPages)
        {
          hasMorePages = false;
        }
        else
        {
          currentPage++;
        }
      }

      this.Invoke(new Action(() =>
      {
        banList.DataSource = null; // Unbind temporarily to prevent layout lag
        m_bans.Clear();

        foreach (var ban in allParsedBans)
        {
          m_bans.Add(ban);
        }

        banList.DataSource = m_bans;
      }));
    }

    private async void BanManagerLoad(object sender, EventArgs e)
    {
      await FetchAllBansAsync();
    }
  }
}
