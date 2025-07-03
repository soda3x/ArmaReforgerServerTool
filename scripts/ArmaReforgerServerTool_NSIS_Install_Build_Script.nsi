!define MUI_PRODUCT "Longbow"
!define MUI_FILE "savefile"
!define MUI_VERSION ""
!define MUI_BRANDINGTEXT "Longbow"
CRCCheck on

Name "Longbow"

!include "MUI2.nsh"

OutFile "install_reforger_server_tool.exe"

!define MUI_ICON "${__FILEDIR__}\..\ArmaReforgerServerTool\Resources\arma_icon_white.ico"
!define MUI_ABORTWARNING

InstallDir $LOCALAPPDATA\Longbow
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
File /r /x *.txt /x *.pdb /x "mod_database.json" /x "properties.json" /x "logs\*" "${__FILEDIR__}\..\ArmaReforgerServerTool\bin\Release\net6.0-windows\*.*"
WriteUninstaller $INSTDIR\uninstall.exe
CreateShortCut "$SMPROGRAMS\Longbow.lnk" "$INSTDIR\Longbow.exe"
CreateShortCut "$SMPROGRAMS\Uninstall Longbow.lnk" "$INSTDIR\uninstall.exe"
CreateShortCut "$DESKTOP\Longbow.lnk" "$INSTDIR\Longbow.exe"
SectionEnd

# Uninstall Section
Section "Uninstall"
RMDir /r "$INSTDIR\*.*"
RMDir $INSTDIR
Delete "$SMPROGRAMS\Longbow\Longbow.lnk"
Delete "$SMPROGRAMS\Longbow\Uninstall Longbow.lnk"
Delete "$DESKTOP\Longbow.lnk"
SectionEnd
