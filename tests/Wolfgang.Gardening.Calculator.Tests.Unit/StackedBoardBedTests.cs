namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class StackedBoardBedTests
{
    private static StackedBoardBed EightByFourTwoCourses(BoardProfile? cornerPost) =>
        new(BoardProfile.TwoBySix, Treatment.PressureTreated, 96m, 48m, 2, cornerPost);



    [Fact]
    public void HeightInches_when_two_courses_of_2x6_is_eleven()
    {
        Assert.Equal(11m, EightByFourTwoCourses(cornerPost: null).HeightInches);
    }



    [Fact]
    public void OutsideDimensions_when_boards_are_8_by_4_ft_add_one_board_thickness()
    {
        var bed = EightByFourTwoCourses(cornerPost: null);

        Assert.Equal(97.5m, bed.OutsideLengthInches);
        Assert.Equal(49.5m, bed.OutsideWidthInches);
    }



    [Fact]
    public void InsideDimensions_when_boards_are_8_by_4_ft_subtract_one_board_thickness()
    {
        var bed = EightByFourTwoCourses(cornerPost: null);

        Assert.Equal(94.5m, bed.InsideLengthInches);
        Assert.Equal(46.5m, bed.InsideWidthInches);
    }



    [Fact]
    public void BoardEnds_when_two_courses_is_sixteen()
    {
        Assert.Equal(16, EightByFourTwoCourses(cornerPost: null).BoardEnds);
    }



    [Fact]
    public void Properties_when_constructed_return_the_inputs()
    {
        var bed = EightByFourTwoCourses(BoardProfile.TwoByTwo);

        Assert.Equal(BoardProfile.TwoBySix, bed.Board);
        Assert.Equal(Treatment.PressureTreated, bed.Treatment);
        Assert.Equal(96m, bed.LengthInches);
        Assert.Equal(48m, bed.WidthInches);
        Assert.Equal(2, bed.Courses);
        Assert.Equal(BoardProfile.TwoByTwo, bed.CornerPost);
    }



    [Fact]
    public void CutList_when_bed_has_corner_posts_includes_four_posts_as_tall_as_the_wall()
    {
        var wall = new LumberKind(BoardProfile.TwoBySix, Treatment.PressureTreated);
        var post = new LumberKind(BoardProfile.TwoByTwo, Treatment.PressureTreated);

        Assert.Equal
        (
            [
                new CutPiece(wall, 96m, 4, PiecePurpose.LengthWall),
                new CutPiece(wall, 48m, 4, PiecePurpose.WidthWall),
                new CutPiece(post, 11m, 4, PiecePurpose.CornerPost),
            ],
            EightByFourTwoCourses(BoardProfile.TwoByTwo).CutList()
        );
    }



    [Fact]
    public void CutList_when_bed_has_no_corner_posts_lists_only_wall_boards()
    {
        Assert.DoesNotContain
        (
            EightByFourTwoCourses(cornerPost: null).CutList(),
            piece => piece.Purpose == PiecePurpose.CornerPost
        );
    }



    [Fact]
    public void Constructor_when_board_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StackedBoardBed(null!, Treatment.Untreated, 96m, 48m, 1, cornerPost: null));
    }



    [Theory]
    [InlineData(1.5, 48, 1)]
    [InlineData(96, 1.5, 1)]
    [InlineData(96, 48, 0)]
    public void Constructor_when_size_is_out_of_range_throws_ArgumentOutOfRangeException(decimal length, decimal width, int courses)
    {
        Assert.Throws<ArgumentOutOfRangeException>
        (
            () => new StackedBoardBed(BoardProfile.TwoBySix, Treatment.Untreated, length, width, courses, cornerPost: null)
        );
    }
}
