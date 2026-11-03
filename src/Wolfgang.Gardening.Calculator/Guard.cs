using System.Runtime.CompilerServices;

namespace Wolfgang.Gardening.Calculator;

/// <summary>
/// Argument checks for decimals that treat negative zero as zero.
/// </summary>
internal static class Guard
{
    /// <summary>
    /// Throws when <paramref name="value"/> is below zero.
    /// </summary>
    /// <remarks>
    /// <see cref="ArgumentOutOfRangeException.ThrowIfNegative{T}(T, string?)"/> reads the sign bit, so it
    /// rejects the negative zero that subtraction can produce (11.0m - 11m). This compares the value instead.
    /// </remarks>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than zero.</exception>
    public static void ThrowIfNegative(decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0m)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative value.");
        }
    }
}
