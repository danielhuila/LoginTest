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
using SQLite.Net.Interop;
using Xamarin.Forms;
using LoginTest.Interface;

[assembly: Dependency(typeof(LoginTest.Droid.Config))]

namespace LoginTest.Droid
{
    class Config : IConfig
    {
        private string directoryDB;
        private ISQLitePlatform platform;
        public string DirectoryDB
        {
            get
            {
                if (string.IsNullOrEmpty(directoryDB))
                {
                    directoryDB = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                }
                return directoryDB;
            }
        }

        public ISQLitePlatform Plattform
        {
            get
            {
                platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
                return platform;
            }
            
        }
    }
}