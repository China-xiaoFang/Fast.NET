using CSRedis.Internal.IO;

namespace CSRedis.Internal.Commands;

class RedisSlowLogCommand : RedisCommand<RedisSlowLogEntry>
{
    public RedisSlowLogCommand(string command, object[] args) : base(command, args)
    {
    }

    public override RedisSlowLogEntry Parse(RedisReader reader)
    {
        reader.ExpectType(RedisMessage.MultiBulk);
        reader.ExpectSize(4);
        var id = reader.ReadInt();
        var timestamp = reader.ReadInt();
        var microseconds = reader.ReadInt();
        reader.ExpectType(RedisMessage.MultiBulk);
        var arguments = new string[reader.ReadInt(false)];
        for (var i = 0; i < arguments.Length; i++)
            arguments[i] = reader.ReadBulkString();

        return new RedisSlowLogEntry(id, RedisDate.FromTimestamp(timestamp), RedisDate.Micro.FromMicroseconds(microseconds),
            arguments);
    }
}