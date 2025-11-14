/******************************************************************************
 * File Name:    Constants.cs
 * Project:      Longbow
 * Description:  This file contains all big constants used in the program
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  /// <summary>
  /// This class contains constants used throughout the application.
  /// Hopefully increases readability of other code having all these in one place.
  /// </summary>
  internal class Constants
  {
    public const int SERVER_PARAM_MIN_PORT = 1;
    public const int SERVER_PARAM_MAX_PORT = ushort.MaxValue;

    // TODO String for development
    public const string TODO_STR = "TODO: This needs to be implemented";

    public const string ERROR_MESSAGEBOX_TITLE_STR = "Longbow - Error";
    public const string WARN_MESSAGEBOX_TITLE_STR = "Longbow - Warning";
    public const string INFO_MESSAGEBOX_TITLE_STR = "Longbow - Information";
    public const string ENABLE_ALL_MODS_STR = "Enable All Mods";
    public const string DISABLE_ALL_MODS_STR = "Disable All Mods";
    public const string ENABLE_MOD_STR = "Enable Mod";
    public const string DISABLE_MOD_STR = "Disable Mod";
    public const string SCENARIO_ID_TOOLTIP_STR = "Enter the Scenario ID found in the Scenario's serverData.json file, or select one of the default ones";
    public const string REGION_TOOLTIP_STR = "Enter an ISO 3166-1 alpha-2 region code, or select one of the default ones";
    public const string NDS_TOOLTIP_STR = "Network Dynamic Simulation (nds) is a server feature that only streams in relevant replicated entities for each client." +
            "\r\nThe provided value stands for diameter, or the number of cells which are being replicated - default is 2 in each direction." +
            "\r\nTo turn the feature off use '0'." +
            "\r\nA higher diameter will result in a bigger networked view range, lower server performance.";
    public const string NWK_RESOL_TOOLTIP_STR = "Defines what resolution Spatial Map cells should be set at in a 100m - 1000m range." +
            "\r\nSmaller resolution will result in less \"pop-in\" but lower networked view range." +
            "\r\nFor high view range use high resolution, but small diameter.";
    public const string STAGGER_BDGT_TOOLTIP_STR = "Defines how many stationary spatial map cells are allowed to be processed in one tick in a 1 - 10201 range." +
            "\r\nIf not set it uses the NDS diameter. A lower number will limit how many cells the server has to process per tick, but increase the time it takes for a client to have all relevant entities streamed in." +
            "\r\nIf the server experiences significant performance drops on spawning/teleporting then the number is set too high." +
            "\r\nIf the client experiences \"pop-in\" of replicated items then the number is set too low.";
    public const string STREAMING_BDGT_TOOLTIP_STR = "The global streaming budget that is equally distributed between all connections." +
            "\r\nTo decrement the budget, it uses the replicated hierarchy size of each entity that needs to be streamed in.\r\n" +
            "It cannot go under 100 to prevent the system stalling." +
            "\r\nA lower number will limit how many entities the server has to process per tick, but increase the time it takes for a client to have that entity streamed in." +
            "\r\nIf the server experiences significant performance drops on spawning/teleporting then the number is set too high." +
            "\r\nIf the client experiences \"pop-in\" of replicated items then the number is set too low.";
    public const string STREAMS_DELTA_TOOLTIP_STR = "A tool to limit the amount of streams being opened for a client in range 1 - 1000 (default 100)." +
            "\r\nIf the difference between 'the number of streams the server has open' and 'the number of streams the client has open' is larger than the NUMBER then the server will not open any more streams this tick." +
            "\r\nTo be adjusted based on average client networking speed.";
    public const string LIST_SCENARIOS_TOOLTIP_STR = "Prints to game logs the scenario .conf file paths.\r\nNote that all other launch arguments are ignored if this is selected.";
    public const string LOAD_SESSION_SAVE_TOOLTIP_STR = "If this option is enabled and the text field is empty, the latest savegame will be loaded." +
        "\r\nEnter the path to a savegame file to load a specific save.";
    public const string SERVER_CURRENTLY_RUNNING_STR = "Server is currently running. To modify the configuration, you will need to stop it first.";
    public const string START_SERVER_STR = "Start / Stop Server";
    public const string DOWNLOAD_SERVER_FILES_STR = "Download dedicated server files";
    public const string LOCATE_SERVER_FILES_STR = "Locate already downloaded server";
    public const string DELETE_SERVER_FILES_STR = "Delete server files";
    public const string SERVER_JSON_STR = "\\server.json";
    public const string CURRENTLY_SELECTED_STR = "Currently selected scenario is:";
    public const string SERVER_FILES_NOT_FOUND_SCENARIO_SELECT_STR = "Arma Reforger server files not found, you will need to install them from the Server Management tab first.";
    public const string SELECT_SCENARIO_STR = "Select a scenario from the list";

    public const string SUPPORTED_PLATFORM_PC = "PLATFORM_PC";
    public const string SUPPORTED_PLATFORM_XBOX = "PLATFORM_XBL";
    public const string SUPPORTED_PLATFORM_PSN = "PLATFORM_PSN";

    public const string USE_UPNP_STR = "Enable this to attempt to open required ports using UPnP.\nIf this is successful, you will not need to port-forward.";
    public const string USE_EXPERIMENTAL_STR = "Enable this to use the Experimental version of Arma Reforger.\nNote that this is not guaranteed to work and not all parameters may be available in Longbow.";

    public const string SERVER_PARAM_DISABLE_AI_TOOLTIP_STR = "If enabled, the server will prevent initialization and ticking of AIWorld and its components.\r\n" +
        "Will completely disable AI functionality on the server.";

    public const string SERVER_PARAM_AI_LIMIT_TOOLTIP_STR = "Sets the top limit of AIs. No system will be able to spawn any AIs when this ceiling is reached.\r\n" +
        "A negative number is not considered as valid value and is then ignored - limit is not applied.";

    public const string SERVER_PARAM_PLAYER_SAVE_TIME_TOOLTIP_STR = "Default period in seconds for saving players for both Online and Local storage (player save can still be requested on demand).";
    public const string SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_TOOLTIP_STR = "Sets the duration (in seconds) for how long will the backend and server reserve a slot for kicked player.\r\n" +
        "It is considered disabled when set to the minimal value, the value being the same as for a normal disconnect.";

    public const string SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_TOOLTIP_STR = "If enabled, the server will not automatically shutdown if connection to backend is lost.\r\n" +
        "Related to room requests errors - other causes like corrupted config will still shutdown the server.";

    public const string SERVER_PARAM_DISABLE_NAVMESH_STREAMING_TOOLTIP_STR = "If enabled, the server will disable navmesh streaming on all navmesh components and load the entire navmesh into memory.\r\n" +
        "This setting provides slightly better server performance and reaction times of moving AIs at the cost of higher memory consumption (up to hundreds of MB depending on the terrain).";

    public const string SERVER_PARAM_DISABLE_SPECIFIC_NAVMESH_STREAMING_TOOLTIP_STR = "Used in conjunction with 'Disable Navmesh Streaming'.\r\n" +
        "If any navmeshes are specified here, it will disable streaming of listed navmeshes, while streaming all remaining navmeshes.";

    public const string SERVER_PARAM_DISABLE_CRASH_REPORT_TOOLTIP_STR = "If enabled, the automatic server-side Crash Report is disabled.";

    public const string SERVER_PARAM_LOBBY_PLAYER_SYNC_TOOLTIP_STR = "If enabled, the list of player identities present on server is sent to the GameAPI along with the server's heartbeat.";

    public const string SERVER_PARAM_VON_CAN_TRANSMIT_ACROSS_FACTION_TOOLTIP_STR = "Option to allow players to transmit on other factions radios. true is allow to communicate, false is listen-only";

    public const string SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_TOOLTIP_STR = "Force clients to not have VON (Voice Over Network) Direct Speech UI.";

    public const string SERVER_PARAM_VON_DISABLE_UI_TOOLTIP_STR = "Force clients to not have VON (Voice Over Network) UI.";

    public const string SERVER_PARAM_DISABLE_THIRD_PERSON_TOOLTIP_STR = "Force clients to use the first-person view.";

    public const string SERVER_PARAM_BATTLEYE_TOOLTIP_STR = "Enable BattlEye Anti-Cheat.";

    public const string SERVER_PARAM_NETWORK_VIEW_DISTANCE_TOOLTIP_STR = "Maximum network streaming range of replicated entities.";

    public const string SERVER_PARAM_FAST_VALIDATION_TOOLTIP_STR = "Validation of map entities and components loaded on client when it joins, ensuring things match with initial server state.\r\n\r\n" +
        "Enabled - minimum information required to make sure data matches is exchanged between client.\r\n" +
        "\tWhen a mismatch occurs, no additional information will be available for determining where client and server states start to differ.\r\n" +
        "\tAll servers that expect clients to connect over internet should have fast validation enabled.\r\n\r\n" +
        "Disabled - extra data for every replicated entity and component in the map will be transferred when new client connects to the server.\r\n" +
        "\tWhen a mismatch occurs, it is possible to point at particular entity or component where things start to differ.\r\n" +
        "\tWhen developing locally (ie. both server and client run on the same machine), it is fine to disable fast validation to more easily pin point source of the problem.";

    public const string SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_TOOLTIP_STR = "Minimum grass distance in meters. Lowest allowed value is 50";

    public const string SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_TOOLTIP_STR = "Maximum view distance enforced by the server.";

    public const string SERVER_PARAM_CROSS_PLATFORM_TOOLTIP_STR = "If enabled, Xbox and PlayStation clients will be able to connect to the server.";

    public const string SERVER_PARAM_VISIBLE_TOOLTIP_STR = "Set the visibility of the server in the Server Browser.";

    public const string SERVER_PARAM_MAX_PLAYERS_TOOLTIP_STR = "Set the maximum amount of players on the server.";

    public const string SERVER_PARAM_ADMIN_PASSWORD_TOOLTIP_STR = "Defines the server's admin password, allows a server administrator to login and control the server.\r\n" +
        "To access this either open the chat input box by pressing C in the lobby or Enter in-game followed by: #login [the admin password]";

    public const string SERVER_PARAM_ADMINS_TOOLTIP_STR = "List of admins by their identityIds and/or steamIds. These users can use admin commands without using the Admin Password.";

    public const string SERVER_PARAM_PASSWORD_TOOLTIP_STR = "Password required to join the server.";

    public const string SERVER_PARAM_NAME_TOOLTIP_STR = "Server name (what the Server will be seen as in the Server Browser)";

    public const string SERVER_PARAM_A2S_ADDRESS_TOOLTIP_STR = "IP address to which A2S socket will be bound.\r\n" +
        "It can be used to restrict A2S queries to a particular network interface. ";

    public const string SERVER_PARAM_A2S_PORT_TOOLTIP_STR = "Change Steam Query UDP port on which game listens to A2S requests.";

    public const string SERVER_PARAM_PUBLIC_PORT_TOOLTIP_STR = "UDP port registered in backend. If the server itself has a public IP address, this should be the same value as in bindPort.\r\n" +
        "Otherwise, this is the UDP port that is forwarded to the server.";

    public const string SERVER_PARAM_PUBLIC_ADDRESS_TOOLTIP_STR = "IP address registered in backend. This should be set to the public IP address to which clients can connect in order to reach the server (either IP of the server itself or IP of the machine that will forward data to the server).\r\n" +
        "If the entry is missing, empty or 0.0.0.0, then the public IP address will be automatically detected and used by the backend.";

    public const string SERVER_PARAM_BIND_PORT_TOOLTIP_STR = "UDP port to which the server socket will be bound.";

    public const string SERVER_PARAM_BIND_ADDRESS_TOOLTIP_STR = "IP address to which the server socket will be bound. In most cases, this should be left empty.\r\n" +
        "It can be used to restrict connections to a particular network interface. When left out or empty, 0.0.0.0 is used, which allows connections through any IP address.";

    public const string SERVER_PARAM_ENABLE_RCON_TOOLTIP_STR = "Enable RCON on your server. You must also specify a password for it successfully start.";

    public const string SERVER_PARAM_RCON_ADDRESS_TOOLTIP_STR = "IP address to which the RCON socket will be bound.\r\n" +
        "It can be used to restrict connection to a particular network interface.";

    public const string SERVER_PARAM_RCON_PORT_TOOLTIP_STR = "RCON protocol port on which the game listens.";

    public const string SERVER_PARAM_RCON_PASSWORD_TOOLTIP_STR = "RCON password for RCON clients to login with.\r\n" +
        "It is required for RCON to start, does not support spaces and must be at least 3 characters long.";

    public const string SERVER_PARAM_RCON_MAX_CLIENTS_TOOLTIP_STR = "The maximum number of clients that can connect to RCON at the same time.";

    public const string SERVER_PARAM_RCON_PERMISSION_TOOLTIP_STR = "Permission for all RCON clients:\r\n" +
        "\tadmin - The admin can perform any command.\r\n" +
        "\tmonitor - The monitor can only perform commands which do not change the server's state.";

    public const string SERVER_PARAM_RCON_WHITELIST_TOOLTIP_STR = "Specifies the list of commands that can be executed, and no other command is allowed. Should not be used in conjunction with RCON Blacklist.";

    public const string SERVER_PARAM_RCON_BLACKLIST_TOOLTIP_STR = "A list of commands excluded from execution. Should not be used in conjunction with RCON Whitelist.";

    public const string SERVER_PARAM_JOIN_QUEUE_MAX_SIZE_TOOLTIP_STR = "Sets the maximum number of players that can queue to join the server.";

    public const string SERVER_PARAM_MODS_REQUIRED_BY_DEFAULT_TOOLTIP_STR = "Overrides default value for 'required' for all mods.";

    public const string MIGRATE_LEGACY_MOD_DB_PROMPT_STR = "A mod database from a previous version of Longbow was found.\r\n\r\n" +
        "This version of the tool is not compatible with this file type.\r\n\r\n" +
        "Would you like to migrate this mod database to the new format?\r\n\r\n" +
        "(If you select Yes, the legacy file will be deleted after the migration is complete, selecting No will create a new Mod Database in the new format)";
    public const string EXPORT_MODS_STR = "Export mod list to file";
    public const string IMPORT_MODS_STR = "Import mod list from file";
    public const string STOCK_MOD_ID = "591AF5BDA9F7CE8B";
    public const string NO_BACKEND_SCENARIO_LOADER_MOD_ID = "6324F7124A9768FB";
  }
}
