using System;
using System.Collections.Generic;
using SimpleCalculator.Common.Styles;
using Xamarin.Forms;

namespace SimpleCalculator
{
    public partial class MainPage : ContentPage
    {
        

        public MainPage()
        {
            InitializeComponent();
            

            this.BindingContext = new MainPageViewModel();
        }

        int themeIndex = 0;

        ResourceDictionary[] themes = new ResourceDictionary[]
        {
            new DesertTheme(),
            new LavaTheme(),
            new OceanTheme(),
            new SunTheme(),
            new ClayTheme()
        };

        void ThemeSwitcher_Clicked(System.Object sender, System.EventArgs e)
        {
            themeIndex += 1;
            if(themeIndex >= themes.Length)
            {
                themeIndex = 0;
            }

            App.Current.Resources = themes[themeIndex];
            
        }
    }
}
