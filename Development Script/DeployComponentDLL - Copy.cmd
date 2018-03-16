echo off
cd..

ECHO COPY DLL
xcopy Components\SnapGate.Framework.BTSPipelineComponent\bin\Debug\SnapGate.Framework.BTSPipelineComponent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Components\* /y
xcopy Components\SnapGate.Framework.BTSPipelineComponent\bin\Release\SnapGate.Framework.BTSPipelineComponent.dll Setup\bin\Release\Deploy\Root_SnapGate\Components\* /y

xcopy Components\SnapGate.Framework.BTSTransformComponent\bin\Debug\SnapGate.Framework.BTSTransformComponent.dll Setup\bin\Debug\Deploy\Root_SnapGate\Components\* /y
xcopy Components\SnapGate.Framework.BTSTransformComponent\bin\Release\SnapGate.Framework.BTSTransformComponent.dll Setup\bin\Release\Deploy\Root_SnapGate\Components\* /y

ECHO COPY PDB
xcopy Components\SnapGate.Framework.BTSPipelineComponent\bin\Debug\SnapGate.Framework.BTSPipelineComponent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Components\* /y
xcopy Components\SnapGate.Framework.BTSTransformComponent\bin\Debug\SnapGate.Framework.BTSTransformComponent.pdb Setup\bin\Debug\Deploy\Root_SnapGate\Components\* /y

cd %~dp0