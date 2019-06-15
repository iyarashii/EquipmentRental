using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;


namespace EquipmentRental
{
    public class IsWaitingForPermissionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value ? Color.Yellow : Color.Default);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
