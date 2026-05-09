/******************************************************************************
 * File Name:    SavedState.cs
 * Project:      Longbow
 * Description:  This file contains the SavedState class which is a model
 *               which represents any parameters that can save state and
 *               should survive exits and restarts
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ReforgerServerApp.Utils.JsonUtils;

namespace Longbow.Models
{
  [JsonConverter(typeof(SavedStateConverter))]
  internal class SavedState
  {
    // Default Advanced Server Parameters
    public static readonly AdvancedSetting DEFAULT_MAX_FPS = new("maxFPS", 60, true);
    public static readonly AdvancedSetting DEFAULT_VERIFY_AND_REPAIR_ADDONS = new("addonsRepair", true);
    public static readonly AdvancedSetting DEFAULT_USE_UPNP = new("useUpnp", true);
    public static readonly AdvancedSetting DEFAULT_KEEP_SERVER_UPDATED = new("keepServerUpdated", true);
    public static readonly AdvancedSetting DEFAULT_USE_EXPERIMENTAL = new("useExperimental", false);
    public static readonly AdvancedSetting DEFAULT_LOAD_SESSION_SAVE = new("loadSessionSave", "", false);
    public static readonly AdvancedSetting DEFAULT_AUTO_RESTART_ON_CRASH = new("autoRestartOnCrash", false);
    public static readonly AdvancedSetting DEFAULT_AUTO_RELOAD = new("autoreload", 10, false);
    public static readonly AdvancedSetting DEFAULT_NO_BACKEND = new("noBackend", false);
    public static readonly AdvancedSetting DEFAULT_AUTO_SHUTDOWN = new("autoShutdown", false);
    public static readonly AdvancedSetting DEFAULT_LOG_VOTING = new("logVoting", false);
    public static readonly AdvancedSetting DEFAULT_OVERRIDE_PORT = new("bindPort", 2001, false);
    public static readonly AdvancedSetting DEFAULT_NETWORK_DYNAMIC_SIM = new("nds", 2, false);
    public static readonly AdvancedSetting DEFAULT_SPATIAL_MAP_RES = new("nwkResolution", 500, false);
    public static readonly AdvancedSetting DEFAULT_STAGGERING_BUDGET = new("staggeringBudget", 5000, false);
    public static readonly AdvancedSetting DEFAULT_STREAMING_BUDGET = new("streamingBudget", 500, false);
    public static readonly AdvancedSetting DEFAULT_STREAMS_DELTA = new("streamsDelta", 100, false);
    public static readonly AdvancedSetting DEFAULT_RPL_TIMEOUT_MS = new("rpl-timeout-ms", 10000, false);
    public static readonly AdvancedSetting DEFAULT_AI_PARTIAL_SIM = new("aiPartialSim", false);
    public static readonly AdvancedSetting DEFAULT_CREATE_DB = new("createDB", false);
    public static readonly AdvancedSetting DEFAULT_DEBUGGER_ADDRESS = new("debugger", "127.0.0.1", false);
    public static readonly AdvancedSetting DEFAULT_DEBUGGER_PORT = new("debuggerPort", 1000, false);
    public static readonly AdvancedSetting DEFAULT_DISABLE_SHADERS_BUILD = new("disableShadersBuild", false);
    public static readonly AdvancedSetting DEFAULT_GENERATE_SHADERS = new("generateShaders", false);
    public static readonly AdvancedSetting DEFAULT_RPL_ENCODE_AS_LONG_JOBS = new("rplEncodeAsLongJobs", false);
    public static readonly AdvancedSetting DEFAULT_JOB_SYS_SHORT_WORKER_COUNT = new("jobsysShortWorkerCount", Utilities.GetNumberAvailableThreads(), false);
    public static readonly AdvancedSetting DEFAULT_JOB_SYS_LONG_WORKER_COUNT = new("jobsysLongWorkerCount", Utilities.GetNumberAvailableThreads() / 2, false);
    public static readonly AdvancedSetting DEFAULT_FREEZE_CHECK = new("freezeCheck", 300, false);
    public static readonly AdvancedSetting DEFAULT_FREEZE_CHECK_MODE = new("freezeCheckMode", "minidump", false);
    public static readonly AdvancedSetting DEFAULT_FORCE_DISABLE_NIGHT_GRAIN = new("forceDisableNightGrain", false);

    public Dictionary<string, AdvancedSetting> advancedSettings { get; set; }
    public string serverLocation { get; set; }

    public SavedState(Dictionary<string, AdvancedSetting> advancedSettings, string serverLocation)
    {
      this.advancedSettings = advancedSettings;
      this.serverLocation = serverLocation;
    }

    public static SavedState Default => new(GetDefaultAdvancedSettings(), string.Empty);

    /// <summary>
    /// Display <c>SavedState</c> in readable Json format.
    /// </summary>
    /// <returns>Json string representation of the <c>SavedState</c></returns>
    public string AsJsonString()
    {
      return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Create a default Advanced Settings dictionary
    /// </summary>
    /// <returns>Advanced Settings dictionary containing defaults</returns>
    private static Dictionary<string, AdvancedSetting> GetDefaultAdvancedSettings()
    {
      Dictionary<string, AdvancedSetting> advancedSettings = new();
      advancedSettings[DEFAULT_MAX_FPS.Name] = DEFAULT_MAX_FPS;
      advancedSettings[DEFAULT_VERIFY_AND_REPAIR_ADDONS.Name] = DEFAULT_VERIFY_AND_REPAIR_ADDONS;
      advancedSettings[DEFAULT_USE_UPNP.Name] = DEFAULT_USE_UPNP;
      advancedSettings[DEFAULT_USE_EXPERIMENTAL.Name] = DEFAULT_USE_EXPERIMENTAL;
      advancedSettings[DEFAULT_KEEP_SERVER_UPDATED.Name] = DEFAULT_KEEP_SERVER_UPDATED;
      advancedSettings[DEFAULT_MAX_FPS.Name] = DEFAULT_MAX_FPS;
      advancedSettings[DEFAULT_VERIFY_AND_REPAIR_ADDONS.Name] = DEFAULT_VERIFY_AND_REPAIR_ADDONS;
      advancedSettings[DEFAULT_USE_UPNP.Name] = DEFAULT_USE_UPNP;
      advancedSettings[DEFAULT_USE_EXPERIMENTAL.Name] = DEFAULT_USE_EXPERIMENTAL;
      advancedSettings[DEFAULT_LOAD_SESSION_SAVE.Name] = DEFAULT_LOAD_SESSION_SAVE;
      advancedSettings[DEFAULT_AUTO_RESTART_ON_CRASH.Name] = DEFAULT_AUTO_RESTART_ON_CRASH;
      advancedSettings[DEFAULT_AUTO_RELOAD.Name] = DEFAULT_AUTO_RELOAD;
      advancedSettings[DEFAULT_NO_BACKEND.Name] = DEFAULT_NO_BACKEND;
      advancedSettings[DEFAULT_AUTO_SHUTDOWN.Name] = DEFAULT_AUTO_SHUTDOWN;
      advancedSettings[DEFAULT_LOG_VOTING.Name] = DEFAULT_LOG_VOTING;
      advancedSettings[DEFAULT_OVERRIDE_PORT.Name] = DEFAULT_OVERRIDE_PORT;
      advancedSettings[DEFAULT_NETWORK_DYNAMIC_SIM.Name] = DEFAULT_NETWORK_DYNAMIC_SIM;
      advancedSettings[DEFAULT_SPATIAL_MAP_RES.Name] = DEFAULT_SPATIAL_MAP_RES;
      advancedSettings[DEFAULT_STAGGERING_BUDGET.Name] = DEFAULT_STAGGERING_BUDGET;
      advancedSettings[DEFAULT_STREAMING_BUDGET.Name] = DEFAULT_STREAMING_BUDGET;
      advancedSettings[DEFAULT_STREAMS_DELTA.Name] = DEFAULT_STREAMS_DELTA;
      advancedSettings[DEFAULT_RPL_TIMEOUT_MS.Name] = DEFAULT_RPL_TIMEOUT_MS;
      advancedSettings[DEFAULT_AI_PARTIAL_SIM.Name] = DEFAULT_AI_PARTIAL_SIM;
      advancedSettings[DEFAULT_CREATE_DB.Name] = DEFAULT_CREATE_DB;
      advancedSettings[DEFAULT_DEBUGGER_ADDRESS.Name] = DEFAULT_DEBUGGER_ADDRESS;
      advancedSettings[DEFAULT_DEBUGGER_PORT.Name] = DEFAULT_DEBUGGER_PORT;
      advancedSettings[DEFAULT_DISABLE_SHADERS_BUILD.Name] = DEFAULT_DISABLE_SHADERS_BUILD;
      advancedSettings[DEFAULT_GENERATE_SHADERS.Name] = DEFAULT_GENERATE_SHADERS;
      advancedSettings[DEFAULT_RPL_ENCODE_AS_LONG_JOBS.Name] = DEFAULT_RPL_ENCODE_AS_LONG_JOBS;
      advancedSettings[DEFAULT_JOB_SYS_SHORT_WORKER_COUNT.Name] = DEFAULT_JOB_SYS_SHORT_WORKER_COUNT;
      advancedSettings[DEFAULT_JOB_SYS_LONG_WORKER_COUNT.Name] = DEFAULT_JOB_SYS_LONG_WORKER_COUNT;
      advancedSettings[DEFAULT_FREEZE_CHECK.Name] = DEFAULT_FREEZE_CHECK;
      advancedSettings[DEFAULT_FREEZE_CHECK_MODE.Name] = DEFAULT_FREEZE_CHECK_MODE;
      advancedSettings[DEFAULT_FORCE_DISABLE_NIGHT_GRAIN.Name] = DEFAULT_FORCE_DISABLE_NIGHT_GRAIN;
      return advancedSettings;
    }
  }
}
