rem @echo off
setlocal

SET PATH=..\..\..\bin\modelcompiler;%PATH%;

echo Building DataTypes
Opc.Ua.ModelCompiler.exe compile -version v104 -d2 ".\SampleDataTypes\SampleDataTypesModelDesign.xml" -cg ".\SampleDataTypes\SampleDataTypesModelDesign.csv" -o2 ".\SampleDataTypes"
echo Success!

pause
