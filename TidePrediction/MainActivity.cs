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
using System;
using System.Globalization;
using TidePrediction_Library;


namespace TidePrediction
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        DateTime parsedDate;
        const string CITY = "City";
        const string DATE = "Date";

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

            //spinner initialized
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

            //datepicker initialization
            var datePicker = FindViewById<DatePicker>(Resource.Id.datePicker);
            PredictionItem firstItem =
                db.Get<PredictionItem>((from p in db.Table<PredictionItem>() select p).Min(p => p.ID));
            string firstDate = firstItem.Date;

            datePicker.DateTime = StringToDate(firstDate);
           


            Button show = FindViewById<Button>(Resource.Id.showButton);
            show.Click += delegate {
                var back = new Intent(this, typeof(SecondActivity));
                
                //get the date in a string format that matches the database
                string chosenDate = datePicker.DateTime.ToString("yyyy/MM/dd");

                back.PutExtra(CITY, selectedCity);
                back.PutExtra(DATE, chosenDate);
                StartActivity(back);
            };

        }

        public DateTime StringToDate(string date)
        {
            parsedDate = new DateTime(2000, 1, 1, 12, 32, 30);
            DateTime.TryParse(date, out parsedDate);
            return parsedDate;
        }

        }
    }