/******************************************************************************
 * File Name:    ConfigurationManager.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This file contains the singleton ConfigurationManager class
 *               in charge of manipulating and representing the server's 
 *               configuration
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using Serilog;
using ReforgerServerApp.Managers;
using ReforgerServerApp.Utils;
using System.ComponentModel;
using ReforgerServerApp.Components;

namespace ReforgerServerApp
{ 

    internal class ScenarioIdEventArgs : EventArgs
    {
        public string scenarioId;
        public ScenarioIdEventArgs(string scenarioId) { this.scenarioId = scenarioId; }
    }

    /// <summary>
    /// This class manages the manipulation of the Server Configuration
    /// </summary>
    internal class ConfigurationManager
    {
        private static ConfigurationManager?                         m_instance;
        private readonly Dictionary<string, ServerParameter>         m_serverParamsDictionary;
        private readonly Dictionary<string, AdvancedServerParameter> m_advServerParamsDictionary;
        private ServerConfiguration                                  m_serverConfig;
        private BindingList<Mod>                                     m_availableMods;
        private BindingList<Mod>                                     m_enabledMods;

        public bool useExperimentalServer {  get; set; }

        public delegate void UpdateScenarioIdFromLoadedConfig(object sender, ScenarioIdEventArgs e);
        public event UpdateScenarioIdFromLoadedConfig UpdateScenarioIdFromLoadedConfigEvent;

        private ConfigurationManager()
        {
            m_serverParamsDictionary    = new Dictionary<string, ServerParameter>();
            m_advServerParamsDictionary = new Dictionary<string, AdvancedServerParameter>();
            m_availableMods             = new BindingList<Mod>();
            m_enabledMods               = new BindingList<Mod>();
            m_serverConfig              = new ServerConfiguration();
        }

        public static ConfigurationManager GetInstance()
        {
            m_instance ??= new();
            return m_instance;
        }

        public ServerConfiguration GetServerConfiguration()
        {
            return m_serverConfig;
        }

        public BindingList<Mod> GetAvailableMods()
        {
            return m_availableMods;
        }

        public BindingList<Mod> GetEnabledMods()
        { 
            return m_enabledMods;
        }

        public Dictionary<string, ServerParameter> GetServerParametersDictionary()
        {
            return m_serverParamsDictionary;
        }

        public Dictionary<string, AdvancedServerParameter> GetAdvancedServerParametersDictionary()
        {
            return m_advServerParamsDictionary;
        }

        /// <summary>
        /// This method populates the GUI controls from the server config model
        /// </summary>
        /// <param name="input"></param>
        public void PopulateServerConfiguration(string input)
        {
            // First move mods back to available mods so we don't lose them
            for (int i = 0; i < m_enabledMods.Count; i++)
            {
                MoveMod(m_enabledMods[i], m_enabledMods, m_availableMods);
            }

            try
            {
                Log.Debug("ConfigurationManager - Populating Server Configuration from {input}", input);
                m_serverConfig.SetServerConfigurationFromJson(input);

                m_serverParamsDictionary["bindAddress"].ParameterValue   = m_serverConfig.root.bindAddress!;
                m_serverParamsDictionary["bindPort"].ParameterValue      = m_serverConfig.root.bindPort;
                m_serverParamsDictionary["publicAddress"].ParameterValue = m_serverConfig.root.publicAddress!;
                m_serverParamsDictionary["publicPort"].ParameterValue    = m_serverConfig.root.publicPort;

                m_serverParamsDictionary["address"].ParameterValue = m_serverConfig.root.a2s.address;
                m_serverParamsDictionary["port"].ParameterValue    = m_serverConfig.root.a2s.port;

                if (m_serverConfig.root.rcon == null)
                {
                    Log.Debug("ConfigurationManager - Rcon fields were absent from config, using defaults");
                    m_serverConfig.root.rcon = Rcon.Default;
                }
                
                m_serverParamsDictionary["rconAddress"].ParameterValue    = m_serverConfig.root.rcon.address;
                m_serverParamsDictionary["rconPort"].ParameterValue       = m_serverConfig.root.rcon.port == 0 ? 
                    Rcon.Default.port : m_serverConfig.root.rcon.port;
                m_serverParamsDictionary["rconPassword"].ParameterValue   = m_serverConfig.root.rcon.password;
                m_serverParamsDictionary["rconMaxClients"].ParameterValue = m_serverConfig.root.rcon.maxClients == 0 ? 
                    Rcon.Default.maxClients : m_serverConfig.root.rcon.maxClients;
                m_serverParamsDictionary["rconWhitelist"].ParameterValue  = m_serverConfig.root.rcon.whitelist;
                m_serverParamsDictionary["rconBlacklist"].ParameterValue  = m_serverConfig.root.rcon.blacklist;
                ((ServerParameterSelect) m_serverParamsDictionary["rconPermission"])
                    .ParameterValueSelection(Utilities.RconPermissionToString(m_serverConfig.root.rcon.permission));

                m_serverParamsDictionary["passwordAdmin"].ParameterValue = m_serverConfig.root.game.passwordAdmin!;
                m_serverParamsDictionary["name"].ParameterValue          = m_serverConfig.root.game.name!;
                m_serverParamsDictionary["password"].ParameterValue      = m_serverConfig.root.game.password!;
                m_serverParamsDictionary["admins"].ParameterValue        = m_serverConfig.root.game.admins;

                ScenarioIdEventArgs scenarioId = new(m_serverConfig.root.game.scenarioId);
                OnUpdateScenarioIdFromLoadedConfig(scenarioId);

                m_serverParamsDictionary["maxPlayers"].ParameterValue    = m_serverConfig.root.game.maxPlayers;
                m_serverParamsDictionary["visible"].ParameterValue       = m_serverConfig.root.game.visible;
                m_serverParamsDictionary["crossPlatform"].ParameterValue = m_serverConfig.root.game.crossPlatform;
                m_serverConfig.root.game.supportedPlatforms              = Utilities.GetSupportedPlatforms(m_serverConfig.root.game.crossPlatform);

                m_serverParamsDictionary["serverMaxViewDistance"].ParameterValue      = m_serverConfig.root.game.gameProperties.serverMaxViewDistance;
                m_serverParamsDictionary["serverMinGrassDistance"].ParameterValue     = m_serverConfig.root.game.gameProperties.serverMinGrassDistance;
                m_serverParamsDictionary["networkViewDistance"].ParameterValue        = m_serverConfig.root.game.gameProperties.networkViewDistance;
                m_serverParamsDictionary["disableThirdPerson"].ParameterValue         = m_serverConfig.root.game.gameProperties.disableThirdPerson;
                m_serverParamsDictionary["fastValidation"].ParameterValue             = m_serverConfig.root.game.gameProperties.fastValidation;
                m_serverParamsDictionary["battlEye"].ParameterValue                   = m_serverConfig.root.game.gameProperties.battlEye;
                m_serverParamsDictionary["VONCanTransmitCrossFaction"].ParameterValue = m_serverConfig.root.game.gameProperties.vonCanTransmitCrossFaction;
                m_serverParamsDictionary["VONDisableUI"].ParameterValue               = m_serverConfig.root.game.gameProperties.vonDisableUI;
                m_serverParamsDictionary["VONDisableDirectSpeechUI"].ParameterValue   = m_serverConfig.root.game.gameProperties.vonDisableDirectSpeechUI;

                m_serverParamsDictionary["lobbyPlayerSynchronise"].ParameterValue  = m_serverConfig.root.operating.lobbyPlayerSynchronise;
                m_serverParamsDictionary["playerSaveTime"].ParameterValue          = m_serverConfig.root.operating.playerSaveTime;
                m_serverParamsDictionary["aiLimit"].ParameterValue                 = m_serverConfig.root.operating.aiLimit;
                m_serverParamsDictionary["disableCrashReporter"].ParameterValue    = m_serverConfig.root.operating.disableCrashReporter;

                // If there is either a valid empty list or list with elements loaded in, assume disableNavmeshStreaming is enabled
                bool disableNavmeshStreaming = m_serverConfig.root.operating.disableNavmeshStreaming != null;

                if (disableNavmeshStreaming)
                {
                    Log.Debug("ConfigurationManager - disableNavmeshStreaming field was found in the config, enabling it");
                    m_serverParamsDictionary["disableNavmeshStreaming"].ParameterValue = m_serverConfig.root.operating.disableNavmeshStreaming!;
                }
                m_serverParamsDictionary["toggleDisableNavmeshStreaming"].ParameterValue = disableNavmeshStreaming;

                m_serverParamsDictionary["disableServerShutdown"].ParameterValue  = m_serverConfig.root.operating.disableServerShutdown;
                m_serverParamsDictionary["slotReservationTimeout"].ParameterValue = m_serverConfig.root.operating.slotReservationTimeout;
                m_serverParamsDictionary["disableAI"].ParameterValue              = m_serverConfig.root.operating.disableAI;

                foreach (Mod m in m_serverConfig.root.game.mods)
                {
                    m_enabledMods.Add(m);
                    if (m_availableMods.Contains(m))
                    {
                        m_availableMods.Remove(m);
                    }
                }

                AlphabetiseModLists();

                FileIOManager.GetInstance().WriteModsDatabase();
            }
            catch (Exception e)
            {
                Log.Debug(e, "ConfigurationManager - Failed to read config file");
                Utilities.DisplayErrorMessage($"An error occurred while attempting to load the configuration file.\r\n" +
                $"It may have been created for an earlier version.\r\n" +
                $"The configuration has not been loaded.", e.Message);
            }
        }

        /// <summary>
        /// This method is used to populate the server config model from the GUI controls.
        /// </summary>
        public void CreateConfiguration()
        {
            Log.Debug("ConfigurationManager - Creating server configuration from GUI controls state...");
            m_serverConfig.root.bindAddress   = (string)m_serverParamsDictionary["bindAddress"].ParameterValue;
            m_serverConfig.root.bindPort      = Convert.ToInt32(m_serverParamsDictionary["bindPort"].ParameterValue);
            m_serverConfig.root.publicAddress = (string)m_serverParamsDictionary["publicAddress"].ParameterValue;
            m_serverConfig.root.publicPort    = Convert.ToInt32(m_serverParamsDictionary["publicPort"].ParameterValue);

            m_serverConfig.root.a2s.port    = Convert.ToInt32(m_serverParamsDictionary["port"].ParameterValue);
            m_serverConfig.root.a2s.address = (string)m_serverParamsDictionary["address"].ParameterValue;

            // Determine whether Rcon should be included in the server configuration
            GetServerConfiguration().rconEnabled = (bool) m_serverParamsDictionary["rconEnabled"].ParameterValue;
            m_serverConfig.root.rcon             = Rcon.Default;
            m_serverConfig.root.rcon.address     = (string) m_serverParamsDictionary["rconAddress"].ParameterValue;
            m_serverConfig.root.rcon.port        = Convert.ToInt32(m_serverParamsDictionary["rconPort"].ParameterValue);
            m_serverConfig.root.rcon.password    = (string) m_serverParamsDictionary["rconPassword"].ParameterValue;
            m_serverConfig.root.rcon.permission  = Utilities.StringToEnum<RconPermission>((string) m_serverParamsDictionary["rconPermission"].ParameterValue);
            m_serverConfig.root.rcon.maxClients  = Convert.ToInt32(m_serverParamsDictionary["rconMaxClients"].ParameterValue);
            m_serverConfig.root.rcon.whitelist   = (string[]) m_serverParamsDictionary["rconWhitelist"].ParameterValue;
            m_serverConfig.root.rcon.blacklist   = (string[]) m_serverParamsDictionary["rconBlacklist"].ParameterValue;

            if (!GetServerConfiguration().rconEnabled)
            {
                Log.Debug("ConfigurationManager - Rcon is not enabled, it will not be included in the resultant config file");
                // Setting rcon to null will stop it from being added as rcon: null in the Json output
                m_serverConfig.root.rcon = null;
            }

            m_serverConfig.root.game.passwordAdmin      = (string)m_serverParamsDictionary["passwordAdmin"].ParameterValue;
            m_serverConfig.root.game.name               = (string)m_serverParamsDictionary["name"].ParameterValue;
            m_serverConfig.root.game.password           = (string)m_serverParamsDictionary["password"].ParameterValue;
            m_serverConfig.root.game.maxPlayers         = Convert.ToInt32(m_serverParamsDictionary["maxPlayers"].ParameterValue);
            m_serverConfig.root.game.visible            = (bool)m_serverParamsDictionary["visible"].ParameterValue;
            m_serverConfig.root.game.crossPlatform      = (bool)m_serverParamsDictionary["crossPlatform"].ParameterValue;
            m_serverConfig.root.game.mods               = m_enabledMods.ToArray();
            m_serverConfig.root.game.supportedPlatforms = Utilities.GetSupportedPlatforms(m_serverConfig.root.game.crossPlatform);
            m_serverConfig.root.game.admins             = (string[]) m_serverParamsDictionary["admins"].ParameterValue;
            // m_serverConfig.root.game.scenarioId - Don't need to set scenarioId as its set directly from the Scenario Selector Form

            m_serverConfig.root.game.gameProperties.serverMaxViewDistance      = Convert.ToInt32(m_serverParamsDictionary["serverMaxViewDistance"].ParameterValue);
            m_serverConfig.root.game.gameProperties.serverMinGrassDistance     = Convert.ToInt32(m_serverParamsDictionary["serverMinGrassDistance"].ParameterValue);
            m_serverConfig.root.game.gameProperties.networkViewDistance        = Convert.ToInt32(m_serverParamsDictionary["networkViewDistance"].ParameterValue);
            m_serverConfig.root.game.gameProperties.disableThirdPerson         = (bool)m_serverParamsDictionary["disableThirdPerson"].ParameterValue;
            m_serverConfig.root.game.gameProperties.fastValidation             = (bool)m_serverParamsDictionary["fastValidation"].ParameterValue;
            m_serverConfig.root.game.gameProperties.battlEye                   = (bool)m_serverParamsDictionary["battlEye"].ParameterValue;
            m_serverConfig.root.game.gameProperties.vonDisableUI               = (bool)m_serverParamsDictionary["VONDisableUI"].ParameterValue;
            m_serverConfig.root.game.gameProperties.vonDisableDirectSpeechUI   = (bool)m_serverParamsDictionary["VONDisableDirectSpeechUI"].ParameterValue;
            m_serverConfig.root.game.gameProperties.vonCanTransmitCrossFaction = (bool)m_serverParamsDictionary["VONCanTransmitCrossFaction"].ParameterValue;
            // m_serverConfig.root.game.gameProperties.missionHeader - Don't need to set missionHeader as its set directly from the Edit Mission Header Form

            m_serverConfig.root.operating.lobbyPlayerSynchronise  = (bool)m_serverParamsDictionary["lobbyPlayerSynchronise"].ParameterValue;
            m_serverConfig.root.operating.playerSaveTime          = Convert.ToInt32(m_serverParamsDictionary["playerSaveTime"].ParameterValue);
            m_serverConfig.root.operating.aiLimit                 = Convert.ToInt32(m_serverParamsDictionary["aiLimit"].ParameterValue);
            m_serverConfig.root.operating.slotReservationTimeout  = Convert.ToInt32(m_serverParamsDictionary["slotReservationTimeout"].ParameterValue);

            // Determine whether Navmesh streaming is to be disabled
            GetServerConfiguration().toggleDisableNavmeshStreaming = (bool) m_serverParamsDictionary["toggleDisableNavmeshStreaming"].ParameterValue;

            if (GetServerConfiguration().toggleDisableNavmeshStreaming)
            {
                Log.Debug("ConfigurationManager - Disable Navmesh Streaming is enabled");
                m_serverConfig.root.operating.disableNavmeshStreaming = (string[]) m_serverParamsDictionary["disableNavmeshStreaming"].ParameterValue;
            }
            m_serverConfig.root.operating.disableServerShutdown   = (bool)m_serverParamsDictionary["disableServerShutdown"].ParameterValue;
            m_serverConfig.root.operating.disableCrashReporter    = (bool)m_serverParamsDictionary["disableCrashReporter"].ParameterValue;
            m_serverConfig.root.operating.disableAI               = (bool)m_serverParamsDictionary["disableAI"].ParameterValue;
        }

        /// <summary>
        /// Convenience method for alphabetising both the Available mods and Enable mods lists
        /// </summary>
        public void AlphabetiseModLists()
        {
            if (!Utilities.IsSorted(m_availableMods))
            {
                var tempAvailableMods = Utilities.AlphabetiseModList(m_availableMods);
                m_availableMods.Clear();
                foreach (Mod mod in tempAvailableMods) { m_availableMods.Add(mod); }
            }
            
            if (!Utilities.IsSorted(m_enabledMods))
            {
                var tempEnabledMods = Utilities.AlphabetiseModList(m_enabledMods);
                m_enabledMods.Clear();
                foreach (Mod mod in tempEnabledMods) { m_enabledMods.Add(mod); }
            }
        }

        /// <summary>
        /// Utility method for moving a mod from one list to another
        /// </summary>
        /// <param name="m">Mod to move</param>
        /// <param name="from">List to move mod from</param>
        /// <param name="to">List to move mod to</param>
        public static void MoveMod(Mod m, BindingList<Mod> from, BindingList<Mod> to)
        {
            if (to.Contains(m))
            {
                from.Remove(m);
            } else
            {
                to.Add(m);
                from.Remove(m);
            }
        }

        /// <summary>
        /// Sender for the 'UpdateScenarioIdFromLoadedConfig' Event
        /// </summary>
        /// <param name="e">Arguments to pass to the GUI to inform it that it needs to update the Scenario ID</param>
        protected virtual void OnUpdateScenarioIdFromLoadedConfig(ScenarioIdEventArgs e)
        {
            UpdateScenarioIdFromLoadedConfigEvent?.Invoke(this, e);
        }
    }
}
