using System.Reflection;

using Shift.Common;

namespace Shift.Service
{
    internal interface IDbContext
    {
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QueryServiceRegistration
    {
        public static IServiceCollection AddQueries(this IServiceCollection services, Assembly assembly)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a =>
                {
                    try
                    {
                        // Trigger type load to force early failure
                        _ = a.GetTypes();
                        return true;
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        // Log or break here to see which assembly causes the error
                        Console.WriteLine($"Failed to load types from: {a.FullName}");
                        foreach (var loaderEx in ex.LoaderExceptions)
                            Console.WriteLine($" - {loaderEx?.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Non-typeload failure from {a.FullName}: {ex.Message}");
                        return false;
                    }
                })
                .ToArray();

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo<IQueryRunner>())
                .As<IQueryRunner>()
                .WithScopedLifetime());

            services.AddScoped<QueryDispatcher>();

            services.AddSingleton(x => new QueryTypeCollection(assembly));

            return services;
        }
    }
}