using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Lab2.Tools.Persistence.Migrations;

[Migration(1728239412, "Initial")]
public class Initial : SqlMigration 
{
    protected override string GetUpSql(IServiceProvider serviceProvider) => """
    create table configurations
    (
        configuration_id bigint primary key generated always as identity ,
        configuration_key text not null ,
        configuration_value text not null 
    );
    
    create unique index configuration_key_idx on configurations (configuration_key);
    """;

    protected override string GetDownSql(IServiceProvider serviceProvider) => """
    drop table configurations;
    """;
}