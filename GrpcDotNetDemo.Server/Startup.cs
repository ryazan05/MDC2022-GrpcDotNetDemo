using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using GrpcDotNetDemo.Server.gRPC;
using GrpcDotNetDemo.Server.Mapping;
using GrpcDotNetDemo.Server.Middleware;
using GrpcDotNetDemo.Server.Middleware.Interceptors;
using GrpcDotNetDemo.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GrpcDotNetDemo.Server
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var securityKey = "Grpc Security Key used for signature validation";
            services.AddSingleton<IJwtTokenAuthService>(new JwtTokenAuthService(securityKey));

            services.AddAuthentication()
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.RequireHttpsMetadata = false;
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey))
                    };
                });

            services
                .AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee API", Version = "v1" });
            });

            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile<RestMappingProfile>();
            });
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<IGrpcMapper, GrpcMapper>();
            services.AddTransient<IEmployeeService, EmployeeService>();

            services.AddTransient<LoggingMiddleware>();

            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.Interceptors.Add<LoggingInterceptor>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee.API V1");
                });
            }

            app.UseMiddleware<LoggingMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGrpcService<EmployeeGrpcService>();
            });
        }
    }
}
