using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

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

            // Create a table
            db.DropTable<PredictionItem>();
            if (db.CreateTable<PredictionItem>() == 0)
            {
                // A table already exixts, delete any data it contains
                db.DeleteAll<PredictionItem>();
            }

            AddPredictionsToDb(db, currentDir + @"/../../../../TidePrediction/Assets/9434032_annual.xml", "Florence");
            AddPredictionsToDb(db, currentDir + @"/../../../../TidePrediction/Assets/depoebay_annual.xml", "Depoe Bay");
            AddPredictionsToDb(db, currentDir + @"/../../../../TidePrediction/Assets/reedsport_annual.xml", "Reedsport");

        }

        private static void AddPredictionsToDb(SQLiteConnection db, string file, string c)
        {
            XmlTideFileParser parser = new XmlTideFileParser(File.Open(@file, FileMode.Open));
            List<IDictionary<string, object>> tideList = parser.TideList;

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

            Console.WriteLine("{0} finished", c);
        }
    }
}
