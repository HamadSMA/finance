using Finance.Application.Categories;
using Finance.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(IFinanceDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await db
            .Categories.AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync(ct);

        return Ok(items);
    }
}
