@echo off
setlocal

echo Building TestData
..\..\..\bin\modelcompiler\Opc.Ua.ModelCompiler.exe compile -version v104 -d2 ".\TestData\TestDataDesign.xml" -cg ".\TestData\TestDataDesign.csv" -o2 ".\TestData"
echo Success!

pause
