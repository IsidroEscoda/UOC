using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration;
using DeltaApps.PositiveApps.Common.Model;

namespace DeltaApps.PositiveThings.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabsPage : Xamarin.Forms.TabbedPage
    {
        public TabsPage(Configuration configuration)
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            
            //configuration.ReadProperty("Points");
           
            //Xamarin.Forms.Application.Current.MainPage.SetValue(property: NavigationPage.BarBackgroundColorProperty, Color.Black);

        }
    }
}