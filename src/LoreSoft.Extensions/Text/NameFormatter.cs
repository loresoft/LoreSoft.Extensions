using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LoreSoft.Extensions.Text;

/// <summary>
/// Named string formatter.
/// </summary>
public static class NameFormatter
{
    /// <summary>
    /// Replaces each named format item in a specified string with the text equivalent of a corresponding object's property value.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="source">The object to format.</param>
    /// <returns>A copy of format in which any named format items are replaced by the string representation.</returns>
    /// <example>
    /// <code>
    /// var o = new { First = "John", Last = "Doe" };
    /// string result = NameFormatter.Format("Full Name: {First} {Last}", o);
    /// </code>
    /// </example>
    public static string FormatName(this string format, object source)
    {
        if (format == null)
            throw new ArgumentNullException(nameof(format));

        if (format.Length == 0)
            return string.Empty;

        var result = new StringBuilder(format.Length * 2);
        var expression = new StringBuilder();

        var e = format.GetEnumerator();
        while (e.MoveNext())
        {
            var ch = e.Current;
            if (ch == '{')
            {
                while (true)
                {
                    if (!e.MoveNext())
                        throw new FormatException();

                    ch = e.Current;
                    if (ch == '}')
                    {
                        string value = Evaluate(source, expression.ToString());
                        result.Append(value);
                        expression.Length = 0;
                        break;
                    }
                    if (ch == '{')
                    {
                        result.Append(ch);
                        break;
                    }
                    expression.Append(ch);
                }
            }
            else if (ch == '}')
            {
                if (!e.MoveNext() || e.Current != '}')
                    throw new FormatException();

                result.Append('}');
            }
            else
            {
                result.Append(ch);
            }
        }

        return result.ToString();
    }

    private static string Evaluate(object source, string expression)
    {
        if (source is null || string.IsNullOrEmpty(expression))
            return string.Empty;

        string format = null;

        int colonIndex = expression.IndexOf(':');
        if (colonIndex > 0)
        {
            format = expression[(colonIndex + 1)..];
            expression = expression[..colonIndex];
        }

        if (source is IDictionary<string, string> stringDictionary)
        {
            stringDictionary.TryGetValue(expression, out var value);
            return FormatValue(format, value);
        }
        else if (source is IDictionary<string, object> objectDictionary)
        {
            objectDictionary.TryGetValue(expression, out var value);
            return FormatValue(format, value);
        }
        else if (source is System.Collections.IDictionary dictionary)
        {
            var value = dictionary[expression];
            return FormatValue(format, value);
        }
        else
        {
            var value = GetValue(source, expression);
            return FormatValue(format, value);
        }
    }

    private static object GetValue(object target, string name)
    {
        var currentType = target.GetType();
        var currentTarget = target;

        PropertyInfo property = null;

        if (!name.Contains('.'))
        {
            property = currentType.GetRuntimeProperty(name);
            return property?.GetValue(currentTarget);
        }

        // support nested property
        foreach (var part in name.Split('.'))
        {
            if (property != null)
            {
                currentTarget = property.GetValue(currentTarget);
                currentType = property.PropertyType;
            }

            property = currentType.GetRuntimeProperty(part);
        }

        return property?.GetValue(currentTarget);
    }

    private static string FormatValue<T>(string format, T value)
    {
        if (value == null)
            return string.Empty;

        return string.IsNullOrEmpty(format)
          ? value.ToString()
          : string.Format("{0:" + format + "}", value);
    }
}
