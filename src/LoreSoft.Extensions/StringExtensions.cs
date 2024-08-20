using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#nullable enable

namespace System;

/// <summary>
/// Extension methods for <see cref="String"/>.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Truncates the specified text.
    /// </summary>
    /// <param name="text">The text to truncate.</param>
    /// <param name="keep">The number of characters to keep.</param>
    /// <param name="ellipsis">The ellipsis string to use when truncating. (Default ...)</param>
    /// <returns>
    /// A truncate string.
    /// </returns>
    [return: NotNullIfNotNull(nameof(text))]
    public static string? Truncate(this string? text, int keep, string ellipsis = "...")
    {
        if (string.IsNullOrEmpty(text))
            return text;

        if (text!.Length <= keep)
            return text;

        ellipsis ??= string.Empty;

        if (text.Length <= keep + ellipsis.Length || keep < ellipsis.Length)
            return text[..keep];

        int prefix = keep - ellipsis.Length;
        return string.Concat(text[..prefix], ellipsis);
    }

    /// <summary>
    /// Indicates whether the specified String object is null or an empty string
    /// </summary>
    /// <param name="item">A String reference</param>
    /// <returns>
    ///     <c>true</c> if is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? item)
    {
        return string.IsNullOrEmpty(item);
    }

    /// <summary>
    /// Indicates whether a specified string is null, empty, or consists only of white-space characters
    /// </summary>
    /// <param name="item">A String reference</param>
    /// <returns>
    ///      <c>true</c> if is null or empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? item)
    {
        if (item == null)
            return true;

        for (int i = 0; i < item.Length; i++)
            if (!char.IsWhiteSpace(item[i]))
                return false;

        return true;
    }

    /// <summary>
    /// Determines whether the specified string is not <see cref="IsNullOrEmpty"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    ///   <c>true</c> if the specified <paramref name="value"/> is not <see cref="IsNullOrEmpty"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasValue([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
    /// </summary>
    /// <param name="format">A composite format string</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args</returns>
    public static string FormatWith([StringSyntax(StringSyntaxAttribute.CompositeFormat)] this string format, params object?[] args)
    {
        return string.Format(format, args);
    }

    /// <summary>
    /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
    /// </summary>
    /// <param name="format">A composite format string</param>
    /// <param name="provider">Format provider</param>
    /// <param name="args">An object array that contains zero or more objects to format</param>
    /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args</returns>
    public static string FormatWith([StringSyntax(StringSyntaxAttribute.CompositeFormat)] this string format, IFormatProvider? provider, params object?[] args)
    {
        return string.Format(provider, format, args);
    }


    public static string ToTitle(this string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length < 2)
            return text;

        var words = Regex.Matches(text, "([A-Z][a-z]*)|([0-9]+)");

        var builder = new StringBuilder();
        foreach (Match word in words)
        {
            if (builder.Length > 0)
                builder.Append(" ");

            builder.Append(word.Value);
        }

        return builder.ToString();
    }

}
