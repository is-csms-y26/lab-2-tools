using Lab2.Tools.Abstractions.Models;

namespace Lab2.Tools.Abstractions.Persistence;

public interface IConfigurationRepository
{
    IAsyncEnumerable<ConfigurationItem> QueryAsync(ConfigurationQuery query, CancellationToken cancellationToken);

    Task AddOrUpdateAsync(ConfigurationItem item, CancellationToken cancellationToken);

    Task RemoveAsync(ConfigurationKey key, CancellationToken cancellationToken);
}