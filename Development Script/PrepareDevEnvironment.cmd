echo off
echo Build the solution before running the bach.
cd..
rd /s /q Framework\bin\Debug\Root_SnapGate
rd /s /q Framework\bin\Release\Root_SnapGate
cd %~dp0

call DevDefaultBubbligFiles2BubblingDirs.cmd
call DevDefaultConfigurationFile2DebugReleaseDirs.cmd
call DevCopyBatchFiles2DebugReleaseDirs.cmd
call DevCopyEventsDLLDebugversion.cmd
call DevCopyTriggerDLLDebugversion.cmd

call DevCopyEventsDLLReleaseversion.cmd

call DevCopyTriggerDLLReleaseversion.cmd
call DevCopyComponentDLL.cmd

cd..
xcopy "Batch Files\Create new Clone.cmd" Framework\bin\Debug\*  /y
xcopy "Batch Files\Create new Clone.cmd" Framework\bin\Release\*  /y
copy Framework.Log.EventHubs\bin\Debug\SnapGate.Framework.Log.EventHubs.dll Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Log.EventHubs\bin\Debug\SnapGate.Framework.Log.EventHubs.pdb Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Log.EventHubs\bin\Release\SnapGate.Framework.Log.EventHubs.dll Framework\bin\Release\Root_SnapGate\* /y

copy Framework.Log.File\bin\Debug\SnapGate.Framework.Log.File.dll Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Log.File\bin\Debug\SnapGate.Framework.Log.File.pdb Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Log.File\bin\Release\SnapGate.Framework.Log.File.dll Framework\bin\Release\Root_SnapGate\* /y

copy Framework.Log.AzureTableStorage\bin\Debug\SnapGate.Framework.Log.AzureTableStorage.dll Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Log.AzureTableStorage\bin\Debug\SnapGate.Framework.Log.AzureTableStorage.pdb Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Log.AzureTableStorage\bin\Release\SnapGate.Framework.Log.AzureTableStorage.dll Framework\bin\Release\Root_SnapGate\* /y

copy Framework.Dcp.Azure\bin\Debug\SnapGate.Framework.Dcp.Azure.dll Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Dcp.Azure\bin\Debug\SnapGate.Framework.Dcp.Azure.pdb Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Dcp.Azure\bin\Release\SnapGate.Framework.Dcp.Azure.dll Framework\bin\Release\Root_SnapGate\* /y

copy Framework.Dcp.Redis\bin\Debug\SnapGate.Framework.Dcp.Redis.dll Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Dcp.Redis\bin\Debug\SnapGate.Framework.Dcp.Redis.pdb Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Dcp.Redis\bin\Release\SnapGate.Framework.Dcp.Redis.dll Framework\bin\Release\Root_SnapGate\* /y

copy Framework.Dpp.Azure\bin\Debug\SnapGate.Framework.Dpp.Azure.dll Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Dpp.Azure\bin\Debug\SnapGate.Framework.Dpp.Azure.pdb Framework\bin\Debug\Root_SnapGate\* /y
copy Framework.Dpp.Azure\bin\Release\SnapGate.Framework.Dpp.Azure.dll Framework\bin\Release\Root_SnapGate\* /y

echo copy the deployment features
copy Framework.Deployment\bin\Debug\SnapGate.Framework.Deployment.dll Framework\bin\Debug\* /y
copy Framework.Deployment\bin\Debug\SnapGate.Framework.Deployment.pdb Framework\bin\Debug\* /y
copy Framework.Deployment\bin\Release\SnapGate.Framework.Deployment.dll Framework\bin\Release\* /y

copy ..\DefaultFiles\RestSharp.dll Framework\bin\Debug\* /y
copy ..\DefaultFiles\Twilio.Api.dll Framework\bin\Debug\* /y
copy ..\DefaultFiles\RestSharp.dll Framework\bin\Release\* /y
copy ..\DefaultFiles\Twilio.Api.dll Framework\bin\Release\* /y

xcopy ..\DefaultFiles\DynamicDeployment\* Framework\bin\Debug\Deploy\ /s /y
xcopy ..\DefaultFiles\DynamicDeployment\* Framework\bin\Release\Deploy\ /s /y


xcopy ..\DefaultFiles\Log Framework\bin\Debug\Log\* /s /y /e
xcopy ..\DefaultFiles\Log Framework\bin\Debug\Log\* /s /y /e

echo copy in embedded folder

xcopy Framework\bin\Debug\* Laboratory.ConsoleEmbedded\bin\Debug\* /s /y /e
xcopy Framework\bin\Debug\Root_SnapGate\* Laboratory.ConsoleEmbedded\bin\Debug\Root_SnapGate.Laboratory.ConsoleEmbedded\* /s /y /e
xcopy Framework\bin\Release\* Laboratory.ConsoleEmbedded\bin\Release\* /s /y /e
xcopy Framework\bin\Release\Root_SnapGate\* Laboratory.ConsoleEmbedded\bin\Release\Root_SnapGate.Laboratory.ConsoleEmbedded\* /s /y /e

cd %~dp0

