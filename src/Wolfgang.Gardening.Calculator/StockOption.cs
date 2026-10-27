namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A board that can be bought: a kind of lumber in one stock length, at a price.
/// </summary>
public sealed record StockOption
{
    /// <summary>
    /// Initializes a new <see cref="StockOption"/>.
    /// </summary>
    /// <param name="kind">The lumber kind.</param>
    /// <param name="lengthInches">The stock length in inches.</param>
    /// <param name="price">The price of one board.</param>
    /// <exception cref="ArgumentNullException"><paramref name="kind"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lengthInches"/> is not positive or <paramref name="price"/> is negative.
    /// </exception>
    public StockOption(LumberKind kind, decimal lengthInches, decimal price)
    {
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lengthInches);
        Guard.ThrowIfNegative(price);

        Kind = kind;
        LengthInches = lengthInches;
        Price = price;
    }



    /// <summary>The lumber kind.</summary>
    public LumberKind Kind { get; }



    /// <summary>The stock length in inches.</summary>
    public decimal LengthInches { get; }



    /// <summary>The price of one board.</summary>
    public decimal Price { get; }
}
