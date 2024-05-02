using CSRedis.Internal.IO;

namespace CSRedis.Internal.Commands;

class RedisIsMasterDownByAddrCommand : RedisCommand<RedisMasterState>
{
    public RedisIsMasterDownByAddrCommand(string command, params object[] args) : base(command, args)
    {
    }

    public override RedisMasterState Parse(RedisReader reader)
    {
        reader.ExpectType(RedisMessage.MultiBulk);
        reader.ExpectSize(3);
        var down_state = reader.ReadInt();
        var leader = reader.ReadBulkString();
        var vote_epoch = reader.ReadInt();
        return new RedisMasterState(down_state, leader, vote_epoch);
    }
}