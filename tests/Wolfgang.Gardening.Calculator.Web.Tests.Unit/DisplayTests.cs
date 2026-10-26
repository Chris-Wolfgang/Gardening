namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class DisplayTests
{
    [Fact]
    public void Money_when_given_an_amount_formats_it_as_US_dollars()
    {
        Assert.Equal("$1,234.50", Display.Money(1234.5m));
    }



    [Theory]
    [InlineData(97.5, "97.5 in")]
    [InlineData(27.97265625, "27.97 in")]
    [InlineData(11, "11 in")]
    public void Inches_when_given_a_length_rounds_to_two_places(decimal inches, string expected)
    {
        Assert.Equal(expected, Display.Inches(inches));
    }



    [Fact]
    public void Feet_when_given_inches_converts_to_feet()
    {
        Assert.Equal("8 ft", Display.Feet(96m));
    }



    [Fact]
    public void CubicFeet_when_given_a_volume_rounds_to_two_places()
    {
        Assert.Equal("27.97 cu ft", Display.CubicFeet(27.97265625m));
    }



    [Theory]
    [InlineData(Treatment.PressureTreated, "pressure-treated 2x6")]
    [InlineData(Treatment.Untreated, "untreated 2x6")]
    public void Kind_when_given_a_kind_names_its_treatment_and_profile(Treatment treatment, string expected)
    {
        Assert.Equal(expected, Display.Kind(new LumberKind(BoardProfile.TwoBySix, treatment)));
    }



    [Fact]
    public void Kind_when_kind_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Display.Kind(null!));
    }



    [Fact]
    public void Kind_when_treatment_is_undefined_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Display.Kind(new LumberKind(BoardProfile.TwoBySix, (Treatment)99)));
    }



    [Theory]
    [InlineData(PiecePurpose.LengthWall, "Long walls")]
    [InlineData(PiecePurpose.WidthWall, "End walls")]
    [InlineData(PiecePurpose.CornerPost, "Corner posts")]
    public void Purpose_when_given_a_purpose_describes_it(PiecePurpose purpose, string expected)
    {
        Assert.Equal(expected, Display.Purpose(purpose));
    }



    [Fact]
    public void Purpose_when_purpose_is_undefined_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Display.Purpose((PiecePurpose)99));
    }
}
