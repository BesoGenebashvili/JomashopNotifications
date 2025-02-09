using JomashopNotifications.Application.Product.Commands;
using JomashopNotifications.Application.Product.Contracts;
using JomashopNotifications.Application.Product.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JomashopNotifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IMediator mediator,
        ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetAsync(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery() { Id = id });

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<int>> PostAsync([FromBody] CreateProductCommand request)
    {
        var id = await _mediator.Send(request);

        return Created("/", new { id });
    }
}
