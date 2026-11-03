using Wolfgang.Gardening.Calculator.Web.Pages;

namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class AppTests : BunitContext
{
    public AppTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }



    [Fact]
    public void Render_when_at_the_root_shows_the_calculator_in_the_layout()
    {
        var app = Render<App>();

        Assert.Equal("Raised bed calculator", app.Find("main h1").TextContent);
    }



    [Fact]
    public void Render_when_page_is_NotFound_links_back_to_the_calculator()
    {
        var page = Render<NotFound>();

        Assert.Equal("Not found", page.Find("h1").TextContent);
    }
}
