﻿using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaApps.PositiveThings.ViewModels;
using DeltaApps.PositiveThings.Model;
using DeltaApps.CommonLibrary.Pages;
using DeltaApps.CommonLibrary.Model;
using DeltaApps.PositiveApps.Common.Model;
using System.Reflection;
using DeltaApps.PositiveThings.Helpers;
using Xamarin.Essentials;
using Plugin.Connectivity;
using DeltaApps.PositiveThings.Pages;

namespace DistraidaMente.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]

    public partial class AboutPage : ContentPage
    {
        FirebaseHelper firebaseHelper = new FirebaseHelper();
        private Configuration _configuration;

        private ObservableCollection<InfoGroupViewModel> getContents;
        private ObservableCollection<InfoGroupViewModel> _expandedContent;

        public AboutPage()
        {
            _configuration = new DeltaApps.PositiveApps.Common.Model.Configuration();

            InitializeComponent();

            getContents = InfoGroupViewModel.Contents;
            UpdateListContent();
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            // Do your thing
            getContents.Clear();
            try
            {
                //CheckConnectivity();
                var isConnected = CrossConnectivity.Current.IsConnected;

                if (isConnected == true)
                {
                    await StartInfoAsync();
                }
                else
                {
                    await DisplayAlert("No Internet", "Para poder ver la info actualizada se necesita una conexión a Internet!!", "Ok");
                }
            }
            catch (System.Exception ex)
            {
                await DisplayAlert("No Internet", "Para poder ver la info actualizada se necesita una conexión a Internet!!", "Ok");
            }
            /*var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
            }*/
        }

        private async Task StartInfoAsync()
        {
            var response = await firebaseHelper.GetInfo();
            foreach (InfoClassModel info in response)
            {
                try
                {
                    getContents.Add(new InfoGroupViewModel(info.Name)
                    {
                        new InfoClassModel
                        {
                            Description = info.Description
                        },
                    });
                }
                catch (Exception ex)
                {

                }
            }

            UpdateListContent();
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
           Device.BeginInvokeOnMainThread(() => { Application.Current.MainPage = new VideoPage("tutorial.mp4", VideoType.Resource, () => { exitWhenFinish(); }, Assembly.GetExecutingAssembly(), true, true); });
        }

        private void exitWhenFinish()
        {
            TabPageCS tabbed = new TabPageCS(_configuration);
            tabbed.CurrentPage = tabbed.Children[1];
            Device.BeginInvokeOnMainThread(() => { Xamarin.Forms.Application.Current.MainPage = tabbed; });

            //Navigation.PopAsync();

        }
    }

    public class HtmlConverter : IValueConverter
    {
        #region Converter

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string text = (string)value;
                return System.Net.WebUtility.HtmlDecode(text);
                //return "html converter";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}