namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>The saved bed list. Internal to <see cref="SavedBeds"/>; public only for the JSON source generator.</summary>
/// <param name="Version">The format version.</param>
/// <param name="Beds">The beds.</param>
public sealed record SavedList(int Version, IReadOnlyList<SavedBed> Beds);
