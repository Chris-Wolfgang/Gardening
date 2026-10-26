namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class EstimateOutcomeTests
{
    [Fact]
    public void Succeeded_when_estimate_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EstimateOutcome.Succeeded(null!));
    }



    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Failed_when_error_is_blank_throws_ArgumentException(string error)
    {
        Assert.ThrowsAny<ArgumentException>(() => EstimateOutcome.Failed(error));
    }
}
