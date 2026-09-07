namespace Finance.Application.Expenses;

public record CreateExpenseRequest(
    int CategoryId,
    decimal Amount,
    string Description,
    DateOnly ExpenseDate
);

public record UpdateExpenseRequest(
    int CategoryId,
    decimal Amount,
    string Description,
    DateOnly ExpenseDate
);

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
