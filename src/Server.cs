using System.Net;
using System.Net.Sockets;

const string PONG = "+PONG\r\n";
Console.WriteLine("Program started!");

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
//server.AcceptSocket();

try
{
    var bytes = new byte[256];
    while (true)
    {
        Console.WriteLine("Waiting for connection...");
        // Perform a blocking call to accept requests.
        // You could also use server.AcceptSocket() here.
        using TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected!");

        var stream = client.GetStream();


        // Loop to receive all the data sent by the client.
        var byteSize = 0;
        while ((byteSize = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            string request = System.Text.Encoding.ASCII.GetString(bytes, 0, byteSize);
            request = request.ToUpper();
            Console.WriteLine("Received: {0}", request);

            var response = System.Text.Encoding.ASCII.GetBytes(PONG);

            stream.Write(response, 0, response.Length);
            Console.WriteLine("Sent: {0}", response);
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
