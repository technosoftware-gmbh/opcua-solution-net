rem @echo off
setlocal

set MODELCOMPILER=Opc.Ua.ModelCompiler.exe
SET PATH=..\..\..\bin\modelcompiler;%PATH%;
set MODELROOT=.

echo Building DataTypes
%MODELCOMPILER% compile -version v104 -d2 "%MODELROOT%/SampleDataTypes/SampleDataTypesModelDesign.xml" -cg "%MODELROOT%/SampleDataTypes/SampleDataTypesModelDesign.csv" -o2 "%MODELROOT%/SampleDataTypes"
IF %ERRORLEVEL% EQU 0 echo Success!

echo Building TestData
%MODELCOMPILER% compile -version v104 -id 1000 -d2 "%MODELROOT%/TestData/TestDataDesign.xml" -cg "%MODELROOT%/TestData/TestDataDesign.csv" -o2 "%MODELROOT%/TestData"
IF %ERRORLEVEL% EQU 0 echo Success!

echo Building MemoryBuffer
%MODELCOMPILER% compile -version v104 -id 1000 -d2 "%MODELROOT%/MemoryBuffer/MemoryBufferDesign.xml" -cg "%MODELROOT%/MemoryBuffer/MemoryBufferDesign.csv" -o2 "%MODELROOT%/MemoryBuffer" 
IF %ERRORLEVEL% EQU 0 echo Success!

pause
