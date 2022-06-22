using System;
using GrpcDotNetDemo.Client.Services;
using Microsoft.Extensions.CommandLineUtils;

namespace GrpcDotNetDemo.Client.CommandExtensions
{
    public static class EmployeeCommands
    {
        public static CommandLineApplication AddRestEmployeeManagementCommands(this CommandLineApplication commandLineApplication, IEmployeeClientExecutorFactory employeeClientExecutorFactory)
        {
            return commandLineApplication.AddEmployeeManagementCommands(employeeClientExecutorFactory, Constants.Rest);
        }

        public static CommandLineApplication AddGrpcEmployeeManagementCommands(this CommandLineApplication commandLineApplication, IEmployeeClientExecutorFactory employeeClientExecutorFactory)
        {
            return commandLineApplication.AddEmployeeManagementCommands(employeeClientExecutorFactory, Constants.Grpc);
        }

        public static CommandLineApplication AddGrpcEmployeeManagementStreamingCommands(this CommandLineApplication commandLineApplication, IEmployeeClientExecutorFactory employeeClientExecutorFactory)
        {
            commandLineApplication.Command("getallserver", command =>
            {
                command.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(Constants.Grpc);

                    Console.WriteLine("Executing gRPC Server streaming command");

                    await employeeClientExecutor.GetAllStreaming();

                    return (int)ExecutionResult.Continue;
                });
            });

            commandLineApplication.Command("bulkcreateclient", command =>
            {
                var numOption = command.Option("--num <value>", "Number of employees to create", CommandOptionType.SingleValue);

                command.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(Constants.Grpc);

                    int num = 1;
                    if (numOption.Value() != null && int.TryParse(numOption.Value(), out var value))
                    {
                        num = value;
                    }

                    Console.WriteLine("Executing gRPC Client streaming command");

                    await employeeClientExecutor.BulkCreateClientStreaming(num);

                    numOption.Values.Clear();

                    return (int)ExecutionResult.Continue;
                });
            });

            commandLineApplication.Command("bulkcreateduplex", command =>
            {
                var numOption = command.Option("--num <value>", "Number of employees to create", CommandOptionType.SingleValue);

                command.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(Constants.Grpc);

                    int num = 1;
                    if (numOption.Value() != null && int.TryParse(numOption.Value(), out var value))
                    {
                        num = value;
                    }

                    Console.WriteLine("Executing gRPC Bidirectional streaming command");

                    await employeeClientExecutor.BulkCreateBidirectionalStreaming(num);

                    numOption.Values.Clear();

                    return (int)ExecutionResult.Continue;
                });
            });

            return commandLineApplication;
        }

        private static CommandLineApplication AddEmployeeManagementCommands(this CommandLineApplication commandLineApplication, IEmployeeClientExecutorFactory employeeClientExecutorFactory, string executorType)
        {
            commandLineApplication.Command("get", getCommand =>
            {
                var idOption = getCommand.Option("--id <value>", "Id of the employee", CommandOptionType.SingleValue);

                getCommand.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(executorType);

                    if (idOption.HasValue())
                    {
                        var id = idOption.Value();
                        Console.WriteLine($"Executing GET Employee command with EmployeeId: {id}");

                        Guid.TryParse(id, out Guid employeeId);

                        await employeeClientExecutor.Get(employeeId);
                    }
                    else
                    {
                        Console.WriteLine("Executing GET Employees command");

                        await employeeClientExecutor.GetAll();
                    }

                    idOption.Values.Clear();

                    return (int)ExecutionResult.Continue;
                });
            });

            commandLineApplication.Command("create", createCommand =>
            {
                createCommand.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(executorType);

                    Console.WriteLine("Executing CREATE Employee command");

                    await employeeClientExecutor.Create();

                    return (int)ExecutionResult.Continue;
                });
            });

            commandLineApplication.Command("update", updateCommand =>
            {
                var idOption = updateCommand.Option("--id <value>", "Id of the employee", CommandOptionType.SingleValue);

                updateCommand.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(executorType);

                    var id = idOption.Value();
                    Console.WriteLine($"Executing UPDATE Employee command with EmployeeId: {id}");

                    Guid.TryParse(id, out Guid employeeId);

                    await employeeClientExecutor.Update(employeeId);

                    idOption.Values.Clear();

                    return (int)ExecutionResult.Continue;
                });
            });

            commandLineApplication.Command("delete", deleteCommand =>
            {
                var idOption = deleteCommand.Option("--id <value>", "Id of the employee", CommandOptionType.SingleValue);

                deleteCommand.OnExecute(async () =>
                {
                    var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(executorType);

                    var id = idOption.Value();
                    Console.WriteLine($"Executing DELETE Employee command with EmployeeId: {id}");

                    Guid.TryParse(id, out Guid employeeId);

                    await employeeClientExecutor.Delete(employeeId);

                    idOption.Values.Clear();

                    return (int)ExecutionResult.Continue;
                });
            });

            commandLineApplication.OnExecute(() =>
            {
                Console.WriteLine("Specify get|create|update|delete command");

                return (int)ExecutionResult.Continue;
            });

            return commandLineApplication;
        }
    }
}
