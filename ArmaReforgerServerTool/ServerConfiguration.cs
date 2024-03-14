using System.Text;

namespace ReforgerServerApp
{
    public class ServerConfiguration
    {
        public string BindAddress { get; set; }
        public int BindPort { get; set; }
        public string PublicAddress { get; set; }
        public int PublicPort { get; set; }
        public string PasswordAdmin { get; set; }
        public string ServerName { get; set; }
        public string Password { get; set; }
        public string ScenarioId { get; set; }
        public int MaxPlayers { get; set; }
        public bool Visible { get; set; }
        public int ServerMaxViewDistance { get; set; }
        public int ServerMinGrassDistance { get; set; }
        public int NetworkViewDistance { get; set; }
        public bool DisableThirdPerson { get; set; }
        public bool FastValidation { get; set; }
        public bool BattlEye { get; set; }
        public int SteamQueryPort { get; set; }
        public bool CrossPlatform { get; set; }
        public bool LobbyPlayerSynchronise { get; set; }
        public bool VONDisableUI { get; set; }
        public bool VONDisableDirectSpeechUI { get; set; }
        public int PlayerSaveTime { get; set; }
        public int AiLimit { get; set; }
        public bool VONCanTransmitCrossFaction { get; set; }
        public int SlotReservationTimeout { get; set; }
        public bool DisableNavmeshStreaming { get; set; }
        public bool DisableServerShutdown { get; set; }
        public bool DisableCrashReporter { get; set; }
        public string MissionHeader { get; set; }
        public string Admins { get; set; }

        public bool DisableAI { get; set; }
        public List<Mod> Mods { get; }

        public ServerConfiguration()
        {
            BindAddress = string.Empty;
            PublicAddress = string.Empty;
            PasswordAdmin = string.Empty;
            ServerName = string.Empty;
            Password = string.Empty;
            ScenarioId = string.Empty;
            MissionHeader = string.Empty;
            Admins = string.Empty;
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
            sb.AppendLine($"\"bindAddress\": \"{BindAddress}\",");
            sb.AppendLine($"\"bindPort\": {BindPort},");
            sb.AppendLine($"\"publicAddress\": \"{PublicAddress}\",");
            sb.AppendLine($"\"publicPort\": {PublicPort},");
            sb.AppendLine("\"a2s\": {");
            sb.AppendLine($"\"address\": \"{(PublicAddress.Equals(String.Empty) ? "0.0.0.0" : PublicAddress)}\",");
            sb.AppendLine($"\"port\": {SteamQueryPort}");
            sb.AppendLine("},");
            sb.AppendLine("\"game\": {");
            sb.AppendLine($"\"passwordAdmin\": \"{PasswordAdmin}\",");
            sb.AppendLine($"\"name\": \"{ServerName}\",");
            sb.AppendLine($"\"password\": \"{Password}\",");

            if (Admins != string.Empty)
            {
                sb.Append($"\"admins\": [ ");
                string[] splitAdmins = Admins.Split(",");
                for (int i = 0; i < splitAdmins.Length; i++)
                {
                    sb.Append($"\"{splitAdmins[i]}\"");
                    if (i != splitAdmins.Length - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.AppendLine(" ],");
            }

            sb.AppendLine($"\"scenarioId\": \"{ScenarioId}\",");
            sb.AppendLine($"\"maxPlayers\": {MaxPlayers},");
            sb.AppendLine($"\"visible\": {Visible.ToString().ToLowerInvariant()},");
            sb.AppendLine("\"supportedPlatforms\": [");

            if (CrossPlatform)
            {
                sb.AppendLine("\"PLATFORM_PC\",");
                sb.AppendLine("\"PLATFORM_XBL\"");
            }
            else
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
            sb.Append($"\"VONCanTransmitCrossFaction\": {VONCanTransmitCrossFaction.ToString().ToLowerInvariant()}");

            if (MissionHeader != string.Empty)
            {
                sb.AppendLine(",");
                sb.AppendLine("\"missionHeader\": {");
                sb.AppendLine(ConvertMissionHeaderLineEndingsToJson());
                sb.AppendLine("}");
            }
            else
            {
                sb.AppendLine();
            }

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
            sb.AppendLine("\"operating\": {");
            sb.AppendLine($"\"lobbyPlayerSynchronise\": {LobbyPlayerSynchronise.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"playerSaveTime\": {PlayerSaveTime.ToString()},");
            sb.AppendLine($"\"aiLimit\": {AiLimit.ToString()},");
            sb.AppendLine($"\"slotReservationTimeout\": {SlotReservationTimeout.ToString()},");
            sb.AppendLine($"\"disableNavmeshStreaming\": {DisableNavmeshStreaming.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"disableServerShutdown\": {DisableServerShutdown.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"disableCrashReporter\": {DisableCrashReporter.ToString().ToLowerInvariant()},");
            sb.AppendLine($"\"disableAI\": {DisableAI.ToString().ToLowerInvariant()}");
            sb.AppendLine("}");

            sb.AppendLine("}");

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Display ServerConfiguration parameters in a Key=Value format for saving to file.
        /// </summary>
        /// <returns>Key=Value representation of the Server Configuration</returns>
        public string AsKeyValue(string modFilePath)
        {
            StringBuilder sb = new();
            sb.AppendLine($"bindAddress={BindAddress}");
            sb.AppendLine($"bindPort={BindPort}");
            sb.AppendLine($"publicAddress={PublicAddress}");
            sb.AppendLine($"publicPort={PublicPort}");
            sb.AppendLine($"passwordAdmin={PasswordAdmin}");
            sb.AppendLine($"name={ServerName}");
            sb.AppendLine($"password={Password}");
            sb.AppendLine($"admins={ConvertAdminsListToKV()}");
            sb.AppendLine($"scenarioId={ScenarioId}");
            sb.AppendLine($"maxPlayers={MaxPlayers}");
            sb.AppendLine($"visible={Visible.ToString().ToLowerInvariant()}");
            sb.AppendLine($"crossPlatform={CrossPlatform.ToString().ToLowerInvariant()}");
            sb.AppendLine($"serverMaxViewDistance={ServerMaxViewDistance}");
            sb.AppendLine($"serverMinGrassDistance={ServerMinGrassDistance}");
            sb.AppendLine($"networkViewDistance={NetworkViewDistance}");
            sb.AppendLine($"disableThirdPerson={DisableThirdPerson.ToString().ToLowerInvariant()}");
            sb.AppendLine($"fastValidation={FastValidation.ToString().ToLowerInvariant()}");
            sb.AppendLine($"battlEye={BattlEye.ToString().ToLowerInvariant()}");
            sb.AppendLine($"steamQueryPort={SteamQueryPort}");
            sb.AppendLine($"vonDisableUI={VONDisableUI.ToString().ToLowerInvariant()}");
            sb.AppendLine($"vonDisableDirectSpeechUI={VONDisableDirectSpeechUI.ToString().ToLowerInvariant()}");
            sb.AppendLine($"vonCanTransmitCrossFaction={VONCanTransmitCrossFaction.ToString().ToLowerInvariant()}");
            sb.AppendLine($"lobbyPlayerSynchronise={LobbyPlayerSynchronise.ToString().ToLowerInvariant()}");
            sb.AppendLine($"playerSaveTime={PlayerSaveTime.ToString()}");
            sb.AppendLine($"aiLimit={AiLimit.ToString()}");
            sb.AppendLine($"slotReservationTimeout={SlotReservationTimeout.ToString()}");
            sb.AppendLine($"disableNavmeshStreaming={DisableNavmeshStreaming.ToString().ToLowerInvariant()}");
            sb.AppendLine($"disableServerShutdown={DisableServerShutdown.ToString().ToLowerInvariant()}");
            sb.AppendLine($"disableCrashReporter={DisableCrashReporter.ToString().ToLowerInvariant()}");
            sb.AppendLine($"disableAI={DisableAI.ToString().ToLowerInvariant()}");
            sb.AppendLine($"missionHeader={ConvertMissionHeaderLineEndingsToKV()}");
            sb.AppendLine($"modCollection={modFilePath}");
            return sb.ToString();
        }
        /// <summary>
        /// Display Mods as a comma separated string for saving to file.
        /// </summary>
        /// <returns>Comma-separated string representation of the Server Configuration's Mods</returns>
        public string ModsAsCommaSeparatedString()
        {
            StringBuilder sb = new();
            foreach (Mod m in Mods)
            {
                sb.AppendLine($"modId,{m.GetModID()},modName,{m.GetModName()}");
            }
            return sb.ToString();
        }

        public string ConvertMissionHeaderLineEndingsToJson()
        {
            string[] splitItems = MissionHeader.Split(",");
            return String.Join(",\r\n", splitItems);
        }

        public string ConvertMissionHeaderLineEndingsToKV()
        {
            string[] splitItems = MissionHeader.Split("\r\n");
            return String.Join("", splitItems);
        }

        public string ConvertAdminsListToKV()
        {
            return String.Join(",", Admins);
        }

        public class ServerConfigurationBuilder
        {
            private ServerConfiguration m_serverConfiguration;

            private void InitialiseServerConfigIfNull()
            {
                m_serverConfiguration ??= new ServerConfiguration();
            }

            public ServerConfigurationBuilder WithBindAddress(string bindAddress)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.BindAddress = bindAddress;
                return this;
            }

            public ServerConfigurationBuilder WithPublicAddress(string publicAddress)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PublicAddress = publicAddress;
                return this;
            }

            public ServerConfigurationBuilder WithAdminPassword(string adminPassword)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PasswordAdmin = adminPassword;
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
                m_serverConfiguration.Password = serverPassword;
                return this;
            }

            public ServerConfigurationBuilder WithScenarioId(string scenarioId)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.ScenarioId = scenarioId;
                return this;
            }

            public ServerConfigurationBuilder WithBindPort(int bindPort)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.BindPort = bindPort;
                return this;
            }

            public ServerConfigurationBuilder WithPublicPort(int publicPort)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PublicPort = publicPort;
                return this;
            }

            public ServerConfigurationBuilder WithMaxPlayers(int maxPlayers)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.MaxPlayers = maxPlayers;
                return this;
            }

            public ServerConfigurationBuilder WithVisible(bool visible)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.Visible = visible;
                return this;
            }

            public ServerConfigurationBuilder WithCrossPlatform(bool crossPlatform)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.CrossPlatform = crossPlatform;
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

            public ServerConfigurationBuilder WithVONCanTransmitCrossFaction(bool vonCanTransmitCrossFaction)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.VONCanTransmitCrossFaction = vonCanTransmitCrossFaction;
                return this;
            }

            public ServerConfigurationBuilder WithLobbyPlayerSynchronise(bool lobbyPlayerSync)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.LobbyPlayerSynchronise = lobbyPlayerSync;
                return this;
            }

            public ServerConfigurationBuilder WithPlayerSaveTime(int playerSaveTime)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.PlayerSaveTime = playerSaveTime;
                return this;
            }

            public ServerConfigurationBuilder WithAILimit(int aiLimit)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.AiLimit = aiLimit;
                return this;
            }

            public ServerConfigurationBuilder WithSlotReservationTimeout(int slotReservationTimeout)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.SlotReservationTimeout = slotReservationTimeout;
                return this;
            }

            public ServerConfigurationBuilder WithDisableNavmeshStreaming(bool disableNavmeshStreaming)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.DisableNavmeshStreaming = disableNavmeshStreaming;
                return this;
            }

            public ServerConfigurationBuilder WithDisableServerShutdown(bool disableServerShutdown)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.DisableServerShutdown = disableServerShutdown;
                return this;
            }

            public ServerConfigurationBuilder WithDisableCrashReporter(bool disableCrashReporter)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.DisableCrashReporter = disableCrashReporter;
                return this;
            }

            public ServerConfigurationBuilder WithDisableAI(bool disableAI)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.DisableAI = disableAI;
                return this;
            }

            public ServerConfigurationBuilder WithAdmins(string admins)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.Admins = admins;
                return this;
            }

            public ServerConfigurationBuilder WithMissionHeader(string missionHeader)
            {
                InitialiseServerConfigIfNull();
                m_serverConfiguration.MissionHeader = missionHeader;
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
