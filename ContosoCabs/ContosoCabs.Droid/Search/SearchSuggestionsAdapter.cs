using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ContosoCabs.Service;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Utils;
using ContosoCabs.ServiceModels;
using System.Threading.Tasks;

namespace ContosoCabs.Droid.Search
{
    public class SearchSuggestionsAdapter : SimpleCursorAdapter
    {

        private Context mContext;
        private static string[] mFields = { "_id", "result" };
        private static string[] mVisible = { "result" };
        private static int[] mViewIds = { Android.Resource.Id.Text1 };
        private List<string> mSuggestions;
        public SearchSuggestionsAdapter(Context context) : base(context, Android.Resource.Layout.SimpleListItem1, null, mVisible, mViewIds, 0)
        {
            mContext = context;
        }
        public string GetObjectAt(int position)
        {
            return mSuggestions[position];
        }
        public async Task<List<string>> GetSuggestions(string input)
        {
            CabsAPI api = new CabsAPI();
            string token = mContext.GetSharedPreferences(Constants.MY_PREF, 0).GetString("token", " ");
            SearchResponse searchResponse = await api.GetSuggestions(input, token);
            //return new SuggestionCursor(constraint);
            if (searchResponse.Code == ResponseCode.SUCCESS)
            {
                
                List<string> suggestions = searchResponse.Suggestions.Select(su => su.Text).ToList();
                mSuggestions = suggestions;
                return suggestions;
            }
            else
            {
                List<string> su = new List<string>();
                mSuggestions = su;
                su.Add("No results");
                return su;
            }
        }
        public override ICursor RunQueryOnBackgroundThread(ICharSequence constraint)
        {
            MatrixCursor cursor = new MatrixCursor(mFields);
            if (constraint == null)
            {
                MatrixCursor.RowBuilder builder = cursor.NewRow();
                builder.Add("0");
                builder.Add("Enter a place");
            }
            else
            {
                List<string> suggestions = GetSuggestions(constraint.ToString()).Result;
                for (int i = 0; i < suggestions.Count; i++)
                {
                    MatrixCursor.RowBuilder builder = cursor.NewRow();
                    builder.Add(i.ToString());
                    builder.Add(suggestions[i]);
                }
            }
            return cursor;
        }
    }
}