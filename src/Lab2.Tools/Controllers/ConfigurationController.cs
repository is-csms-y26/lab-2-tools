using Lab2.Tools.Abstractions.Models;
using Lab2.Tools.Abstractions.Persistence;
using Lab2.Tools.Models;
using Lab2.Tools.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Tools.Controllers;

[ApiController]
[Route("configurations")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationRepository _repository;
    private readonly PageTokenSerializer _pageTokenSerializer;

    public ConfigurationController(IConfigurationRepository repository, PageTokenSerializer pageTokenSerializer)
    {
        _repository = repository;
        _pageTokenSerializer = pageTokenSerializer;
    }

    [HttpGet]
    public async Task<ActionResult<QueryConfigurationsResponse>> QueryAsync(
        [FromQuery] int pageSize,
        [FromQuery] string? pageToken = null)
    {
        CancellationToken cancellationToken = HttpContext.RequestAborted;

        ConfigurationPageToken? parsedPageToken = _pageTokenSerializer.TryDeserialize(
            pageToken,
            ConfigurationPageToken.Empty);

        if (parsedPageToken is null)
            return BadRequest("Invalid page token");

        var query = ConfigurationQuery.Build(x => x.WithPageSize(pageSize).WithCursor(parsedPageToken.Id));

        ConfigurationItem[] configurations = await _repository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        pageToken = configurations.Length == pageSize
            ? _pageTokenSerializer.Serialize(new ConfigurationPageToken(configurations[^1].Id))
            : null;

        IEnumerable<ConfigurationItemDto> dto = configurations
            .Select(x => new ConfigurationItemDto(x.Key.Value, x.Value.Value));

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