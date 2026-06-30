/******************************************************************************
 * File Name:    ValidationResult.cs
 * Project:      Longbow
 * Description:  This file contains the ValidationResult class, representing the
 *               output of a mod configuration validation (errors, warnings, sorted mods, etc.)
 *
 * Author:       Bradley Newman
 ******************************************************************************/

namespace ReforgerServerApp
{
  public class ValidationResult
  {
    /// <summary>
    /// True if validation passed (no FATAL errors), false otherwise
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of all validation errors found (FATAL and WARNING levels)
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

    /// <summary>
    /// List of non-fatal warnings (convenience grouping of WARNING/INFO level errors)
    /// </summary>
    public List<ValidationError> Warnings { get; set; } = new List<ValidationError>();

    /// <summary>
    /// The mods in correct load order (dependencies before dependents).
    /// Only populated if validation passed (IsValid == true).
    /// </summary>
    public List<Mod> SortedMods { get; set; } = new List<Mod>();

    /// <summary>
    /// Mods that were automatically added to satisfy missing dependencies.
    /// Empty if no auto-additions were made.
    /// </summary>
    public List<Mod> AutoAddedMods { get; set; } = new List<Mod>();

    /// <summary>
    /// Timestamp of when this validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ValidationResult()
    {
      this.ValidatedAt = DateTime.UtcNow;
      this.IsValid = true;
    }

    /// <summary>
    /// Gets all fatal errors (errors that caused validation to fail)
    /// </summary>
    public List<ValidationError> GetFatalErrors()
    {
      return Errors.Where(e => e.Level == ValidationError.Severity.FATAL).ToList();
    }

    /// <summary>
    /// Gets all warning-level errors
    /// </summary>
    public List<ValidationError> GetWarnings()
    {
      return Errors.Where(e => e.Level == ValidationError.Severity.WARNING).ToList();
    }

    /// <summary>
    /// Gets all info-level errors
    /// </summary>
    public List<ValidationError> GetInfoMessages()
    {
      return Errors.Where(e => e.Level == ValidationError.Severity.INFO).ToList();
    }

    public override string ToString()
    {
      string result = $"Validation Result: {(IsValid ? "PASSED" : "FAILED")}\n";
      result += $"  Validated at: {ValidatedAt:yyyy-MM-dd HH:mm:ss}\n";
      result += $"  Total errors: {Errors.Count}\n";

      var fatal = GetFatalErrors();
      var warnings = GetWarnings();
      var infos = GetInfoMessages();

      if (fatal.Any())
        result += $"  Fatal errors: {fatal.Count}\n";
      if (warnings.Any())
        result += $"  Warnings: {warnings.Count}\n";
      if (infos.Any())
        result += $"  Info messages: {infos.Count}\n";

      result += $"  Mods in load order: {SortedMods.Count}\n";
      if (AutoAddedMods.Any())
        result += $"  Auto-added mods: {AutoAddedMods.Count}\n";

      return result;
    }
  }
}
