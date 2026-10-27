namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A line of the cut list: how many pieces of one kind and length a bed needs.
/// </summary>
/// <param name="Kind">The lumber the pieces are cut from.</param>
/// <param name="LengthInches">The length of each piece in inches.</param>
/// <param name="Quantity">How many pieces.</param>
/// <param name="Purpose">What the pieces are for.</param>
public sealed record CutPiece(LumberKind Kind, decimal LengthInches, int Quantity, PiecePurpose Purpose);
