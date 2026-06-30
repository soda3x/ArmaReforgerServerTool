/******************************************************************************
 * File Name:    Colors.cs
 * Project:      Longbow
 * Description:  Central color palette for the application
 *
 * Author:       Claude Code
 ******************************************************************************/

using System.Drawing;

namespace ReforgerServerApp.Design
{
  /// <summary>
  /// Centralized color palette for the entire application.
  /// Provides a single source of truth for all color values.
  /// Organized by color purpose: primary, accent, neutrals, and status colors.
  /// </summary>
  public static class Colors
  {
    // ========== PRIMARY COLORS ==========
    /// <summary>Primary brand color - Sitrep sky blue #0ea5e9</summary>
    public static readonly Color Primary = Color.FromArgb(14, 165, 233);

    /// <summary>Primary hover state - slightly lighter</summary>
    public static readonly Color PrimaryHover = Color.FromArgb(50, 190, 245);

    /// <summary>Primary pressed state - slightly darker</summary>
    public static readonly Color PrimaryPressed = Color.FromArgb(0, 145, 215);

    // ========== ACCENT COLORS ==========
    /// <summary>Accent color for highlights and active states</summary>
    public static readonly Color Accent = Color.FromArgb(255, 140, 0);

    /// <summary>Accent hover state</summary>
    public static readonly Color AccentHover = Color.FromArgb(255, 160, 20);

    /// <summary>Accent pressed state</summary>
    public static readonly Color AccentPressed = Color.FromArgb(225, 120, 0);

    // ========== NEUTRAL COLORS - DARK THEME ==========
    /// <summary>Main background color - very dark gray</summary>
    public static readonly Color BackgroundPrimary = Color.FromArgb(30, 30, 30);

    /// <summary>Secondary background - slightly lighter for emphasis</summary>
    public static readonly Color BackgroundSecondary = Color.FromArgb(45, 45, 45);

    /// <summary>Tertiary background - for cards, panels</summary>
    public static readonly Color BackgroundTertiary = Color.FromArgb(55, 55, 55);

    /// <summary>Surface color - interactive elements</summary>
    public static readonly Color Surface = Color.FromArgb(50, 50, 50);

    /// <summary>Surface hover - subtle elevation</summary>
    public static readonly Color SurfaceHover = Color.FromArgb(65, 65, 65);

    /// <summary>Border color - subtle divider</summary>
    public static readonly Color Border = Color.FromArgb(70, 70, 70);

    /// <summary>Disabled background - slightly transparent feel</summary>
    public static readonly Color DisabledBackground = Color.FromArgb(40, 40, 40);

    // ========== TEXT COLORS ==========
    /// <summary>Primary text color - high contrast white</summary>
    public static readonly Color TextPrimary = Color.FromArgb(240, 240, 240);

    /// <summary>Secondary text - slightly dimmed for less important info</summary>
    public static readonly Color TextSecondary = Color.FromArgb(180, 180, 180);

    /// <summary>Tertiary text - for subtle labels</summary>
    public static readonly Color TextTertiary = Color.FromArgb(130, 130, 130);

    /// <summary>Disabled text - low contrast</summary>
    public static readonly Color TextDisabled = Color.FromArgb(100, 100, 100);

    /// <summary>Inverse text - dark text on light backgrounds</summary>
    public static readonly Color TextInverse = Color.FromArgb(30, 30, 30);

    // ========== STATUS COLORS ==========
    /// <summary>Success color - green for successful operations</summary>
    public static readonly Color Success = Color.FromArgb(76, 175, 80);

    /// <summary>Success hover state</summary>
    public static readonly Color SuccessHover = Color.FromArgb(100, 195, 100);

    /// <summary>Warning color - amber for alerts</summary>
    public static readonly Color Warning = Color.FromArgb(255, 152, 0);

    /// <summary>Warning hover state</summary>
    public static readonly Color WarningHover = Color.FromArgb(255, 175, 20);

    /// <summary>Error/Danger color - red for errors</summary>
    public static readonly Color Error = Color.FromArgb(244, 67, 54);

    /// <summary>Error hover state</summary>
    public static readonly Color ErrorHover = Color.FromArgb(255, 87, 74);

    /// <summary>Info color - blue for informational messages</summary>
    public static readonly Color Info = Color.FromArgb(33, 150, 243);

    /// <summary>Info hover state</summary>
    public static readonly Color InfoHover = Color.FromArgb(66, 165, 245);

    // ========== UTILITY COLORS ==========
    /// <summary>Transparent - useful for overlays</summary>
    public static readonly Color Transparent = Color.Transparent;

    /// <summary>White - for maximum contrast</summary>
    public static readonly Color White = Color.White;

    /// <summary>Black - for minimum contrast</summary>
    public static readonly Color Black = Color.Black;
  }
}
