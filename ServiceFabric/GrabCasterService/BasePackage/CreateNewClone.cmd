@echo off
set /p clonename= Enter the clone name and press Enters:
xcopy .\\Root_GrabCaster\* .\\Root_GrabCaster_%clonename%\* /s /y
xcopy Engine.exe Engine%clonename%.* /y
xcopy Engine.cfg Engine%clonename%.* /y
@echo Engine.exe -console>  "Engine_%clonename% MS Dos.cmd"
@echo Engine.exe -ntinstall %1>  "Install Engine_%clonename% Windows NT Service.cmd"
@echo Engine.exe -ntuninstall %1>  "Uninstall Engine_%clonename% Windows NT Service.cmd"
echo ----------------------------------------------------------------------
echo                      Clone %clonename% created.
echo ----------------------------------------------------------------------
pause

