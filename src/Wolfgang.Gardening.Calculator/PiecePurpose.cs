namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// What a cut piece is for.
/// </summary>
public enum PiecePurpose
{
    /// <summary>A wall board along the bed's length.</summary>
    LengthWall = 0,

    /// <summary>A wall board along the bed's width.</summary>
    WidthWall = 1,

    /// <summary>A post inside a corner that the wall boards are screwed to.</summary>
    CornerPost = 2,
}
