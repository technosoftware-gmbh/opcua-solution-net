#!/bin/bash
echo Test the .NET Core Server and .NET Core Client
workdir=$(pwd)
simpleclientresult=0
simpleclientreverseresult=0
simpleclienthttpsresult=0
simpleserverresult=0
modeldesignclientresult=0
modeldesignserverresult=0
cd examples/Workshop/SimpleServer
echo build SimpleServer
rm -r obj
dotnet build Technosoftware.SimpleServer.csproj
cd ../SimpleClient
echo build SimpleClient
rm -r obj
dotnet build Technosoftware.SimpleClient.csproj
cd "$workdir"

cd examples/Workshop/SimpleServer
echo start SimpleServer with reverse connection enabled
touch ./SimpleServer.log
dotnet run --no-restore --no-build --project Technosoftware.SimpleServer.csproj -- -t 120 -a -rc=opc.tcp://localhost:55555 >./SimpleServer.log &
simpleserverpid="$!"
echo wait for SimpleServer to be started
grep -m 1 "start" <(tail -f ./SimpleServer.log --pid=$simpleserverpid)
tail -f ./SimpleServer.log --pid=$simpleserverpid &
cd "$workdir"

cd examples/Workshop/SimpleClient
echo start SimpleClient for tcp connection with reverse connection
dotnet run --no-restore --no-build --project Technosoftware.SimpleClient.csproj -- -t 40 -a -rc=opc.tcp://localhost:55555 &
simpleclientreversepid="$!"
cd "$workdir"

cd examples/Workshop/SimpleClient
echo start SimpleClient for tcp connection
dotnet run --no-restore --no-build --project Technosoftware.SimpleClient.csproj -- -t 20 -a &
simpleclientpid="$!"
cd "$workdir"

cd examples/Workshop/SimpleClient
echo start SimpleClient for https connection
dotnet run --no-restore --no-build --project Technosoftware.SimpleClient.csproj -- -t 20 -a https://localhost:55551 &
httpsclientpid="$!"
cd "$workdir"

echo wait for SimpleClient
wait $simpleclientpid
if [ $? -eq 0 ]; then
	echo "SUCCESS - SimpleClient test passed"
else
	simpleclientresult=$?
	echo "FAILED - SimpleClient test failed with a status of $simpleclientresult"
fi

cd examples/Workshop/SimpleClient
cd "$workdir"

echo wait for SimpleClient with reverse connection
wait $simpleclientreversepid
if [ $? -eq 0 ]; then
	echo "SUCCESS - SimpleClient with reverse connection test passed"
else
	simpleclientreverseresult=$?
	echo "FAILED - SimpleClient with reverse connection test failed with a status of $simpleclientreverseresult"
fi

cd examples/Workshop/SimpleClient
cd "$workdir"

echo wait for SimpleClient with https connection
wait $httpsclientpid
if [ $? -eq 0 ]; then
	echo "SUCCESS - SimpleClient with https connection test passed"
else
	simpleclienthttpsresult=$?
	echo "FAILED - Client test failed with a status of $simpleclienthttpsresult"
	echo "FAILED - Client may require to use trusted TLS server cert to pass this test"
	if [[ "$TRAVIS_OS_NAME" == "osx" ]]; then 
		simpleclienthttpsresult=0 
		echo "IGNORED - test requires trusted TLS cert on OSX"
	fi
fi

echo send Ctrl-C to SimpleServer
kill -s SIGINT $simpleserverpid

echo wait for SimpleServer
wait $simpleserverpid
simpleserverresult=$?

if [ $? -eq 0 ]; then
	echo "SUCCESS - SimpleServer test passed"
else
	simpleserverresult=$?
	echo "FAILED - SimpleServer test failed with a status of $simpleserverresult"
fi

cd examples/Workshop/ModelDesignServer
echo build ModelDesignServer
rm -r obj
dotnet build Technosoftware.ModelDesignServer.csproj
cd ../ModelDesignClient
echo build ModelDesignClient
rm -r obj
dotnet build Technosoftware.ModelDesignClient.csproj
cd "$workdir"

cd examples/Workshop/ModelDesignServer
echo start ModelDesignServer
touch ./ModelDesignServer.log
dotnet run --no-restore --no-build --project Technosoftware.ModelDesignServer.csproj -- -t 60 -a >./ModelDesignServer.log &
modelserverpid="$!"
echo wait for ModelDesignServer to be started
grep -m 1 "start" <(tail -f ./ModelDesignServer.log --pid=$modelserverpid)
tail -f ./ModelDesignServer.log --pid=$modelserverpid &
cd "$workdir"

cd examples/Workshop/ModelDesignClient
echo start ModelDesignClient for tcp connection
dotnet run --no-restore --no-build --project Technosoftware.ModelDesignClient.csproj -- -t 10 -a &
modelclientpid="$!"
cd "$workdir"

echo wait for ModelDesignClient
wait $modelclientpid
if [ $? -eq 0 ]; then
	echo "SUCCESS - ModelDesignClient test passed"
else
	modeldesignclientresult=$?
	echo "FAILED - ModelDesignClient test failed with a status of $modeldesignclientresult"
fi

echo send Ctrl-C to ModelDesignServer
kill -s SIGINT $modelserverpid

echo wait for ModelDesignServer
wait $modelserverpid
modeldesignserverresult=$?

if [ $? -eq 0 ]; then
	echo "SUCCESS - ModelDesignServer test passed"
else
	modeldesignserverresult=$?
	echo "FAILED - ModelDesignServer test failed with a status of $modeldesignserverresult"
fi

echo "Test results: SimpleClient:$simpleclientresult SimpleClientReverse:$simpleclientreverseresult SimpleClienthttps:$simpleclienthttpsresult SimpleServer:$simpleserverresult ModelDesignClient:$modeldesignclientresult ModelDesignServer:$modeldesignclientresult"
exit $((simpleclientresult + simpleclientreverseresult + simpleclienthttpsresult + simpleserverresult + modeldesignclientresult + modeldesignserverresult))



