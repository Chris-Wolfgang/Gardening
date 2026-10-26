namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A line of the lumber purchase list.
/// </summary>
/// <param name="Stock">The board to buy.</param>
/// <param name="Quantity">How many to buy.</param>
public sealed record StockPurchase(StockOption Stock, int Quantity)
{
    /// <summary>The cost of the line.</summary>
    public decimal Cost => Stock.Price * Quantity;
}
