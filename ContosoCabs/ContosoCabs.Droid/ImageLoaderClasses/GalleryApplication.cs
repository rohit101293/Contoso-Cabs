
using Android.App;
using UniversalImageLoader.Core;

namespace ContosoCabs.Droid.ImageLoaderClasses
{
    public class GalleryApplication : Application
    {
        public override void OnCreate()
        {
            base.OnCreate();
            DisplayImageOptions displayImageOptions = new DisplayImageOptions.Builder()
                .CacheInMemory(true)
                .CacheOnDisk(true).Build();
            ImageLoaderConfiguration imagaeLoaderConfigurtaion = new ImageLoaderConfiguration.Builder(ApplicationContext)
                .DefaultDisplayImageOptions(displayImageOptions)
                .Build();
            ImageLoader.Instance.Init(imagaeLoaderConfigurtaion);
                
        }
    }
}