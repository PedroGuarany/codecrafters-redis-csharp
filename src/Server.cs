using codecrafters_redis.src;
using System.Net;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        var socketService = new SocketHandlerService();

        Dictionary<string, string> data = new();
        Dictionary<string, Func<List<string>, string>> commands = new()
            {
                { "ping", (_) => Responses.Pong()},
                { "echo", (args) => socketService.Echo(args) },
                { "set", (args) => socketService.Set(args) },
                { "get", (args) => socketService.Get(args)}
            };

        var port = 6379;
        if (args.Length > 0)
        {
            var paramPortIndex = args.ToList().FindIndex(a => a.ToLower().Equals("--port"));
            if (args.Length > paramPortIndex + 1)
            {
                port = Convert.ToInt32(args[paramPortIndex + 1]);
            }
        }
        Console.WriteLine(port);
        var server = new TcpListener(IPAddress.Any, port);
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
                    var requestInfo = socketService.GetRequestInfo(socket);
                    byte[]? response = Responses.Pong().ToByteArray();

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
    }

}