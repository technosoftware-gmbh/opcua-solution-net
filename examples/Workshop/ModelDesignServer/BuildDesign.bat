rem @echo off
setlocal

SET PATH=..\..\..\Scripts;..\..\..\bin\net462;..\..\..\..\bin\net462;%PATH%;

echo Building OperationsDesign
Technosoftware.UaModelCompiler.exe -version v104 -d2 ".\Model\OperationsDesign.xml" -cg ".\Model\OperationsDesign.csv" -o2 ".\Model"
echo Success!

echo Building EngineeringDesign
Technosoftware.UaModelCompiler.exe -version v104 -d2 ".\Model\EngineeringDesign.xml" -cg ".\Model\EngineeringDesign.csv" -o2 ".\Model"
echo Success!

echo Building ModelDesign
Technosoftware.UaModelCompiler.exe -version v104 -d2 ".\Model\ModelDesign.xml" -cg ".\Model\ModelDesign.csv" -o2 ".\Model"
echo Success!

pause
