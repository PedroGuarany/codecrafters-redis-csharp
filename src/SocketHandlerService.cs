using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    public static class SocketHandlerService
    {
        public static List<string> GetRequestInfo(Socket socket)
        {
            var bytes = new byte[256];
            var byteSize = socket.Receive(bytes, SocketFlags.None);

            string request = Encoding.ASCII.GetString(bytes, 0, byteSize);
            return request.FromResp();
        }

        public static string Ping()
        {
            return "PONG".ToRedisSimpleString();
        }

        public static string Echo(List<string> param)
        {
            var response = "Invalid params".ToRedisSimpleString();
            if (param.ToList().Count >= 2)
            {
                var bulkRequestEcho = param[1].ToBulk();
                response = bulkRequestEcho;
            }

            return response;
        }
    }
}
