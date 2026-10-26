namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class SoilCalculatorTests
{
    private static readonly SoilProduct Peat = new("Peat moss", 2m, 15m);

    private static readonly SoilProduct Compost = new("Compost", 2m, 8m);

    private static readonly SoilProduct Vermiculite = new("Vermiculite", 4m, 30m);



    private static StackedBoardBed EightByFourTwoCourses() =>
        new(BoardProfile.TwoBySix, Treatment.PressureTreated, 96m, 48m, 2, cornerPost: null);



    [Fact]
    public void VolumeCubicFeet_when_options_are_default_fills_to_one_inch_below_top_plus_ten_percent()
    {
        // Inside 94.5 x 46.5 in, 11 in wall less 1 in headroom = 10 in deep: 43,942.5 cu in = 25.4296875 cu ft.
        Assert.Equal(27.97265625m, SoilCalculator.VolumeCubicFeet(EightByFourTwoCourses(), new SoilOptions()));
    }



    [Fact]
    public void VolumeCubicFeet_when_headroom_is_at_least_the_wall_height_is_zero()
    {
        Assert.Equal(0m, SoilCalculator.VolumeCubicFeet(EightByFourTwoCourses(), new SoilOptions(headroomInches: 12m)));
    }



    [Fact]
    public void VolumeCubicFeet_when_bed_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SoilCalculator.VolumeCubicFeet(null!, new SoilOptions()));
    }



    [Fact]
    public void VolumeCubicFeet_when_options_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SoilCalculator.VolumeCubicFeet(EightByFourTwoCourses(), null!));
    }



    [Fact]
    public void Split_when_mix_is_equal_parts_gives_each_ingredient_a_third()
    {
        SoilIngredient[] mix = [new(Peat, 1m), new(Compost, 1m), new(Vermiculite, 1m)];

        Assert.Equal
        (
            [new SoilRequirement(Peat, 9m), new SoilRequirement(Compost, 9m), new SoilRequirement(Vermiculite, 9m)],
            SoilCalculator.Split(27m, mix)
        );
    }



    [Fact]
    public void Split_when_mix_is_one_prebagged_product_gives_it_the_whole_volume()
    {
        Assert.Equal([new SoilRequirement(Compost, 27m)], SoilCalculator.Split(27m, [new SoilIngredient(Compost, 1m)]));
    }



    [Fact]
    public void Split_when_ingredients_is_empty_throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SoilCalculator.Split(27m, []));
    }



    [Fact]
    public void Split_when_ingredients_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SoilCalculator.Split(27m, null!));
    }



    [Fact]
    public void Split_when_volume_is_negative_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SoilCalculator.Split(-1m, [new SoilIngredient(Peat, 1m)]));
    }



    [Fact]
    public void SoilRequirement_when_volume_is_not_a_whole_package_rounds_up_and_reports_leftover()
    {
        var requirement = new SoilRequirement(Peat, 9m);

        Assert.Equal(5, requirement.Packages);
        Assert.Equal(1m, requirement.LeftoverCubicFeet);
        Assert.Equal(75m, requirement.Cost);
    }



    [Fact]
    public void SoilOptions_when_constructed_returns_the_inputs()
    {
        var options = new SoilOptions(headroomInches: 2m, settlingAllowance: 0.15m);

        Assert.Equal(2m, options.HeadroomInches);
        Assert.Equal(0.15m, options.SettlingAllowance);
    }



    [Theory]
    [InlineData(-1, 0.1)]
    [InlineData(1, -0.1)]
    public void SoilOptions_when_value_is_negative_throws_ArgumentOutOfRangeException(decimal headroom, decimal settling)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SoilOptions(headroom, settling));
    }



    [Fact]
    public void SoilProduct_when_constructed_returns_the_inputs()
    {
        Assert.Equal(("Peat moss", 2m, 15m), (Peat.Name, Peat.YieldCubicFeet, Peat.Price));
    }



    [Fact]
    public void SoilProduct_when_name_is_blank_throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new SoilProduct(" ", 2m, 15m));
    }



    [Theory]
    [InlineData(0, 15)]
    [InlineData(2, -1)]
    public void SoilProduct_when_value_is_out_of_range_throws_ArgumentOutOfRangeException(decimal yield, decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SoilProduct("Peat moss", yield, price));
    }



    [Fact]
    public void SoilIngredient_when_product_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SoilIngredient(null!, 1m));
    }



    [Fact]
    public void SoilIngredient_when_parts_is_not_positive_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SoilIngredient(Peat, 0m));
    }
}
