using System.Globalization;

namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// Formats calculator values for the page. Everything is shown in US imperial units and US dollars,
/// whatever the browser's culture.
/// </summary>
public static class Display
{
    private static readonly CultureInfo UnitedStates = CultureInfo.GetCultureInfo("en-US");



    /// <summary>Formats a price, e.g. <c>$9.98</c>.</summary>
    /// <param name="amount">The price.</param>
    /// <returns>The formatted price.</returns>
    public static string Money(decimal amount)
    {
        return amount.ToString("C", UnitedStates);
    }



    /// <summary>Formats a length in inches, e.g. <c>97.5 in</c>.</summary>
    /// <param name="inches">The length in inches.</param>
    /// <returns>The formatted length.</returns>
    public static string Inches(decimal inches)
    {
        return $"{Number(inches)} in";
    }



    /// <summary>
    /// Formats a length in inches and, from a foot up, in feet and inches too, e.g. <c>97.5 in (8 ft 1.5 in)</c>,
    /// <c>96 in (8 ft)</c> or <c>11 in</c>.
    /// </summary>
    /// <param name="inches">The length in inches.</param>
    /// <returns>The formatted length.</returns>
    public static string InchesAndFeet(decimal inches)
    {
        // Round first so 23.999 in shows as 2 ft, not 1 ft 12 in.
        var rounded = Math.Round(inches, 2);
        var feet = Math.Floor(rounded / Length.InchesPerFoot);
        if (feet < 1m)
        {
            return Inches(rounded);
        }

        var rest = rounded - (feet * Length.InchesPerFoot);
        var feetAndInches = rest == 0m ? $"{Number(feet)} ft" : $"{Number(feet)} ft {Inches(rest)}";
        return $"{Inches(rounded)} ({feetAndInches})";
    }



    /// <summary>Formats a length as the user entered it, e.g. <c>8 ft</c> or <c>30 in</c>.</summary>
    /// <param name="value">The length in <paramref name="unit"/>.</param>
    /// <param name="unit">The unit the length is in.</param>
    /// <returns>The formatted length.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="unit"/> is not a defined <see cref="LengthUnit"/>.</exception>
    public static string Measure(decimal value, LengthUnit unit)
    {
        var symbol = unit switch
        {
            LengthUnit.Feet => "ft",
            LengthUnit.Inches => "in",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unknown length unit."),
        };

        return $"{Number(value)} {symbol}";
    }



    /// <summary>Formats a length given in inches as feet, e.g. <c>8 ft</c>.</summary>
    /// <param name="inches">The length in inches.</param>
    /// <returns>The formatted length.</returns>
    public static string Feet(decimal inches)
    {
        return $"{Number(inches / Length.InchesPerFoot)} ft";
    }



    /// <summary>Formats a volume, e.g. <c>27.97 cu ft</c>.</summary>
    /// <param name="cubicFeet">The volume in cubic feet.</param>
    /// <returns>The formatted volume.</returns>
    public static string CubicFeet(decimal cubicFeet)
    {
        return $"{Number(cubicFeet)} cu ft";
    }



    /// <summary>Names a kind of lumber, e.g. <c>pressure-treated 2x6</c>.</summary>
    /// <param name="kind">The lumber kind.</param>
    /// <returns>The name.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="kind"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The treatment is not a defined <see cref="Treatment"/>.</exception>
    public static string Kind(LumberKind kind)
    {
        ArgumentNullException.ThrowIfNull(kind);

        var treatment = kind.Treatment switch
        {
            Treatment.PressureTreated => "pressure-treated",
            Treatment.Untreated => "untreated",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind.Treatment, "Unknown treatment."),
        };

        return $"{treatment} {kind.Profile.Name}";
    }



    /// <summary>Says what a cut piece is for, e.g. <c>Long walls</c>.</summary>
    /// <param name="purpose">The piece's purpose.</param>
    /// <returns>The description.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="purpose"/> is not a defined <see cref="PiecePurpose"/>.</exception>
    public static string Purpose(PiecePurpose purpose)
    {
        return purpose switch
        {
            PiecePurpose.LengthWall => "Long walls",
            PiecePurpose.WidthWall => "End walls",
            PiecePurpose.CornerPost => "Corner posts",
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, "Unknown piece purpose."),
        };
    }



    private static string Number(decimal value)
    {
        return Math.Round(value, 2).ToString("0.##", CultureInfo.InvariantCulture);
    }
}
