namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class BoardProfileTests
{
    public static TheoryData<BoardProfile, string, decimal, decimal> KnownProfiles => new()
    {
        { BoardProfile.TwoByTwo, "2x2", 1.5m, 1.5m },
        { BoardProfile.TwoByFour, "2x4", 1.5m, 3.5m },
        { BoardProfile.TwoBySix, "2x6", 1.5m, 5.5m },
        { BoardProfile.TwoByEight, "2x8", 1.5m, 7.25m },
        { BoardProfile.TwoByTen, "2x10", 1.5m, 9.25m },
        { BoardProfile.TwoByTwelve, "2x12", 1.5m, 11.25m },
    };



    [Theory]
    [MemberData(nameof(KnownProfiles))]
    public void KnownProfile_when_read_has_actual_dimensions(BoardProfile profile, string name, decimal thickness, decimal width)
    {
        Assert.Equal(new BoardProfile(name, thickness, width), profile);
    }



    [Fact]
    public void TwoByWallBoards_when_read_lists_2x4_to_2x12_narrowest_first()
    {
        Assert.Equal
        (
            ["2x4", "2x6", "2x8", "2x10", "2x12"],
            BoardProfile.TwoByWallBoards.Select(profile => profile.Name)
        );
    }



    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_when_name_is_blank_throws_ArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => new BoardProfile(name, 1.5m, 3.5m));
    }



    [Theory]
    [InlineData(0, 3.5)]
    [InlineData(1.5, 0)]
    public void Constructor_when_a_dimension_is_not_positive_throws_ArgumentOutOfRangeException(decimal thickness, decimal width)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoardProfile("2x4", thickness, width));
    }
}
