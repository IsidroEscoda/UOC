﻿using DistraidaMente.Controllers;
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
    public partial class FiltersPage : ContentPage
    {
        Configuration _configuration;
        public FiltersPage()
        {
            _configuration = new Configuration();

            InitializeComponent();

            ignoreToogledEvent = true;

            switch1.IsToggled = _configuration.ReadBoolProperty("personal");
            switch2.IsToggled = _configuration.ReadBoolProperty("acertijos");
            switch3.IsToggled = _configuration.ReadBoolProperty("enigmas");
            switch4.IsToggled = _configuration.ReadBoolProperty("diferencias");
            switch5.IsToggled = _configuration.ReadBoolProperty("sopas");
            switch6.IsToggled = _configuration.ReadBoolProperty("relacionarme");
            switch7.IsToggled = _configuration.ReadBoolProperty("moverme");
            switch8.IsToggled = _configuration.ReadBoolProperty("musica");
            switch9.IsToggled = _configuration.ReadBoolProperty("relax");

            ignoreToogledEvent = false;
        }
        bool ignoreToogledEvent;

        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        void OnToggled(object sender, ToggledEventArgs e)
        {
            if (ignoreToogledEvent) return;

            Switch cell = (sender as Switch);
            var classId = cell.ClassId;

            _configuration.SaveBoolProperty(classId, cell.IsToggled);

            //DisplayAlert("Success", String.Format("Switch is now {0} {1}", classId, e.Value), "OK");
            /*Application.Current.Properties.Add(String.Format("{0}", classId), String.Format("{0}", e.Value));
            Application.Current.SavePropertiesAsync();*/
        }
    }
}