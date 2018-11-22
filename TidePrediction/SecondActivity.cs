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
using Android.Util;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Android.Support.Design.Widget;

namespace TidePrediction
{
    [Activity(Label = "Second", ParentActivity = typeof(MainActivity))]
    public class SecondActivity : ListActivity, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, ILocationListener

    {
        PredictionItem[] tidesArray;
        const string CITY = "City";
        const string DATE = "Date";
        const string TODAY = "isToday";
        GoogleApiClient googleApiClient;
        const int REQUEST_CODE = 0; //request code for location permission


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

            if (today == true)
            {
                googleApiClient = new GoogleApiClient.Builder(this, this, this)
                    .AddApi(LocationServices.API).Build();

                googleApiClient.Connect();

                
            }

            else
            {
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
           

        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //show the tide height in inches (convert from feet to inches)
            decimal feet = 0;
            decimal.TryParse(tidesArray[position].Height, out feet);
            var measurement = ((decimal)feet * 12) + " inches".ToString();
            Android.Widget.Toast.MakeText(this, measurement, Android.Widget.ToastLength.Short).Show();
        }

        public void OnConnected(Bundle connectionHint)
        {
            var locRequest = new LocationRequest();

            // Setting location priority to PRIORITY_HIGH_ACCURACY (100)
            locRequest.SetPriority(100);

            // Setting interval between updates, in milliseconds
            // NOTE: the default FastestInterval is 1 minute. If you want to receive location updates more than 
            // once a minute, you _must_ also change the FastestInterval to be less than or equal to your Interval
            locRequest.SetFastestInterval(500);
            locRequest.SetInterval(1000);

            // if (PackageManager.CheckPermission("android.permission.ACCESS_FINE_LOCATION", "TidePrediction.TidePrediction").Equals(-1))
            if(ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                // Permission is not granted
                RequestLocationPermission();


            }
            LocationServices.FusedLocationApi.RequestLocationUpdates(googleApiClient, locRequest, this);
            Android.Locations.Location location = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);
                Log.Info("LastLocation", location.ToString());
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            var r = result.HasResolution;
            Log.Debug("Has Resolution", r.ToString());
            Android.Widget.Toast.MakeText(this, "Connection Failed", Android.Widget.ToastLength.Short).Show();
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            Log.Info("Location has changed", location.ToString());
        }

        void RequestLocationPermission()
        {
            //if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            //{
            //    Snackbar.Make(ListView, Resource.String.permission_location_rationale,
            //        Snackbar.LengthIndefinite).SetAction(Resource.String.ok, new Action<View>(delegate (View obj) {
            //            ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_CODE);
            //        })).Show();
            //}
            //else
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, REQUEST_CODE);
            }
        }

    }
}