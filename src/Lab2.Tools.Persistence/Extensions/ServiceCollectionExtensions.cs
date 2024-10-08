using Itmo.Dev.Platform.Persistence.Abstractions.Extensions;
using Itmo.Dev.Platform.Persistence.Postgres.Extensions;
using Lab2.Tools.Abstractions.Persistence;
using Lab2.Tools.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Tools.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection collection)
    {
        collection.AddPlatformPersistence(persistence => persistence
            .UsePostgres(postgres => postgres
                .WithConnectionOptions(builder => builder.BindConfiguration("Persistence:Postgres"))
                .WithMigrationsFrom(typeof(IAssemblyMarker).Assembly)));

        collection.AddScoped<IConfigurationRepository, ConfigurationRepository>();

        return collection;
    }
}