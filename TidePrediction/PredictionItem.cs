using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TidePrediction
{
    public class PredictionItem
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Height { get; set; }
        public string Hi_Low { get; set; }

        public PredictionItem(string dt, string d, string t,
            string h, string h_l)
        {
            Date = dt;
            Day = d;
            Time = t;
            Height = h;
            Hi_Low = h_l;
        }

        public override string ToString()
        {
            return Day + " " + Date + "\n" + Time + " " + Hi_Low;
        }
    }
}