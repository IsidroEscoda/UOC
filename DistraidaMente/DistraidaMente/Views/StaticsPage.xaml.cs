using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using DistraidaMente.Common.Model;

namespace DistraidaMente.Views
{
	public partial class StaticPage : ContentPage
    {
        private Configuration _configuration;
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool _collapsed;
        string docIdFB;
        public StaticPage()
		{
			InitializeComponent ();

            //docIdFB = _configuration.ReadProperty("docId");
        }
        public StaticPage(string docId)
        {
            InitializeComponent();

            docIdFB = docId;
        }
        async void Rotate_Clicked(object sender, EventArgs e)
        {
            if (_collapsed)
            {
                await Task.WhenAll(new List<Task> { MyContent.LayoutTo(new Rectangle(MyContent.Bounds.X, MyContent.Bounds.Y, MyContent.Bounds.Width, 300), 500, Easing.CubicOut), MyButton.RotateTo(180, 500, Easing.SpringOut) });
                _collapsed = false;
            }
            else
            {
                await Task.WhenAll(new List<Task> { MyContent.LayoutTo(new Rectangle(MyContent.Bounds.X, MyContent.Bounds.Y, MyContent.Bounds.Width, 0), 500, Easing.CubicIn), MyButton.RotateTo(360, 500, Easing.SpringOut) });
                MyButton.Rotation = 0;
                _collapsed = true;
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            // Do your thing
            try
            {
                base.OnAppearing();

                var itemsonline = firebaseHelper.GetPersonDocId(docIdFB);
                var responseFB = await itemsonline;
                prurea.Text = responseFB.PruRea.ToString();
                prusal.Text = responseFB.PruSal.ToString();
                resace.Text = responseFB.ResAce.ToString();
                reserr.Text = responseFB.ResErr.ToString();
                pissol.Text = responseFB.PisSol.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OnAppearing Additional information..." + ex, ex);
            }

        }

        async void OnBackButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
	}
}

