echo off
cd..
mkdir Framework\bin\Debug\Root_SnapGate\Triggers

REM COPY DLL
copy Triggers\AzureBlobTrigger\bin\Debug\SnapGate.Framework.AzureBlobTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\AzureQueueTrigger\bin\Debug\SnapGate.Framework.AzureQueueTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\AzureTopicTrigger\bin\Debug\SnapGate.Framework.AzureTopicTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\BULKSQLServerTrigger\bin\Debug\SnapGate.Framework.BulksqlServerTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\CSharpTrigger\bin\Debug\SnapGate.Framework.CSharpTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\ETW\bin\Debug\SnapGate.Framework.EtwTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\EventHubsTrigger\bin\Debug\SnapGate.Framework.EventHubsTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\EventViewerTrigger\bin\Debug\SnapGate.Framework.EventViewerTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\FileTrigger\bin\Debug\SnapGate.Framework.FileTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\NOPTrigger\bin\Debug\SnapGate.Framework.NopTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\PowershellTrigger\bin\Debug\SnapGate.Framework.PowerShellTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\RfidTrigger\bin\Debug\SnapGate.Framework.RfidTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SQLServerTrigger\bin\Debug\SnapGate.Framework.SqlServerTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\ChatTrigger\bin\Debug\SnapGate.Framework.ChatTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\EmbeddedTrigger\bin\Debug\SnapGate.Framework.EmbeddedTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.RunProcess\bin\Debug\SnapGate.Framework.RunProcess.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\HM.OMS.PageOneMessageTrigger\bin\Debug\HM.OMS.PageOneMessageTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\HM.OMS.PageOneMessageRESTTrigger\bin\Debug\HM.OMS.PageOneMessageRESTTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.DynamicRESTTrigger\bin\Debug\SnapGate.Framework.DynamicRESTTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.WCFTrigger\bin\Debug\SnapGate.Framework.WCFTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.BenchmarkTrigger\bin\Debug\SnapGate.Framework.BenchmarkTrigger.dll Framework\bin\Debug\Root_SnapGate\Triggers\* /y

REM COPY PDB
copy Triggers\AzureBlobTrigger\bin\Debug\SnapGate.Framework.AzureBlobTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\AzureQueueTrigger\bin\Debug\SnapGate.Framework.AzureQueueTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\AzureTopicTrigger\bin\Debug\SnapGate.Framework.AzureTopicTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\BULKSQLServerTrigger\bin\Debug\SnapGate.Framework.BulksqlServerTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\CSharpTrigger\bin\Debug\SnapGate.Framework.CSharpTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\ETW\bin\Debug\SnapGate.Framework.EtwTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\EventHubsTrigger\bin\Debug\SnapGate.Framework.EventHubsTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\EventViewerTrigger\bin\Debug\SnapGate.Framework.EventViewerTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\FileTrigger\bin\Debug\SnapGate.Framework.FileTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\NOPTrigger\bin\Debug\SnapGate.Framework.NopTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\PowershellTrigger\bin\Debug\SnapGate.Framework.PowerShellTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\RfidTrigger\bin\Debug\SnapGate.Framework.RfidTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SQLServerTrigger\bin\Debug\SnapGate.Framework.SqlServerTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\ChatTrigger\bin\Debug\SnapGate.Framework.ChatTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\EmbeddedTrigger\bin\Debug\SnapGate.Framework.EmbeddedTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.RunProcess\bin\Debug\SnapGate.Framework.RunProcess.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\HM.OMS.PageOneMessageTrigger\bin\Debug\HM.OMS.PageOneMessageTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\HM.OMS.PageOneMessageRESTTrigger\bin\Debug\HM.OMS.PageOneMessageRESTTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.DynamicRESTTrigger\bin\Debug\SnapGate.Framework.DynamicRESTTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.WCFTrigger\bin\Debug\SnapGate.Framework.WCFTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.BenchmarkTrigger\bin\Debug\SnapGate.Framework.BenchmarkTrigger.pdb Framework\bin\Debug\Root_SnapGate\Triggers\* /y

cd %~dp0