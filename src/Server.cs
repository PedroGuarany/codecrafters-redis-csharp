using codecrafters_redis.src;
using System.Net;
using System.Net.Sockets;

string PONG = "PONG".ToRedisSimpleString();

var callbackQueue = new Queue<Action>();
Console.WriteLine("Program started!");

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

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
            var bytes = new byte[256];
            var byteSize = socket.Receive(bytes, SocketFlags.None);

            if (byteSize == 0) break;

            string request = System.Text.Encoding.ASCII.GetString(bytes, 0, byteSize);
            var requestInfo = request.FromResp();
            byte[]? response = PONG.ToByteArray();

            if (requestInfo.Find(r => r.ToLower().Equals("ping")) != null)
            {
                response = PONG.ToByteArray();
            }
            else if (requestInfo.Find(r => r.ToLower().Equals("echo")) != null)
            {
                if (requestInfo.Count >= 2)
                {
                    var bulkRequestEcho = requestInfo[1].ToBulk();
                    response = bulkRequestEcho.ToByteArray();
                }
                else {
                    response = "Invalid params".ToRedisSimpleString().ToByteArray();
                }
            }
            
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