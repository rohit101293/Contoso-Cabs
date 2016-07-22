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
using Android.Speech.Tts;

namespace ContosoCabs.Droid.Voice
{
    [Activity(Label = "VoiceActivity")]
    public class VoiceActivity : Activity, TextToSpeech.IOnInitListener
    {
        TextToSpeech textToSpeech;
        Context context;
        private readonly int MyCheckCode = 101, NeedLang = 103;
        Java.Util.Locale lang;
        //String hello = "String";
        protected override void OnCreate(Bundle savedInstanceState)
        //void speak(string hello,Context con)
        //sending context and string from other activity/frame
        {
            base.OnCreate(savedInstanceState);
            //creating a texttospeech instance for voice commands
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
            //  new TextToSpeech(con, this, "com.google.android.tts");
            lang = Java.Util.Locale.Default;
            //setting language , pitch and speed rate to the voice
            textToSpeech.SetLanguage(lang);
            textToSpeech.SetPitch(1f);
            textToSpeech.SetSpeechRate(1f);
            //Speak the string sent to function
            textToSpeech.Speak("string to speak", QueueMode.Flush, null,null);

        }
        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);

        }
    }
}