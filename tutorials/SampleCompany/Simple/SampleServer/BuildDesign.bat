rem @echo off
setlocal

set MODELCOMPILER=Opc.Ua.ModelCompiler.exe
SET PATH=..\..\..\..\bin\modelcompiler;%PATH%;
set MODELROOT=.

echo Building ModelDesign
%MODELCOMPILER% compile -version v104 -d2 ".\Model\ModelDesign.xml" -cg ".\Model\ModelDesign.csv" -o2 ".\Model"
echo Success!

pause
