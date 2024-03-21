using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    public static class RESPParser
    {
        private static readonly string paramsLengthChar = "*";
        private static readonly string wordLengthChar = "$";
        private static readonly string simpleStringChar = "+";
        private static readonly string breakLineChar = "\r\n";
        private static readonly string[] ignoredCommands = new string[] { "info", "quit" };

        public static string ToRedisSimpleString(this string value)
        {
            return $"{simpleStringChar}{value}{breakLineChar}";
        }

        public static string ToBulk(this string value)
        {
            var length = value.Length > 0 ? value.Length : -1;
            return $"${length}{(length > 0 ? breakLineChar : "")}{value}{breakLineChar}";
        }

        public static List<string> FromResp(this string value)
        {
            var respArray = value.Split(breakLineChar);
            var result = new List<string>();

            result = respArray
                        .Where(r => !string.IsNullOrEmpty(r))
                        .Where(r => ignoredCommands.FirstOrDefault(ignored => r.Equals(ignored)) == null)
                        .Where(r => !r.StartsWith(paramsLengthChar) && !r.StartsWith(wordLengthChar))
                        .ToList();
            return result;
        }

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }
    }
}
