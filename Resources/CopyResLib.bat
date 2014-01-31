@echo off
echo S1 Nyan Title Localization configuration

SET cmd=%1

IF NOT DEFINED cmd goto :Select
IF %1==Release goto :Release
IF %1==Beta goto :Beta 
IF %1==Debug goto :Beta
IF %1==Default goto :Default 

:Select
SETLOCAL
CHOICE /C 12 /M "Choose Version: 1 - Release ; 2 - Beta"

IF %errorlevel%==1 goto :Release
IF %errorlevel%==2 goto :Beta

goto Exit

:Default
	IF EXIST AppResLib/AppResLib.dll goto:Exit ELSE goto :Release

:Release
	SET V=Release
    goto Copy

:Beta
	SET V=Beta
    goto Copy

:Copy
IF EXIST %2 cd %2
IF NOT EXIST AppResLib md AppResLib
copy AppResLibFiles\%V%\*.* AppResLib\"

:Exit

endlocal
