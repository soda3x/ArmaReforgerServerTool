using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReforgerServerApp.Models
{
    public class LaunchArgument
    {
        private readonly Dictionary<string, string> m_underlyingDict;
        private string m_key;

        public LaunchArgument(string key, string val)
        {
            if (key.Trim().Length < 1)
            {
                throw new Exception("Key length is invalid");
            }

            m_key = key.Trim();
            m_underlyingDict = new();

            if (val.Trim().Length > 0)
            {
                m_underlyingDict[m_key] = val.Trim();
            }
            else
            {
                // In some circumstances, the launch arg has no value, making this valid
                m_underlyingDict[m_key] = string.Empty;
            }
        }

        public string Get()
        {
            return m_underlyingDict[m_key];
        }

        public void Set(string val)
        {
            m_underlyingDict[m_key] = val;
        }

        override public string ToString()
        {
            return $"-{m_key} {m_underlyingDict[m_key]}".Trim();
        }
    }
    public struct LaunchArguments
    {
        public LaunchArgument config;
        public LaunchArgument profile;
        public LaunchArgument logStats;
        public LaunchArgument maxFPS;
        public LaunchArgument bindPort;
        public LaunchArgument nds;
        public LaunchArgument nwkResolution;
        public LaunchArgument staggeringBudget;
        public LaunchArgument streamingBudget;
        public LaunchArgument streamsDelta;
        public LaunchArgument loadSessionSave;
        public LaunchArgument logLevel;
    }
}
