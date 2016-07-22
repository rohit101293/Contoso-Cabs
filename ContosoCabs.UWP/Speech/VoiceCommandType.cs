using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.UWP.Speech
{
    public class VoiceCommandType
    {
        public const int BOOK_ME_A_CAB_FROM_X_TO_Y = 10;
        public const int BOOK_ME_CHEAPEST_CAB_FROM_X_TO_Y = 20;
        public const int SHOW_COST_FROM_X_TO_Y = 40;
        public const int IF_FROM_APP = 30;
        public const int ESTIMATE_FROM = 40;
        public const int NO_VOICE = 50;
        public static int BOOK_TO_CUSTOM_LOCATION = 60;
        public static int BOOK_CHEAPEST_TO_DEST { get; internal set; }
    }
}
