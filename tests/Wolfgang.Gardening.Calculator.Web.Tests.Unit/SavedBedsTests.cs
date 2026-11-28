using System.Text.Json.Nodes;

namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class SavedBedsTests
{
    [Fact]
    public void Deserialize_when_given_Serialize_output_restores_every_field()
    {
        var form = new BedForm
        {
            Treatment = Treatment.Untreated,
            BoardName = "2x10",
            Length = 144m,
            LengthUnit = LengthUnit.Inches,
            Width = 3m,
            WidthUnit = LengthUnit.Feet,
            Courses = 3,
            CornerBracing = CornerBracing.None,
            SoilMode = SoilMode.Mix,
            FillDepthInches = 20m,
            SettlingPercent = 5m,
        };

        var restored = Assert.Single(SavedBeds.Deserialize(SavedBeds.Serialize([new BedOrder(form) { Quantity = 3 }]))!);

        Assert.Equal
        (
            (Treatment.Untreated, "2x10", 144m, LengthUnit.Inches, 3m, LengthUnit.Feet, 3, CornerBracing.None, SoilMode.Mix, 20m, 5m, 3),
            (
                restored.Form.Treatment,
                restored.Form.BoardName,
                restored.Form.Length,
                restored.Form.LengthUnit,
                restored.Form.Width,
                restored.Form.WidthUnit,
                restored.Form.Courses,
                restored.Form.CornerBracing,
                restored.Form.SoilMode,
                restored.Form.FillDepthInches,
                restored.Form.SettlingPercent,
                restored.Quantity
            )
        );
    }



    [Fact]
    public void Deserialize_when_fill_depth_was_not_set_keeps_it_following_the_height()
    {
        var restored = SavedBeds.Deserialize(SavedBeds.Serialize([new BedOrder(new BedForm())]))![0].Form;

        restored.Courses = 3;

        Assert.False(restored.HasCustomFillDepth);
        Assert.Equal(15.5m, restored.FillDepthInches);
    }



    [Fact]
    public void Serialize_when_given_a_bed_saves_enums_by_name()
    {
        Assert.Contains("\"treatment\":\"PressureTreated\"", SavedBeds.Serialize([new BedOrder(new BedForm())]), StringComparison.Ordinal);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("null")]
    [InlineData("{\"version\":2,\"beds\":[]}")]
    [InlineData("{\"version\":1,\"beds\":[]}")]
    public void Deserialize_when_nothing_usable_was_saved_returns_null(string? json)
    {
        Assert.Null(SavedBeds.Deserialize(json));
    }



    [Fact]
    public void Deserialize_when_a_saved_bed_is_null_returns_null()
    {
        Assert.Null(SavedBeds.Deserialize("{\"version\":1,\"beds\":[null]}"));
    }



    [Theory]
    [InlineData("boardName", "null")]
    [InlineData("boardName", "\"2x3\"")]
    [InlineData("boardName", "\"2X6\"")]
    [InlineData("treatment", "7")]
    [InlineData("lengthUnit", "5")]
    [InlineData("widthUnit", "5")]
    [InlineData("cornerBracing", "9")]
    [InlineData("soilMode", "9")]
    [InlineData("quantity", "0")]
    [InlineData("quantity", "201")]
    [InlineData("courses", "0")]
    [InlineData("courses", "9")]
    [InlineData("length", "0")]
    [InlineData("length", "100.5")]
    [InlineData("width", "-1")]
    [InlineData("width", "79228162514264337593543950335")]
    [InlineData("settlingPercent", "-1")]
    [InlineData("settlingPercent", "101")]
    [InlineData("fillDepthInches", "-1")]
    public void Deserialize_when_a_saved_bed_has_an_unusable_value_returns_null(string property, string value)
    {
        var json = JsonNode.Parse(SavedBeds.Serialize([new BedOrder(new BedForm()), new BedOrder(new BedForm())]))!;
        json["beds"]![1]![property] = JsonNode.Parse(value);

        Assert.Null(SavedBeds.Deserialize(json.ToJsonString()));
    }



    [Fact]
    public void Deserialize_when_a_saved_bed_has_inch_sizes_up_to_100_ft_restores_it()
    {
        var form = new BedForm { Length = 1200m, LengthUnit = LengthUnit.Inches, Width = 1200m, WidthUnit = LengthUnit.Inches };

        Assert.NotNull(SavedBeds.Deserialize(SavedBeds.Serialize([new BedOrder(form)])));
    }



    [Fact]
    public void Serialize_when_orders_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SavedBeds.Serialize(null!));
    }
}
