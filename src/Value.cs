using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_redis.src
{
    public class Value<T>
    {
        public T value;
        public DateTime? ExpiresIn;

        public Value(T value)
        {
            this.value = value;
        }

        public Value(T value, double millisecondsToExpiry)
        {
            this.value = value;
            this.ExpiresIn = DateTime.UtcNow.AddMilliseconds(millisecondsToExpiry);
        }
    }
}
