echo off
cd..
mkdir Framework\bin\Release\Root_SnapGate\Events
copy Events\AzureBlobEvent\bin\Release\SnapGate.Framework.AzureBlobEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\AzureQueueEvent\bin\Release\SnapGate.Framework.AzureQueueEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\AzureTopicEvent\bin\Release\SnapGate.Framework.AzureTopicEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\BULKSQLServerEvent\bin\Release\SnapGate.Framework.BulksqlServerEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\CSharpEvent\bin\Release\SnapGate.Framework.CSharpEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\DialogBoxEvent\bin\Release\SnapGate.Framework.DialogBoxEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\EventHubEvent\bin\Release\SnapGate.Framework.EventHubEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\FileEvent\bin\Release\SnapGate.Framework.FileEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\MessageBoxEvent\bin\Release\SnapGate.Framework.MessageBoxEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\NOPEvent\bin\Release\SnapGate.Framework.NopEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\PowershellEvent\bin\Release\SnapGate.Framework.PowerShellEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\TwilioEvent\bin\Release\SnapGate.Framework.TwilioEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
copy Events\ChatEvent\bin\Release\SnapGate.Framework.ChatEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
xcopy Events\EmbeddedEvent\bin\Release\SnapGate.Framework.EmbeddedEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
xcopy Events\HM.OMS.PageOneMessageEvent\bin\Release\HM.OMS.PageOneMessageEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y
xcopy Events\HTTPSendContentEvent\bin\Release\SnapGate.Framework.HTTPSendContentEvent.dll Framework\bin\Release\Root_SnapGate\Events\* /y

cd %~dp0