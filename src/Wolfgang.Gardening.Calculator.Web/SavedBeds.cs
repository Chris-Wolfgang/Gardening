using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// Turns the bed list into JSON for the browser's local storage and back, so the list is still there
/// when the user comes back.
/// </summary>
public static class SavedBeds
{
    /// <summary>The local storage key the list is saved under.</summary>
    public const string StorageKey = "gardening.beds";

    private const int CurrentVersion = 1;



    /// <summary>
    /// Writes the bed list as JSON.
    /// </summary>
    /// <param name="orders">The beds and how many of each.</param>
    /// <returns>The JSON.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="orders"/> is <see langword="null"/>.</exception>
    public static string Serialize(IReadOnlyList<BedOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        var beds = orders
            .Select(order => new SavedBed
            (
                order.Form.Treatment,
                order.Form.BoardName,
                order.Form.Length,
                order.Form.LengthUnit,
                order.Form.Width,
                order.Form.WidthUnit,
                order.Form.Courses,
                order.Form.CornerBracing,
                order.Form.SoilMode,
                order.Form.HasCustomFillDepth ? order.Form.FillDepthInches : null,
                order.Form.SettlingPercent,
                order.Quantity
            ))
            .ToList();

        return JsonSerializer.Serialize(new SavedList(CurrentVersion, beds), SavedBedsJson.Default.SavedList);
    }



    /// <summary>
    /// Reads a bed list written by <see cref="Serialize"/>.
    /// </summary>
    /// <param name="json">The saved JSON, or <see langword="null"/> when nothing was saved.</param>
    /// <returns>
    /// The beds, or <see langword="null"/> when nothing usable was saved: no JSON, JSON that does not
    /// parse, another version, or an empty list.
    /// </returns>
    public static List<BedOrder>? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        SavedList? saved;
        try
        {
            saved = JsonSerializer.Deserialize(json, SavedBedsJson.Default.SavedList);
        }
        catch (JsonException)
        {
            return null;
        }

        if (saved is not { Version: CurrentVersion, Beds.Count: > 0 })
        {
            return null;
        }

        return saved.Beds
            .Select(bed => new BedOrder(ToForm(bed)) { Quantity = bed.Quantity })
            .ToList();
    }



    private static BedForm ToForm(SavedBed bed)
    {
        var form = new BedForm
        {
            Treatment = bed.Treatment,
            BoardName = bed.BoardName,
            Length = bed.Length,
            LengthUnit = bed.LengthUnit,
            Width = bed.Width,
            WidthUnit = bed.WidthUnit,
            Courses = bed.Courses,
            CornerBracing = bed.CornerBracing,
            SoilMode = bed.SoilMode,
            SettlingPercent = bed.SettlingPercent,
        };

        if (bed.FillDepthInches is { } fillDepth)
        {
            form.FillDepthInches = fillDepth;
        }

        return form;
    }
}
