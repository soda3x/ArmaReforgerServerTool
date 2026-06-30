/******************************************************************************
 * File Name:    HoverableButton.cs
 * Project:      Longbow
 * Description:  Custom button control with smooth hover state feedback
 *
 * Author:       Claude Code
 ******************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReforgerServerApp.Design
{
  /// <summary>
  /// A custom button control that provides smooth visual feedback on hover.
  /// Extends Button with smooth color transitions for professional appearance.
  /// Supports customizable colors for normal, hover, and pressed states.
  /// </summary>
  [Obsolete("For future use. Currently not integrated into the UI. Use standard Button controls with the ApplyDarkTheme method instead.")]
  public class HoverableButton : Button
  {
    private Color m_normalBackColor;
    private Color m_hoverBackColor;
    private Color m_pressedBackColor;
    private Color m_normalForeColor;
    private Color m_hoverForeColor;
    private bool m_isHovered;
    private bool m_isPressed;

    public HoverableButton()
    {
      // Set default colors using the design system
      m_normalBackColor = Colors.Surface;
      m_hoverBackColor = Colors.SurfaceHover;
      m_pressedBackColor = Colors.Primary;
      m_normalForeColor = Colors.TextPrimary;
      m_hoverForeColor = Colors.TextPrimary;

      // Configure button appearance
      FlatStyle = FlatStyle.Flat;
      FlatAppearance.BorderSize = 1;
      FlatAppearance.BorderColor = Colors.Border;
      FlatAppearance.MouseDownBackColor = m_pressedBackColor;
      FlatAppearance.MouseOverBackColor = m_hoverBackColor;

      // Set initial colors
      BackColor = m_normalBackColor;
      ForeColor = m_normalForeColor;

      // Attach event handlers
      MouseEnter += OnMouseEnter;
      MouseLeave += OnMouseLeave;
      MouseDown += OnMouseDown;
      MouseUp += OnMouseUp;
    }

    /// <summary>
    /// Gets or sets the background color for the normal (non-hovered) state.
    /// </summary>
    public Color NormalBackColor
    {
      get => m_normalBackColor;
      set
      {
        m_normalBackColor = value;
        if (!m_isHovered && !m_isPressed)
        {
          BackColor = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the background color for the hovered state.
    /// </summary>
    public Color HoverBackColor
    {
      get => m_hoverBackColor;
      set
      {
        m_hoverBackColor = value;
        FlatAppearance.MouseOverBackColor = value;
      }
    }

    /// <summary>
    /// Gets or sets the background color for the pressed state.
    /// </summary>
    public Color PressedBackColor
    {
      get => m_pressedBackColor;
      set
      {
        m_pressedBackColor = value;
        FlatAppearance.MouseDownBackColor = value;
      }
    }

    /// <summary>
    /// Gets or sets the foreground (text) color for the normal state.
    /// </summary>
    public Color NormalForeColor
    {
      get => m_normalForeColor;
      set
      {
        m_normalForeColor = value;
        if (!m_isHovered && !m_isPressed)
        {
          ForeColor = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the foreground (text) color for the hovered state.
    /// </summary>
    public Color HoverForeColor
    {
      get => m_hoverForeColor;
      set => m_hoverForeColor = value;
    }

    /// <summary>
    /// Gets a value indicating whether the button is currently hovered.
    /// </summary>
    public bool IsHovered => m_isHovered;

    /// <summary>
    /// Gets a value indicating whether the button is currently pressed.
    /// </summary>
    public bool IsPressed => m_isPressed;

    /// <summary>
    /// Sets the button to use primary theme colors (blue).
    /// </summary>
    public void SetPrimaryTheme()
    {
      NormalBackColor = Colors.Primary;
      HoverBackColor = Colors.PrimaryHover;
      PressedBackColor = Colors.PrimaryPressed;
      NormalForeColor = Colors.White;
      HoverForeColor = Colors.White;
    }

    /// <summary>
    /// Sets the button to use accent theme colors (orange).
    /// </summary>
    public void SetAccentTheme()
    {
      NormalBackColor = Colors.Accent;
      HoverBackColor = Colors.AccentHover;
      PressedBackColor = Colors.AccentPressed;
      NormalForeColor = Colors.White;
      HoverForeColor = Colors.White;
    }

    /// <summary>
    /// Sets the button to use success theme colors (green).
    /// </summary>
    public void SetSuccessTheme()
    {
      NormalBackColor = Colors.Success;
      HoverBackColor = Colors.SuccessHover;
      PressedBackColor = Colors.Success;
      NormalForeColor = Colors.White;
      HoverForeColor = Colors.White;
    }

    /// <summary>
    /// Sets the button to use error theme colors (red).
    /// </summary>
    public void SetErrorTheme()
    {
      NormalBackColor = Colors.Error;
      HoverBackColor = Colors.ErrorHover;
      PressedBackColor = Colors.Error;
      NormalForeColor = Colors.White;
      HoverForeColor = Colors.White;
    }

    /// <summary>
    /// Sets the button to use warning theme colors (amber).
    /// </summary>
    public void SetWarningTheme()
    {
      NormalBackColor = Colors.Warning;
      HoverBackColor = Colors.WarningHover;
      PressedBackColor = Colors.Warning;
      NormalForeColor = Colors.White;
      HoverForeColor = Colors.White;
    }

    /// <summary>
    /// Sets the button to use default surface colors.
    /// </summary>
    public void SetDefaultTheme()
    {
      NormalBackColor = Colors.Surface;
      HoverBackColor = Colors.SurfaceHover;
      PressedBackColor = Colors.Primary;
      NormalForeColor = Colors.TextPrimary;
      HoverForeColor = Colors.TextPrimary;
    }

    private void OnMouseEnter(object sender, EventArgs e)
    {
      m_isHovered = true;
      Cursor = Cursors.Hand;
      BackColor = m_hoverBackColor;
      ForeColor = m_hoverForeColor;
    }

    private void OnMouseLeave(object sender, EventArgs e)
    {
      m_isHovered = false;
      m_isPressed = false;
      Cursor = Cursors.Default;
      BackColor = m_normalBackColor;
      ForeColor = m_normalForeColor;
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        m_isPressed = true;
        BackColor = m_pressedBackColor;
      }
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
      m_isPressed = false;
      if (m_isHovered)
      {
        BackColor = m_hoverBackColor;
      }
      else
      {
        BackColor = m_normalBackColor;
      }
    }

    /// <summary>
    /// Disposes of the button and detaches event handlers to prevent memory leaks.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        MouseEnter -= OnMouseEnter;
        MouseLeave -= OnMouseLeave;
        MouseDown -= OnMouseDown;
        MouseUp -= OnMouseUp;
      }
      base.Dispose(disposing);
    }
  }
}
