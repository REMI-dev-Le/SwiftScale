using SwiftScale.BuildingBlocks;
using System.Reflection;

namespace SwiftScale.WebApi.Infrastructure
{
    public static class ModuleExtensions
    {
        private static readonly List<IModule> RegisteredModules = new();

        public static IServiceCollection RegisterModules(this IServiceCollection services, IConfiguration configuration)
        {
            // Architect Rule: Scan the assemblies you've referenced in WebApi
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            // 2. Force load referenced assemblies that start with "SwiftScale.Modules"
            // This ensures that even if a module isn't "active" yet, its IModule is found.
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "SwiftScale.Modules.*.Presentation.dll");

            foreach (var path in referencedPaths)
            {
                var assemblyName = AssemblyName.GetAssemblyName(path);
                if (assemblies.All(a => a.FullName != assemblyName.FullName))
                {
                    assemblies.Add(Assembly.Load(assemblyName));
                }
            }

            var modules = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IModule>()
                .ToList();

            foreach (var module in modules)
            {
                module.RegisterModule(services, configuration);
                RegisteredModules.Add(module);
            }

            return services;
        }

        public static WebApplication MapModuleEndpoints(this WebApplication app)
        {
            foreach (var module in RegisteredModules)
            {
                module.MapEndpoints(app);
            }
            return app;
        }
    }
}
