namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A kind of lumber that is bought as one stock item: a profile and a treatment.
/// Pieces are only cut from stock of the same kind.
/// </summary>
/// <param name="Profile">The board's cross-section.</param>
/// <param name="Treatment">Whether the board is pressure treated.</param>
public sealed record LumberKind(BoardProfile Profile, Treatment Treatment);
