echo off
cls
echo Prepare the BizTalk environment before test.
pause

call PrepareDeployPackage.cmd
cd %~dp0
cd..


pause
xcopy Setup\bin\Debug\Deploy\*.* "C:\Program Files (x86)\Microsoft BizTalk Server 2013 R2\*" /y

cd %~dp0
