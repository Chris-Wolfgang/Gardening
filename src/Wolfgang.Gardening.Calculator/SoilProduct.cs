namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// A soil or amendment that is bought by the package: prebagged raised-bed soil, peat moss, compost,
/// vermiculite and so on.
/// </summary>
/// <remarks>
/// <see cref="YieldCubicFeet"/> is the loose volume one package fills, which for compressed products such
/// as peat moss is more than the volume printed on the bag.
/// </remarks>
public sealed record SoilProduct
{
    /// <summary>
    /// Initializes a new <see cref="SoilProduct"/>.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="yieldCubicFeet">The loose volume one package fills, in cubic feet.</param>
    /// <param name="price">The price of one package.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="yieldCubicFeet"/> is not positive or <paramref name="price"/> is negative.
    /// </exception>
    public SoilProduct(string name, decimal yieldCubicFeet, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(yieldCubicFeet);
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        Name = name;
        YieldCubicFeet = yieldCubicFeet;
        Price = price;
    }



    /// <summary>The product name.</summary>
    public string Name { get; }



    /// <summary>The loose volume one package fills, in cubic feet.</summary>
    public decimal YieldCubicFeet { get; }



    /// <summary>The price of one package.</summary>
    public decimal Price { get; }
}
