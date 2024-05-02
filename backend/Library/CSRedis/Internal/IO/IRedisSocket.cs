using System.Net;

namespace CSRedis.Internal.IO;

interface IRedisSocket : IDisposable
{
    bool Connected { get; }
    int ReceiveTimeout { get; set; }
    int SendTimeout { get; set; }

    void Connect(EndPoint endpoint, int timeout);
#if net40
#else
    Task<bool> ConnectAsync(EndPoint endpoint);
#endif

    Stream GetStream();
}