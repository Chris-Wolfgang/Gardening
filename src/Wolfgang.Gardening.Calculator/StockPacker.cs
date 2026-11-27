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
/// <see cref="MaxSearchSteps"/> the packer falls back to a heuristic: for each stock length it fills boards
/// of that length first-fit-decreasing and buys each board at the cheapest length that holds its cuts, then
/// keeps the cheapest of those plans.
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
    /// <exception cref="ArgumentException">
    /// <paramref name="pieces"/> contains a <see langword="null"/> piece, a piece with no
    /// <see cref="CutPiece.Kind"/>, a piece whose length is not positive or a piece whose quantity is negative;
    /// or <paramref name="stock"/> contains a <see langword="null"/> option.
    /// </exception>
    /// <exception cref="InvalidOperationException">No stock of a piece's kind is long enough for it.</exception>
    public PackingResult Pack(IEnumerable<CutPiece> pieces, IEnumerable<StockOption> stock)
    {
        ArgumentNullException.ThrowIfNull(pieces);
        ArgumentNullException.ThrowIfNull(stock);

        // The cut list is read twice (validated, then grouped), so copy it only when it is a lazy sequence.
        var cutList = pieces as ICollection<CutPiece> ?? pieces.ToList();
        foreach (var piece in cutList)
        {
            Validate(piece, nameof(pieces));
        }

        var options = stock.ToList();
        if (options.Contains(null!))
        {
            throw new ArgumentException("The stock list contains a null option.", nameof(stock));
        }

        var boards = new List<PackedBoard>();

        foreach (var group in cutList.GroupBy(piece => piece.Kind))
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



    // CutPiece is a positional record, so a `with` expression can bypass any constructor check; validate here.
    private static void Validate(CutPiece? piece, string paramName)
    {
        if (piece is null)
        {
            throw new ArgumentException("The cut list contains a null piece.", paramName);
        }

        if (piece.Kind is null)
        {
            throw new ArgumentException("A piece in the cut list has no lumber kind.", paramName);
        }

        if (piece.LengthInches <= 0m)
        {
            throw new ArgumentException
            (
                $"A {piece.Purpose} piece is {piece.LengthInches} in long; pieces must be longer than 0 in.",
                paramName
            );
        }

        if (piece.Quantity < 0)
        {
            throw new ArgumentException
            (
                $"A {piece.Purpose} piece has a quantity of {piece.Quantity}; the quantity cannot be negative.",
                paramName
            );
        }
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
            var pattern = memo[StateKey(remaining)].Pattern!;
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
    // first size still needed, so the same set of boards is not tried in every order. The search is a
    // depth-first walk with an explicit stack: one level per board, so a recursive walk would overflow the
    // call stack on a cut list of a few thousand pieces.
    private static void Solve(int[] counts, List<Pattern> patterns, Dictionary<string, Choice> memo)
    {
        // PackKind only searches a non-empty cut list, so the first frame always has a piece to cut.
        var frames = new Stack<Frame>();
        frames.Push(new Frame(counts, patterns));

        while (frames.Count > 0)
        {
            var frame = frames.Peek();

            if (frame.Next == frame.Candidates.Count)
            {
                // A pattern of one piece of the first size always fits, so there is always a choice.
                memo[frame.Key] = frame.Best!;
                frames.Pop();
                continue;
            }

            var pattern = frame.Candidates[frame.Next];
            var rest = frame.Remaining.Zip(pattern.Counts, (have, use) => have - use).ToArray();

            var after = Array.TrueForAll(rest, count => count == 0)
                ? new Choice(0m, 0, Pattern: null)
                : memo.GetValueOrDefault(StateKey(rest));

            if (after is null)
            {
                // Solve what is left first; this pattern is priced when the walk comes back to this frame.
                frames.Push(new Frame(rest, patterns));
                continue;
            }

            var choice = new Choice(after.Cost + pattern.Board.Stock.Price, after.Boards + 1, pattern);
            if (frame.Best is null || choice.Cost < frame.Best.Cost || (choice.Cost == frame.Best.Cost && choice.Boards < frame.Best.Boards))
            {
                frame.Best = choice;
            }

            frame.Next++;
        }
    }



    private static string StateKey(int[] counts)
    {
        return string.Join(',', counts);
    }



    // Fallback, cheapest of two kinds of plan (a tie goes to fewer boards):
    // - for each stock length that holds the longest piece, fill boards of that length first-fit-decreasing
    //   and buy each board at its cheapest length;
    // - split the sizes, longest first, into runs small enough for the exact search, and search each run.
    // Filling only the longest stock overbuys when longer boards cost more per foot.
    private List<PackedBoard> Heuristic(List<decimal> lengths, List<StockOption> options)
    {
        var longest = lengths[0];

        var plans = options
            .Select(option => option.LengthInches)
            .Where(capacity => capacity >= longest)
            .Distinct()
            .Select
            (
                capacity => FirstFitDecreasing(lengths, capacity)
                    .Select(cuts => Cheapest(cuts, options))
                    .ToList()
            )
            .ToList();

        plans.Add(SearchInRuns(lengths, options));

        return plans
            .OrderBy(plan => plan.Sum(board => board.Stock.Price))
            .ThenBy(plan => plan.Count)
            .First();
    }



    // Adds sizes, longest first, to a run while the exact search can still solve it, then starts a new run.
    // A run of one size that still cannot be searched is packed first-fit-decreasing.
    private List<PackedBoard> SearchInRuns(List<decimal> lengths, List<StockOption> options)
    {
        var boards = new List<PackedBoard>();
        var run = new List<decimal>();
        List<PackedBoard>? solved = null;

        foreach (var size in lengths.Distinct())
        {
            var pieces = lengths.FindAll(length => length == size);
            var attempt = Search([.. run, .. pieces], options);

            if (attempt is null && run.Count > 0)
            {
                boards.AddRange(solved!);
                run.Clear();
                attempt = Search(pieces, options);
            }

            run.AddRange(pieces);
            solved = attempt ?? FirstFitDecreasing(run, options.Max(option => option.LengthInches))
                .Select(cuts => Cheapest(cuts, options))
                .ToList();
        }

        boards.AddRange(solved!);
        return boards;
    }



    private List<List<decimal>> FirstFitDecreasing(List<decimal> lengths, decimal capacity)
    {
        var bins = new List<List<decimal>>();
        var free = new List<decimal>();

        // Room on a board only shrinks, so for a run of equal lengths the first board that fits never moves
        // back: each run resumes the scan where the previous piece of that length went.
        var index = 0;
        var previous = -1m;

        foreach (var length in lengths)
        {
            if (length != previous)
            {
                index = 0;
                previous = length;
            }

            while (index < bins.Count && free[index] < KerfInches + length)
            {
                index++;
            }

            if (index == bins.Count)
            {
                bins.Add([length]);
                free.Add(capacity - length);
            }
            else
            {
                bins[index].Add(length);
                free[index] -= KerfInches + length;
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

        return new PackedBoard(stock, cuts, Leftover(stock.LengthInches - used));
    }



    // What is left after the last piece: cutting it off the offcut takes one more kerf, unless the piece
    // already ends at (or within a kerf of) the end of the board.
    private decimal Leftover(decimal afterCuts)
    {
        return Math.Max(0m, afterCuts - KerfInches);
    }



    private decimal Used(List<decimal> cuts)
    {
        return cuts.Sum() + (KerfInches * (cuts.Count - 1));
    }



    // How many pieces of each size one board takes, and that board at its cheapest stock length.
    private sealed record Pattern(int[] Counts, PackedBoard Board);



    // The cost and board count of cutting the remaining pieces, and the pattern to cut first.
    private sealed record Choice(decimal Cost, int Boards, Pattern? Pattern);



    // One level of the search: the pieces still to cut, the patterns that take a piece of the first size
    // still needed, the next of those patterns to price, and the cheapest choice found so far.
    private sealed class Frame
    {
        public Frame(int[] remaining, List<Pattern> patterns)
        {
            var first = Array.FindIndex(remaining, count => count > 0);

            Remaining = remaining;
            Key = StateKey(remaining);
            Candidates = patterns
                .Where(pattern => pattern.Counts[first] > 0 && Fits(pattern.Counts, remaining))
                .ToList();
        }



        public int[] Remaining { get; }



        public string Key { get; }



        public List<Pattern> Candidates { get; }



        public int Next { get; set; }



        public Choice? Best { get; set; }



        private static bool Fits(int[] use, int[] have)
        {
            return use.Zip(have, (needed, available) => needed <= available).All(fits => fits);
        }
    }
}
