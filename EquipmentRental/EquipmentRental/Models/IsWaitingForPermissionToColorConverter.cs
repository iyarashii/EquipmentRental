// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;


namespace EquipmentRental.Models
{
    // custom value converter used to convert boolean value into Color object
    // this class is used to change background color of the grid in listview viewcells
    public class IsWaitingForPermissionToColorConverter : IValueConverter
    {
        // implementation of IValueConverter.Convert method
        // this method returns Color based on bool value of received property from Binding
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // return yellow color if binding property equals true, default color if it is false
            return ((bool)value ? Color.Yellow : Color.Default);
        }

        // IValueConverter.ConvertBack method implementation template
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // not implemented as it is never used
            throw new NotImplementedException();
        }
    }
}
