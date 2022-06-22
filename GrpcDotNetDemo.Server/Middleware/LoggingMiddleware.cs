using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GrpcDotNetDemo.Server.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var contentType = context?.Request?.ContentType;
            if (contentType != null && contentType.StartsWith("application/grpc"))
            {
                context.Request.EnableBuffering();

                var requestReader = new StreamReader(context.Request.Body);
                var grpcRawMessage = await requestReader.ReadToEndAsync();

                _logger.LogInformation("Received gRPC call. gRPC request path: {GrpcRequestPath}", context.Request.Path.Value);
                _logger.LogDebug("gRPC request message content: {GrpcMessage}", grpcRawMessage);

                context.Request.Body.Position = 0;
            }
            
            await next(context);
        }
    }
}
