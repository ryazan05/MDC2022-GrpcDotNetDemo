using AutoMapper;
using ApiRestContracts = GrpcDotNetDemo.Contracts.Rest;

namespace GrpcDotNetDemo.Server.Mapping
{
    public class RestMappingProfile : Profile
    {
        public RestMappingProfile()
        {
            CreateMap<Models.EmployeeType, ApiRestContracts.EmployeeType>()
                .ReverseMap();

            CreateMap<Models.Employee, ApiRestContracts.Employee>()
                .ForMember(dest => dest.EmployeeId, m => m.MapFrom(src => src.Id))
                .ReverseMap()
                .ForMember(dest => dest.Id, m => m.Ignore());
        }
    }
}
