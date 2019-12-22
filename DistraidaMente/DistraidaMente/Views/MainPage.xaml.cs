using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DistraidaMente.Models;

namespace DistraidaMente.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        int clickTotal;

        public MainPage()
        {
            InitializeComponent();
        }
        protected void OnImageButtonClicked(object sender, EventArgs e)
        {
            ImageButton cell = (sender as ImageButton);
            var classId = cell.ClassId;
            //DisplayAlert("¿Cómo te encuentras?", "Has seleccionado " + classId, "OK");
            clickTotal += 1;
            TabsPage tabsPage = new TabsPage();
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabsPage; });
        }
    }
}