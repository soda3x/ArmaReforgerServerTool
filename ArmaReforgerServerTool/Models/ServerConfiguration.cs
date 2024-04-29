using System.Text;
using System.Text.Json;

namespace ReforgerServerApp
{
    /// <summary>
    /// Enum representing the permissions for
    /// RCon clients
    /// </summary>
    public enum RconPermission { ADMIN, MONITOR }

    /// <summary>
    /// Structure representing the root of the Server Config
    /// </summary>
    public struct Root
    {
        public string    bindAddress;
        public int       bindPort;
        public string    publicAddress;
        public int       publicPort;
        public A2S       a2s;
        // public Rcon rcon; TODO: This is not implemented yet, uncomment this once its ready to go
        public Game      game;
        public Operating operating;
    }

    /// <summary>
    /// Structure representing the a2s block of the Server Config
    /// </summary>
    public struct A2S
    {
        public string address;
        public int    port;
    }

    /// <summary>
    /// Structure representing the rcon block of the Server Config
    /// </summary>
    public struct Rcon
    {
        public string         address;
        public int            port;
        public string         password;
        public RconPermission permission;
        public string[]       blacklist;
        public string[]       whitelist;
        public int            maxClients;
    }

    /// <summary>
    /// Structure representing the game block of the Server Config
    /// </summary>
    public struct Game
    {
        public string         name;
        public string         password;
        public string         passwordAdmin;
        public string[]       admins;
        public string         scenarioId;
        public int            maxPlayers;
        public bool           visible;
        public bool           crossPlatform;
        public string[]       supportedPlatforms;
        public GameProperties gameProperties;
        public Mod[]          mods;
    }

    /// <summary>
    /// Structure representing the gameProperties block of the Server Config
    /// </summary>
    public struct GameProperties
    {
        public int                        serverMaxViewDistance;
        public int                        serverMinGrassDistance;
        public int                        networkViewDistance;
        public bool                       disableThirdPerson;
        public bool                       fastValidation;
        public bool                       battlEye;
        public bool                       vonDisableUI;
        public bool                       vonDisableDirectSpeechUI;
        public Dictionary<string, string> missionHeader;
        public bool                       vonCanTransmitCrossFaction;
    }

    /// <summary>
    /// Structure representing the operating block of the Server Config
    /// </summary>
    public struct Operating
    {
        public bool lobbyPlayerSynchronise;
        public int  playerSaveTime;
        public int  aiLimit;
        public int  slotReservationTimeout;
        public bool disableNavmeshStreaming;
        public bool disableServerShutdown;
        public bool disableCrashReporter;
        public bool disableAI;
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
            return JsonSerializer.Serialize(root);
        }

        /// <summary>
        /// Display Mods from the configuration in a JSON format
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration's Mods</returns>
        public string ModsAsJsonString()
        {
            return JsonSerializer.Serialize(root.game.mods);
        }

        /// <summary>
        /// Display Mission Header from the configuration in a JSON format
        /// </summary>
        /// <returns>JSON string representation of the Server Configuration's Mission Header</returns>
        public string MissionHeaderAsJsonString()
        {
            return JsonSerializer.Serialize(root.game.gameProperties.missionHeader);
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
            root.game.gameProperties.missionHeader = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        }
    }
}
