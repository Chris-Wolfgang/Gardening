namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class FastenerRuleTests
{
    public static TheoryData<BoardProfile, int> DefaultScrewsPerEnd => new()
    {
        { BoardProfile.TwoByFour, 2 },
        { BoardProfile.TwoBySix, 2 },
        { BoardProfile.TwoByEight, 3 },
        { BoardProfile.TwoByTen, 3 },
        { BoardProfile.TwoByTwelve, 4 },
    };



    [Theory]
    [MemberData(nameof(DefaultScrewsPerEnd))]
    public void ScrewsPerBoardEnd_when_rule_is_default_uses_one_per_3_5_inches_with_minimum_two(BoardProfile board, int expected)
    {
        Assert.Equal(expected, new FastenerRule().ScrewsPerBoardEnd(board));
    }



    [Fact]
    public void ScrewsPerBoardEnd_when_minimum_exceeds_spacing_count_uses_minimum()
    {
        Assert.Equal(5, new FastenerRule(minimumPerBoardEnd: 5, spacingInches: 3.5m).ScrewsPerBoardEnd(BoardProfile.TwoByTwelve));
    }



    [Fact]
    public void ScrewsFor_when_bed_is_two_courses_of_2x6_is_thirty_two()
    {
        var bed = new StackedBoardBed(BoardProfile.TwoBySix, Treatment.Untreated, 96m, 48m, 2, cornerPost: null);

        Assert.Equal(32, new FastenerRule().ScrewsFor(bed));
    }



    [Fact]
    public void Properties_when_constructed_return_the_inputs()
    {
        var rule = new FastenerRule(minimumPerBoardEnd: 3, spacingInches: 4m);

        Assert.Equal(3, rule.MinimumPerBoardEnd);
        Assert.Equal(4m, rule.SpacingInches);
    }



    [Theory]
    [InlineData(0, 3.5)]
    [InlineData(2, 0)]
    public void Constructor_when_value_is_out_of_range_throws_ArgumentOutOfRangeException(int minimum, decimal spacing)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FastenerRule(minimum, spacing));
    }



    [Fact]
    public void ScrewsPerBoardEnd_when_board_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FastenerRule().ScrewsPerBoardEnd(null!));
    }



    [Fact]
    public void ScrewsFor_when_bed_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FastenerRule().ScrewsFor(null!));
    }
}
