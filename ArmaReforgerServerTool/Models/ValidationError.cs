/******************************************************************************
 * File Name:    ValidationError.cs
 * Project:      Longbow
 * Description:  This file contains the ValidationError class, representing a
 *               validation issue detected during mod configuration validation
 *               (missing dependencies, version mismatches, circular deps, etc.)
 *
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  public class ValidationError
  {
    /// <summary>
    /// Enumeration of validation error types
    /// </summary>
    public enum ErrorType
    {
      /// <summary>Mod X requires Y, but Y is not in the enabled mods list</summary>
      MissingDependency,

      /// <summary>Mod X needs Y v1.5+, but found v1.2</summary>
      VersionMismatch,

      /// <summary>Circular dependency detected (X -> Y -> X)</summary>
      CircularDependency,

      /// <summary>Mod A conflicts with Mod B</summary>
      ConflictingMods,

      /// <summary>Mod requires game v1.2+, but server is running v1.0</summary>
      IncompatibleGameVersion
    }

    /// <summary>
    /// Enumeration of error severity levels
    /// </summary>
    public enum Severity
    {
      /// <summary>Critical error that prevents loading - makes validation fail</summary>
      FATAL,

      /// <summary>Non-critical issue that may cause reduced functionality</summary>
      WARNING,

      /// <summary>Informational message (e.g., optional dependency not found)</summary>
      INFO
    }

    /// <summary>
    /// The type of validation error
    /// </summary>
    public ErrorType Type { get; set; }

    /// <summary>
    /// The mod ID that has the problem (e.g., "ace_core")
    /// </summary>
    public string ModId { get; set; }

    /// <summary>
    /// The mod name for display purposes (e.g., "ACE3 Core")
    /// </summary>
    public string ModName { get; set; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The severity level of this error
    /// </summary>
    public Severity Level { get; set; }

    /// <summary>
    /// Optional: the related mod ID (for conflicts, missing deps, etc.)
    /// </summary>
    public string RelatedModId { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ValidationError() { }

    /// <summary>
    /// Constructor for creating a validation error
    /// </summary>
    /// <param name="type">The error type</param>
    /// <param name="modId">The mod ID with the issue</param>
    /// <param name="modName">The mod name</param>
    /// <param name="message">Human-readable message</param>
    /// <param name="severity">Error severity level</param>
    /// <param name="relatedModId">Optional related mod ID</param>
    public ValidationError(ErrorType type, string modId, string modName, string message, Severity severity, string relatedModId = null)
    {
      this.Type = type;
      this.ModId = modId;
      this.ModName = modName;
      this.Message = message;
      this.Level = severity;
      this.RelatedModId = relatedModId;
    }

    public override string ToString()
    {
      return $"[{Level}] {Type}: {ModName} ({ModId}) - {Message}";
    }

    public override bool Equals(object? obj)
    {
      if (obj == this)
        return true;

      if (obj == null || obj.GetType() != typeof(ValidationError))
        return false;

      ValidationError other = (ValidationError)obj;
      return Type == other.Type &&
             ModId.Equals(other.ModId) &&
             Message.Equals(other.Message) &&
             Level == other.Level;
    }

    public override int GetHashCode()
    {
      return ModId.GetHashCode() ^ Type.GetHashCode() ^ Level.GetHashCode();
    }
  }
}
