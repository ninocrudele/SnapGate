echo off
cd..
xcopy Events\AzureBlobEvent\bin\Release\SnapGate.Framework.AzureBlobEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\AzureQueueEvent\bin\Release\SnapGate.Framework.AzureQueueEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\AzureTopicEvent\bin\Release\SnapGate.Framework.AzureTopicEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\BULKSQLServerEvent\bin\Release\SnapGate.Framework.BulksqlServerEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\CSharpEvent\bin\Release\SnapGate.Framework.CSharpEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\DialogBoxEvent\bin\Release\SnapGate.Framework.DialogBoxEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\EventHubEvent\bin\Release\SnapGate.Framework.EventHubEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\FileEvent\bin\Release\SnapGate.Framework.FileEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\MessageBoxEvent\bin\Release\SnapGate.Framework.MessageBoxEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\NOPEvent\bin\Release\SnapGate.Framework.NOPEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\PowershellEvent\bin\Release\SnapGate.Framework.PowershellEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\RunProcessEvent\bin\Release\SnapGate.Framework.RunProcessEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\TwilioEvent\bin\Release\SnapGate.Framework.TwilioEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\ChatEvent\bin\Release\SnapGate.Framework.ChatEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\EmbeddedEvent\bin\Release\SnapGate.Framework.EmbeddedEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y
xcopy Events\HTTPSendContentEvent\bin\Release\SnapGate.Framework.HTTPSendContentEvent.dll Setup\bin\Release\Deploy\Root_SnapGate\Events\* /y

cd %~dp0