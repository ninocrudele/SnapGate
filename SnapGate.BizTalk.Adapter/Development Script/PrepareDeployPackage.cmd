echo off
cls
echo Build the solution before running the bach.
cd..
rd /s /q Setup\bin\Debug\Deploy
echo Deploy directory removed.
cd %~dp0
pause
xcopy ..\Runtime\bin\Debug\GrabCaster.Framework.BizTalk.Adapter.dll ..\Setup\bin\Debug\Deploy\*  /y
xcopy ..\Runtime\bin\Debug\GrabCaster.Framework.BizTalk.Common.dll ..\Setup\bin\Debug\Deploy\*  /y
xcopy ..\Runtime\bin\Debug\GrabCaster.Framework.BizTalk.Adapter.Designtime.dll ..\Setup\bin\Debug\Deploy\*  /y
xcopy ..\Runtime\bin\Debug\GrabCaster.Framework.BizTalk.Adapter.pdb ..\Setup\bin\Debug\Deploy\*  /y
xcopy ..\Runtime\bin\Debug\GrabCaster.Framework.BizTalk.Common.pdb ..\Setup\bin\Debug\Deploy\*  /y
xcopy ..\Runtime\bin\Debug\GrabCaster.Framework.BizTalk.Adapter.Designtime.pdb ..\Setup\bin\Debug\Deploy\*  /y

xcopy ..\BubblingDeploy C:\Users\Nino\Documents\GrabCaster\GrabCaster.BizTalk.Adapter\Setup\bin\Debug\Deploy\Root_BTSNTSvc\ /s /y /e

xcopy ..\GrabCaster.reg ..\Setup\bin\Debug\Deploy\*  /y
xcopy "..\Register BizTalk Adapter.txt" ..\Setup\bin\Debug\Deploy\*  /y
copy ..\BTSNTSvc.cfg ..\Setup\bin\Debug\Deploy\BTSNTSvc.cfg  /y

cd %~dp0
echo Deployment package ready to go.

