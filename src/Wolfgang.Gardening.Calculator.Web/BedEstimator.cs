namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// Turns what the user entered into a full estimate, or a plain-language message when the input
/// cannot be built.
/// </summary>
public static class BedEstimator
{
    /// <summary>
    /// Calculates the bed, cut list, lumber, screws and soil for <paramref name="form"/>.
    /// </summary>
    /// <param name="form">What the user entered.</param>
    /// <param name="catalog">The products to buy from.</param>
    /// <returns>The estimate, or a message saying what to fix.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="form"/> or <paramref name="catalog"/> is <see langword="null"/>.</exception>
    public static EstimateOutcome Estimate(BedForm form, MaterialCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentNullException.ThrowIfNull(catalog);

        var board = form.Board;
        var lengthInches = Length.ToInches(form.Length, form.LengthUnit);
        var widthInches = Length.ToInches(form.Width, form.WidthUnit);

        var invalid = FindInvalidInput(form, lengthInches, widthInches);
        if (invalid is not null)
        {
            return EstimateOutcome.Failed(invalid);
        }

        var bed = new StackedBoardBed
        (
            board,
            form.Treatment,
            lengthInches,
            widthInches,
            form.Courses,
            form.CornerPosts ? BoardProfile.TwoByTwo : null
        );

        var tooLong = FindPieceLongerThanStock(bed.CutList(), catalog.Stock);
        if (tooLong is not null)
        {
            return EstimateOutcome.Failed(tooLong);
        }

        var lumber = new StockPacker(form.KerfInches).Pack(bed.CutList(), catalog.Stock);
        var soilCubicFeet = SoilCalculator.VolumeCubicFeet
        (
            bed,
            // Max: 11.0m - 11m is a negative zero, which SoilOptions rejects as negative.
            new SoilOptions(Math.Max(0m, bed.HeightInches - form.FillDepthInches), form.SettlingPercent / 100m)
        );
        IReadOnlyList<SoilIngredient> mix = form.SoilMode == SoilMode.Prebagged
            ? [new SoilIngredient(catalog.PrebaggedSoil, 1m)]
            : catalog.MixIngredients;

        return EstimateOutcome.Succeeded
        (
            new BedEstimate
            (
                bed,
                lumber,
                new FastenerRule().ScrewsPerBoardEnd(board),
                catalog.Screws,
                soilCubicFeet,
                SoilCalculator.Split(soilCubicFeet, mix)
            )
        );
    }



    private static string? FindInvalidInput(BedForm form, decimal lengthInches, decimal widthInches)
    {
        var thickness = form.Board.ThicknessInches;
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

        if (form.SettlingPercent < 0m || form.KerfInches < 0m)
        {
            return "Settling and blade width cannot be negative.";
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
