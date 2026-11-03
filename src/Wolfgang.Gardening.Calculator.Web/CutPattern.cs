namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// One way of cutting a stock board, and how many boards are cut that way.
/// </summary>
/// <param name="Stock">The board bought.</param>
/// <param name="Cuts">The piece lengths cut from it, in inches, longest first.</param>
/// <param name="LeftoverInches">What is left of each board after the cuts.</param>
/// <param name="Count">How many boards are cut this way.</param>
public sealed record CutPattern(StockOption Stock, IReadOnlyList<decimal> Cuts, decimal LeftoverInches, int Count);
