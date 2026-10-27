namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class BedEstimatorTests
{
    private static readonly LumberKind WallKind = new(BoardProfile.TwoBySix, Treatment.PressureTreated);

    private static readonly LumberKind PostKind = new(BoardProfile.TwoByTwo, Treatment.PressureTreated);



    private static BedEstimate EstimateOf(BedForm form)
    {
        var outcome = BedEstimator.Estimate(form, MaterialCatalog.Placeholder);
        Assert.Null(outcome.Error);
        return outcome.Estimate!;
    }



    private static string ErrorOf(BedForm form, MaterialCatalog? catalog = null)
    {
        var outcome = BedEstimator.Estimate(form, catalog ?? MaterialCatalog.Placeholder);
        Assert.Null(outcome.Estimate);
        return outcome.Error!;
    }



    [Fact]
    public void Estimate_when_form_is_the_default_8_by_4_ft_bed_buys_six_2x6_and_one_2x2()
    {
        var lumber = EstimateOf(new BedForm()).Lumber;

        Assert.Equal
        (
            [(WallKind, 96m, 6), (PostKind, 96m, 1)],
            lumber.Purchases.Select(purchase => (purchase.Stock.Kind, purchase.Stock.LengthInches, purchase.Quantity))
        );
        Assert.Equal(60.64m, lumber.TotalCost);
    }



    [Fact]
    public void Estimate_when_form_is_the_default_needs_32_screws_in_one_box()
    {
        var estimate = EstimateOf(new BedForm());

        Assert.Equal((2, 32, 1, 11.98m), (estimate.ScrewsPerBoardEnd, estimate.Screws, estimate.ScrewBoxes, estimate.ScrewCost));
    }



    [Fact]
    public void Estimate_when_soil_is_prebagged_buys_19_bags()
    {
        var estimate = EstimateOf(new BedForm());

        var bags = Assert.Single(estimate.Soil);
        Assert.Equal(MaterialCatalog.Placeholder.PrebaggedSoil, bags.Product);
        Assert.Equal(19, bags.Packages);
        Assert.Equal(27.97265625m, estimate.SoilCubicFeet);
        Assert.Equal(189.43m, estimate.SoilCost);
    }



    [Fact]
    public void Estimate_when_form_is_the_default_totals_lumber_screws_and_soil()
    {
        Assert.Equal(262.05m, EstimateOf(new BedForm()).TotalCost);
    }



    [Fact]
    public void Estimate_when_soil_is_a_mix_splits_the_volume_between_three_products()
    {
        var soil = EstimateOf(new BedForm { SoilMode = SoilMode.Mix }).Soil;

        Assert.Equal
        (
            MaterialCatalog.Placeholder.MixIngredients.Select(ingredient => ingredient.Product),
            soil.Select(requirement => requirement.Product)
        );
        Assert.All(soil, requirement => Assert.Equal(27.97265625m / 3m, requirement.CubicFeet));
    }



    [Fact]
    public void Estimate_when_sizes_are_in_inches_matches_the_same_bed_in_feet()
    {
        var inches = new BedForm { Length = 96m, LengthUnit = LengthUnit.Inches, Width = 48m, WidthUnit = LengthUnit.Inches };

        Assert.Equal(EstimateOf(new BedForm()).TotalCost, EstimateOf(inches).TotalCost);
    }



    [Fact]
    public void Estimate_when_corner_posts_are_off_cuts_no_posts()
    {
        var cuts = EstimateOf(new BedForm { CornerPosts = false }).CutList;

        Assert.DoesNotContain(cuts, piece => piece.Purpose == PiecePurpose.CornerPost);
    }



    [Fact]
    public void Estimate_when_boards_are_cut_alike_groups_them_into_patterns()
    {
        var patterns = EstimateOf(new BedForm()).CutPatterns;

        Assert.Equal(3, patterns.Count);
        Assert.Contains(patterns, pattern => pattern is { Count: 4, LeftoverInches: 0m } && pattern.Cuts.SequenceEqual([96m]));
        Assert.Contains(patterns, pattern => pattern is { Count: 2, LeftoverInches: 0m } && pattern.Cuts.SequenceEqual([48m, 48m]));
        Assert.Contains(patterns, pattern => pattern is { Count: 1, LeftoverInches: 52m } && pattern.Stock.Kind == PostKind);
    }



    [Fact]
    public void Estimate_when_a_side_is_no_longer_than_the_board_thickness_says_so()
    {
        var form = new BedForm { Width = 1.5m, WidthUnit = LengthUnit.Inches };

        Assert.Equal("The length and width must each be more than 1.5 in, the board thickness.", ErrorOf(form));
    }



    [Fact]
    public void Estimate_when_length_is_too_small_says_so()
    {
        Assert.StartsWith("The length and width", ErrorOf(new BedForm { Length = 0m }));
    }



    [Fact]
    public void Estimate_when_courses_is_zero_asks_for_a_height()
    {
        Assert.Equal("Pick a height of at least one board.", ErrorOf(new BedForm { Courses = 0 }));
    }



    [Theory]
    [InlineData(-1)]
    [InlineData(11.5)]
    public void Estimate_when_fill_depth_is_outside_the_wall_says_so(decimal fillDepth)
    {
        Assert.Equal
        (
            "The fill depth must be between 0 and the wall height, 11 in.",
            ErrorOf(new BedForm { FillDepthInches = fillDepth })
        );
    }



    [Fact]
    public void Estimate_when_fill_depth_is_the_full_wall_height_fills_to_the_top()
    {
        // 94.5 x 46.5 x 11 in = 27.97 cu ft, plus 10% settling.
        Assert.Equal(30.769921875m, EstimateOf(new BedForm { FillDepthInches = 11m }).SoilCubicFeet);
    }



    [Theory]
    [InlineData(-1, 0)]
    [InlineData(10, -0.125)]
    public void Estimate_when_an_option_is_negative_says_so(decimal settling, decimal kerf)
    {
        var form = new BedForm { SettlingPercent = settling, KerfInches = kerf };

        Assert.Equal("Settling and blade width cannot be negative.", ErrorOf(form));
    }



    [Fact]
    public void Estimate_when_a_wall_is_longer_than_any_board_for_sale_says_so()
    {
        Assert.Equal
        (
            "A 240 in piece is longer than the longest pressure-treated 2x6 board for sale (16 ft). Make the bed smaller.",
            ErrorOf(new BedForm { Length = 20m })
        );
    }



    [Fact]
    public void Estimate_when_no_board_of_a_kind_is_for_sale_says_so()
    {
        var placeholder = MaterialCatalog.Placeholder;
        var noPosts = new MaterialCatalog
        (
            [.. placeholder.Stock.Where(option => option.Kind != PostKind)],
            placeholder.Screws,
            placeholder.PrebaggedSoil,
            placeholder.MixIngredients
        );

        Assert.Equal("No pressure-treated 2x2 boards are for sale.", ErrorOf(new BedForm(), noPosts));
    }



    [Fact]
    public void Estimate_when_form_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BedEstimator.Estimate(null!, MaterialCatalog.Placeholder));
    }



    [Fact]
    public void Estimate_when_catalog_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BedEstimator.Estimate(new BedForm(), null!));
    }
}
