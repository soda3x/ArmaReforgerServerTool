using ReforgerServerApp.Managers;
using ReforgerServerApp.Utils;
using System.Text.Json;

namespace ReforgerServerApp
{
    /// <summary>
    /// This class manages the manipulation of the Server Configuration
    /// </summary>
    internal class ConfigurationManager
    {
        private static ConfigurationManager?                 m_instance;
        private readonly Dictionary<string, ServerParameter> m_serverParamsDictionary;
        private ServerConfiguration                          m_serverConfig;
        private List<Mod>                                    m_availableMods;
        private List<Mod>                                    m_enabledMods;

        private ConfigurationManager()
        {
            m_serverParamsDictionary = new Dictionary<string, ServerParameter>();
            m_availableMods          = new List<Mod>();
            m_enabledMods            = new List<Mod>();
            m_serverConfig           = new ServerConfiguration();
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

        public List<Mod> GetAvailableMods()
        {
            return m_availableMods;
        }

        public List<Mod> GetEnabledMods()
        { 
            return m_enabledMods;
        }

        public Dictionary<string, ServerParameter> GetServerParametersDictionary()
        {
            return m_serverParamsDictionary;
        }

        /// <summary>
        /// This method populates the GUI controls from the server config model
        /// </summary>
        /// <param name="input"></param>
        public void PopulateServerConfiguration(string input)
        {
            try
            {
                m_serverConfig.SetServerConfigurationFromJson(input);

                m_serverParamsDictionary["bindAddress"].ParameterValue   = m_serverConfig.root.bindAddress!;
                m_serverParamsDictionary["bindPort"].ParameterValue      = m_serverConfig.root.bindPort;
                m_serverParamsDictionary["publicAddress"].ParameterValue = m_serverConfig.root.publicAddress!;
                m_serverParamsDictionary["publicPort"].ParameterValue    = m_serverConfig.root.publicPort;

                m_serverParamsDictionary["address"].ParameterValue = m_serverConfig.root.a2s.address;
                m_serverParamsDictionary["port"].ParameterValue    = m_serverConfig.root.a2s.port;

                m_serverParamsDictionary["passwordAdmin"].ParameterValue = m_serverConfig.root.game.passwordAdmin!;
                m_serverParamsDictionary["name"].ParameterValue          = m_serverConfig.root.game.name!;
                m_serverParamsDictionary["password"].ParameterValue      = m_serverConfig.root.game.password!;
                m_serverParamsDictionary["scenarioId"].ParameterValue    = m_serverConfig.root.game.scenarioId!;
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
                m_serverParamsDictionary["disableNavmeshStreaming"].ParameterValue = m_serverConfig.root.operating.disableNavmeshStreaming;
                m_serverParamsDictionary["disableServerShutdown"].ParameterValue   = m_serverConfig.root.operating.disableServerShutdown;
                m_serverParamsDictionary["slotReservationTimeout"].ParameterValue  = m_serverConfig.root.operating.slotReservationTimeout;
                m_serverParamsDictionary["disableAI"].ParameterValue               = m_serverConfig.root.operating.disableAI;

                m_enabledMods.Clear();

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
                MessageBox.Show($"An error occurred while attempting to load the configuration file.\r\n" +
                    $"It may have been created for an earlier version.\r\n" +
                    $"The configuration has not been loaded.\r\n\r\n" +
                    $"Detail: {e.Message}\r\n\r\n" +
                    $"Include the detail above in your bug reports.",
                    Constants.ERROR_MESSAGEBOX_TITLE_STR);
            }
        }

        /// <summary>
        /// This method is used to populate the server config model from the GUI controls.
        /// </summary>
        public void CreateConfiguration()
        {
            m_serverConfig.root.bindAddress   = (string)m_serverParamsDictionary["bindAddress"].ParameterValue;
            m_serverConfig.root.bindPort      = Convert.ToInt32(m_serverParamsDictionary["bindPort"].ParameterValue);
            m_serverConfig.root.publicAddress = (string)m_serverParamsDictionary["publicAddress"].ParameterValue;
            m_serverConfig.root.publicPort    = Convert.ToInt32(m_serverParamsDictionary["publicPort"].ParameterValue);

            m_serverConfig.root.a2s.port    = Convert.ToInt32(m_serverParamsDictionary["port"].ParameterValue);
            m_serverConfig.root.a2s.address = (string)m_serverParamsDictionary["address"].ParameterValue;

            m_serverConfig.root.game.passwordAdmin      = (string)m_serverParamsDictionary["passwordAdmin"].ParameterValue;
            m_serverConfig.root.game.name               = (string)m_serverParamsDictionary["name"].ParameterValue;
            m_serverConfig.root.game.password           = (string)m_serverParamsDictionary["password"].ParameterValue;
            m_serverConfig.root.game.maxPlayers         = Convert.ToInt32(m_serverParamsDictionary["maxPlayers"].ParameterValue);
            m_serverConfig.root.game.visible            = (bool)m_serverParamsDictionary["visible"].ParameterValue;
            m_serverConfig.root.game.crossPlatform      = (bool)m_serverParamsDictionary["crossPlatform"].ParameterValue;
            m_serverConfig.root.game.mods               = m_enabledMods.ToArray();
            m_serverConfig.root.game.supportedPlatforms = Utilities.GetSupportedPlatforms(m_serverConfig.root.game.crossPlatform);
            // m_serverConfig.root.game.admins     - Don't need to set admins as its set directly from the Edit Admin list Form
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
            m_serverConfig.root.operating.disableNavmeshStreaming = (bool)m_serverParamsDictionary["disableNavmeshStreaming"].ParameterValue;
            m_serverConfig.root.operating.disableServerShutdown   = (bool)m_serverParamsDictionary["disableServerShutdown"].ParameterValue;
            m_serverConfig.root.operating.disableCrashReporter    = (bool)m_serverParamsDictionary["disableCrashReporter"].ParameterValue;
            m_serverConfig.root.operating.disableAI               = (bool)m_serverParamsDictionary["disableAI"].ParameterValue;
        }

        /// <summary>
        /// Convenience method for alphabetising both the Available mods and Enable mods lists
        /// </summary>
        public void AlphabetiseModLists()
        {
            Utilities.AlphabetiseModList(ref m_availableMods);
            Utilities.AlphabetiseModList(ref m_enabledMods);
        }
    }
}
