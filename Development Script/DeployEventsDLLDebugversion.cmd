echo off
cd..
xcopy Events\AzureBlobEvent\bin\Debug\SnapGate.Framework.AzureBlobEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\AzureQueueEvent\bin\Debug\SnapGate.Framework.AzureQueueEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\AzureTopicEvent\bin\Debug\SnapGate.Framework.AzureTopicEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\BULKSQLServerEvent\bin\Debug\SnapGate.Framework.BulksqlServerEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\CSharpEvent\bin\Debug\SnapGate.Framework.CSharpEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\DialogBoxEvent\bin\Debug\SnapGate.Framework.DialogBoxEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\EventHubEvent\bin\Debug\SnapGate.Framework.EventHubEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\FileEvent\bin\Debug\SnapGate.Framework.FileEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\MessageBoxEvent\bin\Debug\SnapGate.Framework.MessageBoxEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\NOPEvent\bin\Debug\SnapGate.Framework.NOPEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\PowershellEvent\bin\Debug\SnapGate.Framework.PowerShellEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\RunProcessEvent\bin\Debug\SnapGate.Framework.RunProcessEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\TwilioEvent\bin\Debug\SnapGate.Framework.TwilioEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\ChatEvent\bin\Debug\SnapGate.Framework.ChatEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\EmbeddedEvent\bin\Debug\SnapGate.Framework.EmbeddedEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\HTTPSendContentEvent\bin\Debug\SnapGate.Framework.HTTPSendContentEvent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y

xcopy Events\AzureBlobEvent\bin\Debug\SnapGate.Framework.AzureBlobEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\AzureQueueEvent\bin\Debug\SnapGate.Framework.AzureQueueEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\AzureTopicEvent\bin\Debug\SnapGate.Framework.AzureTopicEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\BULKSQLServerEvent\bin\Debug\SnapGate.Framework.BulksqlServerEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\CSharpEvent\bin\Debug\SnapGate.Framework.CSharpEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\DialogBoxEvent\bin\Debug\SnapGate.Framework.DialogBoxEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\EventHubEvent\bin\Debug\SnapGate.Framework.EventHubEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\FileEvent\bin\Debug\SnapGate.Framework.FileEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\MessageBoxEvent\bin\Debug\SnapGate.Framework.MessageBoxEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\NOPEvent\bin\Debug\SnapGate.Framework.NOPEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\PowershellEvent\bin\Debug\SnapGate.Framework.PowerShellEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\RunProcessEvent\bin\Debug\SnapGate.Framework.RunProcessEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\TwilioEvent\bin\Debug\SnapGate.Framework.TwilioEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\ChatEvent\bin\Debug\SnapGate.Framework.ChatEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\EmbeddedEvent\bin\Debug\SnapGate.Framework.EmbeddedEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y
xcopy Events\HTTPSendContentEvent\bin\Debug\SnapGate.Framework.HTTPSendContentEvent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Events\* /y



cd %~dp0