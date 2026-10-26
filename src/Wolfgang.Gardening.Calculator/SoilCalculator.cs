namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// Works out how much soil a bed holds and how it splits across the products of a mix.
/// </summary>
/// <remarks>
/// Prebagged soil is a mix of one ingredient. For the equal-parts mix of peat moss, compost and vermiculite,
/// pass three ingredients of one part each. <see cref="SoilRequirement.CubicFeet"/> is the raw volume of each
/// item; <see cref="SoilRequirement.Packages"/> rounds it up to whole packages.
/// </remarks>
public static class SoilCalculator
{
    /// <summary>
    /// Returns the soil volume to buy for a bed: the inside area times the fill depth, plus the settling allowance.
    /// </summary>
    /// <param name="bed">The bed.</param>
    /// <param name="options">The fill and settling options.</param>
    /// <returns>The volume in cubic feet. A bed whose headroom is at least its height needs none.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="bed"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public static decimal VolumeCubicFeet(StackedBoardBed bed, SoilOptions options)
    {
        ArgumentNullException.ThrowIfNull(bed);
        ArgumentNullException.ThrowIfNull(options);

        var depth = Math.Max(0m, bed.HeightInches - options.HeadroomInches);
        var cubicInches = bed.InsideLengthInches * bed.InsideWidthInches * depth;

        return Length.ToCubicFeet(cubicInches) * (1m + options.SettlingAllowance);
    }



    /// <summary>
    /// Splits a volume across the ingredients of a mix in proportion to their parts.
    /// </summary>
    /// <param name="cubicFeet">The total volume in cubic feet.</param>
    /// <param name="ingredients">The ingredients of the mix.</param>
    /// <returns>One requirement per ingredient, in the order given.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="ingredients"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="cubicFeet"/> is negative.</exception>
    /// <exception cref="ArgumentException"><paramref name="ingredients"/> is empty.</exception>
    public static IReadOnlyList<SoilRequirement> Split(decimal cubicFeet, IEnumerable<SoilIngredient> ingredients)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cubicFeet);
        ArgumentNullException.ThrowIfNull(ingredients);

        var list = ingredients.ToList();

        if (list.Count == 0)
        {
            throw new ArgumentException("A mix needs at least one ingredient.", nameof(ingredients));
        }

        var totalParts = list.Sum(ingredient => ingredient.Parts);

        return list
            .Select(ingredient => new SoilRequirement(ingredient.Product, cubicFeet * ingredient.Parts / totalParts))
            .ToList();
    }
}
