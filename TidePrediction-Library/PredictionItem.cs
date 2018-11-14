using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TidePrediction_Library
{
    public class PredictionItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Height { get; set; }
        public string Hi_Low { get; set; }

        public PredictionItem()
        {

        }
        public PredictionItem(string c, string dt, string d, string t,
            string h, string h_l)
        {
            City = c;
            Date = dt;
            Day = d;
            Time = t;
            Height = h;
            Hi_Low = h_l;
        }

        public override string ToString()
        {
            return Day + " " + Date + "\n" + ConvertTime(Time) + " " + Hi_Low;
        }

        //Convert to 24 hour time
        public string ConvertTime(string time)
        {
            var date = DateTime.ParseExact(time, "h:mm tt",
            CultureInfo.InvariantCulture).ToString("HH:mm");

            return date;
        }
    }
}
