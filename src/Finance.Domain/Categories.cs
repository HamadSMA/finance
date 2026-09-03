using System;

namespace Finance.Domain.Entities;

public class Categories
{
    public const int Food = 1;
    public const int Transport = 2;
    public const int Housing = 3;
    public const int Shopping = 4;
    public const int Entertainment = 5;
    public const int Bills = 6;
    public const int Healthcare = 7;
    public const int Travel = 8;
    public const int Other = 9;

    public static readonly IReadOnlyDictionary<int, string> All = new Dictionary<int, string>
    {
        [Food] = "Food",
        [Transport] = "Transport",
        [Housing] = "Housing",
        [Shopping] = "Shopping",
        [Entertainment] = "Entertainment",
        [Bills] = "Bills",
        [Healthcare] = "Healthcare",
        [Travel] = "Travel",
        [Other] = "Other",
    };
}
