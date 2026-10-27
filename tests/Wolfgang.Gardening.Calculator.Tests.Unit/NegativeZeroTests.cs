namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class NegativeZeroTests
{
    // Negative zero: what 11.0m - 11m produces at run time.
    private static readonly decimal NegativeZero = decimal.Negate(0.0m);

    private static readonly LumberKind Wall = new(BoardProfile.TwoBySix, Treatment.PressureTreated);



    [Fact]
    public void NegativeZero_is_negative_zero()
    {
        Assert.True(decimal.IsNegative(NegativeZero) && NegativeZero == 0m);
    }



    [Fact]
    public void SoilOptions_when_headroom_and_settling_are_negative_zero_treats_them_as_zero()
    {
        var options = new SoilOptions(NegativeZero, NegativeZero);

        Assert.Equal((0m, 0m), (options.HeadroomInches, options.SettlingAllowance));
    }



    [Fact]
    public void StockPacker_when_kerf_is_negative_zero_does_not_throw()
    {
        Assert.NotNull(new StockPacker(NegativeZero));
    }



    [Fact]
    public void Prices_when_negative_zero_are_accepted()
    {
        Assert.Equal
        (
            (0m, 0m, 0m),
            (
                new StockOption(Wall, 96m, NegativeZero).Price,
                new ScrewProduct("3 in deck screws", 100, NegativeZero).Price,
                new SoilProduct("Peat moss", 2m, NegativeZero).Price
            )
        );
    }



    [Fact]
    public void Split_when_volume_is_negative_zero_needs_no_packages()
    {
        var requirement = Assert.Single(SoilCalculator.Split(NegativeZero, [new SoilIngredient(new SoilProduct("Peat moss", 2m, 15m), 1m)]));

        Assert.Equal(0, requirement.Packages);
    }
}
