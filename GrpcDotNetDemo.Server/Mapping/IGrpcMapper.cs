using GrpcContracts = GrpcDotNetDemo.Contracts.Grpc;

namespace GrpcDotNetDemo.Server.Mapping
{
    public interface IGrpcMapper
    {
        GrpcContracts.Employee MapEmployeeModelToContract(Models.Employee employee);
        GrpcContracts.CommandResult MapActionResponseModelToContract(Models.ActionResponse actionResponse);

        Models.Employee MapContractToEmployeeModel(GrpcContracts.Employee employee);
    }
}
