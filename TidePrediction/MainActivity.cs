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
using Android.Content;

namespace TidePrediction
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            string dbPath = "";
            SQLiteConnection db = null;

            // Get the path to the database that was deployed in Assets
            dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "tides.db3");

            // It seems you can read a file in Assets, but not write to it
            // so we'll copy our file to a read/write location
            using (Stream inStream = Assets.Open("tides.db3"))
            using (Stream outStream = File.Create(dbPath))
                inStream.CopyTo(outStream);

            // Open the database
            db = new SQLiteConnection(dbPath);

            //adapter & spinner initialized
            var locations = db.Table<PredictionItem>().GroupBy(p => p.City).Select(p => p.First());
            var cityName = locations.Select(l => l.City).ToList();
            Spinner spin = FindViewById<Spinner>(Resource.Id.locationSpinner);
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, cityName);

            spin.Adapter = adapter;

            //event handler for spinner selection
            string selectedCity = "";
            spin.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e) {
                Spinner spinner = (Spinner)sender;
                selectedCity = (string)spinner.GetItemAtPosition(e.Position);
            };

            Button show = FindViewById<Button>(Resource.Id.showButton);
            show.Click += delegate {
                var back = new Intent(this, typeof(SecondActivity));
  
                back.PutExtra("City", selectedCity);
                StartActivity(back);
            };

        }

        }
    }