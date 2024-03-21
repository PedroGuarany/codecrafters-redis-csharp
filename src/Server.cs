using System.Net;
using System.Net.Sockets;

const string PONG = "+PONG\r\n";
Console.WriteLine("Program started!");

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

try
{
    var bytes = new byte[256];
    using var socket = server.AcceptSocket();
    while (socket.Connected)
    {
        var byteSize = await socket.ReceiveAsync(bytes, SocketFlags.None);

        if (byteSize == 0) break;

        string request = System.Text.Encoding.ASCII.GetString(bytes, 0, byteSize);
        request = request.ToUpper();
        Console.WriteLine("Received: {0}", request);

        var response = System.Text.Encoding.ASCII.GetBytes(PONG);

        await socket.SendAsync(response, SocketFlags.None);
        Console.WriteLine("Sent: {0}", response);
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
