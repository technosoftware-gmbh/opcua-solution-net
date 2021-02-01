rem @echo off
setlocal

SET PATH=..\..\..\bin\net462;%PATH%;

echo Building ModelDesign
Technosoftware.UaModelCompiler.exe -version v104 -d2 ".\Model\ModelDesign.xml" -cg ".\Model\ModelDesign.csv" -o2 ".\Model"
echo Success!

pause
