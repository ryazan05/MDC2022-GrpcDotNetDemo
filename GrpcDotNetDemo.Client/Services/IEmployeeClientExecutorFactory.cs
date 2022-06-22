namespace GrpcDotNetDemo.Client.Services
{
    public interface IEmployeeClientExecutorFactory
    {
        IEmployeeClientExecutor CreateEmployeeClientExecutor(string executorType);
    }
}
