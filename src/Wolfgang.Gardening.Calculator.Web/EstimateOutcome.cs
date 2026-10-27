namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// The result of running the calculator on a form: an estimate, or a message saying what to fix.
/// </summary>
public sealed class EstimateOutcome
{
    private EstimateOutcome(BedEstimate? estimate, string? error)
    {
        Estimate = estimate;
        Error = error;
    }



    /// <summary>The estimate, or <see langword="null"/> when the form could not be calculated.</summary>
    public BedEstimate? Estimate { get; }



    /// <summary>What to fix on the form, or <see langword="null"/> when there is an estimate.</summary>
    public string? Error { get; }



    /// <summary>Creates a successful outcome.</summary>
    /// <param name="estimate">The estimate.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="estimate"/> is <see langword="null"/>.</exception>
    public static EstimateOutcome Succeeded(BedEstimate estimate)
    {
        ArgumentNullException.ThrowIfNull(estimate);
        return new EstimateOutcome(estimate, error: null);
    }



    /// <summary>Creates a failed outcome.</summary>
    /// <param name="error">What to fix on the form.</param>
    /// <returns>The outcome.</returns>
    /// <exception cref="ArgumentException"><paramref name="error"/> is blank.</exception>
    public static EstimateOutcome Failed(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new EstimateOutcome(estimate: null, error);
    }
}
