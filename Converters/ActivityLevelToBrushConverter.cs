using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ActivityPerson.Converters
{
    public class ActivityLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int level)
            {
                return level switch
                {
                    0 => Brush.Parse("#EBEDF0"),
                    1 => Brush.Parse("#9BE9A8"),
                    2 => Brush.Parse("#40C463"),
                    3 => Brush.Parse("#30A14E"),
                    4 => Brush.Parse("#216E39"),
                    _ => Brush.Parse("#EBEDF0")
                };
            }
            return Brush.Parse("#EBEDF0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}