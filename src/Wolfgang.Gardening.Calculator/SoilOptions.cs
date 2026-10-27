namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// How full the bed is filled and how much extra soil to allow for settling.
/// </summary>
public sealed class SoilOptions
{
    /// <summary>
    /// Initializes a new <see cref="SoilOptions"/>.
    /// </summary>
    /// <param name="headroomInches">How far below the top of the wall the soil stops, in inches.</param>
    /// <param name="settlingAllowance">Extra soil to buy for settling, as a fraction (0.10 is 10%).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="headroomInches"/> or <paramref name="settlingAllowance"/> is negative.
    /// </exception>
    public SoilOptions(decimal headroomInches = 1m, decimal settlingAllowance = 0.10m)
    {
        Guard.ThrowIfNegative(headroomInches);
        Guard.ThrowIfNegative(settlingAllowance);

        HeadroomInches = headroomInches;
        SettlingAllowance = settlingAllowance;
    }



    /// <summary>How far below the top of the wall the soil stops, in inches.</summary>
    public decimal HeadroomInches { get; }



    /// <summary>Extra soil to buy for settling, as a fraction (0.10 is 10%).</summary>
    public decimal SettlingAllowance { get; }
}
