/******************************************************************************
 * File Name:    Mod.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  This file contains the Mod class, a model representing the
 *               Mod structure in the Arma Reforger server configuration
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using HtmlAgilityPack;
using ReforgerServerApp.WinForms.Managers;
using ReforgerServerApp.WinForms.Utils;
using Serilog;
using System.Text.Json.Serialization;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ReforgerServerApp.WinForms
{
  public class Mod
  {
    public string modId { get; set; }
    public string name { get; set; }

    [JsonConverter(typeof(JsonUtils.ConditionalFieldConverter))]
    public string version { get; set; }
    public bool required { get; set; }

    private const string LATEST_MOD_VER_STR = "latest";

    public Mod() { /* Exists only for JSON deserializing */ }

    public Mod(string modId, string modName, string modVersion, bool required = false)
    {
      this.modId = modId;
      this.name = modName;
      this.version = modVersion;
      this.required = required;
    }

    public Mod(string modId, string modName, bool required = false)
    {
      this.modId = modId;
      this.name = modName;
      this.version = LATEST_MOD_VER_STR;
      this.required = required;
    }

    public Mod(Mod m)
    {
      this.modId = m.modId;
      this.name = m.name;
      this.version = m.version;
      this.required = m.required;
    }

    public string GetModID()
    {
      return modId!;
    }

    public string GetModName()
    {
      return name!;
    }

    public string GetModVersion()
    {
      return version!;
    }

    public bool IsModRequired()
    {
      return required!;
    }

    public override string ToString()
    {
      return $"{name} | {version}";
    }

    public override bool Equals(object? obj)
    {
      if (obj == this)
      {
        return true;
      }
      if (obj == null)
      {
        return false;
      }
      if (obj.GetType() == typeof(Mod))
      {
        Mod other = (Mod)obj;
        bool modSame = name!.Equals(other.name);
        modSame &= modId!.Equals(other.modId);
        if (version != null)
        {
          modSame &= version!.Equals(other.version);
        }

        return modSame;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return modId!.GetHashCode() + name!.GetHashCode();
    }

    public static List<string> GetScenariosForMod(string modId)
    {
      List<string> scenarios = new();
      if (!modId.Equals(Constants.STOCK_MOD_ID))
      {
        try
        {
          string fetchUrl = $"{ToolPropertiesManager.GetInstance().GetToolProperties().armaWorkshopUrl}/{modId}/scenarios";
          HtmlWeb web = new();
          HtmlDocument doc = web.Load(fetchUrl);
          const string className = "text-start";
          HtmlNodeCollection rawScenIds = doc.DocumentNode.SelectNodes($"//*[contains(@class,'{className}')]");
          if (rawScenIds != null)
          {
            foreach (HtmlNode field in rawScenIds)
            {
              Log.Debug("Mod - Discovered scenario \"{scenario}\" for Mod \"{mod}\"", field.InnerText, modId);
              scenarios.Add(field.InnerText);
            }
          }
          else
          {
            Log.Debug("Mod - Failed to fetch any scenario ids for \"{mod}\". It may not have any.", modId);
          }
        }
        catch (Exception ex)
        {
          Utilities.DisplayErrorMessage("Unable to fetch Scenario IDs from Arma Reforger Workshop, please check your internet connection.", ex.Message);
        }
      }
      return scenarios;
    }
  }
}
