namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// One part of a soil mix, by volume.
/// </summary>
public sealed record SoilIngredient
{
    /// <summary>
    /// Initializes a new <see cref="SoilIngredient"/>.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="parts">Its share of the mix, relative to the other ingredients.</param>
    /// <exception cref="ArgumentNullException"><paramref name="product"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="parts"/> is not positive.</exception>
    public SoilIngredient(SoilProduct product, decimal parts)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parts);

        Product = product;
        Parts = parts;
    }



    /// <summary>The product.</summary>
    public SoilProduct Product { get; }



    /// <summary>Its share of the mix, relative to the other ingredients.</summary>
    public decimal Parts { get; }
}
