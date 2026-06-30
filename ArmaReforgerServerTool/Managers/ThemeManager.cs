using ReforgerServerApp;
using System;
using System.Collections.Generic;
using System.Text;
using WinForms.Fluent;

namespace Longbow.Managers
{
  internal class ThemeManager
  {
    private static ThemeManager? m_instance;

    private ThemeManager()
    {
      // Disable no-args constructor
    }

    public static ThemeManager GetInstance()
    {
      m_instance ??= new ThemeManager();
      return m_instance;
    }

    public void ConfigureTheme(Form f)
    {
      if (Application.SystemColorMode == SystemColorMode.Dark)
      {
        if (f is Main)
        {
          f.Acrylic();
        }
        else
        {
          f.Mica();
        }
      }
      // Do nothing if SystemColorMode.Classic
    }
  }
}
