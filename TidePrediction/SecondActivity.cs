using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using SQLite;
using System.Linq;
using System.IO;
using TidePrediction_Library;
using System;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Locations;
using Android.Util;
using System.Threading.Tasks;

namespace TidePrediction
{
    [Activity(Label = "Second", ParentActivity = typeof(MainActivity))]
    public class SecondActivity : ListActivity
    {
        PredictionItem[] tidesArray;
        const string CITY = "City";
        const string DATE = "Date";
        const string TODAY = "isToday";
        //GoogleApiClient googleApiClient;
        FusedLocationProviderClient fusedLocationProviderClient;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //get selected city and date from main activity
            Boolean today = Intent.Extras.GetBoolean(TODAY);
            string city = Intent.Extras.GetString(CITY);
            string date = Intent.Extras.GetString(DATE);

            string dbPath = "";
            SQLiteConnection db = null;

            // Get the path to the database that was deployed in Assets
            dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "tides.db3");

            // copy file to a read/write location
            using (Stream inStream = Assets.Open("tides.db3"))
            using (Stream outStream = File.Create(dbPath))
                inStream.CopyTo(outStream);

            // Open the database
            db = new SQLiteConnection(dbPath);

            if(today == true)
            {
                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
                if (IsGoogleServicesInstalled() == true)
                {
                    LocationRequest locationRequest = new LocationRequest()
                                  .SetPriority(100)
                                  .SetInterval(1000)
                                  .SetFastestInterval(500);
                    GetLastLocationFromDevice();
                    //GetLocation(locationRequest, ) MAKE INTERFACE to implement LocationCallback

                }
            }

            var tides = (from t in db.Table<PredictionItem>()
                         where (t.City == city)
                         && (t.Date == date)
                         select t).ToList();
            int count = tides.Count;
            tidesArray = new PredictionItem[count];

            //put tides from database into array for ListAdapter to use
            for (int i = 0; i < count; i++)
            {
                tidesArray[i] =
                    tides[i];
            }

            ListAdapter = new TideAdapter<PredictionItem>(this, Android.Resource.Layout.SimpleListItem1, tidesArray);

            ListView.FastScrollEnabled = true;

        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //show the tide height in inches (convert from feet to inches)
            decimal feet = 0;
            decimal.TryParse(tidesArray[position].Height, out feet);
            var measurement = ((decimal)feet * 12) + " inches".ToString();
            Android.Widget.Toast.MakeText(this, measurement, Android.Widget.ToastLength.Short).Show();
        }

        public bool IsGoogleServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);

            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("SecondActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }

        public async Task GetLastLocationFromDevice()
        {
            // This method assumes that the necessary run-time permission checks have succeeded.
            Android.Locations.Location location = await fusedLocationProviderClient.GetLastLocationAsync();

            if (location == null)
            {
                // Seldom happens, but should code that handles this scenario
            }
            else
            {
                // Do something with the location 
                Log.Debug("Sample", "The latitude is " + location.Latitude);
            }
        }

        //public async Task GetLocation(locReq, locCallBack)
        //{
        //await fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);
        // }
    }
}