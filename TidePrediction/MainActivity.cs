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

namespace TidePrediction
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        List<IDictionary<string, object>> tideList;
        PredictionItem[] prediction;
        PredictionItem singlePrediction;
        string date;
        string day;
        string time;
        string height;
        string hiLow;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


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

            var tidePredictions = db.Table<PredictionItem>().GroupBy(p => p.City).Select(p => p.First());
            var tides = (from t in db.Table<PredictionItem>()
                          where (t.City == "Reedsport")    
                          select t).ToList();
            int count = tides.Count;
            PredictionItem[] tidesArray = new PredictionItem[count];
            for (int i = 0; i < count; i++)
            {
                tidesArray[i] =
                    tides[i];
                    //tides[i].Date + "\t\t" + tides[i].Day + "\n" 
                    //+ tides[i].Time+ "\t\t" + tides[i].Hi_Low + "\t\t" +  tides[i].Height;
            }

               // new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, stockInfoArray);
            //XmlTideFileParser parser = new XmlTideFileParser(Assets.Open(@"9434032_annual.xml"));

            //tideList = parser.TideList;

            //prediction = new PredictionItem[tideList.Count];

            //int i = 0;
            //foreach(JavaDictionary<string, object> d in tideList)
            //{
            //    date = (string)d["date"];
            //    day = (string)d["day"];
            //    time = (string)d["time"];
            //    height = (string)d["pred_in_ft"];
            //    hiLow = (string)d["highlow"];
            //    singlePrediction = new PredictionItem(date, day, time,
            //        height, hiLow);

            //    prediction[i] = singlePrediction;
            //    i++;
            //}

            ListAdapter = new TideAdapter<PredictionItem>(this, Android.Resource.Layout.SimpleListItem1, tidesArray );

            ListView.FastScrollEnabled = true;

        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //show the tide height in inches (convert from feet to inches)
            decimal feet = 0;
            decimal.TryParse(prediction[position].Height, out feet);
            var measurement = ((decimal)feet * 12)+" inches".ToString();
            Android.Widget.Toast.MakeText(this, measurement, Android.Widget.ToastLength.Short).Show();
        }
    }
}