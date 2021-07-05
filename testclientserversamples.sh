#!/bin/bash
echo Test the .NET Core Server and .NET Core Client
workdir=$(pwd)
sampleclientresult=0
sampleclientreverseresult=0
sampleclienthttpsresult=0
sampleserverresult=0
cd examples/Simulation/SampleServer
echo build SampleServer
rm -r obj
dotnet build SampleCompany.SampleServer.csproj
cd ../SampleClient
echo build SampleClient
rm -r obj
dotnet build SampleCompany.SampleClient.csproj
cd "$workdir"

cd examples/Simulation/SampleServer
echo start SampleServer with reverse connection enabled
touch ./SampleServer.log
dotnet run --no-restore --no-build --project SampleCompany.SampleServer.csproj -- -t 120 -a -rc=opc.tcp://localhost:55555 >./SimpleServer.log &
sampleserverpid="$!"
echo wait for SampleServer to be started
grep -m 1 "start" <(tail -f ./SampleServer.log --pid=$sampleserverpid)
tail -f ./SampleServer.log --pid=$sampleserverpid &
cd "$workdir"

#cd examples/Simulation/SampleClient
#echo start SampleClient for tcp connection with reverse connection
#dotnet run --no-restore --no-build --project SampleCompany.SampleClient.csproj -- -t 40 -a -rc=opc.tcp://localhost:55555 &
#sampleclientreversepid="$!"
#cd "$workdir"

cd examples/Simulation/SampleClient
echo start SampleClient for tcp connection
dotnet run --no-restore --no-build --project SampleCompany.SampleClient.csproj -- -t 20 -a &
sampleclientpid="$!"
cd "$workdir"

#cd examples/Simulation/SampleClient
#echo start SampleClient for https connection
#dotnet run --no-restore --no-build --project SampleCompany.SampleClient.csproj -- -t 20 -a https://localhost:55551 &
#httpsclientpid="$!"
#cd "$workdir"

echo wait for SampleClient
wait $sampleclientpid
if [ $? -eq 0 ]; then
	echo "SUCCESS - SampleClient test passed"
else
	sampleclientresult=$?
	echo "FAILED - SampleClient test failed with a status of $sampleclientresult"
fi

cd examples/Simulation/SampleClient
cd "$workdir"

#echo wait for SampleClient with reverse connection
#wait $sampleclientreversepid
#if [ $? -eq 0 ]; then
#	echo "SUCCESS - SampleClient with reverse connection test passed"
#else
#	sampleclientreverseresult=$?
#	echo "FAILED - SimpleClient with reverse connection test failed with a status of $sampleclientreverseresult"
#fi

#cd examples/Simulation/SampleClient
#cd "$workdir"

#echo wait for SampleClient with https connection
#wait $httpsclientpid
#if [ $? -eq 0 ]; then
#	echo "SUCCESS - SampleClient with https connection test passed"
#else
#	sampleclienthttpsresult=$?
#	echo "FAILED - Client test failed with a status of $sampleclienthttpsresult"
#	echo "FAILED - Client may require to use trusted TLS server cert to pass this test"
#	if [[ "$TRAVIS_OS_NAME" == "osx" ]]; then 
#		sampleclienthttpsresult=0 
#		echo "IGNORED - test requires trusted TLS cert on OSX"
#	fi
#fi

echo send Ctrl-C to SampleServer
kill -s SIGINT $sampleserverpid

echo wait for SampleServer
wait $sampleserverpid
sampleserverresult=$?

if [ $? -eq 0 ]; then
	echo "SUCCESS - SampleServer test passed"
else
	sampleserverresult=$?
	echo "FAILED - SampleServer test failed with a status of $sampleserverresult"
fi

echo "Test results: SampleClient:$sampleclientresult SampleClientReverse:$sampleclientreverseresult SampleClienthttps:$sampleclienthttpsresult SampleServer:$sampleserverresult"
exit $((sampleclientresult + sampleclientreverseresult + sampleclienthttpsresult + sampleserverresult))



