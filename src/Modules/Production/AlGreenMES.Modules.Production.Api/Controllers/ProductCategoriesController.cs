using AlGreenMES.Modules.Production.Api.Requests;
using AlGreenMES.Modules.Production.Application.Commands.AddCategoryDependency;
using AlGreenMES.Modules.Production.Application.Commands.AddCategoryProcess;
using AlGreenMES.Modules.Production.Application.Commands.CreateProductCategory;
using AlGreenMES.Modules.Production.Application.Commands.DeactivateProductCategory;
using AlGreenMES.Modules.Production.Application.Commands.RemoveCategoryDependency;
using AlGreenMES.Modules.Production.Application.Commands.RemoveCategoryProcess;
using AlGreenMES.Modules.Production.Application.Commands.UpdateProductCategory;
using AlGreenMES.Modules.Production.Application.Queries.GetProductCategories;
using AlGreenMES.Modules.Production.Application.Queries.GetProductCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Production.Api.Controllers;

[ApiController]
[Route("api/product-categories")]
[Authorize]
public class ProductCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductCategories([FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductCategoriesQuery(tenantId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductCategoryByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateProductCategoryCommand(request.TenantId, request.Name, request.Description, null),
            cancellationToken);
        return CreatedAtAction(nameof(GetProductCategoryById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateProductCategory(Guid id, [FromBody] UpdateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateProductCategoryCommand(id, request.Name, request.Description),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateProductCategory(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeactivateProductCategoryCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{categoryId:guid}/processes")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddCategoryProcess(Guid categoryId, [FromBody] AddCategoryProcessRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddCategoryProcessCommand(categoryId, request.ProcessId, request.SequenceOrder, request.DefaultComplexity),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{categoryId:guid}/processes/{processId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RemoveCategoryProcess(Guid categoryId, Guid processId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RemoveCategoryProcessCommand(categoryId, processId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{categoryId:guid}/dependencies")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddCategoryDependency(Guid categoryId, [FromBody] AddCategoryDependencyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddCategoryDependencyCommand(categoryId, request.ProcessId, request.DependsOnProcessId),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{categoryId:guid}/dependencies/{dependencyId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RemoveCategoryDependency(Guid categoryId, Guid dependencyId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RemoveCategoryDependencyCommand(categoryId, dependencyId),
            cancellationToken);
        return Ok(result);
    }
}
