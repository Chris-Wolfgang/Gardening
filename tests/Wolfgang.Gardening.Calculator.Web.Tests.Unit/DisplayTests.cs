namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class DisplayTests
{
    [Fact]
    public void Money_when_given_an_amount_formats_it_as_US_dollars()
    {
        Assert.Equal("$1,234.50", Display.Money(1234.5m));
    }



    public static TheoryData<decimal, string> InchesCases { get; } = new()
    {
        { 97.5m, "97.5 in" },
        { 27.97265625m, "27.97 in" },
        { 11m, "11 in" },
    };



    [Theory]
    [MemberData(nameof(InchesCases))]
    public void Inches_when_given_a_length_rounds_to_two_places(decimal inches, string expected)
    {
        Assert.Equal(expected, Display.Inches(inches));
    }



    public static TheoryData<decimal, string> InchesAndFeetCases { get; } = new()
    {
        { 97.5m, "97.5 in (8 ft 1.5 in)" },
        { 96m, "96 in (8 ft)" },
        { 12m, "12 in (1 ft)" },
        { 11m, "11 in" },
        { 23.999m, "24 in (2 ft)" },
    };



    [Theory]
    [MemberData(nameof(InchesAndFeetCases))]
    public void InchesAndFeet_when_given_a_length_adds_feet_and_inches_from_a_foot_up(decimal inches, string expected)
    {
        Assert.Equal(expected, Display.InchesAndFeet(inches));
    }



    public static TheoryData<decimal, LengthUnit, string> MeasureCases { get; } = new()
    {
        { 8m, LengthUnit.Feet, "8 ft" },
        { 30.5m, LengthUnit.Inches, "30.5 in" },
    };



    [Theory]
    [MemberData(nameof(MeasureCases))]
    public void Measure_when_given_a_length_and_unit_formats_it(decimal value, LengthUnit unit, string expected)
    {
        Assert.Equal(expected, Display.Measure(value, unit));
    }



    [Fact]
    public void Measure_when_unit_is_undefined_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Display.Measure(1m, (LengthUnit)99));
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
