echo off
cd..
mkdir Framework\bin\Debug\Root_SnapGate\Events

REM COPY DLL
copy Events\AzureBlobEvent\bin\Debug\SnapGate.Framework.AzureBlobEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\AzureQueueEvent\bin\Debug\SnapGate.Framework.AzureQueueEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\AzureTopicEvent\bin\Debug\SnapGate.Framework.AzureTopicEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\BULKSQLServerEvent\bin\Debug\SnapGate.Framework.BulksqlServerEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\CSharpEvent\bin\Debug\SnapGate.Framework.CSharpEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\DialogBoxEvent\bin\Debug\SnapGate.Framework.DialogBoxEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\EventHubEvent\bin\Debug\SnapGate.Framework.EventHubEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\FileEvent\bin\Debug\SnapGate.Framework.FileEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\MessageBoxEvent\bin\Debug\SnapGate.Framework.MessageBoxEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\NOPEvent\bin\Debug\SnapGate.Framework.NopEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\PowershellEvent\bin\Debug\SnapGate.Framework.PowerShellEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\TwilioEvent\bin\Debug\SnapGate.Framework.TwilioEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\ChatEvent\bin\Debug\SnapGate.Framework.ChatEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
xcopy Events\EmbeddedEvent\bin\Debug\SnapGate.Framework.EmbeddedEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
xcopy Events\HTTPSendContentEvent\bin\Debug\SnapGate.Framework.HTTPSendContentEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y
xcopy Events\HM.OMS.PageOneMessageEvent\bin\Debug\HM.OMS.PageOneMessageEvent.dll Framework\bin\Debug\Root_SnapGate\Events\* /y

REM COPY PDB
copy Events\AzureBlobEvent\bin\Debug\SnapGate.Framework.AzureBlobEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\AzureQueueEvent\bin\Debug\SnapGate.Framework.AzureQueueEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\AzureTopicEvent\bin\Debug\SnapGate.Framework.AzureTopicEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\BULKSQLServerEvent\bin\Debug\SnapGate.Framework.BulksqlServerEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\CSharpEvent\bin\Debug\SnapGate.Framework.CSharpEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\DialogBoxEvent\bin\Debug\SnapGate.Framework.DialogBoxEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\EventHubEvent\bin\Debug\SnapGate.Framework.EventHubEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\FileEvent\bin\Debug\SnapGate.Framework.FileEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\MessageBoxEvent\bin\Debug\SnapGate.Framework.MessageBoxEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\NOPEvent\bin\Debug\SnapGate.Framework.NopEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\PowershellEvent\bin\Debug\SnapGate.Framework.PowerShellEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\TwilioEvent\bin\Debug\SnapGate.Framework.TwilioEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
copy Events\ChatEvent\bin\Debug\SnapGate.Framework.ChatEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
xcopy Events\EmbeddedEvent\bin\Debug\SnapGate.Framework.EmbeddedEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
xcopy Events\HTTPSendContentEvent\bin\Debug\SnapGate.Framework.HTTPSendContentEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
xcopy Events\HM.OMS.PageOneMessageEvent\bin\Debug\HM.OMS.PageOneMessageEvent.PDB Framework\bin\Debug\Root_SnapGate\Events\* /y
cd %~dp0