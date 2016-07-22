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
using ContosoCabs.Droid.Speech;
namespace ContosoCabs.Droid.Speech
{
    public class Classifier
    {
        public string Text { get; set; }
        public Classifier(string text)
        {
            Text = text.ToLower();
        }
        public ClassifiedResult Classify()
        {
            List<string> words = new List<string>(Text.Split(null));
            List<Command> commands = VoiceCommandType.GetSupportedCommands();
            ClassifiedResult res = new ClassifiedResult(VoiceCommandType.INVALID_COMMAND);
            for (int i = 0; i < commands.Count; i++)
            {
                Command current = commands[i];
                for (int j = 0; j < current.KeyWords.Count; j++)
                {
                    string curKeyWord = current.KeyWords[j].ToLower();
                    if (words.Contains(curKeyWord))
                    {
                        res.Type = current.Type;
                        List<string> data = VoiceCommandType.RemoveDefaultWords(words);
                        
                        if (data.Count == 2)
                        {
                            res.AddData("source", data[0]);
                            res.AddData("destination", data[1]);
                        }
                        else if (data.Count == 1)
                        {
                            res.AddData("destination", data[0]);
                        }
                        else
                        {
                            res.Type = VoiceCommandType.INVALID_COMMAND;
                        }
                        return res;
                    }
                }
            }
            return res;
        }
    }
}