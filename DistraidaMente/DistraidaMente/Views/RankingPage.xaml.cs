using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Essentials;

using Plugin.Connectivity;

namespace DistraidaMente.Views
{
    public partial class RankingPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        public RankingPage()
        {
            InitializeComponent();

        }

        private void CheckConnectivity()
        {
            var isConnected = CrossConnectivity.Current.IsConnected;
            if (isConnected == true)
            {
                DisplayAlert("Internet", "Tienes Internet!", "Ok");
            }
            else
            {
                DisplayAlert("No Internet", "Para poder ver el ranking se necesita una conexión a Internet!!", "Ok");
            }
        }

        /*public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
   Ranking lvi = value as Ranking;
   int ordinal = 0;

   return ordinal;
}

public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
{
   throw new NotImplementedException("GetIndexMultiConverter_ConvertBack");
}*/

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {

                //CheckConnectivity();
                var isConnected = CrossConnectivity.Current.IsConnected;

                if (isConnected == true)
                {
                    var allRankings = await firebaseHelper.GetRankings();
                    lstRankings.ItemsSource = allRankings;
                }
                else
                {
                    await DisplayAlert("No Internet", "Para poder ver el ranking se necesita una conexión a Internet!!", "Ok");
                }
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("No Internet", "Para poder ver el ranking se necesita una conexión a Internet!!", "Ok");
            }
        }
    }
    
}
