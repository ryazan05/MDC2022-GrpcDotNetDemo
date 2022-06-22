using System;
using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcDotNetDemo.Client.Interceptors
{
    public class ClientLoggingInterceptor : Interceptor
    {
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            Console.WriteLine($"Starting gRPC call: Type: {context.Method.Type}. Method: {context.Method.Name}");
            Console.WriteLine($"    RequestBody: {JsonSerializer.Serialize(request)}");

            return continuation(request, context);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            Console.WriteLine($"Starting gRPC call: Type: {context.Method.Type}. Method: {context.Method.Name}");
            Console.WriteLine($"    RequestBody: {JsonSerializer.Serialize(request)}");

            return continuation(request, context);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            Console.WriteLine($"Starting gRPC call: Type: {context.Method.Type}. Method: {context.Method.Name}");

            return continuation(context);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            Console.WriteLine($"Starting gRPC call: Type: {context.Method.Type}. Method: {context.Method.Name}");

            return continuation(request, context);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            Console.WriteLine($"Starting gRPC call: Type: {context.Method.Type}. Method: {context.Method.Name}");

            return continuation(context);
        }
    }
}
