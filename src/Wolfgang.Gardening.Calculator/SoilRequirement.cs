namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// How much of one soil product a bed needs, and what that means in whole packages.
/// </summary>
/// <param name="Product">The product.</param>
/// <param name="CubicFeet">The volume needed, in cubic feet, before rounding to packages.</param>
public sealed record SoilRequirement(SoilProduct Product, decimal CubicFeet)
{
    /// <summary>The whole packages to buy.</summary>
    public int Packages => (int)Math.Ceiling(CubicFeet / Product.YieldCubicFeet);



    /// <summary>The volume left over after filling the bed, in cubic feet.</summary>
    public decimal LeftoverCubicFeet => (Packages * Product.YieldCubicFeet) - CubicFeet;



    /// <summary>The cost of the packages.</summary>
    public decimal Cost => Packages * Product.Price;
}
