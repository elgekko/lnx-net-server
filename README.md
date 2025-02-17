# EchoServer

This is a simple program that will listen on a specified TCP port and echo back the message to the sender.

### Usage

The program takes a single argument which is the port number. This should be a valid port number 
in the range 1-65535. Example: EchoServer 50000

### Build

dotnet build /work/lnx-net-server/EchoServer.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary -c Release

### Start

dotnet /work/lnx-net-server/bin/Release/net8.0/EchoServer.dll 50000

### Build selfcontained

dotnet publish -c Release -r linux-x64 --self-contained true

or

dotnet publish -c Release -r linux-x64 -p:PublishAot=true

# testclient.py

This is a test client app for server, written in python. Sends and receives sample message.

### Start

python testclient.py
