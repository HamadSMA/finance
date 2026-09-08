using Finance.Application.Common;
using Finance.Application.Expenses;
using Finance.Domain;
using Finance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController(IFinanceDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await db
            .Expenses.AsNoTracking()
            .Where(e => e.UserId == DevUser.Id)
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseResponse(
                e.Id,
                e.CategoryId,
                e.Category.Name,
                e.Amount,
                e.Description,
                e.ExpenseDate,
                e.CreatedAt,
                e.UpdatedAt
            ))
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var expense = await db
            .Expenses.AsNoTracking()
            .Where(e => e.UserId == DevUser.Id && e.Id == id)
            .Select(e => new ExpenseResponse(
                e.Id,
                e.CategoryId,
                e.Category.Name,
                e.Amount,
                e.Description,
                e.ExpenseDate,
                e.CreatedAt,
                e.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);

        if (expense is null)
            return Problem(
                title: "Expense not found",
                statusCode: 404,
                detail: $"No expense with id {id} exists."
            );

        return Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = DevUser.Id,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description,
            ExpenseDate = request.ExpenseDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Expenses.Add(expense);
        await db.SaveChangesAsync(ct);

        var categoryName = await db
            .Categories.Where(c => c.Id == expense.CategoryId)
            .Select(c => c.Name)
            .FirstAsync(ct);

        var response = new ExpenseResponse(
            expense.Id,
            expense.CategoryId,
            categoryName,
            expense.Amount,
            expense.Description,
            expense.ExpenseDate,
            expense.CreatedAt,
            expense.UpdatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateExpenseRequest request,
        CancellationToken ct
    )
    {
        var expense = await db.Expenses.FirstOrDefaultAsync(
            e => e.UserId == DevUser.Id && e.Id == id,
            ct
        );

        if (expense is null)
            return Problem(
                title: "Expense not found",
                statusCode: 404,
                detail: $"No expense with id {id} exists."
            );

        expense.CategoryId = request.CategoryId;
        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.ExpenseDate = request.ExpenseDate;
        expense.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var expense = await db.Expenses.FirstOrDefaultAsync(
            e => e.UserId == DevUser.Id && e.Id == id,
            ct
        );

        if (expense is null)
            return Problem(
                title: "Expense not found",
                statusCode: 404,
                detail: $"No expense with id {id} exists."
            );

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    //For testing unhandled exceptions, kept for reference
    // [HttpGet("exception")]
    // public IActionResult Exception()
    // {
    //     throw new Exception("This is my test exception.");
    // }
}
