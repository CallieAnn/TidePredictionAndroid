using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TidePrediction
{
    public class FusedLocationProviderCallback: LocationCallback
    {
        readonly SecondActivity activity;

        public FusedLocationProviderCallback(SecondActivity activity)
        {
            this.activity = activity;
        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            Log.Debug("FusedLocationProviderSample", "IsLocationAvailable: {0}", locationAvailability.IsLocationAvailable);
        }

        public override void OnLocationResult(LocationResult result)
        {

            if (result.Locations.Any())
            {
                var location = result.Locations.First();
                Log.Debug("Sample", "The latitude is :" + location.Latitude);
            }
            else
            {
                // No locations to work with.
            }
        }
    }
}