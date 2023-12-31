using System;
using System.Collections.Generic;
using System.Text;

namespace ArmaReforgerServerToolWinUI.Models
{
    internal class ServerConfiguration
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
        public string MissionHeader { get; set; }
        public List<Mod> Mods { get; set; }

        public ServerConfiguration()
        {
            BindAddress = string.Empty;
            PublicAddress = string.Empty;
            PasswordAdmin = string.Empty;
            ServerName = string.Empty;
            Password = string.Empty;
            ScenarioId = string.Empty;
            MissionHeader = string.Empty;
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
            sb.Append($"\"VONDisableDirectSpeechUI\": {VONDisableDirectSpeechUI.ToString().ToLowerInvariant()}");

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
                    sb.AppendLine($"\"modId\": \"{Mods[i].ModID}\",");
                    sb.AppendLine($"\"name\": \"{Mods[i].ModName}\"");
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
            sb.AppendLine($"\"aiLimit\": {AiLimit.ToString()}");
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
            sb.AppendLine($"lobbyPlayerSynchronise={LobbyPlayerSynchronise.ToString().ToLowerInvariant()}");
            sb.AppendLine($"playerSaveTime={PlayerSaveTime.ToString()}");
            sb.AppendLine($"aiLimit={AiLimit.ToString()}");
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
                sb.AppendLine($"modId,{m.ModID},modName,{m.ModName}");
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
    }
}
