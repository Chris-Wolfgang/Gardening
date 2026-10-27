namespace Wolfgang.Gardening.Calculator.Web;

/// <summary>
/// One bed design on the list and how many of it to build.
/// </summary>
public sealed class BedOrder
{
    /// <summary>
    /// Creates an order for one bed.
    /// </summary>
    /// <param name="form">The bed design.</param>
    /// <exception cref="ArgumentNullException"><paramref name="form"/> is <see langword="null"/>.</exception>
    public BedOrder(BedForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        Form = form;
    }



    /// <summary>The bed design.</summary>
    public BedForm Form { get; }



    /// <summary>How many beds of this design to build.</summary>
    public int Quantity { get; set; } = 1;
}
