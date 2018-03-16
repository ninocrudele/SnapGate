echo off
cd..
xcopy Triggers\AzureBlobTrigger\bin\Release\SnapGate.Framework.AzureBlobTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\AzureQueueTrigger\bin\Release\SnapGate.Framework.AzureQueueTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\AzureTopicTrigger\bin\Release\SnapGate.Framework.AzureTopicTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\BULKSQLServerTrigger\bin\Release\SnapGate.Framework.BulksqlServerTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\CSharpTrigger\bin\Release\SnapGate.Framework.CSharpTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\ETW\bin\Release\SnapGate.Framework.EtwTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\EventHubsTrigger\bin\Release\SnapGate.Framework.EventHubsTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\EventViewerTrigger\bin\Release\SnapGate.Framework.EventViewerTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\NOPTrigger\bin\Release\SnapGate.Framework.NopTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\PowershellTrigger\bin\Release\SnapGate.Framework.PowerShellTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\RfidTrigger\bin\Release\SnapGate.Framework.RfidTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\SQLServerTrigger\bin\Release\SnapGate.Framework.SqlServerTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\ChatTrigger\bin\Release\SnapGate.Framework.ChatTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\EmbeddedTrigger\bin\Release\SnapGate.Framework.EmbeddedTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\SnapGate.Framework.RunProcess\bin\Release\SnapGate.Framework.RunProcess.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\FileTrigger\bin\Release\SnapGate.Framework.FileTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\SnapGate.Framework.WCFTrigger\bin\Release\SnapGate.Framework.WCFTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y
xcopy Triggers\SnapGate.Framework.BenchmarkTrigger\bin\Release\SnapGate.Framework.BenchmarkTrigger.dll Setup\bin\Release\Deploy\Root_SnapGate\Triggers\* /y

cd %~dp0