echo off
cd..
mkdir Framework\bin\Debug\Root_SnapGate\Components
mkdir Framework\bin\Release\Root_SnapGate\Components

REM COPY DLL

copy Components\SnapGate.Framework.BTSPipelineComponent\bin\Debug\SnapGate.Framework.BTSPipelineComponent.dll Framework\bin\Debug\Root_SnapGate\Components\* /y
copy Components\SnapGate.Framework.BTSTransformComponent\bin\Debug\SnapGate.Framework.BTSTransformComponent.dll Framework\bin\Debug\Root_SnapGate\Components\* /y

copy Components\SnapGate.Framework.BTSPipelineComponent\bin\Release\SnapGate.Framework.BTSPipelineComponent.dll Framework\bin\Release\Root_SnapGate\Components\* /y
copy Components\SnapGate.Framework.BTSTransformComponent\bin\Release\SnapGate.Framework.BTSTransformComponent.dll Framework\bin\Release\Root_SnapGate\Components\* /y

REM COPY PDB
copy Components\SnapGate.Framework.BTSPipelineComponent\bin\Debug\SnapGate.Framework.BTSPipelineComponent.pdb Framework\bin\Debug\Root_SnapGate\Components\* /y
copy Components\SnapGate.Framework.BTSTransformComponent\bin\Debug\SnapGate.Framework.BTSTransformComponent.pdb Framework\bin\Debug\Root_SnapGate\Components\* /y

cd %~dp0