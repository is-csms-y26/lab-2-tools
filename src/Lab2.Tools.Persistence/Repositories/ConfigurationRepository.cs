using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Lab2.Tools.Abstractions.Models;
using Lab2.Tools.Abstractions.Persistence;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Lab2.Tools.Persistence.Repositories;

internal sealed class ConfigurationRepository : IConfigurationRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public ConfigurationRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async IAsyncEnumerable<ConfigurationItem> QueryAsync(
        ConfigurationQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = """
        select  configuration_id, 
                configuration_key, 
                configuration_value
        from configurations
        where configuration_id > :cursor
        order by configuration_id
        limit :page_size
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("cursor", query.Cursor)
            .AddParameter("page_size", query.PageSize);

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ConfigurationItem(
                Id: reader.GetInt64("configuration_id"),
                new ConfigurationKey(reader.GetString("configuration_key")),
                new ConfigurationValue(reader.GetString("configuration_value")));
        }
    }

    public async Task AddOrUpdateAsync(ConfigurationItem item, CancellationToken cancellationToken)
    {
        const string sql = """
        insert into configurations (configuration_key, configuration_value)
        values (:key, :value)
        on conflict (configuration_key)
        do update set configuration_value = excluded.configuration_value;
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("key", item.Key.Value)
            .AddParameter("value", item.Value.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RemoveAsync(ConfigurationKey key, CancellationToken cancellationToken)
    {
        const string sql = """
        delete from configurations
        where configuration_key = :key;
        """;

        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(cancellationToken);

        await using IPersistenceCommand command = connection.CreateCommand(sql)
            .AddParameter("key", key.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}