using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace EquipmentRental
{
    // custom value converter used to convert boolean values into Color objects
    // this class is used to change background color of the grid in listview viewcells
    class IsRentedAndEndDateToColorConverter : IValueConverter
    {
        // implementation of IValueConverter.Convert method
        // this method receives values from 2 binded properties one is passed as value object and other as parameter object
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime endDate;
            // pattern matching used to check if 2nd value is Label
            if (parameter is Label endDateLabel)
            {
                // assignment of Label.Text property to local variable
                var endDateLabelValue = endDateLabel.Text;
                try
                {
                    // parase string to DateTime
                    endDate = DateTime.ParseExact(endDateLabelValue, "D", CultureInfo.CurrentCulture);
                }
                catch
                {
                    // if parsing not successful don't change color
                    return Color.Default;
                }
            }
            else
            {
                // if 2nd object is not Label don't change color
                return Color.Default;
            }

            if ((bool)value && endDate >= DateTime.Today)
            {
                // change color to green if rental end date is today or in the future
                return Color.Green;
            }
            else if((bool)value && endDate < DateTime.Today)
            {
                // change color to red if end date was before today
                return Color.Red;
            }

            // return default color if value of the 1st binding is false
            // if IsRented is false do not change Color
            return Color.Default;
        }

        // IValueConverter.ConvertBack method implementation template
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // not implemented as it is never used
            throw new NotImplementedException();
        }
    }
}
