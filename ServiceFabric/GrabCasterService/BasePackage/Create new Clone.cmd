@echo off
set /p clonename= Enter the clone name and press Enters:
xcopy .\\Root_GrabCaster\* .\\Root_GrabCaster%clonename%\* /s /y
xcopy GrabCaster.exe GrabCaster%clonename%.* /y
xcopy GrabCaster.cfg GrabCaster%clonename%.* /y
echo ----------------------------------------------------------------------
echo                      Clone %clonename% created.
echo ----------------------------------------------------------------------
echo Important note: change the port number in the WebApiEndPoint key in 
echo the GrabCaster%clonename%.cfg configuration file.
echo ----------------------------------------------------------------------
pause

