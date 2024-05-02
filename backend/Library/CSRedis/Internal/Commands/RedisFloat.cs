using System.Globalization;
using CSRedis.Internal.IO;

namespace CSRedis.Internal.Commands;

class RedisFloat : RedisCommand<decimal>
{
    public RedisFloat(string command, params object[] args) : base(command, args)
    {
    }

    public override decimal Parse(RedisReader reader)
    {
        return FromString(reader.ReadBulkString());
    }

    static decimal FromString(string input)
    {
        return decimal.Parse(input, NumberStyles.Any);
    }

    public class Nullable : RedisCommand<decimal?>
    {
        public Nullable(string command, params object[] args) : base(command, args)
        {
        }

        public override decimal? Parse(RedisReader reader)
        {
            var result = reader.ReadBulkString();
            if (string.IsNullOrEmpty(result))
                return null;
            return FromString(result);
        }
    }
}