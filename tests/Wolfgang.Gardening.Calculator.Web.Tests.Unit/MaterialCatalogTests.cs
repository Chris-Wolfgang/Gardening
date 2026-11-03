namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class MaterialCatalogTests
{
    public static TheoryData<BoardProfile, Treatment> WallBoardKinds()
    {
        var data = new TheoryData<BoardProfile, Treatment>();
        foreach (var profile in BoardProfile.TwoByWallBoards)
        {
            data.Add(profile, Treatment.Untreated);
            data.Add(profile, Treatment.PressureTreated);
        }

        return data;
    }



    private static decimal[] LengthsOf(LumberKind kind) =>
        MaterialCatalog.Placeholder.Stock
            .Where(option => option.Kind == kind)
            .Select(option => option.LengthInches)
            .ToArray();



    [Theory]
    [MemberData(nameof(WallBoardKinds))]
    public void Placeholder_when_read_lists_every_wall_board_at_8_10_12_and_16_ft(BoardProfile profile, Treatment treatment)
    {
        Assert.Equal([96m, 120m, 144m, 192m], LengthsOf(new LumberKind(profile, treatment)));
    }



    [Theory]
    [InlineData(Treatment.Untreated)]
    [InlineData(Treatment.PressureTreated)]
    public void Placeholder_when_read_lists_8_ft_2x2_corner_post_stock(Treatment treatment)
    {
        Assert.Equal([96m], LengthsOf(new LumberKind(BoardProfile.TwoByTwo, treatment)));
    }



    [Fact]
    public void Placeholder_when_read_prices_longer_boards_higher_per_foot()
    {
        var kind = new LumberKind(BoardProfile.TwoBySix, Treatment.PressureTreated);
        var perInch = MaterialCatalog.Placeholder.Stock
            .Where(option => option.Kind == kind)
            .Select(option => option.Price / option.LengthInches)
            .ToArray();

        Assert.Equal(perInch.Order(), perInch);
        Assert.Equal(perInch.Length, perInch.Distinct().Count());
    }



    [Fact]
    public void Placeholder_when_read_mixes_equal_parts_of_three_ingredients()
    {
        Assert.Equal([1m, 1m, 1m], MaterialCatalog.Placeholder.MixIngredients.Select(ingredient => ingredient.Parts));
    }



    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Constructor_when_an_argument_is_null_throws_ArgumentNullException(int nullArgument)
    {
        var placeholder = MaterialCatalog.Placeholder;

        Assert.Throws<ArgumentNullException>
        (
            () => new MaterialCatalog
            (
                nullArgument == 0 ? null! : placeholder.Stock,
                nullArgument == 1 ? null! : placeholder.Screws,
                nullArgument == 2 ? null! : placeholder.PrebaggedSoil,
                nullArgument == 3 ? null! : placeholder.MixIngredients
            )
        );
    }
}
