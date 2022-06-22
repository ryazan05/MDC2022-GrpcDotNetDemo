using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GrpcDotNetDemo.Client.Interceptors;

namespace GrpcDotNetDemo.Client.Services
{
    public class GrpcChannelAccessor : IGrpcChannelAccessor
    {
        private readonly GrpcChannel _channel;
        public GrpcChannel Channel => _channel;

        private readonly CallInvoker _callInvoker;
        public CallInvoker CallInvoker => _callInvoker;

        public GrpcChannelAccessor(string address, GrpcChannelOptions grpcChannelOptions)
        {
            _channel = GrpcChannel.ForAddress(address, grpcChannelOptions);
            _callInvoker = _channel.Intercept(new ClientLoggingInterceptor());
        }
    }
}
