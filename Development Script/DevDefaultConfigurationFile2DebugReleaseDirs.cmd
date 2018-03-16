echo off
cd..
copy ..\DefaultFiles\DevDefault.cfg Framework\bin\Debug\SnapGate.cfg /y
copy ..\DefaultFiles\DevDefault.cfg Framework\bin\Release\SnapGate.cfg /y
copy ..\DefaultFiles\DevDefault.cfg Laboratory.ConsoleEmbedded\bin\Debug\SnapGate.Laboratory.ConsoleEmbedded.cfg /y
copy ..\DefaultFiles\DevDefault.cfg Laboratory.ConsoleEmbedded\bin\Release\SnapGate.Laboratory.ConsoleEmbedded.cfg /y
cd %~dp0