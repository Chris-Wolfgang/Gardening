using System.Text.Json;

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
    /// parse, another version, an empty list, or a list with any bed that is missing, names an undefined
    /// option, or is out of the range the page accepts (see <see cref="BedEstimator"/>).
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

        if (saved is not { Version: CurrentVersion, Beds.Count: > 0 } || !saved.Beds.All(IsUsable))
        {
            return null;
        }

        return saved.Beds
            .Select(bed => new BedOrder(ToForm(bed)) { Quantity = bed.Quantity })
            .ToList();
    }



    // Stored state outlives the code that wrote it, so check everything the page would otherwise trip over.
    private static bool IsUsable(SavedBed? bed)
    {
        return bed is not null
            && bed.BoardName is not null
            && Enum.IsDefined(bed.Treatment)
            && Enum.IsDefined(bed.LengthUnit)
            && Enum.IsDefined(bed.WidthUnit)
            && Enum.IsDefined(bed.CornerBracing)
            && Enum.IsDefined(bed.SoilMode)
            && bed.Quantity is >= 1 and <= BedEstimator.MaxQuantity
            && bed.Courses is >= 1 and <= BedEstimator.MaxCourses
            && bed.Length > 0m && bed.Length <= BedEstimator.MaxSide(bed.LengthUnit)
            && bed.Width > 0m && bed.Width <= BedEstimator.MaxSide(bed.WidthUnit)
            && bed.SettlingPercent >= 0m && bed.SettlingPercent <= BedEstimator.MaxSettlingPercent
            && bed.FillDepthInches is null or >= 0m;
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
