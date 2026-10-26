namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A box of screws.
/// </summary>
public sealed record ScrewProduct
{
    /// <summary>
    /// Initializes a new <see cref="ScrewProduct"/>.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="countPerBox">How many screws are in one box.</param>
    /// <param name="price">The price of one box.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="countPerBox"/> is less than one or <paramref name="price"/> is negative.
    /// </exception>
    public ScrewProduct(string name, int countPerBox, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(countPerBox, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        Name = name;
        CountPerBox = countPerBox;
        Price = price;
    }



    /// <summary>The product name.</summary>
    public string Name { get; }



    /// <summary>How many screws are in one box.</summary>
    public int CountPerBox { get; }



    /// <summary>The price of one box.</summary>
    public decimal Price { get; }



    /// <summary>
    /// Returns the whole boxes needed for a number of screws.
    /// </summary>
    /// <param name="screws">The screws needed.</param>
    /// <returns>The boxes to buy.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="screws"/> is negative.</exception>
    public int BoxesFor(int screws)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(screws);

        return (screws + CountPerBox - 1) / CountPerBox;
    }
}
