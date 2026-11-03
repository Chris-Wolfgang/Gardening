namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// One stock board to buy and the pieces cut from it.
/// </summary>
/// <param name="Stock">The board to buy.</param>
/// <param name="Cuts">The lengths cut from it, in inches, longest first.</param>
/// <param name="LeftoverInches">What remains of the board after the cuts and saw kerfs, in inches.</param>
public sealed record PackedBoard(StockOption Stock, IReadOnlyList<decimal> Cuts, decimal LeftoverInches);
