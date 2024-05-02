using CSRedis.Internal.Commands;

namespace CSRedis.Internal;

class RedisTransaction
{
    readonly RedisConnector _connector;
    readonly RedisArray _execCommand;
    readonly List<Tuple<string, object[]>> _pipeCommands = new List<Tuple<string, object[]>>();

    public event EventHandler<RedisTransactionQueuedEventArgs> TransactionQueued;

    public bool Active { get; private set; }

    public RedisTransaction(RedisConnector connector)
    {
        _connector = connector;
        _execCommand = RedisCommands.Exec();
    }

    public string Start()
    {
        Active = true;
        return _connector.Call(RedisCommands.Multi());
    }

    public T Write<T>(RedisCommand<T> command)
    {
        var response = _connector.Call(RedisCommands.AsTransaction(command));
        OnTransactionQueued(command, response);

        _execCommand.AddParser(x => command.Parse(x));
        return default(T);
    }

    public object[] Execute()
    {
        Active = false;

        if (_connector.IsConnected && _connector.IsPipelined)
        {
            _connector.Call(_execCommand);
            var response = _connector.EndPipe();
            for (var i = 1; i < response.Length - 1; i++)
                OnTransactionQueued(_pipeCommands[i - 1].Item1, _pipeCommands[i - 1].Item2, response[i - 1].ToString());

            var transaction_response = response[response.Length - 1];
            if (!(transaction_response is object[]))
                throw new RedisProtocolException("Unexpected response");

            return transaction_response as object[];
        }

        return _connector.Call(_execCommand);
    }

    public string Abort()
    {
        Active = false;
        return _connector.Call(RedisCommands.Discard());
    }

    void OnTransactionQueued<T>(RedisCommand<T> command, string response)
    {
        if (_connector.IsPipelined)
            _pipeCommands.Add(Tuple.Create(command.Command, command.Arguments));
        else
            OnTransactionQueued(command.Command, command.Arguments, response);
    }

    void OnTransactionQueued(string command, object[] args, string response)
    {
        if (TransactionQueued != null)
            TransactionQueued(this, new RedisTransactionQueuedEventArgs(response, command, args));
    }

#if net40
#else
    public Task<string> StartAsync()
    {
        Active = true;
        return _connector.CallAsync(RedisCommands.Multi());
    }

    public Task<T> WriteAsync<T>(RedisCommand<T> command)
    {
        lock (_execCommand)
        {
            _execCommand.AddParser(x => command.Parse(x));
            return _connector.CallAsync(RedisCommands.AsTransaction(command))
                .ContinueWith(t => OnTransactionQueued(command, t.Result)).ContinueWith(t => default(T));
        }
    }

    public Task<object[]> ExecuteAsync()
    {
        Active = false;
        return _connector.CallAsync(_execCommand);
    }

    public Task<string> AbortAsync()
    {
        Active = false;
        return _connector.CallAsync(RedisCommands.Discard());
    }
#endif
}