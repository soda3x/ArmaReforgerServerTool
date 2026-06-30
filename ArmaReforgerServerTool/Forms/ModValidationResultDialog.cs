/******************************************************************************
 * File Name:    ModValidationResultDialog.cs
 * Project:      Longbow
 * Description:  Modal dialog displaying mod validation results and fixes applied
 *
 * Author:       Claude Code
 ******************************************************************************/

using ReforgerServerApp.Design;
using ReforgerServerApp.Managers;
using ReforgerServerApp.Models;
using ReforgerServerApp.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ReforgerServerApp.Forms
{
  /// <summary>
  /// Shows mod validation results with status icons, fixes applied, and errors
  /// </summary>
  public partial class ModValidationResultDialog : Form
  {
    private ValidationResult _validationResult;
    private List<string> _fixesApplied;
    private bool _hasValidationErrors;

    public ModValidationResultDialog(ValidationResult validationResult, List<string> fixesApplied)
    {
      _validationResult = validationResult;
      _fixesApplied = fixesApplied;
      _hasValidationErrors = !validationResult.IsValid;

      InitializeComponent();
      ApplyTheme();
      PopulateResults();
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();

      // Form settings
      this.Text = "Mod Validation Results";
      this.Width = 700;
      this.Height = 600;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Icon = null;
      this.BackColor = Colors.BackgroundPrimary;
      this.ForeColor = Colors.TextPrimary;
      this.MinimumSize = new Size(600, 400);

      // Main layout container with proper spacing
      var mainContainer = new TableLayoutPanel
      {
        Dock = DockStyle.Fill,
        RowCount = 3,
        ColumnCount = 1,
        Padding = new Padding(0),
        AutoSize = false
      };
      mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header
      mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Content
      mainContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Footer

      // Title + Status Icon
      var headerPanel = new FlowLayoutPanel
      {
        Dock = DockStyle.Fill,
        Height = 60,
        BackColor = Colors.BackgroundSecondary,
        Padding = new Padding(15, 12, 15, 12),
        AutoSize = false,
        FlowDirection = FlowDirection.LeftToRight
      };

      var statusIcon = new Label
      {
        Text = _validationResult.IsValid ? "✓" : "✕",
        Font = new Font(FontFamily.GenericSansSerif, 24, FontStyle.Bold),
        ForeColor = _validationResult.IsValid ? Colors.Success : Colors.Error,
        Width = 50,
        Height = 36,
        TextAlign = ContentAlignment.MiddleCenter,
        Margin = new Padding(0, 0, 12, 0)
      };

      var statusLabel = new Label
      {
        Text = _validationResult.IsValid ? "All Mods Valid" : "Validation Issues Found",
        Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
        ForeColor = Colors.TextPrimary,
        AutoSize = true,
        TextAlign = ContentAlignment.MiddleLeft
      };

      headerPanel.Controls.Add(statusIcon);
      headerPanel.Controls.Add(statusLabel);

      // Content area - scrollable panel
      var contentPanel = new Panel
      {
        Dock = DockStyle.Fill,
        BackColor = Colors.BackgroundPrimary,
        AutoScroll = true,
        Padding = new Padding(15, 10, 15, 10)
      };

      int yPosition = 0;

      // Fixes Applied Section (if any)
      if (_fixesApplied.Count > 0)
      {
        var fixesGroup = CreateGroupBox("Fixes Applied", 180, yPosition);
        var fixesText = new TextBox
        {
          Multiline = true,
          ScrollBars = ScrollBars.Vertical,
          ReadOnly = true,
          BackColor = Colors.Surface,
          ForeColor = Colors.Success,
          Dock = DockStyle.Fill,
          Padding = new Padding(8),
          Text = string.Join(Environment.NewLine, _fixesApplied.Select(f => "✓ " + f))
        };
        fixesGroup.Controls.Add(fixesText);
        contentPanel.Controls.Add(fixesGroup);
        yPosition += 190;
      }

      // Errors Section (if any)
      if (!_validationResult.IsValid)
      {
        var errorsGroup = CreateGroupBox("Validation Errors", 180, yPosition);
        var fatalErrors = _validationResult.GetFatalErrors();
        var errorLines = fatalErrors.Select(e =>
        {
          var severity = e.Level == ValidationError.Severity.FATAL ? "[FATAL]" : "[ERROR]";
          return $"{severity} {e.ModId}: {e.Message}";
        });

        var errorsText = new TextBox
        {
          Multiline = true,
          ScrollBars = ScrollBars.Vertical,
          ReadOnly = true,
          BackColor = Colors.Surface,
          ForeColor = Colors.Error,
          Dock = DockStyle.Fill,
          Padding = new Padding(8),
          Text = string.Join(Environment.NewLine, errorLines)
        };
        errorsGroup.Controls.Add(errorsText);
        contentPanel.Controls.Add(errorsGroup);
        yPosition += 190;
      }

      // Warnings Section (if any)
      var warnings = _validationResult.Errors
        .Where(e => e.Level == ValidationError.Severity.WARNING || e.Level == ValidationError.Severity.INFO)
        .ToList();

      if (warnings.Count > 0)
      {
        var warningsGroup = CreateGroupBox("Warnings & Info", 150, yPosition);
        var warningLines = warnings.Select(w =>
          $"[{w.Level}] {w.ModId}: {w.Message}");

        var warningsText = new TextBox
        {
          Multiline = true,
          ScrollBars = ScrollBars.Vertical,
          ReadOnly = true,
          BackColor = Colors.Surface,
          ForeColor = Colors.Warning,
          Dock = DockStyle.Fill,
          Padding = new Padding(8),
          Text = string.Join(Environment.NewLine, warningLines)
        };
        warningsGroup.Controls.Add(warningsText);
        contentPanel.Controls.Add(warningsGroup);
      }

      // Bottom button layout
      var buttonLayout = new FlowLayoutPanel
      {
        Dock = DockStyle.Fill,
        Height = 50,
        FlowDirection = FlowDirection.RightToLeft,
        BackColor = Colors.BackgroundSecondary,
        Padding = new Padding(15, 10, 15, 10),
        AutoSize = false
      };

      var closeBtn = new Button
      {
        Text = "Close",
        Width = 110,
        Height = 32,
        BackColor = Colors.Surface,
        ForeColor = Colors.TextPrimary,
        FlatStyle = FlatStyle.Flat,
        Font = new Font(FontFamily.GenericSansSerif, 10F)
      };
      closeBtn.Click += (s, e) => this.Close();

      buttonLayout.Controls.Add(closeBtn);

      mainContainer.Controls.Add(headerPanel, 0, 0);
      mainContainer.Controls.Add(contentPanel, 0, 1);
      mainContainer.Controls.Add(buttonLayout, 0, 2);

      this.Controls.Add(mainContainer);

      this.ResumeLayout();
    }

    private GroupBox CreateGroupBox(string title, int height, int yPosition)
    {
      return new GroupBox
      {
        Text = title,
        Location = new Point(0, yPosition),
        Width = 650,
        Height = height,
        BackColor = Colors.BackgroundSecondary,
        ForeColor = Colors.TextPrimary,
        Padding = new Padding(8),
        Margin = new Padding(0, 0, 0, 10),
        Font = new Font(FontFamily.GenericSansSerif, 10F, FontStyle.Bold)
      };
    }

    private void ApplyTheme()
    {
      this.BackColor = Colors.BackgroundPrimary;
      this.ForeColor = Colors.TextPrimary;
    }

    private void PopulateResults()
    {
      // Results are displayed in InitializeComponent
    }
  }
}
