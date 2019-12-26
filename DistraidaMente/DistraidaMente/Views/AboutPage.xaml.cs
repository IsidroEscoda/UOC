using DistraidaMente.Controllers;
using DistraidaMente.Helpers;
using DistraidaMente.Model;
using DistraidaMente.Models;
using DistraidaMente.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DistraidaMente.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        private Configuration _configuration;

        FirebaseHelper firebaseHelper = new FirebaseHelper();


        private ObservableCollection<InfoGroupViewModel> getInfoFB;

        private ObservableCollection<InfoGroupViewModel> getContents;
        private ObservableCollection<InfoGroupViewModel> _expandedContent;
        public AboutPage()
        {
            InitializeComponent();
            getContents = InfoGroupViewModel.Contents;
            UpdateListContent();

        }
        public async System.Threading.Tasks.Task StartInfoAsync()
        {
            var response = await firebaseHelper.GetInfo();
            foreach (InfoClassModel item in response)
            {
                try
                {
                    getContents.Add(new InfoGroupViewModel(item.Name) { new InfoClassModel { Description = item.Description }, }); // item.value is a Java.Lang.Object
                    /*ObservableCollection<InfoGroupViewModel> Items = new ObservableCollection<InfoGroupViewModel>{
                            new InfoGroupViewModel("¿Qué es el dolor"){ new InfoClassModel { Description = "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s,"},},
                            new InfoGroupViewModel("¿Para qué sirve la aplicación?"){ new InfoClassModel {Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s,"},},
                            new InfoGroupViewModel("¿Cómo funciona la aplicación?"){ new InfoClassModel { Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s,"},} };
                            */
                }
                catch (Exception ex)
                {
                    //"EXCEPTION WITH DICTIONARY MAP"
                }
            }
            UpdateListContent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                base.OnAppearing();
                getContents.Clear();
                StartInfoAsync();
            }

            catch (Exception ex)
            {
                throw new Exception("OnAppearing  Additional information..." + ex, ex);
            }
        }


        private void HeaderTapped(object sender, EventArgs args)
        {
            int ListselectedIndex = _expandedContent.IndexOf(
                ((InfoGroupViewModel)((Button)sender).CommandParameter));
            getContents[ListselectedIndex].Expanded = !getContents[ListselectedIndex].Expanded;
            UpdateListContent();
        }

        private void UpdateListContent()
        {
            _expandedContent = new ObservableCollection<InfoGroupViewModel>();
            foreach (InfoGroupViewModel group in getContents)
            {
                InfoGroupViewModel jobs = new InfoGroupViewModel(group.Title, group.Expanded);
                jobs.JobItems = group.Count;
                if (group.Expanded)
                {
                    foreach (InfoClassModel job in group)
                    {
                        jobs.Add(job);
                    }
                }
                _expandedContent.Add(jobs);
            }
            MyListView.ItemsSource = _expandedContent;
        }

        async void OpenVideoCommand(object sender, EventArgs e)
        {
            //Device.BeginInvokeOnMainThread(() => { Application.Current.MainPage = new VideoPage("tutorial.mp4", VideoType.Resource, () => { exitWhenFinish(); }, Assembly.GetExecutingAssembly(), true, true); });
        }

        private void exitWhenFinish()
        {
            TabsPage tabbed = new TabsPage();
            tabbed.CurrentPage = tabbed.Children[1];
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });

            //Navigation.PopAsync();

        }
    }
}