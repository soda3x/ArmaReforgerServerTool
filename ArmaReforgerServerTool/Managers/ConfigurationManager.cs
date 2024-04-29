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
        /// Get Config Parameter for Server Configuration file (dictionary), if it's not present, use default value for the parameter
        /// </summary>
        /// <param name="paramKey"></param>
        /// <returns>Value to set config parameter to (likely either an int, boolean or string)</returns>
        private object? GetConfigParameterOrDefault(string paramKey)
        {
            try
            {
                return m_serverParamsDictionary[paramKey];
            }
            catch (Exception)
            {
                foreach (ServerParameter sp in m_serverParamsDictionary.Values)
                {
                    if (sp.ParameterName == paramKey)
                    {
                        return sp.ParameterValue;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// This method populates the GUI controls from an imported comma-separated settings file.
        /// This method also calls the alphabetise list methods and will write the imported mods to the mod_database.txt file.`
        /// </summary>
        /// <param name="input"></param>
        public void PopulateServerConfiguration(string input)
        {
            string[] configLines = input.Trim().Split(Environment.NewLine);
            Dictionary<string, string> configParams = new();

            foreach (string line in configLines)
            {
                string[] splitLine = line.Split("=");
                configParams.Add(splitLine[0], splitLine[1]);
            }

            try
            {
                m_serverConfig = JsonSerializer.Deserialize<ServerConfiguration>(input)!;

                m_serverParamsDictionary["bindAddress"].ParameterValue   = m_serverConfig.root.bindAddress!;
                m_serverParamsDictionary["bindPort"].ParameterValue      = m_serverConfig.root.bindPort;
                m_serverParamsDictionary["publicAddress"].ParameterValue = m_serverConfig.root.publicAddress!;
                m_serverParamsDictionary["publicPort"].ParameterValue    = m_serverConfig.root.publicPort;

                m_serverParamsDictionary["passwordAdmin"].ParameterValue = m_serverConfig.root.game.passwordAdmin!;
                m_serverParamsDictionary["name"].ParameterValue          = m_serverConfig.root.game.name!;
                m_serverParamsDictionary["password"].ParameterValue      = m_serverConfig.root.game.password!;
                m_serverParamsDictionary["scenarioId"].ParameterValue    = m_serverConfig.root.game.scenarioId!;
                m_serverParamsDictionary["maxPlayers"].ParameterValue    = m_serverConfig.root.game.maxPlayers;
                m_serverParamsDictionary["visible"].ParameterValue       = m_serverConfig.root.game.visible;
                m_serverParamsDictionary["crossPlatform"].ParameterValue = m_serverConfig.root.game.crossPlatform;
                // TODO need to add supported platforms

                m_serverParamsDictionary["serverMaxViewDistance"].ParameterValue      = m_serverConfig.root.game.gameProperties.serverMaxViewDistance;
                m_serverParamsDictionary["serverMinGrassDistance"].ParameterValue     = m_serverConfig.root.game.gameProperties.serverMinGrassDistance;
                m_serverParamsDictionary["networkViewDistance"].ParameterValue        = m_serverConfig.root.game.gameProperties.networkViewDistance;
                m_serverParamsDictionary["disableThirdPerson"].ParameterValue         = m_serverConfig.root.game.gameProperties.disableThirdPerson;
                m_serverParamsDictionary["fastValidation"].ParameterValue             = m_serverConfig.root.game.gameProperties.fastValidation;
                m_serverParamsDictionary["battlEye"].ParameterValue                   = m_serverConfig.root.game.gameProperties.battlEye;
                m_serverParamsDictionary["VONCanTransmitCrossFaction"].ParameterValue = m_serverConfig.root.game.gameProperties.vonCanTransmitCrossFaction;
                m_serverParamsDictionary["VONDisableUI"].ParameterValue               = m_serverConfig.root.game.gameProperties.vonDisableUI;
                m_serverParamsDictionary["VONDisableDirectSpeechUI"].ParameterValue   = m_serverConfig.root.game.gameProperties.vonDisableDirectSpeechUI;
                //m_serverParamsDictionary["steamQueryPort"].ParameterValue = // TODO: Need to change this to A2S

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

                Utilities.AlphabetiseModList(ref m_availableMods);
                Utilities.AlphabetiseModList(ref m_enabledMods);

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
        /// This method is used to create a server configuration object, from the GUI controls.
        /// </summary>
        /// <returns></returns>
        public void CreateConfiguration()
        {
            ServerConfiguration sc = new();

            sc.root.bindAddress   = (string)m_serverParamsDictionary["bindAddress"].ParameterValue;
            sc.root.bindPort      = Convert.ToInt32(m_serverParamsDictionary["bindPort"].ParameterValue);
            sc.root.publicAddress = (string)m_serverParamsDictionary["publicAddress"].ParameterValue;
            sc.root.publicPort    = Convert.ToInt32(m_serverParamsDictionary["publicPort"].ParameterValue);

            sc.root.game.passwordAdmin = (string)m_serverParamsDictionary["passwordAdmin"].ParameterValue;
            sc.root.game.name          = (string)m_serverParamsDictionary["name"].ParameterValue;
            sc.root.game.password      = (string)m_serverParamsDictionary["password"].ParameterValue;
            //sc.root.game.scenarioId    = loadedScenarioLabel.Text; // TODO: How do i get the loaded scenario label
            sc.root.game.maxPlayers    = Convert.ToInt32(m_serverParamsDictionary["maxPlayers"].ParameterValue);
            sc.root.game.visible       = (bool)m_serverParamsDictionary["visible"].ParameterValue;
            sc.root.game.admins        = Array.Empty<string>(); // TODO??
            sc.root.game.crossPlatform = (bool)m_serverParamsDictionary["crossPlatform"].ParameterValue;
            sc.root.game.mods          = m_enabledMods.ToArray();
            // TODO Add supported platforms

            sc.root.game.gameProperties.serverMaxViewDistance      = Convert.ToInt32(m_serverParamsDictionary["serverMaxViewDistance"].ParameterValue);
            sc.root.game.gameProperties.serverMinGrassDistance     = Convert.ToInt32(m_serverParamsDictionary["serverMinGrassDistance"].ParameterValue);
            sc.root.game.gameProperties.networkViewDistance        = Convert.ToInt32(m_serverParamsDictionary["networkViewDistance"].ParameterValue);
            sc.root.game.gameProperties.disableThirdPerson         = (bool)m_serverParamsDictionary["disableThirdPerson"].ParameterValue;
            sc.root.game.gameProperties.fastValidation             = (bool)m_serverParamsDictionary["fastValidation"].ParameterValue;
            sc.root.game.gameProperties.battlEye                   = (bool)m_serverParamsDictionary["battlEye"].ParameterValue;
            sc.root.game.gameProperties.vonDisableUI               = (bool)m_serverParamsDictionary["VONDisableUI"].ParameterValue;
            sc.root.game.gameProperties.vonDisableDirectSpeechUI   = (bool)m_serverParamsDictionary["VONDisableDirectSpeechUI"].ParameterValue;
            sc.root.game.gameProperties.missionHeader              = new Dictionary<string, string>(); // TODO
            sc.root.game.gameProperties.vonCanTransmitCrossFaction = (bool)m_serverParamsDictionary["VONCanTransmitCrossFaction"].ParameterValue;

            //.WithSteamQueryPort(Convert.ToInt32(m_serverParamsDictionary["steamQueryPort"].ParameterValue)) TODO: Change to A2S
            sc.root.operating.lobbyPlayerSynchronise  = (bool)m_serverParamsDictionary["lobbyPlayerSynchronise"].ParameterValue;
            sc.root.operating.playerSaveTime          = Convert.ToInt32(m_serverParamsDictionary["playerSaveTime"].ParameterValue);
            sc.root.operating.aiLimit                 = Convert.ToInt32(m_serverParamsDictionary["aiLimit"].ParameterValue);
            sc.root.operating.slotReservationTimeout  = Convert.ToInt32(m_serverParamsDictionary["slotReservationTimeout"].ParameterValue);
            sc.root.operating.disableNavmeshStreaming = (bool)m_serverParamsDictionary["disableNavmeshStreaming"].ParameterValue;
            sc.root.operating.disableServerShutdown   = (bool)m_serverParamsDictionary["disableServerShutdown"].ParameterValue;
            sc.root.operating.disableCrashReporter    = (bool)m_serverParamsDictionary["disableCrashReporter"].ParameterValue;
            sc.root.operating.disableAI               = (bool)m_serverParamsDictionary["disableAI"].ParameterValue;
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
