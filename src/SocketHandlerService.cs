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
        private readonly Dictionary<string, Value<string>> data = new();


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

            var newValue = args.Find(a => a.ToLower().Equals("px")) != null
                    ? new Value<string>(args[2], Convert.ToDouble(args[4])) 
                    : new Value<string>(args[2]);

            if (alreadyExists)
            {
                data[args[1]] = newValue;
            }
            else
            {
                data.TryAdd(args[1], newValue);
            }
            response = Responses.Ok();

            return response;
        }

        public string Get(List<string> args)
        {
            if (args.Count < 2) return "Invalid params".ToRedisSimpleString(); ;

            var found = data.TryGetValue(args[1], out var foundValue);

            if (!found) return Responses.NULLBUCK();

            if (foundValue!.ExpiresIn is not null && foundValue.ExpiresIn < DateTime.UtcNow) return Responses.NULLBUCK();

            return foundValue!.value.ToBulk();
        }
    }
}
