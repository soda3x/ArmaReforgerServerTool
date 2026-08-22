using Longbow.Components;
using Longbow.Managers;
using ReforgerServerApp;

namespace Longbow.Forms
{
  public partial class Dashboard : Form
  {
    private static Dashboard? m_instance;

    private int m_newServerCount = 0;

    private Dashboard()
    {
      InitializeComponent();
      UpdateEmptyState();
      CenterEmptyLabel();
      this.Resize += (s, e) => CenterEmptyLabel();
      ThemeManager.GetInstance().ConfigureTheme(this);
    }

    public static Dashboard GetInstance()
    {
      m_instance ??= new Dashboard();

      if (m_instance.IsDisposed)
      {
        m_instance = new Dashboard();
      }

      return m_instance;
    }

    public void AddServerCard(string serverName, Main serverForm)
    {
      var card = new ServerCard(serverName, serverForm, this);

      flowPanelCards.Controls.Add(card);
    }

    public void RemoveServerCard(ServerCard sc)
    {
      flowPanelCards.Controls.Remove(sc);
      UpdateEmptyState();
    }

    private void OnNewServerPressed(object sender, EventArgs e)
    {
      string serverTitle = $"My Longbow Server {++m_newServerCount}";
      Main server = new Main(serverTitle);
      ServerCard card = new ServerCard(serverTitle, server, this);
      server.SetServerCard(card);
      flowPanelCards.Controls.Add(card);

      UpdateEmptyState();

      // Need to make sure the Card applies our themes when created
      ThemeManager.GetInstance().ConfigureTheme(this);
    }

    private void CenterEmptyLabel()
    {
      lblEmptyState.Location = new Point(
          (this.ClientSize.Width - lblEmptyState.Width) / 2,
          (this.ClientSize.Height - lblEmptyState.Height - 60) / 2 // Offset for bottom panel
      );
    }

    private void UpdateEmptyState()
    {
      // Toggle visibility based on whether any server cards exist
      bool hasServers = flowPanelCards.Controls.Count > 0;
      lblEmptyState.Visible = !hasServers;
    }

    // Prevent the dashboard from completely destroying itself on "X" close, just hide it to tray instead
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        this.Hide();
      }
      else
      {
        base.OnFormClosing(e);
      }
    }
  }
}
