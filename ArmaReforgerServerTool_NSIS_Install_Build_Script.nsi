!define MUI_PRODUCT "Arma Reforger Dedicated Server Tool"
!define MUI_FILE "savefile"
!define MUI_VERSION ""
!define MUI_BRANDINGTEXT "Arma Reforger Dedicated Server Tool"
CRCCheck on

Name "Arma Reforger Dedicated Server Tool"

!include "MUI2.nsh"

OutFile "install_reforger_server_tool.exe"

!define MUI_ICON "ArmaReforgerServerTool\arma_icon_white.ico"
!define MUI_ABORTWARNING

InstallDir $LOCALAPPDATA\ArmaReforgerServerTool
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
 
!insertmacro MUI_LANGUAGE "English"

# Install Section
Section
SetOutPath $INSTDIR
File /r "ArmaReforgerServerTool\bin\Debug\net6.0-windows\*"
File /r "ReforgerToolConfigMigrator\bin\Debug\net6.0-windows\*"
WriteUninstaller $INSTDIR\uninstall.exe
CreateShortCut "$SMPROGRAMS\Arma Reforger Dedicated Server Tool.lnk" "$INSTDIR\ReforgerServerApp.exe"
CreateShortCut "$SMPROGRAMS\Uninstall Arma Reforger Dedicated Server Tool.lnk" "$INSTDIR\uninstall.exe"
CreateShortCut "$DESKTOP\Arma Reforger Dedicated Server Tool.lnk" "$INSTDIR\ReforgerServerApp.exe"
SectionEnd

# Uninstall Section
Section "Uninstall"
RMDir /r "$INSTDIR\*.*"
RMDir $INSTDIR
Delete "$SMPROGRAMS\Arma Reforger Dedicated Server Tool\Arma Reforger Dedicated Server Tool.lnk"
Delete "$SMPROGRAMS\Arma Reforger Dedicated Server Tool\Uninstall Arma Reforger Dedicated Server Tool.lnk"
Delete "$DESKTOP\Arma Reforger Dedicated Server Tool.lnk"
SectionEnd