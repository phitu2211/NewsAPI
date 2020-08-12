using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NewsAPI.Installers
{
    public interface IInstaller
    {
        void InstallerServices(IServiceCollection services, IConfiguration configuration);
    }
}
