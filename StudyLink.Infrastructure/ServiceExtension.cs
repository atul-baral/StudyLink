using Microsoft.Extensions.DependencyInjection;
using StudyLink.Application.Interfaces;
using StudyLink.Application.Services.Implementation;
using StudyLink.Application.Services.Interface;

namespace StudyLink.Infrastructure
{
    public static class ServiceExtension
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
