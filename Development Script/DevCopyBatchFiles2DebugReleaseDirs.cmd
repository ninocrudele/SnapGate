echo off
cd..
copy DefaultFiles\CreateNewClone.cmd Framework\bin\Debug\* /y
copy DefaultFiles\CreateNewClone.cmd Framework\bin\Release\* /y
cd %~dp0