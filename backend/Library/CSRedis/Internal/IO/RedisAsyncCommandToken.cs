namespace CSRedis.Internal.IO;

interface IRedisAsyncCommandToken
{
    Task Task { get; }
    RedisCommand Command { get; }
    void SetResult(RedisReader reader);
    void SetException(Exception e);
}

class RedisAsyncCommandToken<T> : IRedisAsyncCommandToken
{
    readonly RedisCommand<T> _command;

    public TaskCompletionSource<T> TaskSource { get; }

    public RedisCommand Command
    {
        get
        {
            return _command;
        }
    }

    public Task Task
    {
        get
        {
            return TaskSource.Task;
        }
    }

    public RedisAsyncCommandToken(RedisCommand<T> command)
    {
        TaskSource = new TaskCompletionSource<T>();
        _command = command;
    }

    public void SetResult(RedisReader reader)
    {
        if (reader == null)
        {
            TaskSource.SetResult(default(T));
            return;
        }

        TaskSource.SetResult(_command.Parse(reader));
    }

    public void SetException(Exception e)
    {
        TaskSource.SetException(e);
    }
}