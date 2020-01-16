using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using DistraidaMente.Helpers;
using UOCApps.Common.Model;
using DistraidaMente.Model;
using System.Collections.ObjectModel;

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
			//InitializeComponent ();

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
                //await Task.WhenAll(new List<Task> { MyFrame.LayoutTo(new Rectangle(MyFrame.Bounds.X, MyFrame.Bounds.Y, MyFrame.Bounds.Width, 300), 500, Easing.CubicOut), MyButton.RotateTo(180, 500, Easing.SpringOut) });
                await Task.WhenAll(new List<Task> { MyContent.LayoutTo(new Rectangle(MyContent.Bounds.X, MyContent.Bounds.Y, MyContent.Bounds.Width, 300), 500, Easing.CubicOut), MyButton.RotateTo(180, 500, Easing.SpringOut) });
                _collapsed = false;
            }
            else
            {
                //await Task.WhenAll(new List<Task> { MyFrame.LayoutTo(new Rectangle(MyFrame.Bounds.X, MyFrame.Bounds.Y, MyFrame.Bounds.Width, 0), 500, Easing.CubicIn), MyButton.RotateTo(360, 500, Easing.SpringOut) });
                await Task.WhenAll(new List<Task> { MyContent.LayoutTo(new Rectangle(MyContent.Bounds.X, MyContent.Bounds.Y, MyContent.Bounds.Width, 0), 500, Easing.CubicIn), MyButton.RotateTo(360, 500, Easing.SpringOut) });
                MyButton.Rotation = 0;
                _collapsed = true;
            }
        }
        public List<CheckBoxResume> Jugadores = new List<CheckBoxResume>();

        protected async override void OnAppearing()
        {
            ObservableCollection<CheckBoxResume> NewsItemList = new ObservableCollection<CheckBoxResume>();

            base.OnAppearing();
            //await GetCheckBoxResume();
            try
            {
                var responseFB = await firebaseHelper.GetPersonDocId(docIdFB);
                prurea.Text = responseFB.PruRea.ToString();
                prusal.Text = responseFB.PruSal.ToString();
                resace.Text = responseFB.ResAce.ToString();
                reserr.Text = responseFB.ResErr.ToString();
                pissol.Text = responseFB.PisSol.ToString();

                if (responseFB.Sopas != 0)
                {
                    CheckBoxResume initsta1 = new CheckBoxResume()
                    {
                        TypeName = "Sopas de letras",
                        Value = responseFB.Sopas,
                    };
                    NewsItemList.Add(initsta1);
                }
                if(responseFB.Adivinanzas != 0)
                {
                    CheckBoxResume initsta2 = new CheckBoxResume()
                    {
                        TypeName = "Adivinanzas",
                        Value = responseFB.Adivinanzas,
                    };
                    NewsItemList.Add(initsta2);
                }
                if (responseFB.Musica != 0)
                {
                    CheckBoxResume initsta3 = new CheckBoxResume()
                    {
                        TypeName = "Música",
                        Value = responseFB.Musica,
                    };
                    NewsItemList.Add(initsta3);
                }
                if (responseFB.Relax != 0)
                {
                    CheckBoxResume initsta4 = new CheckBoxResume()
                    {
                        TypeName = "Relax",
                        Value = responseFB.Relax,
                    };
                    NewsItemList.Add(initsta4);
                }


                if (responseFB.Personales != 0)
                {
                    CheckBoxResume initsta5 = new CheckBoxResume()
                    {
                        TypeName = "Personales",
                        Value = responseFB.Personales,
                    };
                    NewsItemList.Add(initsta5);
                }
                if (responseFB.Diferencias != 0)
                {
                    CheckBoxResume initsta6 = new CheckBoxResume()
                    {
                        TypeName = "Diferencias",
                        Value = responseFB.Diferencias,
                    };
                    NewsItemList.Add(initsta6);
                }
                if (responseFB.Sociales != 0)
                {
                    CheckBoxResume initsta7 = new CheckBoxResume()
                    {
                        TypeName = "Sociales",
                        Value = responseFB.Sociales,
                    };
                    NewsItemList.Add(initsta7);
                }
                if (responseFB.Accion != 0)
                {
                    CheckBoxResume initsta8 = new CheckBoxResume()
                    {
                        TypeName = "Acción",
                        Value = responseFB.Accion,
                    };
                    NewsItemList.Add(initsta8);
                }
                if (responseFB.Enigmas != 0)
                {
                    CheckBoxResume initsta9 = new CheckBoxResume()
                    {
                        TypeName = "Enigmas",
                        Value = responseFB.Enigmas,
                    };
                    NewsItemList.Add(initsta9);
                }

                list.ItemsSource = NewsItemList;
                //var cbr = await firebaseHelper.GetPersonResume(docIdFB);
                /*var cbr = await firebaseHelper.GetPersonResume(docIdFB);
                var listView = new ListView
                {
                    RowHeight = 40
                };

                listView.ItemsSource = responseFB.CheckBoxResume;

                StackLayout layout = new StackLayout();

                MyContent.Children.Add(listView);*/
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("No User Info", "Para poder ver los datos de usuario se necesita una conexión a Internet!!", "Ok");
            }

            try
            {
                //var allRankings = await firebaseHelper.GetResume("DOC002P23");
                //list.ItemsSource = allRankings;
                //var cbr = await firebaseHelper.GetResume(docIdFB);
                //var cbr = await firebaseHelper.GetRankings();
                //list.ItemsSource = cbr;
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("No Check Box User Info", "Para poder ver los datos de usuario se necesita una conexión a Internet!!", "Ok");
            }
        }

        private async Task GetCheckBoxResume()
        {
            try
            {
                var cbr = await firebaseHelper.GetPersonResume2(docIdFB);
                list.ItemsSource = cbr;
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("No Check Box User Info", "Para poder ver los datos de usuario se necesita una conexión a Internet!!", "Ok");
            }
        }

        async void OnBackButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
	}
}

