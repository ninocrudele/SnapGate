echo off
cd..
xcopy ..\DefaultFiles\Bubbling\* Framework\bin\Debug\Root_SnapGate\Bubbling\ /s /y
xcopy ..\DefaultFiles\Bubbling\* Framework\bin\Release\Root_SnapGate\Bubbling\ /s /y
cd %~dp0