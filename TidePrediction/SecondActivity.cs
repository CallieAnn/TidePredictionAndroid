﻿using Android.App;
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

namespace TidePrediction
{
    [Activity(Label = "Second", ParentActivity = typeof(MainActivity))]
    public class SecondActivity : ListActivity
    {
        PredictionItem[] tidesArray;
        const string CITY = "City";
        const string DATE = "Date";
        const string TODAY = "isToday";

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
    }
}