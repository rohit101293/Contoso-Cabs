using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Droid.Speech
{
    public class VoiceCommandType
    {
        public const int BOOK_ME_A_CAB_FROM_X_TO_Y = 10;
        public const int BOOK_ME_CHEAPEST_CAB_FROM_X_TO_Y = 20;
        public const int SHOW_COST_FROM_X_TO_Y = 40;
        public const int IF_FROM_APP = 30;
        public const int ESTIMATE_FROM = 40;
        public const int NO_VOICE = 50;
        public const int INVALID_COMMAND = 99;
        public static int BOOK_CHEAPEST_TO_DEST { get; internal set; }
        public const int BOOK_TO_CUSTOM_LOCATION = 999;
        public const string CHEAPEST_KEY_WORD = "cheapest";
        public const string ESTIMATE_KEY_WORD_1 = "estimate";
        public const string ESTIMATE_KEY_WORD_2 = "cost";
        public const string PLACES_KEY_WORD_1 = "home";
        public const string PLACES_KEY_WORD_2 = "work";
        public const string PLACES_KEY_WORD_3 = "aunts place";
        public const string PLACES_KEY_WORD_4 = "play";
        public const string CUSTOM_KEY_WORD_1 = "best";
        public static List<Command> GetSupportedCommands()
        {
            List<Command> commands = new List<Command>();
            Command cheapest = new Command(BOOK_ME_CHEAPEST_CAB_FROM_X_TO_Y);
            cheapest.AddKeyWord(CHEAPEST_KEY_WORD);
            commands.Add(cheapest);
            Command estimate = new Command(ESTIMATE_FROM);
            estimate.AddKeyWord(ESTIMATE_KEY_WORD_1);
            estimate.AddKeyWord(ESTIMATE_KEY_WORD_2);
            commands.Add(estimate);
            Command custom = new Command(BOOK_TO_CUSTOM_LOCATION);
            custom.AddKeyWord(CUSTOM_KEY_WORD_1);
            commands.Add(custom);
            Command places = new Command(BOOK_ME_A_CAB_FROM_X_TO_Y);
            places.AddKeyWord(PLACES_KEY_WORD_1);
            places.AddKeyWord(PLACES_KEY_WORD_2);
            places.AddKeyWord(PLACES_KEY_WORD_3);
            places.AddKeyWord(PLACES_KEY_WORD_4);
            commands.Add(places);
            return commands;
        }
        public static List<string> RemoveDefaultWords(List<string> words)
        {
            List<string> modified = new List<string>(words);
            string[] commonWords = { "book", "me", "a", "the", "cab", "cheapest", "estimate", "show", "list"
                , "estimate", "from", "to", "cost", "here"};
            for (int i = 0; i < commonWords.Length; i++)
            {
                if (modified.Contains(commonWords[i]))
                {
                    modified.Remove(commonWords[i]);
                }
            }
            foreach(var word in modified)
            {
                System.Diagnostics.Debug.WriteLine(word);
            }
            return modified;
        }
    }
}
