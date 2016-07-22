using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace ContosoCabs.UWP.Speech
{
    public class Arguments
    {
        public Dictionary<string, object> Values { get; set; }
        public SpeechRecognitionResult SpeechRecognitionResult { get; set; }
        public Dictionary<int, bool> Types { get; set; }
        public Arguments(SpeechRecognitionResult speechRecognitionResults)
        {
            Values = new Dictionary<string, object>();
            this.SpeechRecognitionResult = speechRecognitionResults;
            Types = new Dictionary<int, bool>();
            
        }
        public void AddArgument(string key, object value)
        {
            Values.Add(key, value);
        }
        public object GetArgument(string key)
        {
            return Values[key];
        }
        public void AddType(int key, bool value)
        {
            Types.Add(key, value);
        }
        public bool GetType(int key)
        {
            try
            {
                return Types[key];
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
    }
}
