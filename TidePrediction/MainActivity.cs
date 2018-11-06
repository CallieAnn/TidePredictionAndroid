using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;

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

            XmlTideFileParser parser = new XmlTideFileParser(Assets.Open(@"9434032_annual.xml"));

            tideList = parser.TideList;

            prediction = new PredictionItem[tideList.Count];

            int i = 0;
            foreach(JavaDictionary<string, object> d in tideList)
            {
                date = (string)d["date"];
                day = (string)d["day"];
                time = (string)d["time"];
                height = (string)d["pred_in_ft"];
                hiLow = (string)d["highlow"];
                singlePrediction = new PredictionItem(date, day, time,
                    height, hiLow);

                prediction[i] = singlePrediction;
                i++;
            }

            ListAdapter = new ArrayAdapter<PredictionItem>(this, Android.Resource.Layout.SimpleListItem1, prediction );
            
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