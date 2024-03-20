using System.Net;
using System.Net.Sockets;

Console.WriteLine("Program started!");

var TCPListener = new TcpListener(IPAddress.Any, 6379);
TCPListener.Start();
TCPListener.AcceptSocket();