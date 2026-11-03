namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// Everything needed to build and fill every bed on the list: sizes, cut list, lumber, screws and soil.
/// Lumber, screws and soil are bought for all the beds together, so offcuts and part-used boxes and
/// bags are shared between them.
/// </summary>
public sealed class BedEstimate
{
    /// <summary>
    /// Creates an estimate.
    /// </summary>
    /// <param name="beds">The beds being built.</param>
    /// <param name="lumber">The boards to buy and how each is cut.</param>
    /// <param name="screwProduct">The screws bought.</param>
    /// <param name="soil">The soil products to buy.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public BedEstimate
    (
        IReadOnlyList<BedLine> beds,
        PackingResult lumber,
        ScrewProduct screwProduct,
        IReadOnlyList<SoilRequirement> soil
    )
    {
        ArgumentNullException.ThrowIfNull(beds);
        ArgumentNullException.ThrowIfNull(lumber);
        ArgumentNullException.ThrowIfNull(screwProduct);
        ArgumentNullException.ThrowIfNull(soil);

        Beds = beds;
        CutList = CombineCutLists(beds);
        Lumber = lumber;
        ScrewProduct = screwProduct;
        Soil = soil;
    }



    /// <summary>The beds being built.</summary>
    public IReadOnlyList<BedLine> Beds { get; }



    /// <summary>The pieces to cut for all the beds, alike pieces counted together.</summary>
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



    /// <summary>All the screws the walls of every bed take.</summary>
    public int Screws => Beds.Sum(line => line.Screws);



    /// <summary>The screws bought.</summary>
    public ScrewProduct ScrewProduct { get; }



    /// <summary>Whole boxes of <see cref="ScrewProduct"/> to buy.</summary>
    public int ScrewBoxes => ScrewProduct.BoxesFor(Screws);



    /// <summary>The cost of the screw boxes.</summary>
    public decimal ScrewCost => ScrewBoxes * ScrewProduct.Price;



    /// <summary>The soil every bed needs, in cubic feet.</summary>
    public decimal SoilCubicFeet => Beds.Sum(line => line.SoilCubicFeet * line.Quantity);



    /// <summary>The soil products to buy.</summary>
    public IReadOnlyList<SoilRequirement> Soil { get; }



    /// <summary>The cost of the soil.</summary>
    public decimal SoilCost => Soil.Sum(requirement => requirement.Cost);



    /// <summary>The cost of everything on the list.</summary>
    public decimal TotalCost => Lumber.TotalCost + ScrewCost + SoilCost;



    /// <summary>
    /// Lists the pieces to cut for every bed, multiplied by how many of each bed, with alike pieces
    /// counted together.
    /// </summary>
    /// <param name="beds">The beds being built.</param>
    /// <returns>The combined cut list.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="beds"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<CutPiece> CombineCutLists(IReadOnlyList<BedLine> beds)
    {
        ArgumentNullException.ThrowIfNull(beds);

        return beds
            .SelectMany(line => line.Bed.CutList().Select(piece => piece with { Quantity = piece.Quantity * line.Quantity }))
            .GroupBy(piece => (piece.Kind, piece.LengthInches, piece.Purpose))
            .Select(group => group.First() with { Quantity = group.Sum(piece => piece.Quantity) })
            .ToList();
    }
}
