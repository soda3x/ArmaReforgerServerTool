using System.Text;

namespace ReforgerServerApp
{
    internal class ServerConfiguration
    {
        public string DedicatedServerId { get; set; }
        public string Region { get; set; }
        public string GameHostBindAddress { get; set; }
        public int GameHostBindPort { get; set; }
        public string GameHostRegisterBindAddress { get; set; }
        public int GameHostRegisterBindPort { get; set; }
        public string AdminPassword { get; set; }
        public string ServerName { get; set; }
        public string ServerPassword { get; set; }
        public string ScenarioId { get; set; }
        public int PlayerCountLimit { get; set; }
        public bool AutoJoinable { get; set; }
        public bool Visible { get; set; }
        public int ServerMaxViewDistance { get; set; }
        public int ServerMinGrassDistance { get; set; }
        public int NetworkViewDistance { get; set; }
        public int GameNumber { get; set; }
        public bool DisableThirdPerson { get; set; }
        public bool FastValidation { get; set; }
        public bool BattlEye { get; set; }
        public bool A2sQueryEnabled { get; set; }
        public int SteamQueryPort { get; set; }
        public bool PlatformPC { get; set; }
        public bool PlatformXBL { get; set; }
        public bool LobbyPlayerSynchronise { get; set; }
        public bool VONDisableUI { get; set; }
        public bool VONDisableDirectSpeechUI { get; set; }
        public List<Mod> Mods { get; }

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

        /// <summary>
        /// Display ServerConfiguration in the JSON format required by the Arma Server files.
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration</returns>
        public string AsJsonString()
        {
            StringBuilder sb = new();
            sb.AppendLine("{");
            sb.AppendLine($"\"dedicatedServerId\": \"{DedicatedServerId}\",");
            sb.AppendLine($"\"region\": \"{Region}\",");
            sb.AppendLine($"\"gameHostBindAddress\": \"{GameHostBindAddress}\",");
            sb.AppendLine($"\"gameHostBindPort\": {GameHostBindPort},");
            sb.AppendLine($"\"gameHostRegisterBindAddress\": \"{GameHostRegisterBindAddress}\",");
            sb.AppendLine($"\"gameHostRegisterBindPort\": {GameHostRegisterBindPort},");
            sb.AppendLine($"\"adminPassword\": \"{AdminPassword}\",");
            sb.AppendLine("\"game\": {");
            sb.AppendLine($"\"name\": \"{ServerName}\",");
            sb.AppendLine($"\"password\": \"{ServerPassword}\",");
            sb.AppendLine($"\"scenarioId\": \"{ScenarioId}\",");
            sb.AppendLine($"\"gameNumber\": {GameNumber},");
            sb.AppendLine($"\"playerCountLimit\": {PlayerCountLimit},");
            sb.AppendLine($"\"autoJoinable\": {AutoJoinable.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"visible\": {Visible.ToString().ToLowerInvariant()},");
            sb.AppendLine("\"supportedGameClientTypes\": [");

            if (PlatformPC)
            {
                // Append comma if we're also expecting to add XBL
                if (PlatformXBL)
                {
                    sb.AppendLine("\"PLATFORM_PC\",");
                }
                else
                {
                    sb.AppendLine("\"PLATFORM_PC\"");
                }
            }

            if (PlatformXBL)
            {
                sb.AppendLine("\"PLATFORM_XBL\"");
            }

            // If neither are in the configuration, we will default to PC
            if (!PlatformPC && !PlatformXBL)
            {
                sb.AppendLine("\"PLATFORM_PC\"");
            }

            sb.AppendLine("],");
            sb.AppendLine("\"gameProperties\": {");
            sb.AppendLine($"\"serverMaxViewDistance\": {ServerMaxViewDistance},");
            sb.AppendLine($"\"serverMinGrassDistance\": {ServerMinGrassDistance},");
            sb.AppendLine($"\"networkViewDistance\": {NetworkViewDistance},");
            sb.AppendLine($"\"disableThirdPerson\": {DisableThirdPerson.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"fastValidation\": {FastValidation.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"battlEye\": {BattlEye.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"VONDisableUI\": {VONDisableUI.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"VONDisableDirectSpeechUI\": {VONDisableDirectSpeechUI.ToString().ToLowerInvariant()},");
            
            sb.AppendLine("\"operating\": {");
            sb.AppendLine($"\"lobbyPlayerSynchronise\": {LobbyPlayerSynchronise.ToString().ToLowerInvariant()}");
            sb.AppendLine("}");

            if (Mods.Count > 0)
            {
                sb.AppendLine("},");

                sb.AppendLine("\"mods\": [");
                for (int i = 0; i < Mods.Count; i++)
                {
                    sb.AppendLine("{");
                    sb.AppendLine($"\"modId\": \"{Mods[i].GetModID()}\",");
                    sb.AppendLine($"\"name\": \"{Mods[i].GetModName()}\"");
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
            }
            else
            {
                sb.AppendLine("}");
            }

            sb.AppendLine("},");
            sb.AppendLine($"\"a2sQueryEnabled\": {A2sQueryEnabled.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"steamQueryPort\": {SteamQueryPort}");
            sb.AppendLine("}");

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Display ServerConfiguration as a comma separated string for saving to file.
        /// </summary>
        /// <returns>Comma-separated string representation of the Server Configuration</returns>
        public string AsCommaSeparatedString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"dedicatedServerId,{DedicatedServerId}");
            sb.AppendLine($"region,{Region}");
            sb.AppendLine($"gameHostBindAddress,{GameHostBindAddress}");
            sb.AppendLine($"gameHostBindPort,{GameHostBindPort}");
            sb.AppendLine($"gameHostRegisterBindAddress,{GameHostRegisterBindAddress}");
            sb.AppendLine($"gameHostRegisterBindPort,{GameHostRegisterBindPort}");
            sb.AppendLine($"adminPassword,{AdminPassword}");
            sb.AppendLine($"name,{ServerName}");
            sb.AppendLine($"password,{ServerPassword}");
            sb.AppendLine($"scenarioId,{ScenarioId}");
            sb.AppendLine($"playerCountLimit,{PlayerCountLimit}");
            sb.AppendLine($"autoJoinable,{AutoJoinable.ToString().ToLowerInvariant()}");
            sb.AppendLine($"visible,{Visible.ToString().ToLowerInvariant()}");
            sb.AppendLine($"platformPC,{PlatformPC.ToString().ToLowerInvariant()}");
            sb.AppendLine($"platformXBL,{PlatformXBL.ToString().ToLowerInvariant()}");
            sb.AppendLine($"serverMaxViewDistance,{ServerMaxViewDistance}");
            sb.AppendLine($"serverMinGrassDistance,{ServerMinGrassDistance}");
            sb.AppendLine($"networkViewDistance,{NetworkViewDistance}");
            sb.AppendLine($"gameNumber,{GameNumber}");
            sb.AppendLine($"disableThirdPerson,{DisableThirdPerson.ToString().ToLowerInvariant()}");
            sb.AppendLine($"fastValidation,{FastValidation.ToString().ToLowerInvariant()}");
            sb.AppendLine($"battlEye,{BattlEye.ToString().ToLowerInvariant()}");
            sb.AppendLine($"a2sQueryEnabled,{A2sQueryEnabled.ToString().ToLowerInvariant()}");
            sb.AppendLine($"steamQueryPort,{SteamQueryPort}");
            sb.AppendLine($"vonDisableUI,{VONDisableUI.ToString().ToLowerInvariant()}");
            sb.AppendLine($"vonDisableDirectSpeechUI,{VONDisableDirectSpeechUI.ToString().ToLowerInvariant()}");
            sb.AppendLine($"lobbyPlayerSynchronise,{LobbyPlayerSynchronise.ToString().ToLowerInvariant()}");
            foreach (Mod m in Mods)
            {
                sb.AppendLine($"modId,{m.GetModID()},modName,{m.GetModName()}");
            }
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

            public ServerConfigurationBuilder WithPlatformPC(bool platformPC)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PlatformPC = platformPC;
                return this;
            }

            public ServerConfigurationBuilder WithPlatformXBL(bool platformXBL)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PlatformXBL = platformXBL;
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

            public ServerConfigurationBuilder WithGameNumber(int gameNumber)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.GameNumber = gameNumber;
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

            public ServerConfigurationBuilder WithVONDisableUI(bool vonDisableUI)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.VONDisableUI = vonDisableUI;
                return this;
            }

            public ServerConfigurationBuilder WithVONDisableDirectSpeechUI(bool vonDisableDirectSpeechUI)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.VONDisableDirectSpeechUI = vonDisableDirectSpeechUI;
                return this;
            }

            public ServerConfigurationBuilder WithLobbyPlayerSynchronise(bool lobbyPlayerSync)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.LobbyPlayerSynchronise = lobbyPlayerSync;
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
