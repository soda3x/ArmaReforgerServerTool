using Longbow.Components.ui;
using Longbow.Managers;
using Longbow.Models;
using Longbow.Utils;
using ReforgerServerApp;
using ReforgerServerApp.Utils;

namespace Longbow.Forms
{
  public partial class CreateBanDialog : Form
  {
    private ReconClient m_recon;
    private FluentTextBox m_reconLog;
    private RconPlayer m_playerToBan;

    public CreateBanDialog(ReconClient recon, FluentTextBox reconLog, RconPlayer playerToBan)
    {
      InitializeComponent();
      ThemeManager.GetInstance().ConfigureTheme(this);
      LocalisationManager.GetInstance().ApplyStrings(this, translationKeyProvider1);
      m_recon = recon;
      m_reconLog = reconLog;
      m_playerToBan = playerToBan;
      creatingBanForLabel.Text = Strings.Get(Constants.CREATE_BAN_CREATING_FOR_MESSAGE_STR, ("playerName", playerToBan.PlayerName));
      banDurationSecondsTB.PlaceholderText = Strings.Get(Constants.CREATE_BAN_DURATION_PLACEHOLDER_STR);
      banReasonTB.PlaceholderText = Strings.Get(Constants.CREATE_BAN_REASON_PLACEHOLDER_STR);
    }

    private void OnBanBtnPressed(object sender, EventArgs e)
    {
      Task.Run(async () =>
      {
        m_reconLog.AppendText($"{Utilities.GetTimestamp()} Banning {m_playerToBan.PlayerName} from the server for {banDurationSecondsTB.Text} seconds.{Environment.NewLine}");
        string response = await m_recon.SendCommandAsync($"ban create {m_playerToBan.PlayerId} {banDurationSecondsTB.Text} {banReasonTB.Text}");
      });
      Dispose();
    }

    private void OnBan1HrBtnPressed(object sender, EventArgs e)
    {
      Task.Run(async () =>
      {
        m_reconLog.AppendText($"{Utilities.GetTimestamp()} Banning {m_playerToBan.PlayerName} from the server 1 hour.{Environment.NewLine}");
        string response = await m_recon.SendCommandAsync($"ban create {m_playerToBan.PlayerId} 3600 {banReasonTB.Text}");
      });
      Dispose();
    }

    private void OnBan24HrBtnPressed(object sender, EventArgs e)
    {
      Task.Run(async () =>
      {
        m_reconLog.AppendText($"{Utilities.GetTimestamp()} Banning {m_playerToBan.PlayerName} from the server for 24 hours.{Environment.NewLine}");
        string response = await m_recon.SendCommandAsync($"ban create {m_playerToBan.PlayerId} 86400 {banReasonTB.Text}");
      });
      Dispose();
    }

    private void OnBanForeverBtnPressed(object sender, EventArgs e)
    {
      Task.Run(async () =>
      {
        m_reconLog.AppendText($"{Utilities.GetTimestamp()} Banning {m_playerToBan.PlayerName} from the server forever. To unban them you will need to do so via the Ban Manager.{Environment.NewLine}");
        string response = await m_recon.SendCommandAsync($"ban create {m_playerToBan.PlayerId} 0 {banReasonTB.Text}");
      });
      Dispose();
    }
  }
}
