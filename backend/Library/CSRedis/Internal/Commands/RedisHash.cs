using CSRedis.Internal.IO;
using CSRedis.Internal.Utilities;

namespace CSRedis.Internal.Commands;

class RedisHash : RedisCommand<Dictionary<string, string>>
{
    public RedisHash(string command, params object[] args) : base(command, args)
    {
    }

    public override Dictionary<string, string> Parse(RedisReader reader)
    {
        return ToDict(reader);
    }

    static Dictionary<string, string> ToDict(RedisReader reader)
    {
        reader.ExpectType(RedisMessage.MultiBulk);
        var count = reader.ReadInt(false);
        var dict = new Dictionary<string, string>();
        var key = String.Empty;
        for (var i = 0; i < count; i++)
        {
            if (i % 2 == 0)
                key = reader.ReadBulkString();
            else
                dict[key] = reader.ReadBulkString();
        }

        return dict;
    }

    public class Generic<T> : RedisCommand<T> where T : class
    {
        public Generic(string command, params object[] args) : base(command, args)
        {
        }

        public override T Parse(RedisReader reader)
        {
            return Serializer<T>.Deserialize(ToDict(reader));
        }
    }
}

class RedisHashBytes : RedisCommand<Dictionary<string, byte[]>>
{
    public RedisHashBytes(string command, params object[] args) : base(command, args)
    {
    }

    public override Dictionary<string, byte[]> Parse(RedisReader reader)
    {
        return ToDict(reader);
    }

    static Dictionary<string, byte[]> ToDict(RedisReader reader)
    {
        reader.ExpectType(RedisMessage.MultiBulk);
        var count = reader.ReadInt(false);
        var dict = new Dictionary<string, byte[]>();
        var key = String.Empty;
        for (var i = 0; i < count; i++)
        {
            if (i % 2 == 0)
                key = reader.ReadBulkString();
            else
                dict[key] = reader.ReadBulkBytes();
        }

        return dict;
    }
}