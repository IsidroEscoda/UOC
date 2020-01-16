using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Essentials;

using Xamarin.Forms.Xaml;
using System.Collections;

namespace DistraidaMente.Views
{
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
            try
            {
                var allRankings = await firebaseHelper.GetRankings();
                lstRankings.ItemsSource = allRankings;
        
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("No Internet", "Para poder ver el ranking se necesita una conexión a Internet!!", "Ok");
            }
        }
    }
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var listView = parameter as ListView;
            var collection = listView.ItemsSource as IList;
            object item = value;

            var index = collection.IndexOf(item) + 1;
            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }
}
