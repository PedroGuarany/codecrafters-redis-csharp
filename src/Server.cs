using System.Net;
using System.Net.Sockets;

const string PONG = "+PONG\r\n";
var callbackQueue = new Queue<Action>();
Console.WriteLine("Program started!");

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

callbackQueue.Enqueue(() => { Console.WriteLine("Testando"); });
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

static void HandleSocketConnection(Socket socket)
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
            request = request.ToUpper();

            var response = System.Text.Encoding.ASCII.GetBytes(PONG);

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
