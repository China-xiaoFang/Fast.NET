using CSRedis.Internal.IO;

namespace CSRedis.Internal.Commands;

class RedisScanCommand<T> : RedisCommand<RedisScan<T>>
{
    readonly RedisCommand<T[]> _command;

    public RedisScanCommand(RedisCommand<T[]> command) : base(command.Command, command.Arguments)
    {
        _command = command;
    }

    public override RedisScan<T> Parse(RedisReader reader)
    {
        reader.ExpectType(RedisMessage.MultiBulk);
        if (reader.ReadInt(false) != 2)
            throw new RedisProtocolException("Expected 2 items");

        var cursor = Int64.Parse(reader.ReadBulkString());
        var items = _command.Parse(reader);

        return new RedisScan<T>(cursor, items);
    }
}