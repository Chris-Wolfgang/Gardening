namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// Packs a cut list into stock boards, aiming for the lowest cost.
/// </summary>
/// <remarks>
/// <para>
/// For each lumber kind the packer tries every stock length long enough for the longest piece: it fills
/// boards of that length first-fit-decreasing, then swaps each filled board for the cheapest stock length
/// that still holds its cuts. The cheapest of those plans wins; a tie goes to the plan with fewer boards.
/// </para>
/// <para>
/// This is a heuristic, not an exhaustive search, but it finds the obvious plans such as two 4 ft pieces
/// from one 8 ft board.
/// </para>
/// </remarks>
public sealed class StockPacker
{
    /// <summary>
    /// Initializes a new <see cref="StockPacker"/>.
    /// </summary>
    /// <param name="kerfInches">The material each saw cut removes between two pieces, in inches.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="kerfInches"/> is negative.</exception>
    public StockPacker(decimal kerfInches = 0m)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(kerfInches);

        KerfInches = kerfInches;
    }



    /// <summary>The material each saw cut removes between two pieces, in inches.</summary>
    public decimal KerfInches { get; }



    /// <summary>
    /// Packs the pieces into stock boards.
    /// </summary>
    /// <param name="pieces">The cut list.</param>
    /// <param name="stock">The boards that can be bought.</param>
    /// <returns>The boards to buy and how each is cut.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pieces"/> or <paramref name="stock"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">No stock of a piece's kind is long enough for it.</exception>
    public PackingResult Pack(IEnumerable<CutPiece> pieces, IEnumerable<StockOption> stock)
    {
        ArgumentNullException.ThrowIfNull(pieces);
        ArgumentNullException.ThrowIfNull(stock);

        var options = stock.ToList();
        var boards = new List<PackedBoard>();

        foreach (var group in pieces.GroupBy(piece => piece.Kind))
        {
            var lengths = group
                .SelectMany(piece => Enumerable.Repeat(piece.LengthInches, piece.Quantity))
                .OrderByDescending(length => length)
                .ToList();

            var kindOptions = options
                .Where(option => option.Kind == group.Key)
                .ToList();

            boards.AddRange(PackKind(group.Key, lengths, kindOptions));
        }

        return new PackingResult(boards);
    }



    private List<PackedBoard> PackKind(LumberKind kind, List<decimal> lengths, List<StockOption> options)
    {
        if (lengths.Count == 0)
        {
            return [];
        }

        var longest = lengths[0];
        var candidates = options
            .Where(option => option.LengthInches >= longest)
            .ToList();

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException
            (
                $"No {kind.Treatment} {kind.Profile.Name} stock is long enough for a {longest} in piece."
            );
        }

        var best = Plan(lengths, candidates[0], options);
        var bestCost = best.Sum(board => board.Stock.Price);

        foreach (var candidate in candidates.Skip(1))
        {
            var plan = Plan(lengths, candidate, options);
            var cost = plan.Sum(board => board.Stock.Price);

            if (cost < bestCost || (cost == bestCost && plan.Count < best.Count))
            {
                best = plan;
                bestCost = cost;
            }
        }

        return best;
    }



    private List<PackedBoard> Plan(List<decimal> lengths, StockOption fillWith, List<StockOption> options)
    {
        return FirstFitDecreasing(lengths, fillWith.LengthInches)
            .Select(cuts => Cheapest(cuts, options))
            .ToList();
    }



    private List<List<decimal>> FirstFitDecreasing(List<decimal> lengths, decimal capacity)
    {
        var bins = new List<List<decimal>>();

        foreach (var length in lengths)
        {
            var bin = bins.Find(cuts => Used(cuts) + KerfInches + length <= capacity);

            if (bin is null)
            {
                bins.Add([length]);
            }
            else
            {
                bin.Add(length);
            }
        }

        return bins;
    }



    private PackedBoard Cheapest(List<decimal> cuts, List<StockOption> options)
    {
        var used = Used(cuts);

        var stock = options
            .Where(option => option.LengthInches >= used)
            .OrderBy(option => option.Price)
            .ThenBy(option => option.LengthInches)
            .First();

        return new PackedBoard(stock, cuts, stock.LengthInches - used);
    }



    private decimal Used(List<decimal> cuts)
    {
        return cuts.Sum() + (KerfInches * (cuts.Count - 1));
    }
}
