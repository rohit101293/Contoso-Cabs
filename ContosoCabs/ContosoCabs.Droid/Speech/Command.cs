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
    public class Command
    {
        public int Type { get; set; }
        public List<string> KeyWords { get; set; }
        public Command(int type)
        {
            Type = type;
            KeyWords = new List<string>();
        }
        public void AddKeyWord(string keyword)
        {
            KeyWords.Add(keyword);
        }

    }
}