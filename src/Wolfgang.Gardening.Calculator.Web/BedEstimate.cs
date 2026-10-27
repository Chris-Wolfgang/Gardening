namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// Everything needed to build and fill one bed: sizes, cut list, lumber, screws and soil.
/// </summary>
public sealed class BedEstimate
{
    /// <summary>
    /// Creates an estimate.
    /// </summary>
    /// <param name="bed">The bed being built.</param>
    /// <param name="lumber">The boards to buy and how each is cut.</param>
    /// <param name="screwsPerBoardEnd">Screws driven through each board end.</param>
    /// <param name="screwProduct">The screws bought.</param>
    /// <param name="soilCubicFeet">The soil needed, in cubic feet.</param>
    /// <param name="soil">The soil products to buy.</param>
    /// <exception cref="ArgumentNullException">A reference argument is <see langword="null"/>.</exception>
    public BedEstimate
    (
        StackedBoardBed bed,
        PackingResult lumber,
        int screwsPerBoardEnd,
        ScrewProduct screwProduct,
        decimal soilCubicFeet,
        IReadOnlyList<SoilRequirement> soil
    )
    {
        ArgumentNullException.ThrowIfNull(bed);
        ArgumentNullException.ThrowIfNull(lumber);
        ArgumentNullException.ThrowIfNull(screwProduct);
        ArgumentNullException.ThrowIfNull(soil);

        Bed = bed;
        CutList = bed.CutList();
        Lumber = lumber;
        ScrewsPerBoardEnd = screwsPerBoardEnd;
        ScrewProduct = screwProduct;
        SoilCubicFeet = soilCubicFeet;
        Soil = soil;
    }



    /// <summary>The bed being built.</summary>
    public StackedBoardBed Bed { get; }



    /// <summary>The pieces to cut.</summary>
    public IReadOnlyList<CutPiece> CutList { get; }



    /// <summary>The boards to buy and how each is cut.</summary>
    public PackingResult Lumber { get; }



    /// <summary>
    /// The boards grouped by how they are cut, so identical boards are listed once with a count.
    /// </summary>
    public IReadOnlyList<CutPattern> CutPatterns =>
        Lumber.Boards
            .GroupBy(board => (board.Stock, Cuts: string.Join(',', board.Cuts)))
            .Select(group => new CutPattern(group.Key.Stock, group.First().Cuts, group.First().LeftoverInches, group.Count()))
            .ToList();



    /// <summary>Screws driven through each board end.</summary>
    public int ScrewsPerBoardEnd { get; }



    /// <summary>All the screws the walls take.</summary>
    public int Screws => ScrewsPerBoardEnd * Bed.BoardEnds;



    /// <summary>The screws bought.</summary>
    public ScrewProduct ScrewProduct { get; }



    /// <summary>Whole boxes of <see cref="ScrewProduct"/> to buy.</summary>
    public int ScrewBoxes => ScrewProduct.BoxesFor(Screws);



    /// <summary>The cost of the screw boxes.</summary>
    public decimal ScrewCost => ScrewBoxes * ScrewProduct.Price;



    /// <summary>The soil needed, in cubic feet.</summary>
    public decimal SoilCubicFeet { get; }



    /// <summary>The soil products to buy.</summary>
    public IReadOnlyList<SoilRequirement> Soil { get; }



    /// <summary>The cost of the soil.</summary>
    public decimal SoilCost => Soil.Sum(requirement => requirement.Cost);



    /// <summary>The cost of everything on the list.</summary>
    public decimal TotalCost => Lumber.TotalCost + ScrewCost + SoilCost;
}
