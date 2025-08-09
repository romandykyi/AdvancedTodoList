namespace AdvancedTodoList.UnitTests.Validation;

public static class ValidationConstants
{
    public static readonly string[] EmptyNotNullStrings =
    [
        string.Empty,
        "  ",
        "\r",
        "\t",
        " \r \t  "
    ];
    public static readonly string[] EmptyStrings =
    [
        null!,
        .. EmptyNotNullStrings
    ];
}
