using CSRedis.Internal.IO;

namespace CSRedis.Internal;

internal abstract class RedisListener<TResponse>
{
    public bool Listening { get; private set; }
    protected RedisConnector Connection { get; }

    protected RedisListener(RedisConnector connection)
    {
        Connection = connection;
    }

    protected void Listen(Func<RedisReader, TResponse> func)
    {
        Listening = true;
        do
        {
            try
            {
                var value = Connection.Read(func);
                OnParsed(value);
            }
            catch (IOException)
            {
                if (Connection.IsConnected)
                    throw;
                break;
            }
        } while (Continue());

        Listening = false;
    }

    protected void Write<T>(RedisCommand<T> command)
    {
        Connection.Write(command);
    }

    protected T Call<T>(RedisCommand<T> command)
    {
        return Connection.Call(command);
    }

    protected abstract void OnParsed(TResponse value);

    protected abstract bool Continue();
}