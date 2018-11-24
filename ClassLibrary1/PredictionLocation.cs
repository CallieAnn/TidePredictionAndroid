using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TidePrediction_Library
{
    [Table("Locations")]
    public class PredictionLocation
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
