using DistraidaMente.Controllers;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
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
    public partial class HomePage : ContentPage
    {
        private Configuration _configuration;

        List<Challenge> _challenges;

        bool ignoreToogledEvent;
        FirebaseHelper firebaseHelper = new FirebaseHelper();

        private List<Challenge> _currentChallenges;
        private int _currentChallengeIndex;

        public HomePage()
        {
            _configuration = new Configuration();

            InitializeComponent(); 
            ignoreToogledEvent = true;

            switch10.IsToggled = _configuration.ReadBoolProperty("moverme");
            switch11.IsToggled = _configuration.ReadBoolProperty("relacionarme");

            ignoreToogledEvent = false;

        }
        void OnToggled(object sender, ToggledEventArgs e)
        {
            if (ignoreToogledEvent) return;

            Switch cell = (sender as Switch);
            var classId = cell.ClassId;

            _configuration.SaveBoolProperty(classId, cell.IsToggled);

        }
        async void OpenChallengeCommand(object sender, EventArgs e)
        {
            //StartProcess();
            ChallengePage challengePage = new ChallengePage();
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = challengePage; });
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();

            try
            {
                base.OnAppearing();
               
                var allChallenges = await firebaseHelper.GetAllChallenges();
                lstChallenges.ItemsSource = allChallenges;
            }

            catch (Exception ex)
            {
                throw new Exception("OnAppearing  Additional information..." + ex, ex);
            }
        }
    }
}