using Microsoft.Extensions.Configuration;
using System.Configuration;
using UserService.Interfaces;
using UserService.Services;

namespace UserService.Utility
{
    public class ServiceToScope
    {
        public IConfiguration Configuration { get; }
        public ServiceToScope(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void AddToScope(IServiceCollection service)
        {
            service.AddTransient<IUser>(s => new UserServices());

        }
    }
}
