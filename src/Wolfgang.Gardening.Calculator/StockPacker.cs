namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// Packs a cut list into stock boards, aiming for the lowest cost.
/// </summary>
/// <remarks>
/// <para>
/// For each lumber kind the packer lists every way the pieces can share one board (a pattern), prices
/// each pattern at the cheapest stock length that holds it, and searches for the set of patterns that
/// cuts every piece for the lowest total cost; a tie goes to the plan with fewer boards. Stock lengths
/// can be mixed, so four 12 ft walls and four 4 ft ends buy four 12 ft and two 8 ft boards.
/// </para>
/// <para>
/// A cut list with many different lengths would make that search too slow, so past
/// <see cref="MaxSearchSteps"/> the packer falls back to a heuristic: it fills the longest stock
/// first-fit-decreasing, then buys each board at the cheapest length that holds its cuts.
/// </para>
/// </remarks>
public sealed class StockPacker
{
    /// <summary>
    /// The most search steps (piece-count combinations times patterns) the exact search may take for one
    /// lumber kind before the packer falls back to the heuristic.
    /// </summary>
    public const int MaxSearchSteps = 2_000_000;




    /// <summary>
    /// Initializes a new <see cref="StockPacker"/>.
    /// </summary>
    /// <param name="kerfInches">The material each saw cut removes between two pieces, in inches.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="kerfInches"/> is negative.</exception>
    public StockPacker(decimal kerfInches = 0m)
    {
        Guard.ThrowIfNegative(kerfInches);

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
        if (!options.Exists(option => option.LengthInches >= longest))
        {
            throw new InvalidOperationException
            (
                $"No {kind.Treatment} {kind.Profile.Name} stock is long enough for a {longest} in piece."
            );
        }

        return Search(lengths, options) ?? Heuristic(lengths, options);
    }



    // Exact search: the cheapest set of patterns that cuts every piece, or null when it would take too long.
    private List<PackedBoard>? Search(List<decimal> lengths, List<StockOption> options)
    {
        var sizes = lengths.Distinct().ToArray();
        var counts = sizes.Select(size => lengths.Count(length => length == size)).ToArray();
        var longestStock = options.Max(option => option.LengthInches);

        // Each combination of piece counts still to cut is tried against every pattern.
        var states = 1L;
        foreach (var count in counts)
        {
            states = Math.Min(states * (count + 1), MaxSearchSteps + 1L);
        }

        var patterns = new List<Pattern>();
        if (!AddPatterns(sizes, counts, longestStock, options, new int[sizes.Length], 0, MaxSearchSteps / states, patterns))
        {
            return null;
        }

        var memo = new Dictionary<string, Choice>();
        Solve(counts, patterns, memo);

        var boards = new List<PackedBoard>();
        var remaining = (int[])counts.Clone();
        while (remaining.Any(count => count > 0))
        {
            var pattern = memo[Key(remaining)].Pattern!;
            boards.Add(pattern.Board);
            for (var i = 0; i < remaining.Length; i++)
            {
                remaining[i] -= pattern.Counts[i];
            }
        }

        return boards;
    }



    // Every non-empty way to cut pieces of the given sizes, up to the given counts, from the longest stock.
    // Returns false as soon as there would be more than maxPatterns.
    private bool AddPatterns(decimal[] sizes, int[] counts, decimal capacity, List<StockOption> options, int[] current, int index, long maxPatterns, List<Pattern> patterns)
    {
        if (index == sizes.Length)
        {
            var cuts = sizes
                .SelectMany((size, i) => Enumerable.Repeat(size, current[i]))
                .ToList();

            if (cuts.Count == 0)
            {
                return true;
            }

            patterns.Add(new Pattern((int[])current.Clone(), Cheapest(cuts, options)));
            return patterns.Count <= maxPatterns;
        }

        for (var take = 0; take <= counts[index]; take++)
        {
            current[index] = take;
            var cutsSoFar = sizes
                .Take(index + 1)
                .SelectMany((size, i) => Enumerable.Repeat(size, current[i]))
                .ToList();

            if (Used(cutsSoFar) > capacity)
            {
                break;
            }

            if (!AddPatterns(sizes, counts, capacity, options, current, index + 1, maxPatterns, patterns))
            {
                return false;
            }
        }

        current[index] = 0;
        return true;
    }



    // The cheapest way to cut the remaining pieces. Each step takes a pattern that includes a piece of the
    // first size still needed, so the same set of boards is not tried in every order.
    private static Choice Solve(int[] remaining, List<Pattern> patterns, Dictionary<string, Choice> memo)
    {
        var first = Array.FindIndex(remaining, count => count > 0);
        if (first < 0)
        {
            return new Choice(0m, 0, Pattern: null);
        }

        var key = Key(remaining);
        if (memo.TryGetValue(key, out var known))
        {
            return known;
        }

        Choice? best = null;
        foreach (var pattern in patterns.Where(pattern => pattern.Counts[first] > 0 && Fits(pattern.Counts, remaining)))
        {
            var rest = remaining.Zip(pattern.Counts, (have, use) => have - use).ToArray();
            var after = Solve(rest, patterns, memo);
            var choice = new Choice(after.Cost + pattern.Board.Stock.Price, after.Boards + 1, pattern);

            if (best is null || choice.Cost < best.Cost || (choice.Cost == best.Cost && choice.Boards < best.Boards))
            {
                best = choice;
            }
        }

        // A pattern of one piece of the first size always fits, so there is always a choice.
        memo[key] = best!;
        return best!;
    }



    private static bool Fits(int[] use, int[] have)
    {
        return use.Zip(have, (needed, available) => needed <= available).All(fits => fits);
    }



    private static string Key(int[] counts)
    {
        return string.Join(',', counts);
    }



    // Fallback: fill the longest stock first-fit-decreasing, then buy each board at its cheapest length.
    private List<PackedBoard> Heuristic(List<decimal> lengths, List<StockOption> options)
    {
        return FirstFitDecreasing(lengths, options.Max(option => option.LengthInches))
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



    // How many pieces of each size one board takes, and that board at its cheapest stock length.
    private sealed record Pattern(int[] Counts, PackedBoard Board);



    // The cost and board count of cutting the remaining pieces, and the pattern to cut first.
    private sealed record Choice(decimal Cost, int Boards, Pattern? Pattern);
}
