namespace GrpcDotNetDemo.Server.Services
{
    public interface IJwtTokenAuthService
    {
        string GenerateToken(string username, string password);
    }
}
