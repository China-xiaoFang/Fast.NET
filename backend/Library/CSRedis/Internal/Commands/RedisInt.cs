using CSRedis.Internal.IO;

namespace CSRedis.Internal.Commands;

class RedisInt : RedisCommand<long>
{
    public RedisInt(string command, params object[] args) : base(command, args)
    {
    }

    public override long Parse(RedisReader reader)
    {
        return reader.ReadInt();
    }

    public class Nullable : RedisCommand<long?>
    {
        public Nullable(string command, params object[] args) : base(command, args)
        {
        }


        public override long? Parse(RedisReader reader)
        {
            var type = reader.ReadType();
            if (type == RedisMessage.Int)
                return reader.ReadInt(false);
            reader.ReadBulkString(false);
            return null;
        }
    }
}