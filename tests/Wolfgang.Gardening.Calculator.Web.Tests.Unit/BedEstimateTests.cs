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
    public void Constructor_when_an_argument_is_null_throws_ArgumentNullException(int nullArgument)
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new BedEstimate
            (
                nullArgument == 0 ? null! : [],
                nullArgument == 1 ? null! : NoLumber,
                nullArgument == 2 ? null! : MaterialCatalog.Placeholder.Screws,
                nullArgument == 3 ? null! : []
            )
        );
    }



    [Fact]
    public void CombineCutLists_when_beds_share_pieces_counts_them_together_times_quantity()
    {
        // Two of the bed plus one more line of the same bed: the 96 in and 48 in pieces add up.
        BedLine[] beds = [new BedLine(Bed, 2, 2, 0m), new BedLine(Bed, 1, 2, 0m)];

        var cuts = BedEstimate.CombineCutLists(beds);

        Assert.Equal
        (
            [(96m, 12), (48m, 12)],
            cuts.Select(piece => (piece.LengthInches, piece.Quantity))
        );
    }



    [Fact]
    public void CombineCutLists_when_beds_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BedEstimate.CombineCutLists(null!));
    }



    [Fact]
    public void Screws_and_SoilCubicFeet_when_a_bed_is_built_twice_count_both()
    {
        var estimate = new BedEstimate([new BedLine(Bed, 2, 2, 10m)], NoLumber, MaterialCatalog.Placeholder.Screws, []);

        Assert.Equal((64, 20m), (estimate.Screws, estimate.SoilCubicFeet));
    }
}
