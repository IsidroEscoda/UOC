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
    public partial class StadisticsPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        bool _collapsed;
        public StadisticsPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);

            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Black;
            //((NavigationPage)Application.Current.MainPage).BarTextColor = Color.OrangeRed;
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

            try
            {
                base.OnAppearing();

                var itemsonline = firebaseHelper.GetPersonDocId("DOC002P2");

                var retornodoservidor = await itemsonline;
                prurea.Text = retornodoservidor.PruRea.ToString();
                prusal.Text = retornodoservidor.PruSal.ToString();
                resace.Text = retornodoservidor.ResAce.ToString();
                reserr.Text = retornodoservidor.ResErr.ToString();
                pissol.Text = retornodoservidor.PisSol.ToString();
                //var listitemsnoservidor = retornodoservidor.ToList();
            }

            catch (Exception ex)
            {
                throw new Exception("OnAppearing  Additional information..." + ex, ex);
            }
        }

        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}