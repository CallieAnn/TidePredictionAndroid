using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TidePrediction_Library;

namespace TidePrediction_Console
{
    class Program
    {

        static string currentDir;
        static void Main(string[] args)
        {

            Console.WriteLine("Hello Tide Prediction!");

            // We're using a db file in the Android project's Assets folder
            currentDir = Directory.GetCurrentDirectory();
            // Console.WriteLine(currentDir);
            string dbPath = currentDir + @"/../../../../TidePrediction/Assets/tides.db3";
            var db = new SQLiteConnection(dbPath);

            AddPredictions(db);

            //Add Coordinates
            AddLocationCoordinates(db);
            CheckForPredictionLocationTable(db);

        }

        private static void CheckForPredictionLocationTable(SQLiteConnection db)
        {
            var table = (from t in db.Table<PredictionLocation>()
                         select t.Name).ToList();

            foreach(string name in table)
            {
                Console.WriteLine(name);
            }
        }
        private static void AddLocationCoordinates(SQLiteConnection db)
        {
            // Create a table
            db.DropTable<PredictionLocation>();
            if (db.CreateTable<PredictionLocation>() == 0)
            {
                // A table already exixts, delete any data it contains
                db.DeleteAll<PredictionLocation>();
            }

            PredictionLocation florenceCoordinates = new PredictionLocation { Name = "Florence", Latitude = -124.0998, Longitude = 43.9826 };
            PredictionLocation reedsportCoordinates = new PredictionLocation { Name = "Reedsport", Latitude = -124.0968, Longitude = 43.7023 };
            PredictionLocation depoeBayCoordinates = new PredictionLocation { Name = "Depoe Bay", Latitude = -124.0632, Longitude = 44.8084 };

            int pk = 0;
            pk += db.Insert(florenceCoordinates);
            pk += db.Insert(reedsportCoordinates);
            pk += db.Insert(depoeBayCoordinates);

            //check finished
            Console.WriteLine("{0} finished", pk);
        }

        private static void AddPredictions(SQLiteConnection db)
        {
            // Create a table
            db.DropTable<PredictionItem>();
            if (db.CreateTable<PredictionItem>() == 0)
            {
                // A table already exixts, delete any data it contains
                db.DeleteAll<PredictionItem>();
            }

            AddPredictionsToDb(db, currentDir + @"/../../../../TidePrediction/Assets/9434032_annual.xml", "Florence");

            AddPredictionsToDb(db, currentDir + @"/../../../../TidePrediction/Assets/reedsport_annual.xml", "Reedsport");
            AddPredictionsToDb(db, currentDir + @"/../../../../TidePrediction/Assets/depoebay_annual.xml", "Depoe Bay");
        }

        private static void AddPredictionsToDb(SQLiteConnection db, string file, string c)
        {
            XmlTideFileParser parser = new XmlTideFileParser(File.Open(@file, FileMode.Open));
            List<IDictionary<string, object>> tideList = parser.TideList;

            //db.BeginTransaction();
            // Copy into our Database
            int pk = 0;
            string dt;
            string d;
            string t;
            string h;
            string h_l;

            foreach (IDictionary<string, object> prediction in tideList)
            {
                dt = (string)prediction["date"];
                d = (string)prediction["day"];
                t = (string)prediction["time"];
                h = (string)prediction["pred_in_ft"];
                h_l = (string)prediction["highlow"];

                pk += db.Insert(new PredictionItem(c, dt, d, t, h, h_l)
                {
                    City = c,
                    Date = dt,
                    Day = d,
                    Time = t,
                    Height = h,
                    Hi_Low = h_l

                });
            }
            //db.Commit();
         
            //check how many rows finished for each location
            Console.WriteLine("{0} {1} finished", c, pk);
        }
    }
}
