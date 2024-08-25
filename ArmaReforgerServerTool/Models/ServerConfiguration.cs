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

namespace ReforgerServerApp
{
    /// <summary>
    /// Enum representing the permissions for RCon clients
    /// </summary>
    [JsonConverter(typeof(JsonUtils.LowercaseEnumConverter<RconPermission>))]
    public enum RconPermission { ADMIN, MONITOR }

    /// <summary>
    /// Structure representing the root of the Server Config
    /// </summary>
    public class Root
    {
        public static readonly int DEFAULT_PORT  = 2001;
        public static readonly string DEFAULT_ADDRESS = "0.0.0.0";

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

        public static Root Default => new(
            DEFAULT_ADDRESS,
            DEFAULT_PORT,
            string.Empty,
            DEFAULT_PORT,
            A2S.Default,
            Rcon.Default,
            Game.Default,
            Operating.Default
        );
        
    }

    /// <summary>
    /// Structure representing the a2s block of the Server Config
    /// </summary>
    public class A2S
    {
        public static readonly string DEFAULT_ADDRESS = "0.0.0.0";
        public static readonly int DEFAULT_PORT = 17777;

        public string address { get; set; }
        public int port { get; set; }

        public A2S(string address, int port) 
        {
            this.address = address;
            this.port = port;
        }

        public static A2S Default => new(DEFAULT_ADDRESS, DEFAULT_PORT);
    }

    /// <summary>
    /// Structure representing the rcon block of the Server Config
    /// </summary>
    [JsonConverter(typeof(JsonUtils.RconConditionalConverter))]
    public class Rcon
    {
        public static readonly int DEFAULT_PORT = 19999;
        public static readonly int MIN_CLIENTS = 1;
        public static readonly int MAX_CLIENTS = 16;
        public static readonly int DEFAULT_CLIENTS = 16;
        public static readonly String[] PERMISSIONS = { "admin", "monitor" };
        public static readonly RconPermission DEFAULT_PERMISSION = RconPermission.MONITOR;

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

        public static Rcon Default => new(
            string.Empty,
            DEFAULT_PORT,
            string.Empty,
            DEFAULT_PERMISSION,
            Array.Empty<string>(),
            Array.Empty<string>(),
            DEFAULT_CLIENTS
        );
    }

    /// <summary>
    /// Structure representing the game block of the Server Config
    /// </summary>
    public class Game
    {
        public static readonly int MIN_PLAYERS = 1;
        public static readonly int MAX_PLAYERS = 256;
        public static readonly int DEFAULT_PLAYERS = 64;
        public static readonly bool DEFAULT_VISIBLE = true;
        public static readonly bool DEFAULT_CROSS_PLATFORM = false;

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

        public static Game Default => new(
            string.Empty,
            string.Empty,
            string.Empty,
            Array.Empty<string>(),
            string.Empty,
            DEFAULT_PLAYERS,
            DEFAULT_VISIBLE,
            DEFAULT_CROSS_PLATFORM,
            Array.Empty<string>(),
            GameProperties.Default,
            Array.Empty<Mod>()
        );
    }

    /// <summary>
    /// Structure representing the gameProperties block of the Server Config
    /// </summary>
    public class GameProperties
    {
        public static readonly int MIN_SERVER_VIEW_DISTANCE = 500;
        public static readonly int MAX_SERVER_VIEW_DISTANCE = 10000;
        public static readonly int DEFAULT_SERVER_VIEW_DISTANCE = 1600;
        public static readonly int MIN_SERVER_GRASS_DISTANCE = 0;
        public static readonly int MAX_SERVER_GRASS_DISTANCE = 150;
        public static readonly int DEFAULT_SERVER_GRASS_DISTANCE = 50;
        public static readonly int MIN_NETWORK_VIEW_DISTANCE = 500;
        public static readonly int MAX_NETWORK_VIEW_DISTANCE = 5000;
        public static readonly int DEFAULT_NETWORK_VIEW_DISTANCE = 1500;
        public static readonly bool DEFAULT_DISABLE_THIRD_PERSON = false;
        public static readonly bool DEFAULT_FAST_VALIDATION = true;
        public static readonly bool DEFAULT_BATTLE_EYE = true;
        public static readonly bool DEFAULT_VON_DISABLE_UI = false;
        public static readonly bool DEFAULT_VON_DISABLE_DIRECT_SPEECH_UI = false;
        public static readonly bool DEFAULT_VON_CAN_TRANSMIT_CROSS_FACTION = false;

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

        public static GameProperties Default => new(
            DEFAULT_SERVER_VIEW_DISTANCE,
            DEFAULT_SERVER_GRASS_DISTANCE,
            DEFAULT_NETWORK_VIEW_DISTANCE,
            DEFAULT_DISABLE_THIRD_PERSON,
            DEFAULT_FAST_VALIDATION,
            DEFAULT_BATTLE_EYE,
            DEFAULT_VON_DISABLE_UI,
            DEFAULT_VON_DISABLE_DIRECT_SPEECH_UI,
            DEFAULT_VON_CAN_TRANSMIT_CROSS_FACTION,
            JsonDocument.Parse("{}")
        );
    }

    /// <summary>
    /// Structure representing the operating block of the Server Config
    /// </summary>
    [JsonConverter(typeof(JsonUtils.OperatingConditionalConverter))]
    public class Operating
    {
        public static readonly bool DEFAULT_LOBBY_PLAYER_SYNCHRONISE = true;
        public static readonly int MIN_PLAYER_SAVE_TIME = 1;
        public static readonly int MAX_PLAYER_SAVE_TIME = ushort.MaxValue;
        public static readonly int DEFAULT_PLAYER_SAVE_TIME = 120;
        public static readonly int MIN_AI_LIMIT = -1;
        public static readonly int MAX_AI_LIMIT = 1000;
        public static readonly int DEFAULT_AI_LIMIT = -1;
        public static readonly int MIN_SLOT_RESERVATION_TIMEOUT = 5;
        public static readonly int MAX_SLOT_RESERVATION_TIMEOUT = 300;
        public static readonly int DEFAULT_SLOT_RESERVATION_TIMEOUT = 60;
        public static readonly bool DEFAULT_DISABLE_SERVER_SHUTDOWN = false;
        public static readonly bool DEFAULT_DISABLE_CRASH_REPORTER = false;
        public static readonly bool DEFAULT_DISABLE_AI = false;

        public bool lobbyPlayerSynchronise { get; set; }
        public int playerSaveTime { get; set; } = 120;
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

        public static Operating Default => new(
            DEFAULT_LOBBY_PLAYER_SYNCHRONISE,
            DEFAULT_PLAYER_SAVE_TIME,
            DEFAULT_AI_LIMIT, 
            DEFAULT_SLOT_RESERVATION_TIMEOUT,
            Array.Empty<string>(),
            DEFAULT_DISABLE_SERVER_SHUTDOWN,
            DEFAULT_DISABLE_CRASH_REPORTER,
            DEFAULT_DISABLE_AI
        );
    }

    public class ServerConfiguration
    {
        public Root root { get; set; }

        public bool rconEnabled { get; set; }

        public bool toggleDisableNavmeshStreaming { get; set; }

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
            return Utilities.GetFormattedJsonString(root, new JsonUtils.ModConverter(), new JsonUtils.OperatingConditionalConverter());
        }

        /// <summary>
        /// Display Mods from the configuration in a JSON format
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration's Mods</returns>
        public string ModsAsJsonString()
        {
            return Utilities.GetFormattedJsonString(root.game.mods, new JsonUtils.ModConverter());
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
            root = Utilities.GetServerConfigFromJson(json, new JsonUtils.ModConverter());
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
