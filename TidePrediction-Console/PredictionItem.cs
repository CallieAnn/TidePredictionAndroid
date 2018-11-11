using System;
using System.Collections.Generic;
using System.Text;

namespace TidePrediction_Console
{
    public class PredictionItem
    {
        public string City { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Height { get; set; }
        public string Hi_Low { get; set; }

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
            return Day + " " + Date + "\n" + Time + " " + Hi_Low;
        }
    }
}
