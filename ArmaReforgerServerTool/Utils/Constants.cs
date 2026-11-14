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

    public const string ERROR_MESSAGEBOX_TITLE_STR = "common.error.title";
    public const string WARN_MESSAGEBOX_TITLE_STR = "common.warning.title";
    public const string ERROR_DIALOG_FORMAT_STR = "common.errorDialog.format";
    public const string INFO_MESSAGEBOX_TITLE_STR = "common.info.title";
    public const string ENABLE_ALL_MODS_STR = "main.enableAllMods.tooltip";
    public const string DISABLE_ALL_MODS_STR = "main.disableAllMods.tooltip";
    public const string ENABLE_MOD_STR = "main.enableMod.tooltip";
    public const string DISABLE_MOD_STR = "main.disableMod.tooltip";
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
    public const string SERVER_CURRENTLY_RUNNING_STR = "main.serverRunning.message";
    public const string START_SERVER_STR = "main.startServer.tooltip";
    public const string DOWNLOAD_SERVER_FILES_STR = "main.downloadServerFiles.tooltip";
    public const string LOCATE_SERVER_FILES_STR = "main.locateServerFiles.tooltip";
    public const string DELETE_SERVER_FILES_STR = "main.deleteServerFiles.tooltip";
    public const string SERVER_JSON_STR = "\\server.json";
    public const string CURRENTLY_SELECTED_STR = "scenarioSelector.currentlySelected.label";
    public const string SERVER_FILES_NOT_FOUND_SCENARIO_SELECT_STR = "Arma Reforger server files not found, you will need to install them from the Server Management tab first.";
    public const string SELECT_SCENARIO_STR = "scenarioSelector.selectScenario.label";

    public const string SUPPORTED_PLATFORM_PC = "PLATFORM_PC";
    public const string SUPPORTED_PLATFORM_XBOX = "PLATFORM_XBL";
    public const string SUPPORTED_PLATFORM_PSN = "PLATFORM_PSN";

    public static string USE_UPNP_STR = "main.useUpnp.tooltip";
    public static string USE_EXPERIMENTAL_STR = "main.useExperimental.tooltip";
    public static string KEEP_SERVER_UPDATED_STR = "main.keepServerUpdated.tooltip";

    public const string SERVER_PARAM_DISABLE_AI_TOOLTIP_STR = "serverParam.disableAi.tooltip";

    public const string SERVER_PARAM_AI_LIMIT_TOOLTIP_STR = "serverParam.aiLimit.tooltip";

    public const string SERVER_PARAM_PLAYER_SAVE_TIME_TOOLTIP_STR = "serverParam.playerSaveTime.tooltip";
    public const string SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_TOOLTIP_STR = "serverParam.slotReservationTimeout.tooltip";

    public const string SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_TOOLTIP_STR = "serverParam.disableServerShutdown.tooltip";

    public const string SERVER_PARAM_DISABLE_NAVMESH_STREAMING_TOOLTIP_STR = "serverParam.disableNavmeshStreaming.tooltip";

    public const string SERVER_PARAM_DISABLE_SPECIFIC_NAVMESH_STREAMING_TOOLTIP_STR = "serverParam.disableSpecificNavmeshStreaming.tooltip";

    public const string SERVER_PARAM_DISABLE_CRASH_REPORT_TOOLTIP_STR = "serverParam.disableCrashReport.tooltip";

    public const string SERVER_PARAM_LOBBY_PLAYER_SYNC_TOOLTIP_STR = "serverParam.lobbyPlayerSync.tooltip";

    public const string SERVER_PARAM_VON_CAN_TRANSMIT_ACROSS_FACTION_TOOLTIP_STR = "serverParam.vonCanTransmitAcrossFaction.tooltip";

    public const string SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_TOOLTIP_STR = "serverParam.vonDisableDirectSpeechUi.tooltip";

    public const string SERVER_PARAM_VON_DISABLE_UI_TOOLTIP_STR = "serverParam.vonDisableUi.tooltip";

    public const string SERVER_PARAM_DISABLE_THIRD_PERSON_TOOLTIP_STR = "serverParam.disableThirdPerson.tooltip";

    public const string SERVER_PARAM_BATTLEYE_TOOLTIP_STR = "serverParam.battleye.tooltip";

    public const string SERVER_PARAM_NETWORK_VIEW_DISTANCE_TOOLTIP_STR = "serverParam.networkViewDistance.tooltip";

    public const string SERVER_PARAM_FAST_VALIDATION_TOOLTIP_STR = "serverParam.fastValidation.tooltip";

    public const string SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_TOOLTIP_STR = "serverParam.minGrassDistance.tooltip";

    public const string SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_TOOLTIP_STR = "serverParam.maxViewDistance.tooltip";

    public const string SERVER_PARAM_CROSS_PLATFORM_TOOLTIP_STR = "serverParam.crossPlatform.tooltip";

    public const string SERVER_PARAM_VISIBLE_TOOLTIP_STR = "serverParam.visible.tooltip";

    public const string SERVER_PARAM_MAX_PLAYERS_TOOLTIP_STR = "serverParam.maxPlayers.tooltip";

    public const string SERVER_PARAM_ADMIN_PASSWORD_TOOLTIP_STR = "serverParam.adminPassword.tooltip";

    public const string SERVER_PARAM_ADMINS_TOOLTIP_STR = "serverParam.admins.tooltip";

    public const string SERVER_PARAM_PASSWORD_TOOLTIP_STR = "serverParam.password.tooltip";

    public const string SERVER_PARAM_NAME_TOOLTIP_STR = "serverParam.name.tooltip";

    public const string SERVER_PARAM_A2S_ADDRESS_TOOLTIP_STR = "serverParam.a2sAddress.tooltip";

    public const string SERVER_PARAM_A2S_PORT_TOOLTIP_STR = "serverParam.a2sPort.tooltip";

    public const string SERVER_PARAM_PUBLIC_PORT_TOOLTIP_STR = "serverParam.publicPort.tooltip";

    public const string SERVER_PARAM_PUBLIC_ADDRESS_TOOLTIP_STR = "serverParam.publicAddress.tooltip";

    public const string SERVER_PARAM_BIND_PORT_TOOLTIP_STR = "serverParam.bindPort.tooltip";

    public const string SERVER_PARAM_BIND_ADDRESS_TOOLTIP_STR = "serverParam.bindAddress.tooltip";

    public const string SERVER_PARAM_ENABLE_RCON_TOOLTIP_STR = "serverParam.enableRcon.tooltip";

    public const string SERVER_PARAM_RCON_ADDRESS_TOOLTIP_STR = "serverParam.rconAddress.tooltip";

    public const string SERVER_PARAM_RCON_PORT_TOOLTIP_STR = "serverParam.rconPort.tooltip";

    public const string SERVER_PARAM_RCON_PASSWORD_TOOLTIP_STR = "serverParam.rconPassword.tooltip";

    public const string SERVER_PARAM_RCON_MAX_CLIENTS_TOOLTIP_STR = "serverParam.rconMaxClients.tooltip";

    public const string SERVER_PARAM_RCON_PERMISSION_TOOLTIP_STR = "serverParam.rconPermission.tooltip";

    public const string SERVER_PARAM_RCON_WHITELIST_TOOLTIP_STR = "serverParam.rconWhitelist.tooltip";

    public const string SERVER_PARAM_RCON_BLACKLIST_TOOLTIP_STR = "serverParam.rconBlacklist.tooltip";

    public const string SERVER_PARAM_JOIN_QUEUE_MAX_SIZE_TOOLTIP_STR = "serverParam.joinQueueMaxSize.tooltip";

    public const string SERVER_PARAM_MODS_REQUIRED_BY_DEFAULT_TOOLTIP_STR = "serverParam.modsRequiredByDefault.tooltip";

    public const string SERVER_PARAM_ENABLE_PERSISTENCE_TOOLTIP_STR = "serverParam.enablePersistence.tooltip";

    public static string SERVER_PARAM_AUTO_SAVE_INTERVAL_TOOLTIP_STR = "serverParam.autoSaveInterval.tooltip";

    public static string SERVER_PARAM_HIVE_ID_TOOLTIP_STR = "serverParam.hiveId.tooltip";

    public static string SERVER_PARAM_DATABASES_TOOLTIP_STR = "serverParam.databases.tooltip";

    public static string SERVER_PARAM_STORAGES_TOOLTIP_STR = "serverParam.storages.tooltip";

    public static string SERVER_PARAM_SAVE_RETENTION_TOOLTIP_STR = "serverParam.saveRetention.tooltip";
    public static string SERVER_PARAM_LOAD_SESSION_SAVE_TOOLTIP_STR = "serverParam.loadSessionSave.tooltip";
    public static string SERVER_PARAM_KEEP_SESSION_SAVE_TOOLTIP_STR = "serverParam.keepSessionSave.tooltip";

    public static string MIGRATE_LEGACY_MOD_DB_PROMPT_STR = "modDb.migrateLegacy.message";
    public const string EXPORT_MODS_STR = "main.exportMods.tooltip";
    public const string IMPORT_MODS_STR = "main.importMods.tooltip";
    public const string STOCK_MOD_ID = "591AF5BDA9F7CE8B";
    public const string NO_BACKEND_SCENARIO_LOADER_MOD_ID = "6324F7124A9768FB";

    public const string LANGUAGE_CHANGE_RESTART_REQUIRED_STR = "main.languageChangeRestart.message";
    // Server/Advanced Parameter friendly names (Fase 4)
    public const string SERVER_PARAM_NAME_LABEL_STR = "serverParam.name.label";
    public const string SERVER_PARAM_PASSWORD_LABEL_STR = "serverParam.password.label";
    public const string SERVER_PARAM_PASSWORD_ADMIN_LABEL_STR = "serverParam.passwordAdmin.label";
    public const string SERVER_PARAM_ADMINS_LABEL_STR = "serverParam.admins.label";
    public const string SERVER_PARAM_MODS_REQUIRED_BY_DEFAULT_LABEL_STR = "serverParam.modsRequiredByDefault.label";
    public const string SERVER_PARAM_MAX_PLAYERS_LABEL_STR = "serverParam.maxPlayers.label";
    public const string SERVER_PARAM_VISIBLE_LABEL_STR = "serverParam.visible.label";
    public const string SERVER_PARAM_BIND_ADDRESS_LABEL_STR = "serverParam.bindAddress.label";
    public const string SERVER_PARAM_BIND_PORT_LABEL_STR = "serverParam.bindPort.label";
    public const string SERVER_PARAM_PUBLIC_ADDRESS_LABEL_STR = "serverParam.publicAddress.label";
    public const string SERVER_PARAM_PUBLIC_PORT_LABEL_STR = "serverParam.publicPort.label";
    public const string SERVER_PARAM_ADDRESS_LABEL_STR = "serverParam.address.label";
    public const string SERVER_PARAM_PORT_LABEL_STR = "serverParam.port.label";
    public const string SERVER_PARAM_RCON_ENABLED_LABEL_STR = "serverParam.rconEnabled.label";
    public const string SERVER_PARAM_RCON_ADDRESS_LABEL_STR = "serverParam.rconAddress.label";
    public const string SERVER_PARAM_RCON_PORT_LABEL_STR = "serverParam.rconPort.label";
    public const string SERVER_PARAM_RCON_PASSWORD_LABEL_STR = "serverParam.rconPassword.label";
    public const string SERVER_PARAM_RCON_MAX_CLIENTS_LABEL_STR = "serverParam.rconMaxClients.label";
    public const string SERVER_PARAM_RCON_PERMISSION_LABEL_STR = "serverParam.rconPermission.label";
    public const string SERVER_PARAM_RCON_WHITELIST_LABEL_STR = "serverParam.rconWhitelist.label";
    public const string SERVER_PARAM_RCON_BLACKLIST_LABEL_STR = "serverParam.rconBlacklist.label";
    public const string SERVER_PARAM_PLAYER_SAVE_TIME_LABEL_STR = "serverParam.playerSaveTime.label";
    public const string SERVER_PARAM_SERVER_MAX_VIEW_DISTANCE_LABEL_STR = "serverParam.serverMaxViewDistance.label";
    public const string SERVER_PARAM_SERVER_MIN_GRASS_DISTANCE_LABEL_STR = "serverParam.serverMinGrassDistance.label";
    public const string SERVER_PARAM_NETWORK_VIEW_DISTANCE_LABEL_STR = "serverParam.networkViewDistance.label";
    public const string SERVER_PARAM_DISABLE_THIRD_PERSON_LABEL_STR = "serverParam.disableThirdPerson.label";
    public const string SERVER_PARAM_FAST_VALIDATION_LABEL_STR = "serverParam.fastValidation.label";
    public const string SERVER_PARAM_BATTL_EYE_LABEL_STR = "serverParam.battlEye.label";
    public const string SERVER_PARAM_LOBBY_PLAYER_SYNCHRONISE_LABEL_STR = "serverParam.lobbyPlayerSynchronise.label";
    public const string SERVER_PARAM_VON_DISABLE_UI_LABEL_STR = "serverParam.VONDisableUI.label";
    public const string SERVER_PARAM_VON_DISABLE_DIRECT_SPEECH_UI_LABEL_STR = "serverParam.VONDisableDirectSpeechUI.label";
    public const string SERVER_PARAM_VON_CAN_TRANSMIT_CROSS_FACTION_LABEL_STR = "serverParam.VONCanTransmitCrossFaction.label";
    public const string SERVER_PARAM_CROSS_PLATFORM_LABEL_STR = "serverParam.crossPlatform.label";
    public const string SERVER_PARAM_AI_LIMIT_LABEL_STR = "serverParam.aiLimit.label";
    public const string SERVER_PARAM_SLOT_RESERVATION_TIMEOUT_LABEL_STR = "serverParam.slotReservationTimeout.label";
    public const string SERVER_PARAM_TOGGLE_DISABLE_NAVMESH_STREAMING_LABEL_STR = "serverParam.toggleDisableNavmeshStreaming.label";
    public const string SERVER_PARAM_DISABLE_NAVMESH_STREAMING_LABEL_STR = "serverParam.disableNavmeshStreaming.label";
    public const string SERVER_PARAM_DISABLE_SERVER_SHUTDOWN_LABEL_STR = "serverParam.disableServerShutdown.label";
    public const string SERVER_PARAM_DISABLE_CRASH_REPORTER_LABEL_STR = "serverParam.disableCrashReporter.label";
    public const string SERVER_PARAM_DISABLE_AI_LABEL_STR = "serverParam.disableAI.label";
    public const string SERVER_PARAM_MAX_SIZE_LABEL_STR = "serverParam.maxSize.label";
    public const string SERVER_PARAM_PERSISTENCE_ENABLED_LABEL_STR = "serverParam.persistenceEnabled.label";
    public const string SERVER_PARAM_AUTO_SAVE_INTERVAL_LABEL_STR = "serverParam.autoSaveInterval.label";
    public const string SERVER_PARAM_SAVE_RETENTION_LABEL_STR = "serverParam.saveRetention.label";
    public const string SERVER_PARAM_LOAD_SESSION_SAVE_LABEL_STR = "serverParam.loadSessionSave.label";
    public const string SERVER_PARAM_KEEP_SESSION_SAVE_LABEL_STR = "serverParam.keepSessionSave.label";
    public const string SERVER_PARAM_HIVE_ID_LABEL_STR = "serverParam.hiveId.label";
    public const string SERVER_PARAM_DATABASES_LABEL_STR = "serverParam.databases.label";
    public const string SERVER_PARAM_STORAGES_LABEL_STR = "serverParam.storages.label";
    public const string ADVANCED_PARAM_MAX_FPS_LABEL_STR = "advancedParam.maxFPS.label";
    public const string ADVANCED_PARAM_AUTO_RESTART_DAILY_LABEL_STR = "advancedParam.autoRestartDaily.label";
    public const string ADVANCED_PARAM_ADDONS_REPAIR_LABEL_STR = "advancedParam.addonsRepair.label";
    public const string ADVANCED_PARAM_AUTO_RESTART_ON_CRASH_LABEL_STR = "advancedParam.autoRestartOnCrash.label";
    public const string ADVANCED_PARAM_AUTORELOAD_LABEL_STR = "advancedParam.autoreload.label";
    public const string ADVANCED_PARAM_LOAD_SESSION_SAVE_LABEL_STR = "advancedParam.loadSessionSave.label";
    public const string ADVANCED_PARAM_NO_BACKEND_LABEL_STR = "advancedParam.noBackend.label";
    public const string ADVANCED_PARAM_AUTO_SHUTDOWN_LABEL_STR = "advancedParam.autoShutdown.label";
    public const string ADVANCED_PARAM_LOG_VOTING_LABEL_STR = "advancedParam.logVoting.label";
    public const string ADVANCED_PARAM_BIND_PORT_LABEL_STR = "advancedParam.bindPort.label";
    public const string ADVANCED_PARAM_NDS_LABEL_STR = "advancedParam.nds.label";
    public const string ADVANCED_PARAM_NWK_RESOLUTION_LABEL_STR = "advancedParam.nwkResolution.label";
    public const string ADVANCED_PARAM_STAGGERING_BUDGET_LABEL_STR = "advancedParam.staggeringBudget.label";
    public const string ADVANCED_PARAM_STREAMING_BUDGET_LABEL_STR = "advancedParam.streamingBudget.label";
    public const string ADVANCED_PARAM_STREAMS_DELTA_LABEL_STR = "advancedParam.streamsDelta.label";
    public const string ADVANCED_PARAM_RPL_TIMEOUT_MS_LABEL_STR = "advancedParam.rplTimeoutMs.label";
    public const string ADVANCED_PARAM_AI_PARTIAL_SIM_LABEL_STR = "advancedParam.aiPartialSim.label";
    public const string ADVANCED_PARAM_CREATE_DB_LABEL_STR = "advancedParam.createDB.label";
    public const string ADVANCED_PARAM_DEBUGGER_LABEL_STR = "advancedParam.debugger.label";
    public const string ADVANCED_PARAM_DEBUGGER_PORT_LABEL_STR = "advancedParam.debuggerPort.label";
    public const string ADVANCED_PARAM_DISABLE_SHADERS_BUILD_LABEL_STR = "advancedParam.disableShadersBuild.label";
    public const string ADVANCED_PARAM_GENERATE_SHADERS_LABEL_STR = "advancedParam.generateShaders.label";
    public const string ADVANCED_PARAM_RPL_ENCODE_AS_LONG_JOBS_LABEL_STR = "advancedParam.rplEncodeAsLongJobs.label";
    public const string ADVANCED_PARAM_JOBSYS_SHORT_WORKER_COUNT_LABEL_STR = "advancedParam.jobsysShortWorkerCount.label";
    public const string ADVANCED_PARAM_JOBSYS_LONG_WORKER_COUNT_LABEL_STR = "advancedParam.jobsysLongWorkerCount.label";
    public const string ADVANCED_PARAM_FREEZE_CHECK_LABEL_STR = "advancedParam.freezeCheck.label";
    public const string ADVANCED_PARAM_FREEZE_CHECK_MODE_LABEL_STR = "advancedParam.freezeCheckMode.label";
    public const string ADVANCED_PARAM_FORCE_DISABLE_NIGHT_GRAIN_LABEL_STR = "advancedParam.forceDisableNightGrain.label";
    public const string ADVANCED_PARAM_KEEP_SESSION_SAVE_LABEL_STR = "advancedParam.keepSessionSave.label";
    public const string ADVANCED_PARAM_PLAYER_LIMITS_LABEL_STR = "advancedParam.playerLimits.label";

    // Advanced parameter descriptions (Fase 4)
    public const string ADVANCED_PARAM_MAX_FPS_DESCRIPTION_STR = "advancedParam.maxFPS.description";
    public const string ADVANCED_PARAM_AUTO_RESTART_DAILY_DESCRIPTION_STR = "advancedParam.autoRestartDaily.description";
    public const string ADVANCED_PARAM_ADDONS_REPAIR_DESCRIPTION_STR = "advancedParam.addonsRepair.description";
    public const string ADVANCED_PARAM_AUTO_RESTART_ON_CRASH_DESCRIPTION_STR = "advancedParam.autoRestartOnCrash.description";
    public const string ADVANCED_PARAM_AUTORELOAD_DESCRIPTION_STR = "advancedParam.autoreload.description";
    public const string ADVANCED_PARAM_LOAD_SESSION_SAVE_DESCRIPTION_STR = "advancedParam.loadSessionSave.description";
    public const string ADVANCED_PARAM_NO_BACKEND_DESCRIPTION_STR = "advancedParam.noBackend.description";
    public const string ADVANCED_PARAM_AUTO_SHUTDOWN_DESCRIPTION_STR = "advancedParam.autoShutdown.description";
    public const string ADVANCED_PARAM_LOG_VOTING_DESCRIPTION_STR = "advancedParam.logVoting.description";
    public const string ADVANCED_PARAM_BIND_PORT_DESCRIPTION_STR = "advancedParam.bindPort.description";
    public const string ADVANCED_PARAM_NDS_DESCRIPTION_STR = "advancedParam.nds.description";
    public const string ADVANCED_PARAM_NWK_RESOLUTION_DESCRIPTION_STR = "advancedParam.nwkResolution.description";
    public const string ADVANCED_PARAM_STAGGERING_BUDGET_DESCRIPTION_STR = "advancedParam.staggeringBudget.description";
    public const string ADVANCED_PARAM_STREAMING_BUDGET_DESCRIPTION_STR = "advancedParam.streamingBudget.description";
    public const string ADVANCED_PARAM_STREAMS_DELTA_DESCRIPTION_STR = "advancedParam.streamsDelta.description";
    public const string ADVANCED_PARAM_RPL_TIMEOUT_MS_DESCRIPTION_STR = "advancedParam.rplTimeoutMs.description";
    public const string ADVANCED_PARAM_AI_PARTIAL_SIM_DESCRIPTION_STR = "advancedParam.aiPartialSim.description";
    public const string ADVANCED_PARAM_CREATE_DB_DESCRIPTION_STR = "advancedParam.createDB.description";
    public const string ADVANCED_PARAM_DEBUGGER_DESCRIPTION_STR = "advancedParam.debugger.description";
    public const string ADVANCED_PARAM_DEBUGGER_PORT_DESCRIPTION_STR = "advancedParam.debuggerPort.description";
    public const string ADVANCED_PARAM_DISABLE_SHADERS_BUILD_DESCRIPTION_STR = "advancedParam.disableShadersBuild.description";
    public const string ADVANCED_PARAM_GENERATE_SHADERS_DESCRIPTION_STR = "advancedParam.generateShaders.description";
    public const string ADVANCED_PARAM_RPL_ENCODE_AS_LONG_JOBS_DESCRIPTION_STR = "advancedParam.rplEncodeAsLongJobs.description";
    public const string ADVANCED_PARAM_JOBSYS_SHORT_WORKER_COUNT_DESCRIPTION_STR = "advancedParam.jobsysShortWorkerCount.description";
    public const string ADVANCED_PARAM_JOBSYS_LONG_WORKER_COUNT_DESCRIPTION_STR = "advancedParam.jobsysLongWorkerCount.description";
    public const string ADVANCED_PARAM_FREEZE_CHECK_DESCRIPTION_STR = "advancedParam.freezeCheck.description";
    public const string ADVANCED_PARAM_FREEZE_CHECK_MODE_DESCRIPTION_STR = "advancedParam.freezeCheckMode.description";
    public const string ADVANCED_PARAM_FORCE_DISABLE_NIGHT_GRAIN_DESCRIPTION_STR = "advancedParam.forceDisableNightGrain.description";
    public const string ADVANCED_PARAM_KEEP_SESSION_SAVE_DESCRIPTION_STR = "advancedParam.keepSessionSave.description";
    public const string ADVANCED_PARAM_PLAYER_LIMITS_DESCRIPTION_STR = "advancedParam.playerLimits.description";

    // Placeholders and tooltips set outside the Designer (Fase 4)
    public const string ADVANCED_PARAM_LOAD_SESSION_SAVE_PLACEHOLDER_STR = "advancedParam.loadSessionSave.placeholder";
    public const string ADVANCED_PARAM_PLAYER_LIMITS_PLACEHOLDER_STR = "advancedParam.playerLimits.placeholder";
    public const string ADD_MOD_NAME_PLACEHOLDER_STR = "addMod.name.placeholder";
    public const string ADD_MOD_ID_PLACEHOLDER_STR = "addMod.id.placeholder";
    public const string ADD_MOD_VERSION_PLACEHOLDER_STR = "addMod.version.placeholder";
    public const string MAIN_MODS_SEARCH_PLACEHOLDER_STR = "main.modsSearch.placeholder";
    public const string MAIN_LOAD_SAVE_GAME_TOOLTIP_STR = "main.loadSaveGame.tooltip";

    // Designer control text (Fase 4)
    public const string ABOUT_OK_BUTTON_STR = "about.ok.button";
    public const string ABOUT_REPORT_ISSUE_BUTTON_STR = "about.reportIssue.button";
    public const string ADD_MOD_ADD_BUTTON_STR = "addMod.add.button";
    public const string ADD_MOD_CANCEL_BUTTON_STR = "addMod.cancel.button";
    public const string ADD_MOD_REQUIRED_LABEL_STR = "addMod.required.label";
    public const string ADD_MOD_ADD_TITLE_STR = "addMod.add.title";
    public const string ADD_MOD_EDIT_TITLE_STR = "addMod.edit.title";
    public const string ADD_MOD_EDIT_BUTTON_STR = "addMod.edit.button";
    public const string ADD_MOD_REQUIRED_TOOLTIP_STR = "addMod.required.tooltip";
    public const string BAN_MANAGER_UNBAN_BUTTON_STR = "banManager.unban.button";
    public const string BAN_MANAGER_CANCEL_BUTTON_STR = "banManager.cancel.button";
    public const string BAN_MANAGER_ID_COLUMN_STR = "banManager.id.column";
    public const string BAN_MANAGER_PLAYER_NAME_COLUMN_STR = "banManager.playerName.column";
    public const string CREATE_BAN_1HOUR_BUTTON_STR = "createBan.ban1Hour.button";
    public const string CREATE_BAN_24HOURS_BUTTON_STR = "createBan.ban24Hours.button";
    public const string CREATE_BAN_BAN_BUTTON_STR = "createBan.ban.button";
    public const string CREATE_BAN_QUICK_BANS_TITLE_STR = "createBan.quickBans.title";
    public const string CREATE_BAN_FOREVER_BUTTON_STR = "createBan.banForever.button";
    public const string CREATE_BAN_DURATION_PLACEHOLDER_STR = "createBan.duration.placeholder";
    public const string CREATE_BAN_REASON_PLACEHOLDER_STR = "createBan.reason.placeholder";
    public const string CREATE_BAN_CREATING_FOR_MESSAGE_STR = "createBan.creatingFor.message";
    public const string LIST_FORM_ADD_BUTTON_STR = "listForm.add.button";
    public const string LIST_FORM_REMOVE_BUTTON_STR = "listForm.remove.button";
    public const string LIST_FORM_CLOSE_BUTTON_STR = "listForm.close.button";
    public const string RENAME_FORM_RENAME_BUTTON_STR = "renameForm.rename.button";
    public const string RENAME_FORM_CANCEL_BUTTON_STR = "renameForm.cancel.button";
    public const string SAVE_SELECTOR_SELECT_BUTTON_STR = "saveSelector.select.button";
    public const string SAVE_SELECTOR_KEEP_BUTTON_STR = "saveSelector.keep.button";
    public const string SAVE_SELECTOR_DELETE_BUTTON_STR = "saveSelector.delete.button";
    public const string SAVE_SELECTOR_RENAME_BUTTON_STR = "saveSelector.rename.button";
    public const string SAVE_SELECTOR_LATEST_BUTTON_STR = "saveSelector.latest.button";
    public const string SAVE_SELECTOR_EJECT_BUTTON_STR = "saveSelector.eject.button";
    public const string SAVE_SELECTOR_USING_LATEST_MESSAGE_STR = "saveSelector.usingLatest.message";
    public const string SAVE_SELECTOR_LOADED_SAVE_MESSAGE_STR = "saveSelector.loadedSave.message";
    public const string SAVE_SELECTOR_NO_SAVE_LOADED_MESSAGE_STR = "saveSelector.noSaveLoaded.message";
    public const string SCENARIO_SELECTOR_SELECT_BUTTON_STR = "scenarioSelector.select.button";
    public const string SCENARIO_SELECTOR_RELOAD_BUTTON_STR = "scenarioSelector.reload.button";
    public const string SCENARIO_SELECTOR_MANUAL_ID_PLACEHOLDER_STR = "scenarioSelector.manualId.placeholder";
    public const string SCENARIO_SELECTOR_FETCHING_MESSAGE_STR = "scenarioSelector.fetching.message";
    public const string TEXT_INPUT_OK_BUTTON_STR = "textInput.ok.button";
    public const string MAIN_CONFIGURATION_TAB_STR = "main.configuration.tab";
    public const string MAIN_LOAD_CONFIG_BUTTON_STR = "main.loadConfig.button";
    public const string MAIN_SAVE_CONFIG_BUTTON_STR = "main.saveConfig.button";
    public const string MAIN_MODS_TITLE_STR = "main.mods.title";
    public const string MAIN_ENABLED_MODS_LABEL_STR = "main.enabledMods.label";
    public const string MAIN_AVAILABLE_MODS_LABEL_STR = "main.availableMods.label";
    public const string MAIN_SERVER_CONFIGURATION_TITLE_STR = "main.serverConfiguration.title";
    public const string MAIN_SELECT_SCENARIO_BUTTON_STR = "main.selectScenario.button";
    public const string MAIN_SAVE_MANAGER_BUTTON_STR = "main.saveManager.button";
    public const string MAIN_MISSION_HEADER_BUTTON_STR = "main.missionHeader.button";
    public const string MAIN_MANAGEMENT_TAB_STR = "main.management.tab";
    public const string MAIN_KEEP_SERVER_UPDATED_LABEL_STR = "main.keepServerUpdated.label";
    public const string MAIN_USE_UPNP_LABEL_STR = "main.useUpnp.label";
    public const string MAIN_USE_EXPERIMENTAL_LABEL_STR = "main.useExperimental.label";
    public const string MAIN_LOG_LEVEL_LABEL_STR = "main.logLevel.label";
    public const string MAIN_LANGUAGE_LABEL_STR = "main.language.label";
    public const string MAIN_CLEAR_LOG_BUTTON_STR = "main.clearLog.button";
    public const string MAIN_ADVANCED_TITLE_STR = "main.advanced.title";
    public const string MAIN_STATUS_TAB_STR = "main.status.tab";
    public const string MAIN_SERVER_FPS_TITLE_STR = "main.serverFps.title";
    public const string MAIN_SERVER_FPS_CHART_STR = "main.serverFpsChart.title";
    public const string MAIN_SERVER_MEMORY_TITLE_STR = "main.serverMemory.title";
    public const string MAIN_SERVER_MEMORY_CHART_STR = "main.serverMemoryChart.title";
    public const string MAIN_PLAYER_COUNT_TITLE_STR = "main.playerCount.title";
    public const string MAIN_PING_SITE_TITLE_STR = "main.pingSite.title";
    public const string MAIN_JOIN_CODE_TITLE_STR = "main.joinCode.title";
    public const string MAIN_RCON_ADDRESS_TITLE_STR = "main.rconAddress.title";
    public const string MAIN_SERVER_ADDRESS_TITLE_STR = "main.serverAddress.title";
    public const string MAIN_STEAMCMD_FOUND_MESSAGE_STR = "main.steamCmdFound.message";
    public const string MAIN_STEAMCMD_NOT_FOUND_MESSAGE_STR = "main.steamCmdNotFound.message";
    public const string MAIN_NO_SCENARIO_CHOSEN_MESSAGE_STR = "main.noScenarioChosen.message";
    public const string MAIN_NO_SCENARIO_ID_CHOSEN_MESSAGE_STR = "main.noScenarioIdChosen.message";

    // Dialog messages (Fase 4)
    public const string PROGRAM_LOGGER_CONFIG_FAILED_STR = "program.loggerConfigFailed.message";
    public const string PROGRAM_LOGGER_CONFIG_FAILED_DETAIL_STR = "program.loggerConfigFailedDetail.message";
    public const string PROGRAM_APPLICATION_FAILED_STR = "program.applicationFailed.message";
    public const string PROGRAM_APPLICATION_FAILED_DETAIL_STR = "program.applicationFailedDetail.message";
    public const string MAIN_FAILED_TO_OPEN_LAST_CONFIG_STR = "main.failedToOpenLastConfig.message";
    public const string MAIN_FAILED_TO_OPEN_LAST_CONFIG_DETAIL_STR = "main.failedToOpenLastConfigDetail.message";
    public const string MAIN_ENABLE_NO_BACKEND_CONFIRM_STR = "main.enableNoBackendConfirm.message";
    public const string SAVE_SELECTOR_DELETE_CONFIRM_STR = "saveSelector.deleteConfirm.message";
    public const string SCENARIO_SELECTOR_LOAD_ERROR_STR = "scenarioSelector.loadError.message";
    public const string SCENARIO_SELECTOR_FETCH_THREAD_ERROR_STR = "scenarioSelector.fetchThreadError.message";
    public const string CONFIG_MANAGER_LOAD_FAILED_STR = "configManager.loadFailed.message";
    public const string FILE_IO_IMPORT_MODS_FAILED_STR = "fileIO.importModsFailed.message";
    public const string FILE_IO_IMPORT_MODS_FAILED_DETAIL_STR = "fileIO.importModsFailedDetail.message";
    public const string FILE_IO_WRITE_CONFIG_FAILED_STR = "fileIO.writeConfigFailed.message";
    public const string FILE_IO_IMPORT_LEGACY_MODS_FAILED_STR = "fileIO.importLegacyModsFailed.message";
    public const string FILE_IO_IMPORT_LEGACY_MODS_FAILED_DETAIL_STR = "fileIO.importLegacyModsFailedDetail.message";
    public const string FILE_IO_LEGACY_DB_MIGRATED_STR = "fileIO.legacyDbMigrated.message";
    public const string FILE_IO_UPDATE_AVAILABLE_STR = "fileIO.updateAvailable.message";
    public const string FILE_IO_UPDATE_AVAILABLE_TITLE_STR = "fileIO.updateAvailable.title";
    public const string FILE_IO_UPDATE_CHECK_FAILED_STR = "fileIO.updateCheckFailed.message";
    public const string FILE_IO_VCREDIST_MISSING_STR = "fileIO.vcRedistMissing.message";
    public const string FILE_IO_VCREDIST_MISSING_TITLE_STR = "fileIO.vcRedistMissing.title";
    public const string FILE_IO_DELETE_SERVER_FILES_CONFIRM_STR = "fileIO.deleteServerFilesConfirm.message";
    public const string FILE_IO_DELETE_SERVER_FILES_TITLE_STR = "fileIO.deleteServerFilesConfirm.title";
    public const string FILE_IO_FILES_DELETED_STR = "fileIO.filesDeleted.message";
    public const string FILE_IO_SUCCESS_TITLE_STR = "fileIO.success.title";
    public const string FILE_IO_SERVER_FILES_NOT_FOUND_STR = "fileIO.serverFilesNotFound.message";
    public const string FILE_IO_SERVER_FILES_NOT_FOUND_TITLE_STR = "fileIO.serverFilesNotFound.title";
    public const string FILE_IO_DELETE_FILE_FAILED_STR = "fileIO.deleteFileFailed.message";
    public const string FILE_IO_RENAME_FAILED_STR = "fileIO.renameFailed.message";
    public const string NETWORK_MANAGER_UPNP_FAILED_STR = "networkManager.upnpFailed.message";
    public const string NETWORK_MANAGER_UPNP_NAT_NOT_FOUND_STR = "networkManager.upnpNatNotFound.message";
    public const string NETWORK_MANAGER_UPNP_GENERIC_FAILED_STR = "networkManager.upnpGenericFailed.message";
    public const string PROCESS_MANAGER_GENERIC_ERROR_STR = "processManager.genericError.message";
    public const string SAVED_STATE_MANAGER_MALFORMED_STR = "savedStateManager.malformed.message";
    public const string SAVED_STATE_MANAGER_MALFORMED_DETAIL_STR = "savedStateManager.malformedDetail.message";
    public const string TOOL_PROPERTIES_MANAGER_MALFORMED_STR = "toolPropertiesManager.malformed.message";
    public const string TOOL_PROPERTIES_MANAGER_MALFORMED_DETAIL_STR = "toolPropertiesManager.malformedDetail.message";
    public const string TOOL_PROPERTIES_MANAGER_SAVE_FAILED_STR = "toolPropertiesManager.saveFailed.message";
    public const string MOD_FETCH_SCENARIO_IDS_FAILED_STR = "mod.fetchScenarioIdsFailed.message";
    public const string SERVER_CONFIG_MISSION_HEADER_MALFORMED_STR = "serverConfig.missionHeaderMalformed.message";
    public const string SERVER_CONFIG_MISSION_HEADER_MALFORMED_DETAIL_STR = "serverConfig.missionHeaderMalformedDetail.message";
    public const string JSON_UTILS_LOAD_VALUE_FAILED_STR = "jsonUtils.loadValueFailed.message";

    // Status labels (Fase 4)
    public const string MAIN_SERVER_OFFLINE_STR = "main.serverOffline.status";
    public const string MAIN_FPS_OFFLINE_STR = "main.fpsOffline.status";
    public const string MAIN_MEMORY_OFFLINE_STR = "main.memoryOffline.status";
    public const string MAIN_PLAYER_COUNT_STATUS_STR = "main.playerCount.status";
    public const string MAIN_FPS_STATUS_STR = "main.fps.status";
    public const string MAIN_MEMORY_STATUS_STR = "main.memoryUsage.status";

  }
}
