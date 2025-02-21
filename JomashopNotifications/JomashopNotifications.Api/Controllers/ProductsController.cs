using JomashopNotifications.Application.Product.Commands;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Application.Product.Queries;
using JomashopNotifications.Persistence.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JomashopNotifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetAsync(int id) =>
        await mediator.Send(new GetProductByIdQuery() { Id = id }) switch
        {
            { } product => Ok(product),
            _ => NotFound($"Product with Id = {id} not found")
        };

    [HttpGet("list")]
    public async Task<ActionResult<List<ProductDto>>> ListAsync([FromQuery] ProductStatus? status, [FromQuery] int[]? ids) =>
        await mediator.Send(
            new ListProductsQuery
            {
                Status = status,
                Ids = ids
            });

    [HttpPost]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateProductCommand command)
    {
        var id = await mediator.Send(command);

        return Created($"/api/Products/{id}", id);
    }

    [HttpPut("activate/{id:int}")]
    public async Task<ActionResult> SetStatusAsActiveAsync(int id)
    {
        await mediator.Send(new SetStatusAsActiveCommand(id));
        return Ok();
    }

    [HttpPut("deactivate/{id:int}")]
    public async Task<ActionResult> SetStatusAsInactiveAsync(int id)
    {
        await mediator.Send(new SetStatusAsInactiveCommand(id));
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> DeleteAsync(int id) =>
        await mediator.Send(new DeleteProductCommand(id));
}
