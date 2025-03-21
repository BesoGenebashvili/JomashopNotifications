using JomashopNotifications.Application.ProductProfile.Commands;
using JomashopNotifications.Application.ProductProfile.Contracts;
using JomashopNotifications.Application.ProductProfile.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JomashopNotifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductProfilesController(IMediator mediator) : ControllerBase
{
    [HttpGet("list")]
    public async Task<ActionResult<List<ProductProfileDto>>> ListAsync([FromQuery] int[]? productIds) =>
        await mediator.Send(
            new ListProductProfilesQuery
            {
                ProductIds = productIds
            });

    [HttpPost("upsert")]
    public async Task<ActionResult> UpsertAsync([FromBody] UpsertProductProfileCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpPut("activate/{productId:int}")]
    public async Task<ActionResult> ActivateAsync(int productId)
    {
        await mediator.Send(new ActivateProductProfileCommand(productId));
        return Ok();
    }

    [HttpPut("deactivate/{productId:int}")]
    public async Task<ActionResult> DeactivateAsync(int productId)
    {
        await mediator.Send(new DeactivateProductProfileCommand(productId));
        return Ok();
    }

    [HttpDelete("{productId:int}")]
    public async Task<ActionResult<bool>> DeleteAsync(int productId) =>
        await mediator.Send(new DeleteProductProfileCommand(productId));
}
