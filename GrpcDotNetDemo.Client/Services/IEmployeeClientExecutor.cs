using System;
using System.Threading.Tasks;

namespace GrpcDotNetDemo.Client.Services
{
    public interface IEmployeeClientExecutor
    {
        Task<bool> GetAll();
        Task<bool> Get(Guid employeeId);
        Task<bool> Create();
        Task<bool> Update(Guid employeeId);
        Task<bool> Delete(Guid employeeId);

        // --- Streaming ---
        // Server
        Task<bool> GetAllStreaming();

        // --- Streaming ---
        // Client
        Task<bool> BulkCreateClientStreaming(int num);

        // --- Streaming ---
        // Bidirectional
        Task<bool> BulkCreateBidirectionalStreaming(int num);

        Task GetToken();
    }
}
