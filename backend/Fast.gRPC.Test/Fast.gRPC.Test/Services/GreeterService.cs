using Grpc.Core;

namespace Fast.gRPC.Test.Services;

public interface IGreeterService
{
    Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context);
    Task<HelloReply> SayHello1(HelloRequest request, ServerCallContext context);
}

public class GreeterService : Greeter.GreeterBase, IGreeterService
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// ÄãºÃ°¡
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply {Message = "Hello " + request.Name});
    }

    public Task<HelloReply> SayHello1(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply {Message = "Hello " + request.Name});
    }
}