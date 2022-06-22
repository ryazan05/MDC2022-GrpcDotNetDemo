using System;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDotNetDemo.Client.CommandExtensions;
using GrpcDotNetDemo.Client.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrpcDotNetDemo.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = typeof(Program).Assembly.GetName().Name;

            var hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<IGrpcChannelAccessor>((sp) =>
                    {
                        var grpcChannelOptions = new GrpcChannelOptions
                        {
                            Credentials = ChannelCredentials.Insecure
                            //LoggerFactory
                        };
                        return new GrpcChannelAccessor("http://localhost:5209", grpcChannelOptions);
                    });

                    services.AddSingleton<IEmployeeClientExecutorFactory, EmployeeClientExecutorFactory>();
                    services.AddTransient<RestEmployeeClientExecutor>();
                    services.AddTransient<GrpcEmployeeClientExecutor>();
                })
                .UseConsoleLifetime();

            var host = hostBuilder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var application = new CommandLineApplication
                {
                    Name = "GrpcDotNetDemo.Client"
                };

                application.HelpOption("-?|-h|--help");
                application.VersionOption("--version", "1.0.0");

                application.Command("exit", command =>
                {
                    command.Description = "Exit GrpcDotNetDemo.Client";

                    command.OnExecute(() =>
                    {
                        Console.WriteLine("Exiting GrpcDotNetDemo.Client");

                        return (int)ExecutionResult.Exit;
                    });
                });

                var employeeClientExecutorFactory = serviceScope.ServiceProvider.GetRequiredService<IEmployeeClientExecutorFactory>();

                application.Command(Constants.Rest, restApp =>
                {
                    restApp.Description = "Execute REST commands";
                    restApp.HelpOption("--help");

                    restApp.AddRestEmployeeManagementCommands(employeeClientExecutorFactory);
                });

                application.Command(Constants.Grpc, grpcApp =>
                {
                    grpcApp.Description = "Execute gRPC commands";
                    grpcApp.HelpOption("--help");

                    grpcApp.Command("gettoken", getTokenCommand =>
                    {
                        getTokenCommand.OnExecute(async () =>
                        {
                            var employeeClientExecutor = employeeClientExecutorFactory.CreateEmployeeClientExecutor(Constants.Grpc);

                            Console.WriteLine("Executing gRPC GetToken command");

                            await employeeClientExecutor.GetToken();

                            return (int)ExecutionResult.Continue;
                        });
                    });

                    grpcApp.AddGrpcEmployeeManagementCommands(employeeClientExecutorFactory);
                    grpcApp.AddGrpcEmployeeManagementStreamingCommands(employeeClientExecutorFactory);
                });

                application.ShowHelp();

                var executionResult = ExecutionResult.None;
                do
                {
                    try
                    {
                        var input = Console.ReadLine();
                        executionResult = (ExecutionResult)application.Execute(input.Split(' '));
                    }
                    catch (CommandParsingException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                } while (executionResult != ExecutionResult.Exit);
            }
        }
    }
}
