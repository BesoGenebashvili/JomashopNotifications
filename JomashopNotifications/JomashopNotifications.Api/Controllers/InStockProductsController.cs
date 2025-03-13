using JomashopNotifications.Application.InStockProduct.Commands;
using JomashopNotifications.Application.InStockProduct.Contracts;
using JomashopNotifications.Application.InStockProduct.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JomashopNotifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InStockProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet("list")]
    public async Task<ActionResult<List<InStockProductDto>>> ListAsync() =>
        await mediator.Send(new ListInStockProductsQuery());

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> DeleteAsync(int id) =>
        await mediator.Send(new DeleteInStockProductCommand(id));
}
