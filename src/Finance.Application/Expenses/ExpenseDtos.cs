using System.ComponentModel.DataAnnotations;

namespace Finance.Application.Expenses;

public record CreateExpenseRequest(
    int CategoryId,
    decimal Amount,
    [Required, MaxLength(500)] string Description,
    DateOnly ExpenseDate
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        ExpenseRules.Validate(CategoryId, Amount, ExpenseDate);
}

public record UpdateExpenseRequest(
    int CategoryId,
    decimal Amount,
    [Required, MaxLength(500)] string Description,
    DateOnly ExpenseDate
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        ExpenseRules.Validate(CategoryId, Amount, ExpenseDate);
}

public record ExpenseResponse(
    Guid Id,
    int CategoryId,
    string CategoryName,
    decimal Amount,
    string Description,
    DateOnly ExpenseDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

internal static class ExpenseRules
{
    public static IEnumerable<ValidationResult> Validate(
        int categoryId,
        decimal amount,
        DateOnly expenseDate
    )
    {
        if (amount <= 0)
            yield return new ValidationResult("Amount must be greater than zero.", ["Amount"]);

        if (decimal.Round(amount, 2) != amount)
            yield return new ValidationResult(
                "Amount cannot have more than 2 decimal places.",
                ["Amount"]
            );

        if (!Finance.Domain.Entities.Categories.All.ContainsKey(categoryId))
            yield return new ValidationResult(
                $"Category {categoryId} does not exist.",
                ["CategoryId"]
            );

        if (expenseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            yield return new ValidationResult(
                "Expense date cannot be in the future.",
                ["ExpenseDate"]
            );
    }
}
