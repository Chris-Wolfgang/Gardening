namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>One saved bed. Internal to <see cref="SavedBeds"/>; public only for the JSON source generator.</summary>
/// <param name="Treatment">See <see cref="BedForm.Treatment"/>.</param>
/// <param name="BoardName">See <see cref="BedForm.BoardName"/>.</param>
/// <param name="Length">See <see cref="BedForm.Length"/>.</param>
/// <param name="LengthUnit">See <see cref="BedForm.LengthUnit"/>.</param>
/// <param name="Width">See <see cref="BedForm.Width"/>.</param>
/// <param name="WidthUnit">See <see cref="BedForm.WidthUnit"/>.</param>
/// <param name="Courses">See <see cref="BedForm.Courses"/>.</param>
/// <param name="CornerBracing">See <see cref="BedForm.CornerBracing"/>.</param>
/// <param name="SoilMode">See <see cref="BedForm.SoilMode"/>.</param>
/// <param name="FillDepthInches">The fill depth the user set, or <see langword="null"/> to follow the wall height.</param>
/// <param name="SettlingPercent">See <see cref="BedForm.SettlingPercent"/>.</param>
/// <param name="Quantity">See <see cref="BedOrder.Quantity"/>.</param>
public sealed record SavedBed
(
    Treatment Treatment,
    string BoardName,
    decimal Length,
    LengthUnit LengthUnit,
    decimal Width,
    LengthUnit WidthUnit,
    int Courses,
    CornerBracing CornerBracing,
    SoilMode SoilMode,
    decimal? FillDepthInches,
    decimal SettlingPercent,
    int Quantity
);
