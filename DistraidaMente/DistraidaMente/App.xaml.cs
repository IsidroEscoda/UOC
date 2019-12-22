using DistraidaMente.Controllers;
using DistraidaMente.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DistraidaMente
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DistractController controller = new DistractController();
            controller.StartProcess();
            //MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
