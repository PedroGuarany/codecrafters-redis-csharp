using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    public class SocketHandlerService
    {
        private readonly Dictionary<string, string> data = new();


        public List<string> GetRequestInfo(Socket socket)
        {
            var bytes = new byte[256];
            var byteSize = socket.Receive(bytes, SocketFlags.None);

            string request = Encoding.ASCII.GetString(bytes, 0, byteSize);
            return request.FromResp();
        }

        public string Echo(List<string> args)
        {
            var response = "Invalid params".ToRedisSimpleString();
            if (args.Count < 2) return response;

            var bulkRequestEcho = args[1].ToBulk();
            response = bulkRequestEcho;

            return response;
        }

        public string Set(List<string> args)
        {
            var response = "Invalid params".ToRedisSimpleString();
            if (args.Count < 3) return response;

            var alreadyExists = data.TryGetValue(args[1], out var oldValue);

            if (alreadyExists)
            {
                data[args[1]] = data[args[2]];
            }
            else
            {
                data.TryAdd(args[1], args[2]);
            }
            response = Responses.Ok();

            return response;
        }

        public string Get(List<string> args)
        {
            var response = "Invalid params".ToRedisSimpleString();
            if (args.Count < 2) return response;

            var found = data.TryGetValue(args[1], out var foundValue);
            response = found ? foundValue!.ToBulk() : Responses.NULLBUCK();

            return response;
        }
    }
}
