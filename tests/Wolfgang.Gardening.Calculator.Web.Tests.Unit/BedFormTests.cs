namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class BedFormTests
{
    [Fact]
    public void Board_when_BoardName_is_a_wall_board_returns_that_board()
    {
        Assert.Equal(BoardProfile.TwoByTen, new BedForm { BoardName = "2x10" }.Board);
    }



    [Fact]
    public void FillDepthInches_when_not_set_is_1_in_below_the_wall_height()
    {
        Assert.Equal(15.5m, new BedForm { Courses = 3 }.FillDepthInches);
    }



    [Fact]
    public void FillDepthInches_when_set_keeps_the_value_when_the_height_changes()
    {
        var form = new BedForm { FillDepthInches = 6m };

        form.Courses = 4;

        Assert.Equal(6m, form.FillDepthInches);
    }



    [Fact]
    public void Copy_when_the_copy_is_edited_leaves_the_original_alone()
    {
        var original = new BedForm { FillDepthInches = 6m };

        var copy = original.Copy();
        copy.Length = 12m;

        Assert.Equal((8m, 12m, 6m, true), (original.Length, copy.Length, copy.FillDepthInches, copy.HasCustomFillDepth));
    }



    [Fact]
    public void BedOrder_when_form_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BedOrder(null!));
    }



    [Fact]
    public void Board_when_BoardName_is_not_a_wall_board_returns_a_2x6()
    {
        Assert.Equal(BoardProfile.TwoBySix, new BedForm { BoardName = "2x2" }.Board);
    }
}
