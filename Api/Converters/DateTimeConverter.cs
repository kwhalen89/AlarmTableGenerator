using System;
using System.Globalization;
using System.Windows.Data;

namespace AlarmTableGenerator.Api.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        /// <summary>
        /// This is poorly described but if true then you will get a date only from datetime, if false you will get a time only from datetime
        /// </summary>
        public bool IsDate { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                return new DateTime();

            var returnVal = ((DateTime)value).ToLocalTime();

            return (IsDate) ? returnVal.ToShortDateString() : returnVal.ToLongTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
