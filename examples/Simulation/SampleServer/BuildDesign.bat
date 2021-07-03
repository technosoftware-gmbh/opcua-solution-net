rem @echo off
setlocal

SET PATH=..\..\..\bin\modelcompiler;%PATH%;

echo Building ModelDesign
Opc.Ua.ModelCompiler.exe -console -version v104 -d2 ".\Model\ModelDesign.xml" -cg ".\Model\ModelDesign.csv" -o2 ".\Model"
echo Success!

pause
