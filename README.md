# graph-chat-app
A simple client-server chat app, that remembers and displays which message replies to which.
## Cloning
See the instructions provided above, under the "Code" button.
## Building
### With Visual Studio
Required: Visual Studio 2019, with "classic applications for .NET" features installed.
Clone the repository. Open the solution, by double clicking on the solution file (graph-chat-app.sln).
When project is loaded locate the dropdown on the top, saying "Debug" or "Release" and make sure it's in Release.
Locate Solution 'graph-chat-app' in Solution Explorer, right click it and select Build Solution.
Your binaries should now be located in:
Server under `.\graph-chat-app\ChatServer\bin\Release\net5.0-windows\ChatServer.exe`
Client under `.\graph-chat-app\ChatClient\bin\Release\net5.0-windows\ChatClient.exe`
### With `dotnet` cli
Navigate to project root
Type: `dotnet build --configuration Release`
Your binaries should now be located in:
Server under `.\graph-chat-app\ChatServer\bin\Release\net5.0-windows\ChatServer.exe`
Client under `.\graph-chat-app\ChatClient\bin\Release\net5.0-windows\ChatClient.exe`
## Running the application
### Server
Open the application in any terminal.
Program has 3 **required** positional parameters:
1. IPV4 Address of the network interface, on which the server should operate
2. Port number (50000 is recommended)
3. Number of clients to be served simultaneously
After launching program runs continuously, until interrupted, displaying logs.
### Client
Launch with 2 **required** positional parameters:
1. IPV4 Address or domain name of the **server** that it attempts to connect to
2. Port number (50000 is recommended)
After launching program displays further usage instructions.