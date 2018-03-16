@echo off
set /p clonename= Enter the clone name and press Enters:
xcopy .\\Root_SnapGate\* .\\Root_SnapGate%clonename%\* /s /y
xcopy SnapGate.exe SnapGate%clonename%.* /y
xcopy SnapGate.cfg SnapGate%clonename%.* /y
echo ----------------------------------------------------------------------
echo                      Clone %clonename% created.
echo ----------------------------------------------------------------------
echo Important note: change the port number in the WebApiEndPoint key in 
echo the SnapGate%clonename%.cfg configuration file.
echo ----------------------------------------------------------------------
pause

