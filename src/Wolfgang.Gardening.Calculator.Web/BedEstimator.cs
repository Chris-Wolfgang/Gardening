namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// Turns what the user entered into a full estimate, or a plain-language message when the input
/// cannot be built.
/// </summary>
public static class BedEstimator
{
    /// <summary>
    /// Calculates the beds, cut list, lumber, screws and soil for every bed in <paramref name="orders"/>,
    /// buying lumber, screws and soil for all of them together.
    /// </summary>
    /// <param name="orders">The bed designs and how many of each.</param>
    /// <param name="catalog">The products to buy from.</param>
    /// <returns>The estimate, or a message saying what to fix.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> or <paramref name="catalog"/> is <see langword="null"/>.</exception>
    public static EstimateOutcome Estimate(IReadOnlyList<BedOrder> orders, MaterialCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(catalog);

        if (orders.Count == 0)
        {
            return EstimateOutcome.Failed("Add a bed.");
        }

        var lines = new List<BedLine>();
        var soil = new List<SoilRequirement>();
        for (var index = 0; index < orders.Count; index++)
        {
            var form = orders[index].Form;
            var invalid = FindInvalidInput(orders[index]);
            if (invalid is not null)
            {
                // Say which bed is wrong only when there is more than one.
                return EstimateOutcome.Failed(orders.Count == 1 ? invalid : $"Bed {index + 1}: {invalid}");
            }

            var line = Line(form, orders[index].Quantity);
            lines.Add(line);

            IReadOnlyList<SoilIngredient> mix = form.SoilMode == SoilMode.Prebagged
                ? [new SoilIngredient(catalog.PrebaggedSoil, 1m)]
                : catalog.MixIngredients;
            soil.AddRange(SoilCalculator.Split(line.SoilCubicFeet * line.Quantity, mix));
        }

        var cutList = BedEstimate.CombineCutLists(lines);
        var tooLong = FindPieceLongerThanStock(cutList, catalog.Stock);
        if (tooLong is not null)
        {
            return EstimateOutcome.Failed(tooLong);
        }

        return EstimateOutcome.Succeeded
        (
            new BedEstimate
            (
                lines,
                new StockPacker().Pack(cutList, catalog.Stock),
                catalog.Screws,
                soil
                    .GroupBy(requirement => requirement.Product)
                    .Select(group => new SoilRequirement(group.Key, group.Sum(requirement => requirement.CubicFeet)))
                    .ToList()
            )
        );
    }



    private static BedLine Line(BedForm form, int quantity)
    {
        var bed = new StackedBoardBed
        (
            form.Board,
            form.Treatment,
            Length.ToInches(form.Length, form.LengthUnit),
            Length.ToInches(form.Width, form.WidthUnit),
            form.Courses,
            form.CornerBracing == CornerBracing.TwoByTwoPosts ? BoardProfile.TwoByTwo : null
        );

        var soilCubicFeet = SoilCalculator.VolumeCubicFeet
        (
            bed,
            new SoilOptions(bed.HeightInches - form.FillDepthInches, form.SettlingPercent / 100m)
        );

        return new BedLine(bed, quantity, new FastenerRule().ScrewsPerBoardEnd(form.Board), soilCubicFeet);
    }



    private static string? FindInvalidInput(BedOrder order)
    {
        var form = order.Form;
        var lengthInches = Length.ToInches(form.Length, form.LengthUnit);
        var widthInches = Length.ToInches(form.Width, form.WidthUnit);
        var thickness = form.Board.ThicknessInches;

        if (order.Quantity < 1)
        {
            return "Build at least 1 of each bed.";
        }

        if (lengthInches <= thickness || widthInches <= thickness)
        {
            return $"The length and width must each be more than {Display.Inches(thickness)}, the board thickness.";
        }

        if (form.Courses < 1)
        {
            return "Pick a height of at least one board.";
        }

        if (form.FillDepthInches < 0m || form.FillDepthInches > form.HeightInches)
        {
            return $"The fill depth must be between 0 and the wall height, {Display.Inches(form.HeightInches)}.";
        }

        if (form.SettlingPercent < 0m)
        {
            return "Settling cannot be negative.";
        }

        return null;
    }



    private static string? FindPieceLongerThanStock(IReadOnlyList<CutPiece> pieces, IReadOnlyList<StockOption> stock)
    {
        foreach (var piece in pieces)
        {
            var longest = stock
                .Where(option => option.Kind == piece.Kind)
                .Select(option => option.LengthInches)
                .DefaultIfEmpty(0m)
                .Max();

            if (piece.LengthInches > longest)
            {
                var kind = Display.Kind(piece.Kind);
                return longest == 0m
                    ? $"No {kind} boards are for sale."
                    : $"A {Display.Inches(piece.LengthInches)} piece is longer than the longest {kind} board for sale ({Display.Feet(longest)}). Make the bed smaller.";
            }
        }

        return null;
    }
}
