// Copyright (c) 2022 iyarashii @ https://github.com/iyarashii 
// Licensed under the GNU General Public License v3.0,
// go to https://github.com/iyarashii/EquipmentRental/blob/master/LICENSE for license details.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EquipmentRental.Models
{
    // class containing logic for ActivityIndicator
    public sealed class ActivityIndicatorScope : IDisposable
    {
        // declaration of private fields
        private readonly bool showIndicator;
        private readonly ActivityIndicator indicator;
        private readonly Task indicatorDelay;

        // public constructor
        public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
        {
            // assigning constructor parameters to local fields   
            this.indicator = indicator;
            this.showIndicator = showIndicator;

            if (showIndicator)
            {
                // set indicator delay to 2 seconds and call SetIndicatorActivity method with parameter true
                indicatorDelay = Task.Delay(2000);
                SetIndicatorActivity(true);
            }
            else
            {
                // set indicator delay to the completed task with result 0
                indicatorDelay = Task.FromResult(0);
            }
        }

        // method used for setting properties of ActivityIndicator visual element
        private void SetIndicatorActivity(bool isActive)
        {
            // set IsVisible and IsRunning properties to isActive parameter
            this.indicator.IsVisible = isActive;
            this.indicator.IsRunning = isActive;
        }

        // implementation of IDisposable.Dispose 
        public void Dispose()
        {
            if (showIndicator)
            {
                // sets properties of ActivityIndicator visual element to false during disposing of ActivityIndicatorScope object
                indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}
