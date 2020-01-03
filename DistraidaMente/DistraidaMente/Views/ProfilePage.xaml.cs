using DeltaApps.PositiveApps.Common.Model;
using System;
using Xamarin.Forms;

namespace DeltaApps.PositiveThings.Views
{
	public partial class ProfilePage : ContentPage
    {
        private Configuration _configuration;

        public ProfilePage ()
        {
            _configuration = new DeltaApps.PositiveApps.Common.Model.Configuration();


            InitializeComponent();
            label_name.Text = _configuration.ReadProperty("name");
            NavigationPage.SetBackButtonTitle(this, "");
        }

		async void OnUpcomingAppointmentsButtonClicked2 (object sender, EventArgs e)
		{
			await Navigation.PushAsync (new FiltersPage ());
		}
        async void OnUpcomingAppointmentsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StaticPage(_configuration.ReadProperty("docId")));
        }
        async void OnUpcomingAppointmentsButtonClicked3(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotificationsPage());
        }
    }
}

