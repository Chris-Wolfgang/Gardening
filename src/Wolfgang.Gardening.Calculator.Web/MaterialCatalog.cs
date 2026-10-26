namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// The products the calculator can put on a purchase list: lumber at each stock length, screws, and soil.
/// </summary>
public sealed class MaterialCatalog
{
    // Longer boards cost a little more per foot, as they do in stores.
    private static readonly (decimal Feet, decimal Premium)[] StockLengths = [(8m, 1.00m), (10m, 1.05m), (12m, 1.10m), (16m, 1.20m)];



    /// <summary>
    /// Creates a catalog.
    /// </summary>
    /// <param name="stock">The lumber for sale, one entry per kind and length.</param>
    /// <param name="screws">The screws used for every joint.</param>
    /// <param name="prebaggedSoil">The ready-made raised-bed soil.</param>
    /// <param name="mixIngredients">The ingredients of the make-your-own mix and their parts.</param>
    /// <exception cref="ArgumentNullException">Any argument is <see langword="null"/>.</exception>
    public MaterialCatalog
    (
        IReadOnlyList<StockOption> stock,
        ScrewProduct screws,
        SoilProduct prebaggedSoil,
        IReadOnlyList<SoilIngredient> mixIngredients
    )
    {
        ArgumentNullException.ThrowIfNull(stock);
        ArgumentNullException.ThrowIfNull(screws);
        ArgumentNullException.ThrowIfNull(prebaggedSoil);
        ArgumentNullException.ThrowIfNull(mixIngredients);

        Stock = stock;
        Screws = screws;
        PrebaggedSoil = prebaggedSoil;
        MixIngredients = mixIngredients;
    }



    /// <summary>
    /// A catalog with made-up prices, used until real store data is loaded. Every 2x wall board is listed
    /// treated and untreated at 8, 10, 12 and 16 ft, and 2x2 corner-post stock at 8 ft.
    /// </summary>
    public static MaterialCatalog Placeholder { get; } = new
    (
        BuildPlaceholderStock(),
        new ScrewProduct("3 in exterior screws, 100 count", 100, 11.98m),
        new SoilProduct("Raised bed soil, 1.5 cu ft bag", 1.5m, 9.97m),
        [
            new SoilIngredient(new SoilProduct("Peat moss, 3.8 cu ft bale", 3.8m, 16.98m), 1m),
            new SoilIngredient(new SoilProduct("Compost, 1 cu ft bag", 1m, 6.98m), 1m),
            new SoilIngredient(new SoilProduct("Vermiculite, 4 cu ft bag", 4m, 29.98m), 1m),
        ]
    );



    /// <summary>The lumber for sale.</summary>
    public IReadOnlyList<StockOption> Stock { get; }



    /// <summary>The screws used for every joint.</summary>
    public ScrewProduct Screws { get; }



    /// <summary>The ready-made raised-bed soil.</summary>
    public SoilProduct PrebaggedSoil { get; }



    /// <summary>The ingredients of the make-your-own mix.</summary>
    public IReadOnlyList<SoilIngredient> MixIngredients { get; }



    private static List<StockOption> BuildPlaceholderStock()
    {
        (BoardProfile Profile, decimal PricePerFoot)[] wallBoards =
        [
            (BoardProfile.TwoByFour, 0.60m),
            (BoardProfile.TwoBySix, 0.90m),
            (BoardProfile.TwoByEight, 1.20m),
            (BoardProfile.TwoByTen, 1.60m),
            (BoardProfile.TwoByTwelve, 2.00m),
        ];

        var stock = new List<StockOption>();
        foreach (var (profile, pricePerFoot) in wallBoards)
        {
            foreach (var (feet, premium) in StockLengths)
            {
                stock.Add(Board(profile, Treatment.Untreated, feet, pricePerFoot * premium));
                stock.Add(Board(profile, Treatment.PressureTreated, feet, pricePerFoot * premium * 1.3m));
            }
        }

        stock.Add(Board(BoardProfile.TwoByTwo, Treatment.Untreated, 8m, 0.44m));
        stock.Add(Board(BoardProfile.TwoByTwo, Treatment.PressureTreated, 8m, 0.56m));
        return stock;
    }



    private static StockOption Board(BoardProfile profile, Treatment treatment, decimal feet, decimal pricePerFoot)
    {
        return new StockOption
        (
            new LumberKind(profile, treatment),
            Length.ToInches(feet, LengthUnit.Feet),
            Math.Round(pricePerFoot * feet, 2)
        );
    }
}
