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

namespace ContosoCabs.Droid.Speech
{
    public class ClassifiedResult
    {
        public int Type;
        public Dictionary<string, string> Data;
        public ClassifiedResult(int type)
        {
            Type = type;
            Data = new Dictionary<string, string>();
        }
        public void AddData(string key, string value)
        {
            Data.Add(key, value);
        }
        public string GetData(string key)
        {
            string val = "";
            Data.TryGetValue(key, out val);
            return val;
        }
    }
}