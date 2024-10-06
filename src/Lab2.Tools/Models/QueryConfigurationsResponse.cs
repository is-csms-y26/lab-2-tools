namespace Lab2.Tools.Models;

public record QueryConfigurationsResponse(IEnumerable<ConfigurationItemDto> Items, string? PageToken);