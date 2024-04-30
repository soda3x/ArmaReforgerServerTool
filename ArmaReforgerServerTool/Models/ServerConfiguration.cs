using ReforgerServerApp.Utils;
using System.Diagnostics;
using System.Text.Json;

namespace ReforgerServerApp
{
    /// <summary>
    /// Enum representing the permissions for RCon clients
    /// </summary>
    public enum RconPermission { ADMIN, MONITOR }

    /// <summary>
    /// Structure representing the root of the Server Config
    /// </summary>
    public struct Root
    {
        public string    bindAddress   = string.Empty;
        public int       bindPort      = 0;
        public string    publicAddress = string.Empty;
        public int       publicPort    = 0;
        public A2S       a2s           = new();
        //public Rcon      rcon          = new(); TODO: This is not implemented yet, uncomment this once its ready to go
        public Game      game          = new();
        public Operating operating     = new();

        public Root() {}
    }

    /// <summary>
    /// Structure representing the a2s block of the Server Config
    /// </summary>
    public struct A2S
    {
        public string address = string.Empty;
        public int    port    = 0;

        public A2S() {}
    }

    /// <summary>
    /// Structure representing the rcon block of the Server Config
    /// </summary>
    public struct Rcon
    {
        public string         address    = string.Empty;
        public int            port       = 0;
        public string         password   = string.Empty;
        public RconPermission permission = RconPermission.MONITOR;
        public string[]       blacklist  = Array.Empty<string>();
        public string[]       whitelist  = Array.Empty<string>();
        public int            maxClients = 0;

        public Rcon() {}
    }

    /// <summary>
    /// Structure representing the game block of the Server Config
    /// </summary>
    public struct Game
    {
        public string         name               = string.Empty;
        public string         password           = string.Empty;
        public string         passwordAdmin      = string.Empty;
        public string[]       admins             = Array.Empty<string>();
        public string         scenarioId         = string.Empty;
        public int            maxPlayers         = 0;
        public bool           visible            = false;
        public bool           crossPlatform      = false;
        public string[]       supportedPlatforms = Array.Empty<string>();
        public GameProperties gameProperties     = new();
        public Mod[]          mods               = Array.Empty<Mod>();

        public Game() {}
    }

    /// <summary>
    /// Structure representing the gameProperties block of the Server Config
    /// </summary>
    public struct GameProperties
    {
        public int          serverMaxViewDistance      = 0;
        public int          serverMinGrassDistance     = 0;
        public int          networkViewDistance        = 0;
        public bool         disableThirdPerson         = false;
        public bool         fastValidation             = false;
        public bool         battlEye                   = false;
        public bool         vonDisableUI               = false;
        public bool         vonDisableDirectSpeechUI   = false;
        public bool         vonCanTransmitCrossFaction = false;
        public JsonDocument missionHeader              = JsonDocument.Parse("{}");

        public GameProperties() {}
    }

    /// <summary>
    /// Structure representing the operating block of the Server Config
    /// </summary>
    public struct Operating
    {
        public bool lobbyPlayerSynchronise  = false;
        public int  playerSaveTime          = 0;
        public int  aiLimit                 = 0;
        public int  slotReservationTimeout  = 0;
        public bool disableNavmeshStreaming = false;
        public bool disableServerShutdown   = false;
        public bool disableCrashReporter    = false;
        public bool disableAI               = false;

        public Operating() {}
    }

    public class ServerConfiguration
    {
        public Root root;

        public ServerConfiguration()
        {
            root = new();
        }

        /// <summary>
        /// Display ServerConfiguration in the JSON format required by the Arma Server files.
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration</returns>
        public string AsJsonString()
        {
            return Utilities.GetFormattedJsonString(root);
        }

        /// <summary>
        /// Display Mods from the configuration in a JSON format
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration's Mods</returns>
        public string ModsAsJsonString()
        {
            return Utilities.GetFormattedJsonString(root.game.mods);
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
            root = JsonSerializer.Deserialize<Root>(json);
        }

        /// <summary>
        /// Deserialize a JSON string into the Mission Header
        /// </summary>
        /// <param name="json">to convert into the Mission Header</param>
        public void SetMissionHeaderFromJson(string json)
        {
            root.game.gameProperties.missionHeader = JsonSerializer.Deserialize<JsonDocument>(json)!;
        }
    }
}
