using System.Globalization;

namespace DailyBibleVerseApp.Converters
{
    // InverseBoolConverter — flips bool; used for IsVisible/IsEnabled bindings.
    // Design Pattern: Converter (Value Object / Adapter pattern).
    public class InverseBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type t, object? p, CultureInfo c) =>
            value is bool b ? !b : value;
        public object? ConvertBack(object? value, Type t, object? p, CultureInfo c) =>
            value is bool b ? !b : value;
    }

    // BoolToColorConverter — maps true/false to two colours for active states.
    public class BoolToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Color.FromArgb("#1976D2");
        public Color FalseColor { get; set; } = Color.FromArgb("#424242");
        public object Convert(object? value, Type t, object? p, CultureInfo c) =>
            value is true ? TrueColor : FalseColor;
        public object ConvertBack(object? value, Type t, object? p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // TruncateTextConverter — trims long strings for list previews.
    public class TruncateTextConverter : IValueConverter
    {
        public object Convert(object? value, Type t, object? p, CultureInfo c)
        {
            if (value is not string s) return string.Empty;
            int max = p is string ps && int.TryParse(ps, out int n) ? n : 80;
            return s.Length <= max ? s : s[..max] + "…";
        }
        public object ConvertBack(object? value, Type t, object? p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // NullToBoolConverter — returns false when value is null (for IsEnabled etc.)
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type t, object? p, CultureInfo c) => value is not null;
        public object ConvertBack(object? value, Type t, object? p, CultureInfo c) =>
            throw new NotImplementedException();
    }

    // StringToBoolConverter — true when string is not null/empty
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type t, object? p, CultureInfo c) =>
            !string.IsNullOrWhiteSpace(value as string);
        public object ConvertBack(object? value, Type t, object? p, CultureInfo c) =>
            throw new NotImplementedException();
    }
}
