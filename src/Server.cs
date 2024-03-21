using codecrafters_redis.src;
using System.Net;
using System.Net.Sockets;


byte[] PONG = "PONG".ToRedisSimpleString().ToByteArray();
string NULLBULK = "".ToBulk();

Dictionary<string, string> data = new();
Dictionary<string, Func<List<string>, string>> commands = new()
{
    { "ping", (_) => SocketHandlerService.Ping()},
    { "echo", (param) => SocketHandlerService.Echo(param) }
};
var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
Console.WriteLine("Program started!");

try
{
    while (true)
    {
        var socket = server.AcceptSocket();
        if (socket.Connected)
        {
            var newThread = new Thread(() => HandleSocketConnection(socket));
            newThread.Start();
        }
    }
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
finally
{
    server.Stop();
}

void HandleSocketConnection(Socket socket)
{
    try
    {
        Console.WriteLine("Connected");
        while (socket.Connected)
        {
            var requestInfo = SocketHandlerService.GetRequestInfo(socket);
            byte[]? response = PONG;

            if (requestInfo.Count < 1 || !commands.ContainsKey(requestInfo[0]))
            {
                socket.Send(response, SocketFlags.None);
                continue;
            }

            var command = commands.GetValueOrDefault(requestInfo[0]);
            if (command is null)
            {
                socket.Send(response, SocketFlags.None);
                continue;
            }


            response = command(requestInfo).ToByteArray();
            socket.Send(response, SocketFlags.None);
        }
    }
    catch (SocketException e)
    {
        Console.WriteLine("SocketException: {0}", e.Message);
    }
    finally
    {
        socket.Dispose();
    }
}