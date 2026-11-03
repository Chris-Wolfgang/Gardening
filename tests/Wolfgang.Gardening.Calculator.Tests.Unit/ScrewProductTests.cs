namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class ScrewProductTests
{
    private static readonly ScrewProduct Box = new("3 in deck screws", 100, 12m);



    [Theory]
    [InlineData(0, 0)]
    [InlineData(100, 1)]
    [InlineData(101, 2)]
    public void BoxesFor_when_given_screw_count_rounds_up_to_whole_boxes(int screws, int expected)
    {
        Assert.Equal(expected, Box.BoxesFor(screws));
    }



    [Fact]
    public void BoxesFor_when_count_is_negative_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Box.BoxesFor(-1));
    }



    [Fact]
    public void Constructor_when_constructed_returns_the_inputs()
    {
        Assert.Equal(("3 in deck screws", 100, 12m), (Box.Name, Box.CountPerBox, Box.Price));
    }



    [Fact]
    public void Constructor_when_name_is_blank_throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ScrewProduct("", 100, 12m));
    }



    [Theory]
    [InlineData(0, 12)]
    [InlineData(100, -1)]
    public void Constructor_when_value_is_out_of_range_throws_ArgumentOutOfRangeException(int count, decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ScrewProduct("3 in deck screws", count, price));
    }
}
