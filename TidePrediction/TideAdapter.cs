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
using Java.Lang;

namespace TidePrediction
{
    public class TideAdapter<T> : ArrayAdapter<PredictionItem>, ISectionIndexer
    {
        PredictionItem[] items; 
        Activity context;
        Java.Lang.Object[] sectionsObjects;
        Dictionary<string, int> alphaIndex;
        string[] sections;

        public TideAdapter(Context c, int resource, PredictionItem[] objects) : base(c, resource, objects)
        {
            items = objects;
            context = (Activity)c;
            BuildSectionIndex();
        }

        public int GetPositionForSection(int section)
        {
            return alphaIndex[sections[section]];
        }

        public int GetSectionForPosition(int position)
        {
            return 1;
        }

        public Java.Lang.Object[] GetSections()
        {
            return sectionsObjects;
        }

        private void BuildSectionIndex()
        {
            alphaIndex = new Dictionary<string, int>();     // Map sequential numbers
            for (var i = 0; i < items.Length; i++)
            {
                // Use the part of speech as a key
                string[] dateSplit = items[i].Date.Split('/');
                int year, month, day;

                int.TryParse(dateSplit[0], out day);
                int.TryParse(dateSplit[1], out month);
                int.TryParse(dateSplit[2], out year);

                var key = month.ToString();
                if (!alphaIndex.ContainsKey(key))
                {
                    alphaIndex.Add(key, i);
                }
            }

            // Get the count of sections
            sections = new string[alphaIndex.Keys.Count];
            // Copy section names into the sections array
            alphaIndex.Keys.CopyTo(sections, 0);

            // Copy section names into a Java object array
            sectionsObjects = new Java.Lang.Object[sections.Length];
            for (var i = 0; i < sections.Length; i++)
            {
                sectionsObjects[i] = new Java.Lang.String(sections[i]);
            }
        }
    }
}