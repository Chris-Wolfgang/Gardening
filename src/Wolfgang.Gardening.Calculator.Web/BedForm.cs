namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// What the user has entered on the calculator page. The defaults are an 8 x 4 ft, two-board-high
/// pressure-treated 2x6 bed with corner posts, filled with bagged soil.
/// </summary>
public sealed class BedForm
{
    /// <summary>Whether the lumber is pressure treated.</summary>
    public Treatment Treatment { get; set; } = Treatment.PressureTreated;



    /// <summary>The wall board's name, one of <see cref="BoardProfile.TwoByWallBoards"/>.</summary>
    public string BoardName { get; set; } = BoardProfile.TwoBySix.Name;



    /// <summary>The bed length as entered, in <see cref="LengthUnit"/>.</summary>
    public decimal Length { get; set; } = 8m;



    /// <summary>The unit <see cref="Length"/> is in.</summary>
    public LengthUnit LengthUnit { get; set; } = LengthUnit.Feet;



    /// <summary>The bed width as entered, in <see cref="WidthUnit"/>.</summary>
    public decimal Width { get; set; } = 4m;



    /// <summary>The unit <see cref="Width"/> is in.</summary>
    public LengthUnit WidthUnit { get; set; } = LengthUnit.Feet;



    /// <summary>How many boards high the walls are.</summary>
    public int Courses { get; set; } = 2;



    /// <summary>Whether to add 2x2 corner posts.</summary>
    public bool CornerPosts { get; set; } = true;



    /// <summary>How the bed is filled.</summary>
    public SoilMode SoilMode { get; set; } = SoilMode.Prebagged;



    /// <summary>How far below the top of the wall the soil stops, in inches.</summary>
    public decimal HeadroomInches { get; set; } = 1m;



    /// <summary>Extra soil bought to make up for settling, as a percentage.</summary>
    public decimal SettlingPercent { get; set; } = 10m;



    /// <summary>The saw blade width lost on each cut, in inches.</summary>
    public decimal KerfInches { get; set; }



    /// <summary>
    /// The wall board named by <see cref="BoardName"/>, or a 2x6 when the name is not a known wall board.
    /// </summary>
    public BoardProfile Board =>
        BoardProfile.TwoByWallBoards.FirstOrDefault(board => string.Equals(board.Name, BoardName, StringComparison.Ordinal))
        ?? BoardProfile.TwoBySix;
}
