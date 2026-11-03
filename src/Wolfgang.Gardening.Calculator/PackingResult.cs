namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// The boards to buy for a cut list and how each one is cut.
/// </summary>
public sealed class PackingResult
{
    /// <summary>
    /// Initializes a new <see cref="PackingResult"/>.
    /// </summary>
    /// <param name="boards">The packed boards.</param>
    /// <exception cref="ArgumentNullException"><paramref name="boards"/> is <see langword="null"/>.</exception>
    public PackingResult(IReadOnlyList<PackedBoard> boards)
    {
        ArgumentNullException.ThrowIfNull(boards);

        Boards = boards;
    }



    /// <summary>Every board to buy, with its cuts.</summary>
    public IReadOnlyList<PackedBoard> Boards { get; }



    /// <summary>The total cost of the boards.</summary>
    public decimal TotalCost => Boards.Sum(board => board.Stock.Price);



    /// <summary>The boards grouped into a purchase list, one line per stock option.</summary>
    public IReadOnlyList<StockPurchase> Purchases =>
        Boards
            .GroupBy(board => board.Stock)
            .Select(group => new StockPurchase(group.Key, group.Count()))
            .ToList();
}
