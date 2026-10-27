using Wolfgang.Gardening.Calculator.Web.Pages;

namespace Wolfgang.Gardening.Calculator.Web.Tests.Unit;

public class HomeBedListTests : BunitContext
{
    public HomeBedListTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }



    private static string Editing(IRenderedComponent<Home> home) => home.Find("#editing").TextContent;



    // Three beds, 8, 10 and 12 ft long, with the given one selected.
    private IRenderedComponent<Home> ThreeBeds(int selected)
    {
        var home = Render<Home>();
        home.Find("#add-bed").Click();
        home.Find("#length").Change("10");
        home.Find("#add-bed").Click();
        home.Find("#length").Change("12");
        home.Find($"#edit-{selected}").Click();
        return home;
    }



    [Fact]
    public void Render_when_nothing_is_saved_lists_one_bed_that_cannot_be_removed()
    {
        var home = Render<Home>();

        Assert.Single(home.FindAll("#beds tbody tr"));
        Assert.Equal("8 ft x 4 ft, pressure-treated 2x6, 11 in high", home.Find("#beds tbody tr").Children[1].TextContent);
        Assert.True(home.Find("#remove-0").HasAttribute("disabled"));
    }



    [Fact]
    public void AddBed_when_clicked_copies_the_current_bed_and_edits_the_copy()
    {
        var home = Render<Home>();
        home.Find("#length").Change("12");

        home.Find("#add-bed").Click();
        home.Find("#width").Change("3");

        Assert.Equal("Bed 2", Editing(home));
        Assert.Equal("12 ft x 4 ft, pressure-treated 2x6, 11 in high", home.Find("#beds tbody tr").Children[1].TextContent);
        Assert.Equal("12 ft x 3 ft, pressure-treated 2x6, 11 in high", home.FindAll("#beds tbody tr")[1].Children[1].TextContent);
    }



    [Fact]
    public void Edit_when_another_bed_is_picked_loads_it_into_the_form()
    {
        var home = ThreeBeds(selected: 0);

        Assert.Equal(("Bed 1", "8"), (Editing(home), home.Find("#length").GetAttribute("value")));
        Assert.Equal("true", home.Find("#edit-0").GetAttribute("aria-pressed"));
    }



    [Fact]
    public void Change_when_a_count_is_raised_builds_that_many()
    {
        var home = Render<Home>();

        home.Find("#count-0").Change("3");

        Assert.Equal("3", home.Find("#bed-size-0").Children[1].TextContent);
        Assert.Contains("96 screws", home.Find("#screws").TextContent, StringComparison.Ordinal);
    }



    [Fact]
    public void Remove_when_a_bed_before_the_selected_one_is_removed_keeps_editing_the_same_bed()
    {
        var home = ThreeBeds(selected: 2);

        home.Find("#remove-0").Click();

        Assert.Equal(("Bed 2", "12"), (Editing(home), home.Find("#length").GetAttribute("value")));
    }



    [Fact]
    public void Remove_when_the_selected_last_bed_is_removed_edits_the_one_before()
    {
        var home = ThreeBeds(selected: 2);

        home.Find("#remove-2").Click();

        Assert.Equal(("Bed 2", "10"), (Editing(home), home.Find("#length").GetAttribute("value")));
    }



    [Fact]
    public void Remove_when_a_bed_after_the_selected_one_is_removed_keeps_editing_the_same_bed()
    {
        var home = ThreeBeds(selected: 0);

        home.Find("#remove-1").Click();

        Assert.Equal(("Bed 1", "8"), (Editing(home), home.Find("#length").GetAttribute("value")));
        Assert.Equal(2, home.FindAll("#beds tbody tr").Count);
    }



    [Fact]
    public void Render_when_beds_were_saved_restores_them()
    {
        var saved = new BedOrder(new BedForm { Length = 12m }) { Quantity = 2 };
        JSInterop.Setup<string?>("localStorage.getItem", SavedBeds.StorageKey).SetResult(SavedBeds.Serialize([saved, new BedOrder(new BedForm())]));

        var home = Render<Home>();

        Assert.Equal(2, home.FindAll("#beds tbody tr").Count);
        Assert.Equal("2", home.Find("#count-0").GetAttribute("value"));
        Assert.Equal("12", home.Find("#length").GetAttribute("value"));
    }



    [Fact]
    public void Change_when_a_bed_changes_saves_the_list()
    {
        var home = Render<Home>();

        home.Find("#length").Change("12");

        var saved = JSInterop.VerifyInvoke("localStorage.setItem", calledTimes: 1)[0].Arguments;
        Assert.Equal(SavedBeds.StorageKey, saved[0]);
        Assert.Equal(12m, SavedBeds.Deserialize((string)saved[1]!)![0].Form.Length);
    }
}
