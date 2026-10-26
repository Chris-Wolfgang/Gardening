using Wolfgang.Gardening.Calculator.Web.Pages;

namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class HomeTests : BunitContext
{
    private IRenderedComponent<Home> RenderHome() => Render<Home>();



    [Fact]
    public void Render_when_form_is_the_default_shows_the_total()
    {
        Assert.Equal("$262.05", RenderHome().Find("#total").TextContent);
    }



    [Fact]
    public void Render_when_form_is_the_default_shows_the_bed_sizes()
    {
        var home = RenderHome();

        Assert.Equal
        (
            ("11 in", "97.5 in x 49.5 in", "94.5 in x 46.5 in"),
            (home.Find("#height").TextContent, home.Find("#outside").TextContent, home.Find("#inside").TextContent)
        );
    }



    [Fact]
    public void Render_when_form_is_the_default_lists_cut_patterns_and_screws()
    {
        var home = RenderHome();

        Assert.Equal(3, home.FindAll("#cut-patterns li").Count);
        Assert.Contains("32 screws (2 per board end x 16 ends)", home.Find("#screws").TextContent, StringComparison.Ordinal);
    }



    [Fact]
    public void Render_when_height_options_are_shown_labels_each_with_inches_and_boards()
    {
        var options = RenderHome().FindAll("#courses option");

        Assert.Equal(8, options.Count);
        Assert.Equal("5.5 in (1 board)", options[0].TextContent);
        Assert.Equal("11 in (2 boards)", options[1].TextContent);
    }



    [Fact]
    public void Change_when_board_is_2x8_recalculates_the_height()
    {
        var home = RenderHome();

        home.Find("#board").Change("2x8");

        Assert.Equal("14.5 in", home.Find("#height").TextContent);
    }



    [Fact]
    public void Change_when_material_is_untreated_buys_untreated_lumber()
    {
        var home = RenderHome();

        home.Find("#treatment").Change(nameof(Treatment.Untreated));

        Assert.All(home.FindAll("#lumber tbody tr"), row => Assert.StartsWith("untreated", row.Children[1].TextContent, StringComparison.Ordinal));
    }



    [Fact]
    public void Change_when_length_is_too_long_for_any_board_shows_the_error()
    {
        var home = RenderHome();

        home.Find("#length").Change("20");

        Assert.StartsWith("A 240 in piece", home.Find("#error").TextContent, StringComparison.Ordinal);
    }



    [Fact]
    public void Change_when_sizes_are_entered_in_inches_keeps_the_same_bed()
    {
        var home = RenderHome();

        home.Find("#length-unit").Change(nameof(LengthUnit.Inches));
        home.Find("#length").Change("96");
        home.Find("#width-unit").Change(nameof(LengthUnit.Inches));
        home.Find("#width").Change("48");

        Assert.Equal("$262.05", home.Find("#total").TextContent);
    }



    [Fact]
    public void Change_when_courses_is_three_raises_the_walls()
    {
        var home = RenderHome();

        home.Find("#courses").Change("3");

        Assert.Equal("16.5 in", home.Find("#height").TextContent);
    }



    [Fact]
    public void Change_when_corner_posts_are_off_drops_the_post_row()
    {
        var home = RenderHome();

        home.Find("#corner-posts").Change(false);

        Assert.Equal(2, home.FindAll("#cut-list tbody tr").Count);
    }



    [Fact]
    public void Change_when_soil_is_a_mix_lists_three_products()
    {
        var home = RenderHome();

        home.Find("#soil-mode").Change(nameof(SoilMode.Mix));

        Assert.Equal(3, home.FindAll("#soil tbody tr").Count);
    }



    [Fact]
    public void Change_when_more_options_are_set_recalculates()
    {
        var home = RenderHome();

        home.Find("#headroom").Change("0");
        home.Find("#settling").Change("0");
        home.Find("#kerf").Change("0.125");

        // 94.5 x 46.5 x 11 in = 27.97 cu ft with no headroom and no settling allowance.
        Assert.Equal("27.97 cu ft to fill the bed.", home.Find("#soil-volume").TextContent);
    }
}
