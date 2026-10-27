namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// The cross-section of a board: its nominal name and its actual thickness and width in inches.
/// </summary>
/// <remarks>
/// In a stacked-board bed the width is the height of one course and the thickness is the wall thickness.
/// </remarks>
public sealed record BoardProfile
{
    /// <summary>
    /// Initializes a new <see cref="BoardProfile"/>.
    /// </summary>
    /// <param name="name">The nominal name, for example <c>2x6</c>.</param>
    /// <param name="thicknessInches">The actual thickness in inches.</param>
    /// <param name="widthInches">The actual width in inches.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="thicknessInches"/> or <paramref name="widthInches"/> is not positive.</exception>
    public BoardProfile(string name, decimal thicknessInches, decimal widthInches)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(thicknessInches);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(widthInches);

        Name = name;
        ThicknessInches = thicknessInches;
        WidthInches = widthInches;
    }



    /// <summary>A 2x2, used for corner posts: 1.5 in x 1.5 in.</summary>
    public static BoardProfile TwoByTwo { get; } = new("2x2", 1.5m, 1.5m);



    /// <summary>A 2x4: 1.5 in x 3.5 in.</summary>
    public static BoardProfile TwoByFour { get; } = new("2x4", 1.5m, 3.5m);



    /// <summary>A 2x6: 1.5 in x 5.5 in.</summary>
    public static BoardProfile TwoBySix { get; } = new("2x6", 1.5m, 5.5m);



    /// <summary>A 2x8: 1.5 in x 7.25 in.</summary>
    public static BoardProfile TwoByEight { get; } = new("2x8", 1.5m, 7.25m);



    /// <summary>A 2x10: 1.5 in x 9.25 in.</summary>
    public static BoardProfile TwoByTen { get; } = new("2x10", 1.5m, 9.25m);



    /// <summary>A 2x12: 1.5 in x 11.25 in.</summary>
    public static BoardProfile TwoByTwelve { get; } = new("2x12", 1.5m, 11.25m);



    /// <summary>The 2x boards offered for bed walls, narrowest first.</summary>
    public static IReadOnlyList<BoardProfile> TwoByWallBoards { get; } =
    [
        TwoByFour,
        TwoBySix,
        TwoByEight,
        TwoByTen,
        TwoByTwelve,
    ];



    /// <summary>The nominal name, for example <c>2x6</c>.</summary>
    public string Name { get; }



    /// <summary>The actual thickness in inches.</summary>
    public decimal ThicknessInches { get; }



    /// <summary>The actual width in inches.</summary>
    public decimal WidthInches { get; }
}
