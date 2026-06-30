/******************************************************************************
 * File Name:    AppSpacing.cs
 * Project:      Longbow
 * Description:  Spacing scale and padding/margin values for consistent layout
 *
 * Author:       Claude Code
 ******************************************************************************/

namespace ReforgerServerApp.Design
{
  /// <summary>
  /// Centralized spacing system based on 4px grid.
  /// All spacing values are multiples of 4px for consistency.
  /// Used for margins, padding, and gaps in layouts.
  /// </summary>
  public static class AppSpacing
  {
    // ========== BASE UNIT ==========
    /// <summary>Base spacing unit (4px) - foundation of the spacing system</summary>
    public const int BaseUnit = 4;

    // ========== SPACING TOKENS ==========
    /// <summary>Extra small spacing (4px) - tight spacing</summary>
    public const int XSmall = 4;

    /// <summary>Small spacing (8px) - standard compact spacing</summary>
    public const int Small = 8;

    /// <summary>Medium spacing (12px) - standard spacing</summary>
    public const int Medium = 12;

    /// <summary>Normal spacing (16px) - default padding/margin</summary>
    public const int Normal = 16;

    /// <summary>Large spacing (24px) - generous spacing</summary>
    public const int Large = 24;

    /// <summary>Extra large spacing (32px) - section separation</summary>
    public const int XLarge = 32;

    /// <summary>Double extra large spacing (48px) - major section separation</summary>
    public const int XXLarge = 48;

    // ========== COMMON PADDING VALUES ==========
    /// <summary>Button padding - horizontal (12px)</summary>
    public const int ButtonPaddingHorizontal = 12;

    /// <summary>Button padding - vertical (6px)</summary>
    public const int ButtonPaddingVertical = 6;

    /// <summary>Control padding - typical (8px)</summary>
    public const int ControlPadding = 8;

    /// <summary>Container padding - typical (16px)</summary>
    public const int ContainerPadding = 16;

    /// <summary>Card padding - generous (20px)</summary>
    public const int CardPadding = 20;

    // ========== COMMON GAP VALUES ==========
    /// <summary>Gap between tightly packed items (4px)</summary>
    public const int GapTight = 4;

    /// <summary>Gap between items (8px)</summary>
    public const int GapNormal = 8;

    /// <summary>Gap between groups (16px)</summary>
    public const int GapLarge = 16;

    /// <summary>Gap between major sections (24px)</summary>
    public const int GapXLarge = 24;

    // ========== BORDER & CORNER RADIUS ==========
    /// <summary>Small border radius (2px) - subtle rounded corners</summary>
    public const int BorderRadiusSmall = 2;

    /// <summary>Medium border radius (4px) - standard rounded corners</summary>
    public const int BorderRadiusMedium = 4;

    /// <summary>Large border radius (8px) - prominent rounded corners</summary>
    public const int BorderRadiusLarge = 8;

    /// <summary>Border width - standard (1px)</summary>
    public const int BorderWidth = 1;

    // ========== CONVENIENCE METHODS ==========
    /// <summary>Get spacing value as multiple of base unit</summary>
    /// <param name="multiplier">Number of base units (4px each)</param>
    /// <returns>Spacing value in pixels</returns>
    public static int GetSpacing(int multiplier)
      => multiplier * BaseUnit;

    /// <summary>Get symmetric padding for a control (equal horizontal and vertical)</summary>
    /// <param name="padding">Padding size constant (e.g., Normal, Large)</param>
    /// <returns>Padding value in pixels</returns>
    public static int GetPadding(int padding)
      => padding;

    /// <summary>Get margin value</summary>
    /// <param name="margin">Margin size constant (e.g., Normal, Large)</param>
    /// <returns>Margin value in pixels</returns>
    public static int GetMargin(int margin)
      => margin;
  }
}
