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
    public partial class RankingPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public RankingPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            var allRankings = await firebaseHelper.GetRankings();
            lstRankings.ItemsSource = allRankings;
        }
    }
}