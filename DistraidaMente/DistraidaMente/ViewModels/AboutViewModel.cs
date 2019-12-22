using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DistraidaMente.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "Info";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.uoc.edu/portal/es/"));
        }

        public ICommand OpenWebCommand { get; }
    }
}