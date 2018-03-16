echo off
cd..
mkdir Framework\bin\Release\Root_SnapGate\Triggers
copy Triggers\AzureBlobTrigger\bin\Release\SnapGate.Framework.AzureBlobTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\AzureQueueTrigger\bin\Release\SnapGate.Framework.AzureQueueTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\AzureTopicTrigger\bin\Release\SnapGate.Framework.AzureTopicTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\BULKSQLServerTrigger\bin\Release\SnapGate.Framework.BulksqlServerTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\CSharpTrigger\bin\Release\SnapGate.Framework.CSharpTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\ETW\bin\Release\SnapGate.Framework.EtwTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\EventHubsTrigger\bin\Release\SnapGate.Framework.EventHubsTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\EventViewerTrigger\bin\Release\SnapGate.Framework.EventViewerTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\FileTrigger\bin\Release\SnapGate.Framework.FileTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\NOPTrigger\bin\Release\SnapGate.Framework.NopTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\PowershellTrigger\bin\Release\SnapGate.Framework.PowershellTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\RfidTrigger\bin\Release\SnapGate.Framework.RfidTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\SQLServerTrigger\bin\Release\SnapGate.Framework.SqlServerTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\ChatTrigger\bin\Release\SnapGate.Framework.ChatTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\EmbeddedTrigger\bin\Release\SnapGate.Framework.EmbeddedTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.RunProcess\bin\Release\SnapGate.Framework.RunProcess.pdb Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\HM.OMS.PageOneMessageTrigger\bin\Release\HM.OMS.PageOneMessageTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\HM.OMS.PageOneMessageRESTTrigger\bin\Release\HM.OMS.PageOneMessageRESTTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.DynamicRESTTrigger\bin\Release\SnapGate.Framework.DynamicRESTTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.WCFTrigger\bin\Release\SnapGate.Framework.WCFTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y
copy Triggers\SnapGate.Framework.BenchmarkTrigger\bin\Release\SnapGate.Framework.BenchmarkTrigger.dll Framework\bin\Release\Root_SnapGate\Triggers\* /y

cd %~dp0