#!/bin/bash
echo Test .NET 6.0 Reference Server and Refererence Client
workdir=$(pwd)
referenceclientresult=0
referenceserverresult=0
cd examples/SampleCompany/ReferenceServer
echo build ReferenceServer
rm -r obj
dotnet build SampleCompany.ReferenceServer.csproj -f net6.0
cd ../ReferenceClient
echo build ReferenceClient
rm -r obj
dotnet build SampleCompany.ReferenceClient.csproj -f net6.0
cd "$workdir"

cd examples/SampleCompany/ReferenceServer
echo start ReferenceServer
touch ./ReferenceServer.log
dotnet run --framework net6.0 --no-restore --no-build --project SampleCompany.ReferenceServer.csproj -- -t 120 -a >./ReferenceServer.log &
referenceserverpid="$!"
echo wait for ReferenceServer to be started
grep -m 1 "start" <(tail -f ./ReferenceServer.log --pid=$referenceserverpid)
tail -f ./ReferenceServer.log --pid=$referenceserverpid &
cd "$workdir"

cd examples/SampleCompany/ReferenceClient
echo start ReferenceClient for tcp connection
dotnet run --framework net6.0 --no-restore --no-build --project SampleCompany.ReferenceClient.csproj -- -t 20 -a &
referenceclientpid="$!"
cd "$workdir"

echo wait for ReferenceClient
wait $referenceclientpid
if [ $? -eq 0 ]; then
	echo "SUCCESS - ReferenceClient test passed"
else
	referenceclientresult=$?
	echo "FAILED - ReferenceClient test failed with a status of $referenceclientresult"
fi

cd "$workdir"

echo send Ctrl-C to ReferenceServer
kill -s SIGINT $referenceserverpid

echo wait for ReferenceServer
wait $referenceserverpid
referenceserverresult=$?

if [ $? -eq 0 ]; then
	echo "SUCCESS - ReferenceServer test passed"
else
	referenceserverresult=$?
	echo "FAILED - ReferenceServer test failed with a status of $referenceserverresult"
fi

echo "Test results: ReferenceClient:$referenceclientresult ReferenceServer:$referenceserverresult"
exit $((referenceclientresult + referenceserverresult))



