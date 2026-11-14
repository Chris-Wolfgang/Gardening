namespace Wolfgang.Gardening.Calculator.Tests.Unit;

public class LengthTests
{
    public static TheoryData<decimal, LengthUnit, decimal> ValidLengths { get; } = new()
    {
        { 8m, LengthUnit.Feet, 96m },
        { 4.5m, LengthUnit.Feet, 54m },
        { 30m, LengthUnit.Inches, 30m },
    };



    [Theory]
    [MemberData(nameof(ValidLengths))]
    public void ToInches_when_unit_is_valid_returns_inches(decimal value, LengthUnit unit, decimal expected)
    {
        Assert.Equal(expected, Length.ToInches(value, unit));
    }



    [Fact]
    public void ToInches_when_unit_is_undefined_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Length.ToInches(1m, (LengthUnit)99));
    }



    [Fact]
    public void ToCubicFeet_when_given_one_cubic_foot_of_inches_returns_one()
    {
        Assert.Equal(1m, Length.ToCubicFeet(1728m));
    }
}
