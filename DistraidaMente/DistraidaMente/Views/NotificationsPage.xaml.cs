using System;
using UOCApps.Common.Model;
using Xamarin.Forms;

namespace DistraidaMente.Views
{
	public partial class NotificationsPage : ContentPage
    {
        private Configuration _configuration;
        bool ignoreToogledEvent;

        public NotificationsPage()
        {
            _configuration = new Configuration();
            InitializeComponent ();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            // Do your thing
            try
            {
                ignoreToogledEvent = true;

                switch21.IsToggled = _configuration.ReadBoolProperty("notify1");
                switch22.IsToggled = _configuration.ReadBoolProperty("notify2");

                ignoreToogledEvent = false;
            }
            catch (Exception ex)
            {
                throw new Exception("OnAppearing Additional information..." + ex, ex);
            }
        }

        void OnToggled(object sender, ToggledEventArgs e)
        {
            if (ignoreToogledEvent) return;

            Switch cell = (sender as Switch);
            var classId = cell.ClassId;

            _configuration.SaveBoolProperty(classId, cell.IsToggled);

            //DisplayAlert("Success", String.Format("Switch is now {0} {1}", classId, e.Value), "OK");
        }
    }
}

