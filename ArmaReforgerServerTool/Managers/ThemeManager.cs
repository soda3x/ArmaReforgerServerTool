using ReforgerServerApp;
using System;
using System.Collections.Generic;
using System.Text;
using WinForms.Fluent;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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
      SyncThemeColours(f);
    }

    private void SyncThemeColours(Control parent)
    {
      foreach (Control control in parent.Controls)
      {
        if (control is FontAwesome.Sharp.IconButton iconBtn)
        {
          // Set the initial icon color to match the current system text color
          iconBtn.IconColor = iconBtn.ForeColor;

          // Keep it synced if the OS theme changes while the app is running
          iconBtn.ForeColorChanged += (s, e) =>
          {
            iconBtn.IconColor = iconBtn.ForeColor;
          };
        }

        // Recursively check for buttons inside Panels, GroupBoxes, or TabPages
        if (control.HasChildren)
        {
          SyncThemeColours(control);
        }
      }
    }
  }
}
