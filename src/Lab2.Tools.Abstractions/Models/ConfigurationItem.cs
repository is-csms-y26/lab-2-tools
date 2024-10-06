namespace Lab2.Tools.Abstractions.Models;

public readonly record struct ConfigurationItem(long Id, ConfigurationKey Key, ConfigurationValue Value);