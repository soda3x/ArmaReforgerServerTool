/******************************************************************************
 * File Name:    Program.cs
 * Project:      Longbow
 * Description:  Entry point for the application
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp.Managers;
using ReforgerServerApp.Models;
using ReforgerServerApp.Utils;
using Longbow.Managers;
using Longbow.Utils;
using Serilog;

namespace ReforgerServerApp
{
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      try
      {
        Log.Logger = ToolProperties.SetMinimumLogLevel(ToolPropertiesManager.GetInstance().GetToolProperties().minimumLogLevel)
                     .WriteTo.Console()
                     .WriteTo.File(ToolPropertiesManager.GetInstance().GetToolProperties().logFile, rollingInterval: RollingInterval.Day)
                     .CreateLogger();
      }
      catch (Exception ex)
      {
        Utilities.DisplayErrorMessage(Strings.Get(Constants.PROGRAM_LOGGER_CONFIG_FAILED_STR),
            Strings.Get(Constants.PROGRAM_LOGGER_CONFIG_FAILED_DETAIL_STR, ("error", ex.Message)));
        Application.Exit(); // We cannot continue, exit gracefully
        return;
      }

      try
      {
        Log.Information("Longbow starting...");

        // Language is applied once here, at boot, and not reapplied afterwards - changing it
        // requires a restart. See i18n-system.md section 5 for why.
        LocalisationManager.GetInstance().LoadLanguage(ToolPropertiesManager.GetInstance().GetToolProperties().language);

        ApplicationConfiguration.Initialize();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.SetColorMode(SystemColorMode.System);
        Application.Run(new Main());
      }
      catch (Exception ex)
      {
        Utilities.DisplayErrorMessage(Strings.Get(Constants.PROGRAM_APPLICATION_FAILED_STR),
            Strings.Get(Constants.PROGRAM_APPLICATION_FAILED_DETAIL_STR, ("error", ex.Message)));
        Log.Fatal(ex, "Application failed.");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }
  }
}
