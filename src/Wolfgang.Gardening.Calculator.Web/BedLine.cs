namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// One bed design in an estimate, with what a single bed of that design takes.
/// </summary>
/// <param name="Bed">The bed.</param>
/// <param name="Quantity">How many of the bed to build.</param>
/// <param name="ScrewsPerBoardEnd">Screws driven through each board end.</param>
/// <param name="SoilCubicFeet">The soil one bed needs, in cubic feet.</param>
public sealed record BedLine(StackedBoardBed Bed, int Quantity, int ScrewsPerBoardEnd, decimal SoilCubicFeet)
{
    /// <summary>The screws the walls of all <see cref="Quantity"/> beds take.</summary>
    public int Screws => ScrewsPerBoardEnd * Bed.BoardEnds * Quantity;
}
