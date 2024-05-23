/******************************************************************************
 * File Name:    Program.cs
 * Project:      Arma Reforger Dedicated Server Tool for Windows
 * Description:  Entry point for the application
 * 
 * Author:       Bradley Newman
 ******************************************************************************/

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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }
    }
}