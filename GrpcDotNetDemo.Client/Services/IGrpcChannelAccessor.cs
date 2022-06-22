using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcDotNetDemo.Client.Services
{
    public interface IGrpcChannelAccessor
    {
        GrpcChannel Channel { get; }
        CallInvoker CallInvoker { get; }
    }
}
