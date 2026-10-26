namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class BedEstimateTests
{
    private static readonly StackedBoardBed Bed = new(BoardProfile.TwoBySix, Treatment.PressureTreated, 96m, 48m, 2, cornerPost: null);

    private static readonly PackingResult NoLumber = new([]);



    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Constructor_when_a_reference_argument_is_null_throws_ArgumentNullException(int nullArgument)
    {
        var catalog = MaterialCatalog.Placeholder;

        Assert.Throws<ArgumentNullException>
        (
            () => new BedEstimate
            (
                nullArgument == 0 ? null! : Bed,
                nullArgument == 1 ? null! : NoLumber,
                2,
                nullArgument == 2 ? null! : catalog.Screws,
                0m,
                nullArgument == 3 ? null! : []
            )
        );
    }
}
