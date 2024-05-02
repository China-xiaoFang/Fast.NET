using CSRedis.Internal.IO;

namespace CSRedis.Internal.Commands;

class RedisRoleCommand : RedisCommand<RedisRole>
{
    public RedisRoleCommand(string command, params object[] args) : base(command, args)
    {
    }

    public override RedisRole Parse(RedisReader reader)
    {
        reader.ExpectType(RedisMessage.MultiBulk);
        var count = (int) reader.ReadInt(false);

        var role = reader.ReadBulkString();
        switch (role)
        {
            case "master":
                return ParseMaster(count, role, reader);
            case "slave":
                return ParseSlave(count, role, reader);
            case "sentinel":
                return ParseSentinel(count, role, reader);
            default:
                throw new RedisProtocolException("Unexpected role: " + role);
        }
    }

    static RedisMasterRole ParseMaster(int num, string role, RedisReader reader)
    {
        reader.ExpectSize(3, num);
        var offset = reader.ReadInt();
        reader.ExpectType(RedisMessage.MultiBulk);
        var slaves = new Tuple<string, int, long>[reader.ReadInt(false)];
        for (var i = 0; i < slaves.Length; i++)
        {
            reader.ExpectType(RedisMessage.MultiBulk);
            reader.ExpectSize(3);
            var ip = reader.ReadBulkString();
            var port = Int32.Parse(reader.ReadBulkString());
            long.TryParse(reader.ReadBulkString(), out var slave_offset);
            slaves[i] = new Tuple<string, int, long>(ip, port, slave_offset);
        }

        return new RedisMasterRole(role, offset, slaves);
    }

    static RedisSlaveRole ParseSlave(int num, string role, RedisReader reader)
    {
        reader.ExpectSize(5, num);
        var master_ip = reader.ReadBulkString();
        var port = (int) reader.ReadInt();
        var state = reader.ReadBulkString();
        var data = reader.ReadInt();
        return new RedisSlaveRole(role, master_ip, port, state, data);
    }

    static RedisSentinelRole ParseSentinel(int num, string role, RedisReader reader)
    {
        reader.ExpectSize(2, num);
        reader.ExpectType(RedisMessage.MultiBulk);
        var masters = new string[reader.ReadInt(false)];
        for (var i = 0; i < masters.Length; i++)
            masters[i] = reader.ReadBulkString();
        return new RedisSentinelRole(role, masters);
    }
}