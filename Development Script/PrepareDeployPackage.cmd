echo off
cls
echo Build the solution before running the bach.

cd..
rd /s /q Setup\bin\Debug\Deploy
rd /s /q Setup\bin\Release\Deploy

echo Deploy directory removed.
cd %~dp0

call DeployComponentDLL.cmd
call DeployEventsDLLDebugversion.cmd
call DeployTriggerDLLDebugversion.cmd
call DeployEventsDLLReleaseversion.cmd
call DeployTriggerDLLReleaseversion.cmd


cd..
rem copy ..\DefaultFiles\RestSharp.dll Setup\bin\Debug\Deploy\*  /y
rem copy ..\DefaultFiles\Twilio.Api.dll Setup\bin\Debug\Deploy\*  /y
rem copy ..\DefaultFiles\RestSharp.dll Setup\bin\Release\Deploy\*  /y
rem copy ..\DefaultFiles\Twilio.Api.dll Setup\bin\Release\Deploy\*  /y
rem xcopy "Documentation\SnapGate v1.0- Technical Manual.pdf" Setup\bin\Debug\Deploy\Documentation\ /s /y /e

xcopy Framework\bin\Debug\*.dll Setup\bin\Debug\Deploy\*  /y
xcopy Framework\bin\Debug\*.pdb Setup\bin\Debug\Deploy\*  /y
xcopy Framework\bin\Debug\*.exe Setup\bin\Debug\Deploy\*  /y
xcopy Framework\bin\Release\*.dll Setup\bin\Release\Deploy\*  /y
xcopy Framework\bin\Release\*.exe Setup\bin\Release\Deploy\*  /y

xcopy ..\DefaultFiles\License.txt Setup\bin\Debug\Deploy\*  /y
xcopy ..\DefaultFiles\License.txt Setup\bin\Release\Deploy\*  /y

xcopy "Batch Files\Create new Clone.cmd" Setup\bin\Debug\Deploy\*  /y
copy ..\DefaultFiles\DeployDefault.cfg Setup\bin\Debug\Deploy\DevDefault.cfg  /y
copy ..\DefaultFiles\DeployDefault.cfg Setup\bin\Release\Deploy\DevDefault.cfg  /y

xcopy ..\DefaultFiles\BubblingDeploy Setup\bin\Debug\Deploy\Root_SnapGate\Bubbling\ /s /y /e
xcopy ..\DefaultFiles\BubblingDeploy Setup\bin\Release\Deploy\Root_SnapGate\Bubbling\ /s /y /e

xcopy ..\DefaultFiles\ConfigurationTemplates Setup\bin\Release\Deploy\Root_SnapGate\ConfigurationTemplates\ /s /y /e
xcopy ..\DefaultFiles\ConfigurationTemplates Setup\bin\Release\Deploy\Root_SnapGate\ConfigurationTemplates\ /s /y /e


xcopy ..\DefaultFiles\Log Setup\bin\Debug\Deploy\Log\* /s /y /e
xcopy ..\DefaultFiles\Log Setup\bin\Release\Deploy\Log\* /s /y /e

xcopy ..\DefaultFiles\Demo Setup\bin\Debug\Deploy\Demo\* /s /y /e
xcopy ..\DefaultFiles\Demo Setup\bin\Release\Deploy\Demo\* /s /y /e
xcopy ..\DefaultFiles\Samples Setup\bin\Debug\Deploy\Samples\* /s /y /e

xcopy ..\DefaultFiles\PersistentStorage Setup\bin\Debug\Deploy\PersistentStorage\* /s /y /e

copy Framework.Log.EventHubs\bin\Debug\SnapGate.Framework.Log.EventHubs.dll Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Log.EventHubs\bin\Debug\SnapGate.Framework.Log.EventHubs.pdb Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Log.EventHubs\bin\Release\SnapGate.Framework.Log.EventHubs.dll Setup\bin\Release\Deploy\Root_SnapGate\* /y

copy Framework.Log.File\bin\Debug\SnapGate.Framework.Log.File.dll Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Log.File\bin\Debug\SnapGate.Framework.Log.File.pdb Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Log.File\bin\Release\SnapGate.Framework.Log.File.dll Setup\bin\Release\Deploy\Root_SnapGate\* /y

copy Framework.Log.AzureTableStorage\bin\Debug\SnapGate.Framework.Log.AzureTableStorage.dll Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Log.AzureTableStorage\bin\Debug\SnapGate.Framework.Log.AzureTableStorage.pdb Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Log.AzureTableStorage\bin\Release\SnapGate.Framework.Log.AzureTableStorage.dll Setup\bin\Release\Deploy\Root_SnapGate\* /y

copy Framework.Deployment\bin\Debug\SnapGate.Framework.Deployment.dll Setup\bin\Debug\Deploy\* /y
copy Framework.Deployment\bin\Debug\SnapGate.Framework.Deployment.pdb Setup\bin\Debug\Deploy\* /y
copy Framework.Deployment\bin\Release\SnapGate.Framework.Deployment.dll Setup\bin\Release\Deploy\* /y

xcopy ..\DefaultFiles\DynamicDeploymentDeploy\* Setup\bin\Debug\Deploy\DynamicDeployment\ /s /y
xcopy ..\DefaultFiles\DynamicDeploymentDeploy\* Setup\bin\Release\Deploy\DynamicDeployment\ /s /y




copy Framework.Dcp.Azure\bin\Debug\SnapGate.Framework.Dcp.Azure.dll Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Dcp.Azure\bin\Release\SnapGate.Framework.Dcp.Azure.dll Setup\bin\Release\Deploy\Root_SnapGate\* /y

copy Framework.Dcp.Redis\bin\Debug\SnapGate.Framework.Dcp.Redis.dll Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Dcp.Redis\bin\Release\SnapGate.Framework.Dcp.Redis.dll Setup\bin\Release\Deploy\Root_SnapGate\* /y

copy Framework.Dpp.Azure\bin\Debug\SnapGate.Framework.Dpp.Azure.dll Setup\bin\Debug\Deploy\Root_SnapGate\* /y
copy Framework.Dpp.Azure\bin\Release\SnapGate.Framework.Dpp.Azure.dll Setup\bin\Release\Deploy\Root_SnapGate\* /y

echo Service Fabric Setup
rem xcopy ServiceFabric Setup\bin\Debug\Deploy\ServiceFabric\ /s /y /e
rem rd /s /q Setup\bin\Debug\Deploy\ServiceFabric\SnapGateService\BasePackage
rem xcopy Framework\bin\Debug\* Setup\bin\Debug\Deploy\ServiceFabric\SnapGateService\BasePackage\* /s /y /e


rem xcopy ServiceFabric Setup\bin\Release\Deploy\ServiceFabric\ /s /y /e
rem rd /s /q Setup\bin\Release\Deploy\ServiceFabric\SnapGateService\BasePackage
rem xcopy Framework\bin\Release\* Setup\bin\Release\Deploy\ServiceFabric\SnapGateService\BasePackage\* /s /y /e

echo BizTalk Adapter

xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Debug\SnapGate.Framework.BizTalk.Adapter.dll Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Debug\SnapGate.Framework.BizTalk.Common.dll Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Debug\SnapGate.Framework.BizTalk.Adapter.Designtime.dll Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Debug\SnapGate.Framework.BizTalk.Adapter.pdb Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Debug\SnapGate.Framework.BizTalk.Common.pdb Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Debug\SnapGate.Framework.BizTalk.Adapter.Designtime.pdb Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\SnapGate.reg Setup\bin\Debug\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Register BizTalk Adapter.txt" Setup\bin\Debug\Deploy\*  /y
copy ..\DefaultFiles\DeployDefault.cfg Setup\bin\Debug\Deploy\BTSNTSvc.cfg  /y

xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Release\SnapGate.Framework.BizTalk.Adapter.dll Setup\bin\Release\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Release\SnapGate.Framework.BizTalk.Common.dll Setup\bin\Release\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Runtime\bin\Release\SnapGate.Framework.BizTalk.Adapter.Designtime.dll Setup\bin\Release\Deploy\*  /y
pause
xcopy Setup\bin\Debug\Deploy\Root_SnapGate\* Setup\bin\Debug\Deploy\Root_BTSNTSvc\ /s /y /e
xcopy Setup\bin\Release\Deploy\Root_SnapGate\* Setup\bin\Release\Deploy\Root_BTSNTSvc\ /s /y /e
pause
xcopy SnapGate.BizTalk.Adapter\SnapGate.reg Setup\bin\Release\Deploy\*  /y
xcopy SnapGate.BizTalk.Adapter\Register_BizTalk_Adapter.txt Setup\bin\Release\Deploy\*  /y
copy ..\DefaultFiles\DeployDefault.cfg Setup\bin\Release\Deploy\BTSNTSvc.cfg  /y


cd %~dp0
echo Deployment package ready to go.

