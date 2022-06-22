using System;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace GrpcDotNetDemo.Server.Middleware.Interceptors
{
    public class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Received gRPC {GrpcMethodType} call. Calling gRPC method: {GrpcMethod}", MethodType.Unary, context.Method);
            _logger.LogDebug("gRPC request message content: {GrpcMessage}", JsonSerializer.Serialize(request));

            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown by gRPC method {GrpcMethod}", context.Method);
                throw;
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Received gRPC {GrpcMethodType} call. Calling gRPC method: {GrpcMethod}", MethodType.ClientStreaming, context.Method);

            try
            {
                return await continuation(requestStream, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown by gRPC method {GrpcMethod}", context.Method);
                throw;
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Received gRPC {GrpcMethodType} call. Calling gRPC method: {GrpcMethod}", MethodType.ServerStreaming, context.Method);

            try
            {
                await continuation(request, responseStream, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown by gRPC method {GrpcMethod}", context.Method);
                throw;
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Received gRPC {GrpcMethodType} call. Calling gRPC method: {GrpcMethod}", MethodType.DuplexStreaming, context.Method);

            try
            {
                await continuation(requestStream, responseStream, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown by gRPC method {GrpcMethod}", context.Method);
                throw;
            }
        }
    }
}
