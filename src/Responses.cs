using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    public class Responses
    {
        public static string Pong()
        {
            return "PONG".ToRedisSimpleString();
        }

        public static string Ok()
        {
            return "OK".ToRedisSimpleString();
        }

        public static string NULLBUCK()
        {
            return "".ToBulk();
        }
    }
}
