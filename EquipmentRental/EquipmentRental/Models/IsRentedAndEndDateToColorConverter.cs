using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace EquipmentRental
{
    class IsRentedAndEndDateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime endDate;
            if (parameter is Label endDateLabel)
            {
                var endDateLabelValue = endDateLabel.Text;
                try
                {
                    endDate = DateTime.ParseExact(endDateLabelValue, "D", CultureInfo.CurrentCulture);
                }
                catch
                {
                    return Color.Default;
                }
            }
            else
            {
                return Color.Default;
            }
            if ((bool)value && endDate >= DateTime.Today)
            {
                return Color.Green;
            }
            else if((bool)value && endDate < DateTime.Today)
            {
                return Color.Red;
            }
            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
