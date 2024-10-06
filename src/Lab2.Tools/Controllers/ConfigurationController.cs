using Lab2.Tools.Abstractions.Models;
using Lab2.Tools.Abstractions.Persistence;
using Lab2.Tools.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Tools.Controllers;

[ApiController]
[Route("configurations")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationRepository _repository;

    public ConfigurationController(IConfigurationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<QueryConfigurationsResponse>> QueryAsync(
        [FromQuery] int pageSize,
        [FromQuery] string? pageToken,
        CancellationToken cancellationToken)
    {
        int cursor = int.TryParse(pageToken, out int value) ? value : 0;
        var query = ConfigurationQuery.Build(x => x.WithPageSize(pageSize).WithCursor(cursor));

        ConfigurationItem[] configurations = await _repository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        pageToken = configurations.Length == pageSize ? configurations[^1].Id.ToString() : null;

        IEnumerable<ConfigurationItemDto> dto = configurations
            .Select(x => new ConfigurationItemDto(x.Key.Value, x.Key.Value));

        return Ok(new QueryConfigurationsResponse(dto, pageToken));
    }

    [HttpPost]
    public async Task<ActionResult> AddOrUpdateAsync(
        [FromBody] AddOrUpdateConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var item = new ConfigurationItem(
            Id: default,
            new ConfigurationKey(request.Key),
            new ConfigurationValue(request.Value));

        await _repository.AddOrUpdateAsync(item, cancellationToken);

        return Ok();
    }

    [HttpDelete("{key}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] string key, CancellationToken cancellationToken)
    {
        await _repository.RemoveAsync(new ConfigurationKey(key), cancellationToken);
        return Ok();
    }
}