/******************************************************************************
 * File Name:    Program.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  Entry point for the application
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

using ReforgerServerApp.Managers;
using ReforgerServerApp.Models;
using ReforgerServerApp.Utils;
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
        Utilities.DisplayErrorMessage("Failed to configure logger.", $"Logger configuration failed which prevented the program from launching.\n{ex.Message}");
        Application.Exit(); // We cannot continue, exit gracefully
        return;
      }

      try
      {
        Log.Information("Arma Reforger Dedicated Server Tool starting...");
        ApplicationConfiguration.Initialize();
        Application.Run(new Main());
      }
      catch (Exception ex)
      {
        Utilities.DisplayErrorMessage("Application failed.", $"An uncaught exception occurred.\n\n" +
            $"\"{ex.Message}\"");
        Log.Fatal(ex, "Application failed.");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }
  }
}
