/******************************************************************************
 * File Name:    ModDependency.cs
 * Project:      Longbow
 * Description:  This file contains the ModDependency class, representing a
 *               dependency relationship between two mods (e.g., "ACE Core depends on CBA")
 *
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  public class ModDependency
  {
    /// <summary>
    /// The mod ID of the required dependency (e.g., "ace_core", "cba")
    /// </summary>
    public string ModId { get; set; }

    /// <summary>
    /// Human-readable name of the dependency (e.g., "CBA - Community Based Addons")
    /// </summary>
    public string ModName { get; set; }

    /// <summary>
    /// Minimum required version of the dependency (e.g., "3.15.0").
    /// Null or "any" means no minimum version requirement.
    /// </summary>
    public string MinVersion { get; set; }

    /// <summary>
    /// Maximum allowed version of the dependency (e.g., "4.0.0").
    /// Null or unbounded means no maximum version requirement.
    /// </summary>
    public string MaxVersion { get; set; }

    /// <summary>
    /// Indicates whether this is a soft dependency (optional) or hard dependency (required).
    /// If true, loading without this dependency may work but with reduced functionality.
    /// If false, the mod will not load without this dependency.
    /// </summary>
    public bool IsOptional { get; set; }

    /// <summary>
    /// Default constructor for JSON deserialization
    /// </summary>
    public ModDependency() { }

    /// <summary>
    /// Constructor for creating a required dependency with any version
    /// </summary>
    /// <param name="modId">The dependency mod ID</param>
    /// <param name="modName">The dependency mod name</param>
    public ModDependency(string modId, string modName)
      : this(modId, modName, null, null, false) { }

    /// <summary>
    /// Constructor for creating a dependency with version constraints
    /// </summary>
    /// <param name="modId">The dependency mod ID</param>
    /// <param name="modName">The dependency mod name</param>
    /// <param name="minVersion">Minimum required version (null = any)</param>
    /// <param name="maxVersion">Maximum allowed version (null = unbounded)</param>
    /// <param name="isOptional">Whether this is a soft dependency</param>
    public ModDependency(string modId, string modName, string minVersion, string maxVersion, bool isOptional = false)
    {
      this.ModId = modId;
      this.ModName = modName;
      this.MinVersion = minVersion;
      this.MaxVersion = maxVersion;
      this.IsOptional = isOptional;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public ModDependency(ModDependency other)
    {
      this.ModId = other.ModId;
      this.ModName = other.ModName;
      this.MinVersion = other.MinVersion;
      this.MaxVersion = other.MaxVersion;
      this.IsOptional = other.IsOptional;
    }

    public override string ToString()
    {
      string versionStr = "";
      if (MinVersion != null || MaxVersion != null)
      {
        versionStr = $" [{MinVersion ?? "any"} - {MaxVersion ?? "any"}]";
      }
      string optionalStr = IsOptional ? " (optional)" : "";
      return $"{ModName} ({ModId}){versionStr}{optionalStr}";
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
      if (obj.GetType() == typeof(ModDependency))
      {
        ModDependency other = (ModDependency)obj;
        return ModId.Equals(other.ModId) &&
               ModName.Equals(other.ModName) &&
               (MinVersion ?? "").Equals(other.MinVersion ?? "") &&
               (MaxVersion ?? "").Equals(other.MaxVersion ?? "") &&
               IsOptional == other.IsOptional;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return ModId.GetHashCode() + ModName.GetHashCode();
    }
  }
}
