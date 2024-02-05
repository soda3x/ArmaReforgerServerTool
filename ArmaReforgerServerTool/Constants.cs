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
        public static string AI_LIMIT_TOOLTIP_STR = "A negative number is not considered as valid value and is then ignored - limit is not applied.";
        public static string LOAD_SESSION_SAVE_TOOLTIP_STR = "If this option is enabled and the text field is empty, the latest savegame will be loaded." +
            "\r\nEnter the path to a savegame file to load a specific save.";
        public static string SERVER_CURRENTLY_RUNNING_STR = "Server is currently running. To modify the configuration, you will need to stop it first.";
        public static string START_SERVER_STR = "Start Server";
        public static string STOP_SERVER_STR = "Stop Server";
        public static string SERVER_JSON_STR = "\\server.json";
        public static string CURRENTLY_SELECTED_STR = "Currently selected scenario is:";
        public static string SERVER_FILES_NOT_FOUND_SCENARIO_SELECT_STR = "Arma Reforger server files not found, you will need to install them from the Server Management tab first.";
        public static string SELECT_SCENARIO_STR = "Select a scenario from the list";
    }
}
