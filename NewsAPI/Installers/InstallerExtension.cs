using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Installers
{
    public static class InstallerExtension
    {
		public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
		{
			//Lấy tất cả class implement IInstaller -> tạo instance -> cast sang kiểu IInstaller -> sang List
			var installers = typeof(Program).Assembly.ExportedTypes.Where(x =>
				typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(x =>
				Activator.CreateInstance(x)).Cast<IInstaller>().ToList();
			//inject to all class services and configuration
			installers.ForEach(installer => installer.InstallerServices(services, configuration));
		}
	}
}
