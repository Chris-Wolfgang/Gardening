namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class BedListEstimatorTests
{
    private static readonly LumberKind UntreatedSix = new(BoardProfile.TwoBySix, Treatment.Untreated);



    private static BedOrder Untreated(decimal lengthFeet, decimal widthFeet, int quantity = 1) =>
        new(new BedForm { Treatment = Treatment.Untreated, Length = lengthFeet, Width = widthFeet, CornerBracing = CornerBracing.None })
        {
            Quantity = quantity,
        };



    [Fact]
    public void Estimate_when_the_list_is_empty_asks_for_a_bed()
    {
        Assert.Equal("Add a bed.", BedEstimator.Estimate([], MaterialCatalog.Placeholder).Error);
    }



    [Fact]
    public void Estimate_when_a_quantity_is_below_one_says_so()
    {
        Assert.Equal
        (
            "Build at least 1 of each bed.",
            BedEstimator.Estimate([Untreated(8m, 4m, quantity: 0)], MaterialCatalog.Placeholder).Error
        );
    }



    [Fact]
    public void Estimate_when_one_of_several_beds_is_invalid_names_the_bed()
    {
        var outcome = BedEstimator.Estimate([Untreated(8m, 4m), Untreated(0m, 4m)], MaterialCatalog.Placeholder);

        Assert.StartsWith("Bed 2: The length and width", outcome.Error, StringComparison.Ordinal);
    }



    [Fact]
    public void Estimate_when_a_bed_is_built_twice_buys_twice_the_lumber_and_soil()
    {
        var one = BedEstimator.Estimate([Untreated(8m, 4m)], MaterialCatalog.Placeholder).Estimate!;
        var two = BedEstimator.Estimate([Untreated(8m, 4m, quantity: 2)], MaterialCatalog.Placeholder).Estimate!;

        Assert.Equal(one.Lumber.Boards.Count * 2, two.Lumber.Boards.Count);
        Assert.Equal(one.SoilCubicFeet * 2, two.SoilCubicFeet);
        Assert.Equal(one.Screws * 2, two.Screws);
    }



    [Fact]
    public void Estimate_when_beds_differ_shares_offcuts_between_them()
    {
        // A 12 x 4 ft bed has four 4 ft ends; a 4 x 4 ft bed has eight 4 ft walls. All twelve pack three to a
        // 12 ft board or two to an 8 ft board, alongside the four 12 ft walls.
        var estimate = BedEstimator.Estimate([Untreated(12m, 4m), Untreated(4m, 4m)], MaterialCatalog.Placeholder).Estimate!;

        Assert.All(estimate.Lumber.Boards, board => Assert.Equal(0m, board.LeftoverInches));
        Assert.Equal
        (
            16,
            estimate.CutList.Where(piece => piece.Kind == UntreatedSix).Sum(piece => piece.Quantity)
        );
    }



    [Fact]
    public void Estimate_when_beds_use_different_soil_buys_each_product_once()
    {
        var bagged = Untreated(8m, 4m);
        var mixed = Untreated(8m, 4m);
        mixed.Form.SoilMode = SoilMode.Mix;
        var alsoBagged = Untreated(4m, 4m);

        var soil = BedEstimator.Estimate([bagged, mixed, alsoBagged], MaterialCatalog.Placeholder).Estimate!.Soil;

        Assert.Equal(4, soil.Count);
        Assert.Single(soil, requirement => requirement.Product == MaterialCatalog.Placeholder.PrebaggedSoil);
    }



    [Fact]
    public void Estimate_when_orders_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BedEstimator.Estimate(null!, MaterialCatalog.Placeholder));
    }
}
