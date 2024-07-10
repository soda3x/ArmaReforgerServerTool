/******************************************************************************
 * File Name:    ServerConfiguration.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This file contains the ServerConfiguration class, which is
 *               a model representing the Arma Reforger server configuration
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ReforgerServerApp.Utils.Utilities;

namespace ReforgerServerApp
{
    /// <summary>
    /// Enum representing the permissions for RCon clients
    /// </summary>
    [JsonConverter(typeof(LowercaseEnumConverter<RconPermission>))]
    public enum RconPermission { ADMIN, MONITOR }

    /// <summary>
    /// Structure representing the root of the Server Config
    /// </summary>
    public class Root
    {
        public string bindAddress { get; set; }
        public int bindPort { get; set; }
        public string publicAddress { get; set; }
        public int publicPort { get; set; }
        public A2S a2s { get; set; }
        public Rcon rcon { get; set; }
        public Game game { get; set; }
        public Operating operating { get; set; }

        public Root(string bindAddress, int bindPort, string publicAddress, 
            int publicPort, A2S a2s, Rcon rcon, Game game, Operating operating)
        {
            this.bindAddress = bindAddress;
            this.bindPort = bindPort;
            this.publicAddress = publicAddress;
            this.publicPort = publicPort;
            this.a2s = a2s;
            this.rcon = rcon;
            this.game = game;
            this.operating = operating;
        }

        public static Root Default => new(string.Empty, 0, string.Empty, 0, A2S.Default, 
            Rcon.Default, Game.Default, Operating.Default);
        
    }

    /// <summary>
    /// Structure representing the a2s block of the Server Config
    /// </summary>
    public class A2S
    {
        public string address { get; set; }
        public int port { get; set; }

        public A2S(string address, int port) 
        {
            this.address = address;
            this.port = port;
        }

        public static A2S Default => new(string.Empty, 0);
    }

    /// <summary>
    /// Structure representing the rcon block of the Server Config
    /// </summary>
    [JsonConverter(typeof(RconConditionalConverter))]
    public class Rcon
    {
        public string address { get; set; }
        public int port { get; set; }
        public string password { get; set; }
        public RconPermission permission { get; set; }
        public string[] blacklist { get; set; }
        public string[] whitelist { get; set; }
        public int maxClients { get; set; }

        public Rcon(string address, int port, string password, 
            RconPermission permission, string[] blacklist, string[] whitelist, int maxClients) 
        {
            this.address = address;
            this.port = port;
            this.password = password;
            this.permission = permission;
            this.blacklist = blacklist;
            this.whitelist = whitelist;
            this.maxClients = maxClients;
        }

        public static Rcon Default => new(string.Empty, Constants.RCON_PORT_DEFAULT, string.Empty, RconPermission.MONITOR, 
            Array.Empty<string>(), Array.Empty<string>(), Constants.RCON_MAX_CLIENTS_DEFAULT);
    }

    /// <summary>
    /// Structure representing the game block of the Server Config
    /// </summary>
    public class Game
    {
        public string name { get; set; }
        public string password { get; set; }
        public string passwordAdmin { get; set; }
        public string[] admins { get; set; }
        public string scenarioId { get; set; }
        public int maxPlayers { get; set; }
        public bool visible { get; set; }
        public bool crossPlatform { get; set; }
        public string[] supportedPlatforms { get; set; }
        public GameProperties gameProperties { get; set; }
        public Mod[] mods { get; set; }

        public Game(string name, string password, string passwordAdmin, string[] admins, 
            string scenarioId, int maxPlayers, bool visible, bool crossPlatform, 
            string[] supportedPlatforms, GameProperties gameProperties, Mod[] mods) 
        {
            this.name = name;
            this.password = password;
            this.passwordAdmin = passwordAdmin;
            this.admins = admins;
            this.scenarioId = scenarioId;
            this.maxPlayers = maxPlayers;
            this.visible = visible;
            this.crossPlatform = crossPlatform;
            this.supportedPlatforms = supportedPlatforms;
            this.gameProperties = gameProperties;
            this.mods = mods;
        }

        public static Game Default => new(string.Empty, string.Empty, string.Empty, Array.Empty<string>(), 
            string.Empty, 0, false, false, Array.Empty<string>(), GameProperties.Default, Array.Empty<Mod>());
    }

    /// <summary>
    /// Structure representing the gameProperties block of the Server Config
    /// </summary>
    public class GameProperties
    {
        public int serverMaxViewDistance { get; set; }
        public int serverMinGrassDistance { get; set; }
        public int networkViewDistance { get; set; }
        public bool disableThirdPerson { get; set; }
        public bool fastValidation { get; set; }
        public bool battlEye { get; set; }
        [JsonPropertyName("VONDisableUI")]
        public bool vonDisableUI { get; set; }
        [JsonPropertyName("VONDisableDirectSpeechUI")]
        public bool vonDisableDirectSpeechUI { get; set; }
        [JsonPropertyName("VONCanTransmitCrossFaction")]
        public bool vonCanTransmitCrossFaction { get; set; }
        public JsonDocument missionHeader { get; set; }

        public GameProperties(int serverMaxViewDistance, int serverMinGrassDistance, int networkViewDistance, 
            bool disableThirdPerson, bool fastValidation, bool battlEye, bool vonDisableUI, bool vonDisableDirectSpeechUI, 
            bool vonCanTransmitCrossFaction, JsonDocument missionHeader)
        {
            this.serverMaxViewDistance = serverMaxViewDistance;
            this.serverMinGrassDistance = serverMinGrassDistance;
            this.networkViewDistance = networkViewDistance;
            this.disableThirdPerson = disableThirdPerson;
            this.fastValidation = fastValidation;
            this.battlEye = battlEye;
            this.vonDisableUI = vonDisableUI;
            this.vonDisableDirectSpeechUI = vonDisableDirectSpeechUI;
            this.vonCanTransmitCrossFaction = vonCanTransmitCrossFaction;
            this.missionHeader = missionHeader;
        }

        public static GameProperties Default => new(0, 0, 0, false, false, false, false, false, false, JsonDocument.Parse("{}"));
    }

    /// <summary>
    /// Structure representing the operating block of the Server Config
    /// </summary>
    public class Operating
    {
        public bool lobbyPlayerSynchronise { get; set; }
        public int playerSaveTime { get; set; }
        public int aiLimit { get; set; }
        public int slotReservationTimeout { get; set; }
        public string[] disableNavmeshStreaming { get; set; }
        public bool disableServerShutdown { get; set; }
        public bool disableCrashReporter { get; set; }
        public bool disableAI { get; set; }

        public Operating(bool lobbyPlayerSynchronise, int playerSaveTime, int aiLimit, int slotReservationTimeout,
            string[] disableNavmeshStreaming, bool disableServerShutdown, bool disableCrashReporter, bool disableAI)
        {
            this.lobbyPlayerSynchronise = lobbyPlayerSynchronise;
            this.playerSaveTime = playerSaveTime;
            this.aiLimit = aiLimit;
            this.slotReservationTimeout = slotReservationTimeout;
            this.disableNavmeshStreaming = disableNavmeshStreaming;
            this.disableServerShutdown = disableServerShutdown;
            this.disableCrashReporter = disableCrashReporter;
            this.disableAI = disableAI;
        }

        public static Operating Default => new(false, 0, 0, 0, Array.Empty<string>(), false, false, false);
    }

    public class ServerConfiguration
    {
        public Root root { get; set; }

        public bool rconEnabled { get; set; }

        public ServerConfiguration()
        {
            root = Root.Default;
        }

        /// <summary>
        /// Display ServerConfiguration in the JSON format required by the Arma Server files.
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration</returns>
        public string AsJsonString()
        {
            return Utilities.GetFormattedJsonString(root, new Utilities.ModConverter());
        }

        /// <summary>
        /// Display Mods from the configuration in a JSON format
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration's Mods</returns>
        public string ModsAsJsonString()
        {
            return Utilities.GetFormattedJsonString(root.game.mods, new Utilities.ModConverter());
        }

        /// <summary>
        /// Display Mission Header from the configuration in a JSON format
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration's Mission Header</returns>
        public string MissionHeaderAsJsonString()
        {
            return Utilities.GetFormattedJsonString(root.game.gameProperties.missionHeader);
        }

        /// <summary>
        /// Deserialize a JSON string into the Server Configuration
        /// </summary>
        /// <param name="json">to convert into the Server Configuration</param>
        public void SetServerConfigurationFromJson(string json)
        {
            root = Utilities.GetServerConfigFromJson(json, new Utilities.ModConverter());
        }

        /// <summary>
        /// Deserialize a JSON string into the Mission Header
        /// </summary>
        /// <param name="json">to convert into the Mission Header</param>
        public void SetMissionHeaderFromJson(string json)
        {
            try
            {
                root.game.gameProperties.missionHeader = JsonSerializer.Deserialize<JsonDocument>(json)!;
            }
            catch
            {
                Utilities.DisplayErrorMessage("The mission header is malformed. Please check your formatting is valid JSON and try again.",
                    "Unable to parse Mission Header.");
            }
        }
    }
}
