namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class BedFormTests
{
    [Fact]
    public void Board_when_BoardName_is_a_wall_board_returns_that_board()
    {
        Assert.Equal(BoardProfile.TwoByTen, new BedForm { BoardName = "2x10" }.Board);
    }



    [Fact]
    public void Board_when_BoardName_is_not_a_wall_board_returns_a_2x6()
    {
        Assert.Equal(BoardProfile.TwoBySix, new BedForm { BoardName = "2x2" }.Board);
    }
}
