namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// Unit conversions. Every length inside the calculator is in inches and every volume in cubic feet;
/// this is the one place that knows how the imperial units relate.
/// </summary>
public static class Length
{
    /// <summary>Inches in one foot.</summary>
    public static decimal InchesPerFoot => 12m;



    /// <summary>Cubic inches in one cubic foot.</summary>
    public static decimal CubicInchesPerCubicFoot => 1728m;



    /// <summary>
    /// Converts a length entered in <paramref name="unit"/> to inches.
    /// </summary>
    /// <param name="value">The length as entered.</param>
    /// <param name="unit">The unit <paramref name="value"/> is in.</param>
    /// <returns>The length in inches.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="unit"/> is not a defined <see cref="LengthUnit"/>.</exception>
    /// <example>
    /// <code>
    /// var inches = Length.ToInches(8m, LengthUnit.Feet); // 96
    /// </code>
    /// </example>
    public static decimal ToInches(decimal value, LengthUnit unit)
    {
        return unit switch
        {
            LengthUnit.Inches => value,
            LengthUnit.Feet => value * InchesPerFoot,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown length unit."),
        };
    }



    /// <summary>
    /// Converts a volume in cubic inches to cubic feet.
    /// </summary>
    /// <param name="cubicInches">The volume in cubic inches.</param>
    /// <returns>The volume in cubic feet.</returns>
    public static decimal ToCubicFeet(decimal cubicInches)
    {
        return cubicInches / CubicInchesPerCubicFoot;
    }
}
