namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// How many screws fasten each board end: at least <see cref="MinimumPerBoardEnd"/>, otherwise one for
/// every <see cref="SpacingInches"/> of board width.
/// </summary>
/// <remarks>
/// The rule is the same whether a board end is screwed into a corner post or into the face of the next board.
/// </remarks>
public sealed class FastenerRule
{
    /// <summary>
    /// Initializes a new <see cref="FastenerRule"/>.
    /// </summary>
    /// <param name="minimumPerBoardEnd">The fewest screws in any board end.</param>
    /// <param name="spacingInches">The board width each screw covers, in inches.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minimumPerBoardEnd"/> is less than one or <paramref name="spacingInches"/> is not positive.
    /// </exception>
    public FastenerRule(int minimumPerBoardEnd = 2, decimal spacingInches = 3.5m)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(minimumPerBoardEnd, 1);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(spacingInches);

        MinimumPerBoardEnd = minimumPerBoardEnd;
        SpacingInches = spacingInches;
    }



    /// <summary>The fewest screws in any board end.</summary>
    public int MinimumPerBoardEnd { get; }



    /// <summary>The board width each screw covers, in inches.</summary>
    public decimal SpacingInches { get; }



    /// <summary>
    /// Returns the number of screws in one end of a board of the given profile.
    /// </summary>
    /// <param name="board">The board profile.</param>
    /// <returns>The screws per board end.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="board"/> is <see langword="null"/>.</exception>
    public int ScrewsPerBoardEnd(BoardProfile board)
    {
        ArgumentNullException.ThrowIfNull(board);

        var bySpacing = (int)Math.Ceiling(board.WidthInches / SpacingInches);

        return Math.Max(MinimumPerBoardEnd, bySpacing);
    }



    /// <summary>
    /// Returns the number of screws the whole bed needs.
    /// </summary>
    /// <param name="bed">The bed.</param>
    /// <returns>The total screw count.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="bed"/> is <see langword="null"/>.</exception>
    public int ScrewsFor(StackedBoardBed bed)
    {
        ArgumentNullException.ThrowIfNull(bed);

        return bed.BoardEnds * ScrewsPerBoardEnd(bed.Board);
    }
}
