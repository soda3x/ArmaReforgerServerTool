using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReforgerServerApp
{
    internal class ServerConfiguration
    {
        private string DedicatedServerId { get; set; }
        private string Region { get; set; }
        private string GameHostBindAddress { get; set; }
        private int GameHostBindPort { get; set; }
        private string GameHostRegisterBindAddress { get; set; }
        private int GameHostRegisterBindPort { get; set; }
        private string AdminPassword { get; set; }
        private string ServerName { get; set; }
        private string ServerPassword { get; set; }
        private string ScenarioId { get; set; }
        private int PlayerCountLimit { get; set; }
        private bool AutoJoinable { get; set; }
        private bool Visible { get; set; }
        private int ServerMaxViewDistance { get; set; }
        private int ServerMinGrassDistance { get; set; }
        private int NetworkViewDistance { get; set; }
        private bool DisableThirdPerson { get; set; }
        private bool FastValidation { get; set; }
        private bool BattlEye { get; set; }
        private bool A2sQueryEnabled { get; set; }
        private int SteamQueryPort { get; set; }
        private List<Mod> Mods { get; }

        private ServerConfiguration()
        {
            DedicatedServerId = string.Empty;
            Region = string.Empty;
            GameHostBindAddress = string.Empty;
            GameHostRegisterBindAddress = string.Empty;
            AdminPassword = string.Empty;
            ServerName = string.Empty;
            ServerPassword = string.Empty;
            ScenarioId = string.Empty;
            Mods = new List<Mod>();
        }

        public string AsJsonString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("\"dedicatedServerId\": " + "\"" + DedicatedServerId + "\",");
            sb.AppendLine("\"region\": " + "\"" + Region + "\",");
            sb.AppendLine("\"gameHostBindAddress\": " + "\"" + GameHostBindAddress + "\",");
            sb.AppendLine("\"gameHostBindPort\": " + GameHostBindPort + ",");
            sb.AppendLine("\"gameHostRegisterBindAddress\": " + "\"" + GameHostRegisterBindAddress + "\",");
            sb.AppendLine("\"gameHostRegisterBindPort\": " + GameHostRegisterBindPort + ",");
            sb.AppendLine("\"adminPassword\": " + "\"" + AdminPassword + "\",");
            sb.AppendLine("\"game\": {");
            sb.AppendLine("\"name\": " + "\"" + ServerName + "\",");
            sb.AppendLine("\"password\":" + "\"" + ServerPassword + "\",");
            sb.AppendLine("\"scenarioId\": " + "\"" + ScenarioId + "\",");
            sb.AppendLine("\"playerCountLimit\": " + PlayerCountLimit + ",");
            sb.AppendLine("\"autoJoinable\": " + AutoJoinable.ToString().ToLowerInvariant() + ",");
            sb.AppendLine("\"visible\": " + Visible.ToString().ToLowerInvariant() + ",");
            sb.AppendLine("\"gameProperties\": {");
            sb.AppendLine("\"serverMaxViewDistance\": " + +ServerMaxViewDistance + ",");
            sb.AppendLine("\"serverMinGrassDistance\": " + ServerMinGrassDistance + ",");
            sb.AppendLine("\"networkViewDistance\": " + NetworkViewDistance + ",");
            sb.AppendLine("\"disableThirdPerson\": " + DisableThirdPerson.ToString().ToLowerInvariant() + ",");
            sb.AppendLine("\"fastValidation\": " + FastValidation.ToString().ToLowerInvariant() + ",");
            sb.AppendLine("\"battlEye\": " + BattlEye.ToString().ToLowerInvariant());
            sb.AppendLine("},");
            sb.AppendLine("\"mods\": [");
            for (int i = 0; i < Mods.Count; i++)
            {
                sb.AppendLine("{");
                sb.AppendLine("\"modId\": \"" + Mods[i].GetModID() + "\",");
                sb.AppendLine("\"name\": \"" + Mods[i].GetModName() + "\",");
                sb.AppendLine("\"version\": \"" + Mods[i].GetModVersion() + "\"");
                if (i == Mods.Count - 1)
                {
                    sb.AppendLine("}");
                }
                else
                {
                    sb.AppendLine("},");
                }
            }
            sb.AppendLine("]");
            sb.AppendLine("},");
            sb.AppendLine("\"a2sQueryEnabled\": " + A2sQueryEnabled.ToString().ToLowerInvariant() + ",");
            sb.AppendLine("\"steamQueryPort\": " + SteamQueryPort);
            sb.AppendLine("}");

            return sb.ToString();
        }

        public class ServerConfigurationBuilder
        {
            private ServerConfiguration m_serverConfiguration;

            private void InitialiseServerConfigIfNull()
            {
                if (m_serverConfiguration == null)
                {
                    m_serverConfiguration = new ServerConfiguration();
                }
            }
            public ServerConfigurationBuilder WithDedicatedServerId(string dedicatedServerId)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.DedicatedServerId = dedicatedServerId;
                return this;
            }

            public ServerConfigurationBuilder WithRegion(string region)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.Region = region;
                return this;
            }

            public ServerConfigurationBuilder WithGameHostBindAddress(string gameHostBindAddress)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.GameHostBindAddress = gameHostBindAddress;
                return this;
            }

            public ServerConfigurationBuilder WithGameHostRegisterBindAddress(string gameHostRegisterBindAddress)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.GameHostRegisterBindAddress = gameHostRegisterBindAddress;
                return this;
            }

            public ServerConfigurationBuilder WithAdminPassword(string adminPassword)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.AdminPassword = adminPassword;
                return this;
            }

            public ServerConfigurationBuilder WithServerName(string serverName)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.ServerName = serverName;
                return this;
            }

            public ServerConfigurationBuilder WithServerPassword(string serverPassword)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.ServerPassword = serverPassword;
                return this;
            }

            public ServerConfigurationBuilder WithScenarioId(string scenarioId)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.ScenarioId = scenarioId;
                return this;
            }

            public ServerConfigurationBuilder WithGameHostBindPort(int gameHostBindPort)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.GameHostBindPort = gameHostBindPort;
                return this;
            }

            public ServerConfigurationBuilder WithGameHostRegisterBindPort(int gameHostRegisterBindPort)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.GameHostRegisterBindPort = gameHostRegisterBindPort;
                return this;
            }

            public ServerConfigurationBuilder WithPlayerCountLimit(int playerCountLimit)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PlayerCountLimit = playerCountLimit;
                return this;
            }

            public ServerConfigurationBuilder WithAutoJoinable(bool autoJoinable)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.AutoJoinable = autoJoinable;
                return this;
            }

            public ServerConfigurationBuilder WithVisible(bool visible)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.Visible = visible;
                return this;
            }

            public ServerConfigurationBuilder WithServerMaxViewDistance(int serverMaxViewDistance)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.ServerMaxViewDistance = serverMaxViewDistance;
                return this;
            }

            public ServerConfigurationBuilder WithServerMinGrassDistance(int serverMinGrassDistance)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.ServerMinGrassDistance = serverMinGrassDistance;
                return this;
            }

            public ServerConfigurationBuilder WithNetworkViewDistance(int networkViewDistance)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.NetworkViewDistance = networkViewDistance;
                return this;
            }

            public ServerConfigurationBuilder WithDisableThirdPerson(bool disableThirdPerson)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.DisableThirdPerson = disableThirdPerson;
                return this;
            }

            public ServerConfigurationBuilder WithFastValidation(bool fastValidation)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.FastValidation = fastValidation;
                return this;
            }

            public ServerConfigurationBuilder WithBattlEye(bool battlEye)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.BattlEye = battlEye;
                return this;
            }

            public ServerConfigurationBuilder WithA2SQueryEnabled(bool a2sQueryEnabled)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.A2sQueryEnabled = a2sQueryEnabled;
                return this;
            }

            public ServerConfigurationBuilder WithSteamQueryPort(int steamQueryPort)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.SteamQueryPort = steamQueryPort;
                return this;
            }

            public void AddModToConfiguration(Mod mod)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.Mods.Add(mod);
            }

            public ServerConfiguration Build()
            {
                return m_serverConfiguration;
            }
        }
    }
}
