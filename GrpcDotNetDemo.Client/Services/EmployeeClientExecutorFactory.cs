using System;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcDotNetDemo.Client.Services
{
    public class EmployeeClientExecutorFactory : IEmployeeClientExecutorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EmployeeClientExecutorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEmployeeClientExecutor CreateEmployeeClientExecutor(string executorType)
        {
            if (executorType.Equals(Constants.Rest, StringComparison.OrdinalIgnoreCase))
            {
                return _serviceProvider.GetRequiredService<RestEmployeeClientExecutor>();
            }
            else if (executorType.Equals(Constants.Grpc, StringComparison.OrdinalIgnoreCase))
            {
                return _serviceProvider.GetRequiredService<GrpcEmployeeClientExecutor>();
            }
            else
            {
                return null;
            }
        }
    }
}
