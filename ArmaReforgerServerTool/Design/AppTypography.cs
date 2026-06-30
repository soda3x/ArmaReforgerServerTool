/******************************************************************************
 * File Name:    AppTypography.cs
 * Project:      Longbow
 * Description:  Typography scale and font definitions for the application
 *
 * Author:       Claude Code
 ******************************************************************************/

using System.Drawing;

namespace ReforgerServerApp.Design
{
  /// <summary>
  /// Centralized typography system with consistent font scales.
  /// Provides fonts for headings, body text, captions, and other text elements.
  /// All fonts use consistent family and weights across the application.
  /// </summary>
  public static class AppTypography
  {
    // ========== FONT FAMILIES ==========
    /// <summary>Primary font family - Segoe UI is the Windows standard</summary>
    private const string PrimaryFontFamily = "Segoe UI";

    /// <summary>Fallback font family</summary>
    private const string FallbackFontFamily = "Arial";

    // ========== HEADING SIZES ==========
    /// <summary>Large heading (32pt) - page titles</summary>
    public static readonly Font HeadingLarge = new Font(PrimaryFontFamily, 32, FontStyle.Bold);

    /// <summary>Medium heading (24pt) - section titles</summary>
    public static readonly Font HeadingMedium = new Font(PrimaryFontFamily, 24, FontStyle.Bold);

    /// <summary>Small heading (18pt) - subsection titles</summary>
    public static readonly Font HeadingSmall = new Font(PrimaryFontFamily, 18, FontStyle.Bold);

    // ========== SUBTITLE SIZES ==========
    /// <summary>Large subtitle (16pt, semi-bold) - prominent secondary text</summary>
    public static readonly Font SubtitleLarge = new Font(PrimaryFontFamily, 16, FontStyle.Bold);

    /// <summary>Regular subtitle (14pt, semi-bold) - secondary headings</summary>
    public static readonly Font SubtitleRegular = new Font(PrimaryFontFamily, 14, FontStyle.Bold);

    // ========== BODY TEXT SIZES ==========
    /// <summary>Large body text (14pt) - main content, readable</summary>
    public static readonly Font BodyLarge = new Font(PrimaryFontFamily, 14, FontStyle.Regular);

    /// <summary>Regular body text (12pt) - standard content</summary>
    public static readonly Font BodyRegular = new Font(PrimaryFontFamily, 12, FontStyle.Regular);

    /// <summary>Small body text (11pt) - secondary content</summary>
    public static readonly Font BodySmall = new Font(PrimaryFontFamily, 11, FontStyle.Regular);

    // ========== BUTTON SIZES ==========
    /// <summary>Button text (12pt, semi-bold) - call-to-action text</summary>
    public static readonly Font ButtonText = new Font(PrimaryFontFamily, 12, FontStyle.Bold);

    /// <summary>Small button text (11pt, semi-bold) - compact buttons</summary>
    public static readonly Font ButtonTextSmall = new Font(PrimaryFontFamily, 11, FontStyle.Bold);

    // ========== CAPTION AND LABEL SIZES ==========
    /// <summary>Label text (11pt) - form labels and field labels</summary>
    public static readonly Font Label = new Font(PrimaryFontFamily, 11, FontStyle.Regular);

    /// <summary>Caption text (10pt) - helper text, tooltips, small notes</summary>
    public static readonly Font Caption = new Font(PrimaryFontFamily, 10, FontStyle.Regular);

    /// <summary>Small caption (9pt) - very small helper text</summary>
    public static readonly Font CaptionSmall = new Font(PrimaryFontFamily, 9, FontStyle.Regular);

    // ========== MONO/CODE SIZES ==========
    /// <summary>Monospace code font (11pt) - for displaying code or logs</summary>
    public static readonly Font MonospaceRegular = new Font("Consolas", 11, FontStyle.Regular);

    /// <summary>Monospace code font (10pt) - for smaller code displays</summary>
    public static readonly Font MonospaceSmall = new Font("Consolas", 10, FontStyle.Regular);

    // ========== CONVENIENCE METHODS ==========
    /// <summary>Get a font with custom size and style</summary>
    /// <param name="size">Font size in points</param>
    /// <param name="style">Font style (Regular, Bold, Italic, etc.)</param>
    /// <returns>Font object with specified parameters</returns>
    public static Font GetFont(float size, FontStyle style = FontStyle.Regular)
      => new Font(PrimaryFontFamily, size, style);

    /// <summary>Get a monospace font with custom size and style</summary>
    /// <param name="size">Font size in points</param>
    /// <param name="style">Font style (Regular, Bold, Italic, etc.)</param>
    /// <returns>Monospace font object with specified parameters</returns>
    public static Font GetMonospaceFont(float size, FontStyle style = FontStyle.Regular)
      => new Font("Consolas", size, style);
  }
}
