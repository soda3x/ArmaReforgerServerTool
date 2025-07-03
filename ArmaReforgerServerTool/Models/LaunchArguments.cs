/******************************************************************************
 * File Name:    LaunchArguments.cs
 * Project:      Longbow
 * Description:  This file contains the LaunchArgument class, a model 
 *               representing a launch argument for the Arma Reforger Server,
 *               it also holds a structure containing all known Launch 
 *               Arguments
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp.Models
{
  public class LaunchArgument
  {
    private readonly Dictionary<string, string> m_underlyingDict;
    private readonly string m_key;

    /// <summary>
    /// Construct a launch argument with a key and value (e.g. -bindPort 2001)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <exception cref="Exception"></exception>
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
        throw new Exception("Value is empty, use the single argument constructor to create switch arguments");
      }
    }

    /// <summary>
    /// Construct a 'switch' LaunchArgument with only a key (e.g. -nobackend)
    /// </summary>
    /// <param name="key"></param>
    /// <exception cref="Exception"></exception>
    public LaunchArgument(string key)
    {
      if (key.Trim().Length < 1)
      {
        throw new Exception("Key length is invalid");
      }

      m_key = key.Trim();
      m_underlyingDict = new()
      {
        [m_key] = string.Empty // the launch arg has no value (switch)
      };
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
    public LaunchArgument addonsDir;
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
    public LaunchArgument autoReload;
    public LaunchArgument rplTimeoutMs;
  }
}
