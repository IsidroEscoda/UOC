using DistraidaMente.Controllers;
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
    public partial class ProfilePage : ContentPage
    {
        private Configuration _configuration;
        public ProfilePage()
        {
            _configuration = new Configuration();
            InitializeComponent();
            label_name.Text = _configuration.ReadProperty("name");
        }

        async void OnFiltersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FiltersPage());
        }
        async void OnStatsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StadisticsPage());
        }
        async void OnNotificationsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotificationsPage());
        }
    }
}