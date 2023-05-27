using System.Text;

namespace ReforgerToolConfigMigrator
{
    public partial class Main : Form
    {
        private string? loadedFilePath;
        public Main()
        {
            InitializeComponent();
            convertBtn.Enabled = false;
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new();
            ofd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            ofd.Filter = "Text files (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                helperLbl.Text = $"Loaded config file '{ofd.FileName}'";
                loadedFilePath = ofd.FileName;
                convertBtn.Enabled = true;
            }
        }

        private void convertBtn_Click(object sender, EventArgs e)
        {
            string[] fileContents = File.ReadAllLines(loadedFilePath);
            StringBuilder sb = new();

            List<string[]> mods = new List<string[]>();

            bool isXPlat = false;

            foreach (string line in fileContents)
            {
                string[] lineContents = line.Split(',');
                switch (lineContents[0])
                {
                    case "gameHostBindAddress":
                        sb.AppendLine($"bindAddress={lineContents[1]}");
                        break;
                    case "gameHostBindPort":
                        sb.AppendLine($"bindPort={lineContents[1]}");
                        break;
                    case "gameHostRegisterBindAddress":
                        sb.AppendLine($"publicAddress={lineContents[1]}");
                        break;
                    case "gameHostRegisterBindPort":
                        sb.AppendLine($"publicPort={lineContents[1]}");
                        break;
                    case "adminPassword":
                        sb.AppendLine($"passwordAdmin={lineContents[1]}");
                        break;
                    case "name":
                        sb.AppendLine($"name={lineContents[1]}");
                        break;
                    case "password":
                        sb.AppendLine($"password={lineContents[1]}");
                        break;
                    case "scenarioId":
                        sb.AppendLine($"scenarioId={lineContents[1]}");
                        break;
                    case "playerCountLimit":
                        sb.AppendLine($"maxPlayers={lineContents[1]}");
                        break;
                    case "autoJoinable":
                        sb.AppendLine($"autoJoinable={lineContents[1]}");
                        break;
                    case "visible":
                        sb.AppendLine($"visible={lineContents[1]}");
                        break;
                    case "platformXBL":
                        sb.AppendLine("crossPlatform=true");
                        isXPlat = true;
                        break;
                    case "serverMaxViewDistance":
                        sb.AppendLine($"serverMaxViewDistance={lineContents[1]}");
                        break;
                    case "serverMinGrassDistance":
                        sb.AppendLine($"serverMinGrassDistance={lineContents[1]}");
                        break;
                    case "networkViewDistance":
                        sb.AppendLine($"networkViewDistance={lineContents[1]}");
                        break;
                    case "gameNumber":
                        sb.AppendLine($"gameNumber={lineContents[1]}");
                        break;
                    case "disableThirdPerson":
                        sb.AppendLine($"disableThirdPerson={lineContents[1]}");
                        break;
                    case "fastValidation":
                        sb.AppendLine($"fastValidation={lineContents[1]}");
                        break;
                    case "battlEye":
                        sb.AppendLine($"battlEye={lineContents[1]}");
                        break;
                    case "a2sQueryEnabled":
                        sb.AppendLine($"a2sQueryEnabled={lineContents[1]}");
                        break;
                    case "steamQueryPort":
                        sb.AppendLine($"steamQueryPort={lineContents[1]}");
                        break;
                    case "modId":
                        mods.Add(lineContents);
                        break;
                    default:
                        break;
                }
            }

            // If no xbox platform in config, we must not be x-play
            if (!isXPlat)
            {
                sb.AppendLine("crossPlatform=false");
            }

            // Set defaults for new parameters
            sb.AppendLine($"lobbyPlayerSynchronise=true");
            sb.AppendLine("vonDisableUI=false");
            sb.AppendLine("vonDisableDirectSpeechUI=false");
            sb.AppendLine("playerSaveTime=120");
            sb.AppendLine("aiLimit=-1");

            // Now process the mods
            StringBuilder modCollection = new();
            foreach (string[] mod in mods)
            {
                modCollection.AppendLine($"{mod[0]},{mod[1]},{mod[2]},{mod[3]}");
            }

            using SaveFileDialog sfd = new();
            sfd.InitialDirectory = Environment.SpecialFolder.UserProfile.ToString();
            sfd.Filter = "Properties files (*.prop)|*.prop";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string modsFilename = $"{sfd.FileName}_mods.txt";
                sb.AppendLine($"modCollection={modsFilename}");
                File.WriteAllText(sfd.FileName, sb.ToString().Trim());
                File.WriteAllText(modsFilename, modCollection.ToString().Trim());
            }
            helperLbl.Text = $"Finished migrating {loadedFilePath}.";
            loadedFilePath = null;
            convertBtn.Enabled = false;
        }
    }
}