using DistraidaMente.Controllers;
using DistraidaMente.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DistraidaMente.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : ContentPage
    {
        private Configuration _configuration;
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public VideoPage()
        {
            _configuration = new Configuration();
            InitializeComponent();
        }
        void OnCloseVideo(object sender, EventArgs e)
        {
            MainPage emoticonsPage = new MainPage();
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = emoticonsPage; });
        }

        protected async override void OnAppearing()
        {
            await firebaseHelper.AddPerson(Convert.ToInt32(_configuration.ReadProperty("docId").Substring(7)), _configuration.ReadProperty("name").ToUpper(), _configuration.ReadProperty("docId").ToUpper(), true);
        }
    }
}