namespace ReforgerServerApp
{
    /// <summary>
    /// This class contains constants used throughout the application.
    /// Hopefully increases readability of other code having all these in one place.
    /// </summary>
    internal class Constants
    {
        public static string ERROR_MESSAGEBOX_TITLE_STR = "Arma Reforger Dedicated Server Tool - Error";
        public static string ENABLE_ALL_MODS_STR = "Enable All Mods";
        public static string DISABLE_ALL_MODS_STR = "Disable All Mods";
        public static string ENABLE_MOD_STR = "Enable Mod";
        public static string DISABLE_MOD_STR = "Disable Mod";
        public static string SCENARIO_ID_TOOLTIP_STR = "Enter the Scenario ID found in the Scenario's serverData.json file, or select one of the default ones";
        public static string REGION_TOOLTIP_STR = "Enter an ISO 3166-1 alpha-2 region code, or select one of the default ones";
        public static string NDS_TOOLTIP_STR = "Network Dynamic Simulation (nds) is a server feature that only streams in relevant replicated entities for each client." +
                "\r\nThe provided value stands for diameter, or the number of cells which are being replicated - default is 2 in each direction." +
                "\r\nTo turn the feature off use '0'." +
                "\r\nA higher diameter will result in a bigger networked view range, lower server performance.";
        public static string NWK_RESOL_TOOLTIP_STR = "Defines what resolution Spatial Map cells should be set at in a 100m - 1000m range." +
                "\r\nSmaller resolution will result in less \"pop-in\" but lower networked view range." +
                "\r\nFor high view range use high resolution, but small diameter.";
        public static string STAGGER_BDGT_TOOLTIP_STR = "Defines how many stationary spatial map cells are allowed to be processed in one tick in a 1 - 10201 range." +
                "\r\nIf not set it uses the NDS diameter. A lower number will limit how many cells the server has to process per tick, but increase the time it takes for a client to have all relevant entities streamed in." +
                "\r\nIf the server experiences significant performance drops on spawning/teleporting then the number is set too high." +
                "\r\nIf the client experiences \"pop-in\" of replicated items then the number is set too low.";
        public static string STREAMING_BDGT_TOOLTIP_STR = "The global streaming budget that is equally distributed between all connections." +
                "\r\nTo decrement the budget, it uses the replicated hierarchy size of each entity that needs to be streamed in.\r\n" +
                "It cannot go under 100 to prevent the system stalling." +
                "\r\nA lower number will limit how many entities the server has to process per tick, but increase the time it takes for a client to have that entity streamed in." +
                "\r\nIf the server experiences significant performance drops on spawning/teleporting then the number is set too high." +
                "\r\nIf the client experiences \"pop-in\" of replicated items then the number is set too low.";
        public static string STREAMS_DELTA_TOOLTIP_STR = "A tool to limit the amount of streams being opened for a client in range 1 - 1000 (default 100)." +
                "\r\nIf the difference between 'the number of streams the server has open' and 'the number of streams the client has open' is larger than the NUMBER then the server will not open any more streams this tick." +
                "\r\nTo be adjusted based on average client networking speed.";
        public static string LIST_SCENARIOS_TOOLTIP_STR = "Prints to game logs the scenario .conf file paths.\r\nNote that all other launch arguments are ignored if this is selected.";
        public static string LOAD_SESSION_SAVE_TOOLTIP_STR = "If this option is enabled and the text field is empty, the latest savegame will be loaded." +
            "\r\nEnter the path to a savegame file to load a specific save.";
        public static string SERVER_CURRENTLY_RUNNING_STR = "Server is currently running. To modify the configuration, you will need to stop it first.";
        public static string START_SERVER_STR = "Start Server";
        public static string STOP_SERVER_STR = "Stop Server";
        public static string SERVER_JSON_STR = "\\server.json";
        public static string CURRENTLY_SELECTED_STR = "Currently selected scenario is:";
        public static string SERVER_FILES_NOT_FOUND_SCENARIO_SELECT_STR = "Arma Reforger server files not found, you will need to install them from the Server Management tab first.";
        public static string SELECT_SCENARIO_STR = "Select a scenario from the list";

        public static string SERVER_PARAM_DISABLE_AI_TOOLTIP_STR = "If enabled, the server will prevent initialization and ticking of AIWorld and its components.\r\n" +
            "Will completely disable AI functionality on the server.";

        public static string SERVER_PARAM_AI_LIMIT_TOOLTIP_STR = "Sets the top limit of AIs. No system will be able to spawn any AIs when this ceiling is reached.\r\n" +
            "A negative number is not considered as valid value and is then ignored - limit is not applied.";

        public static string SERVER_PARAM_PLAYER_SAVE_TIME_TOOLTIP_STR = "Default period in seconds for saving players for both Online and Local storage (player save can still be requested on demand).";
        public static string SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_TOOLTIP_STR = "Sets the duration (in seconds) for how long will the backend and server reserve a slot for kicked player.\r\n" +
            "It is considered disabled when set to the minimal value, the value being the same as for a normal disconnect.";

        public static string SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_TOOLTIP_STR = "If enabled, the server will not automatically shutdown if connection to backend is lost.\r\n" +
            "Related to room requests errors - other causes like corrupted config will still shutdown the server.";

        public static string SERVER_PARAM_DISABLE_NAVMESH_STREAMING_TOOLTIP_STR = "If enabled, the server will disable navmesh streaming on all navmesh components and load the entire navmesh into memory.\r\n" +
            "This setting provides slightly better server performance and reaction times of moving AIs at the cost of higher memory consumption (up to hundreds of MB depending on the terrain).";

        public static string SERVER_PARAM_DISABLE_CRASH_REPORT_TOOLTIP_STR = "If enabled, the automatic server-side Crash Report is disabled.";

        public static string SERVER_PARAM_LOBBY_PLAYER_SYNC_TOOLTIP_STR = "If enabled, the list of player identities present on server is sent to the GameAPI along with the server's heartbeat.";

        public static string SERVER_PARAM_VON_CAN_TRANSMIT_ACROSS_FACTION_TOOLTIP_STR = "Option to allow players to transmit on other factions radios. true is allow to communicate, false is listen-only";

        public static string SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_TOOLTIP_STR = "Force clients to not have VON (Voice Over Network) Direct Speech UI.";

        public static string SERVER_PARAM_VON_DISABLE_UI_TOOLTIP_STR = "Force clients to not have VON (Voice Over Network) UI.";

        public static string SERVER_PARAM_DISABLE_THIRD_PERSON_TOOLTIP_STR = "Force clients to use the first-person view.";

        public static string SERVER_PARAM_BATTLEYE_TOOLTIP_STR = "Enable BattlEye Anti-Cheat.";

        public static string SERVER_PARAM_NETWORK_VIEW_DISTANCE_TOOLTIP_STR = "Maximum network streaming range of replicated entities.";

        public static string SERVER_PARAM_FAST_VALIDATION_TOOLTIP_STR = "Validation of map entities and components loaded on client when it joins, ensuring things match with initial server state.\r\n\r\n" +
            "Enabled - minimum information required to make sure data matches is exchanged between client.\r\n" +
            "\tWhen a mismatch occurs, no additional information will be available for determining where client and server states start to differ.\r\n" +
            "\tAll servers that expect clients to connect over internet should have fast validation enabled.\r\n\r\n" +
            "Disabled - extra data for every replicated entity and component in the map will be transferred when new client connects to the server.\r\n" +
            "\tWhen a mismatch occurs, it is possible to point at particular entity or component where things start to differ.\r\n" +
            "\tWhen developing locally (ie. both server and client run on the same machine), it is fine to disable fast validation to more easily pin point source of the problem.";

        public static string SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_TOOLTIP_STR = "Minimum grass distance in meters. If set to 0 no distance is forced upon clients.";

        public static string SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_TOOLTIP_STR = "Maximum view distance enforced by the server.";

        public static string SERVER_PARAM_CROSS_PLATFORM_TOOLTIP_STR = "If enabled, Xbox clients will be able to connect to the server";

        public static string SERVER_PARAM_VISIBLE_TOOLTIP_STR = "Set the visibility of the server in the Server Browser.";

        public static string SERVER_PARAM_MAX_PLAYERS_TOOLTIP_STR = "Set the maximum amount of players on the server.";

        public static string SERVER_PARAM_ADMIN_PASSWORD_TOOLTIP_STR = "Defines the server's admin password, allows a server administrator to login and control the server.\r\n" +
            "To access this either open the chat input box by pressing C in the lobby or Enter in-game followed by: #login [the admin password]";

        public static string SERVER_PARAM_PASSWORD_TOOLTIP_STR = "Password required to join the server.";

        public static string SERVER_PARAM_NAME_TOOLTIP_STR = "Server name (what the Server will be seen as in the Server Browser)";

        public static string SERVER_PARAM_STEAM_QUERY_PORT_TOOLTIP_STR = "Change Steam Query UDP port on which game listens to A2S requests.";

        public static string SERVER_PARAM_PUBLIC_PORT_TOOLTIP_STR = "UDP port registered in backend. If the server itself has a public IP address, this should be the same value as in bindPort.\r\n" +
            "Otherwise, this is the UDP port that is forwarded to the server.";

        public static string SERVER_PARAM_PUBLIC_ADDRESS_TOOLTIP_STR = "IP address registered in backend. This should be set to the public IP address to which clients can connect in order to reach the server (either IP of the server itself or IP of the machine that will forward data to the server).\r\n" +
            "If the entry is missing, empty or 0.0.0.0, then the public IP address will be automatically detected and used by the backend.";

        public static string SERVER_PARAM_BIND_PORT_TOOLTIP_STR = "UDP port to which the server socket will be bound.";

        public static string SERVER_PARAM_BIND_ADDRESS_TOOLTIP_STR = "IP address to which the server socket will be bound. In most cases, this should be left empty.\r\n" +
            "It can be used to restrict connections to a particular network interface. When left out or empty, 0.0.0.0 is used, which allows connections through any IP address.";
    }
}
