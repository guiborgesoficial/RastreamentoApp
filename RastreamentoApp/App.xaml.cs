using RastreamentoApp.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RastreamentoApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            TabbedPage MeuTabbedPages = new TabbedPage();
            MeuTabbedPages.BarBackgroundColor = Color.RoyalBlue;
            MeuTabbedPages.BarTextColor = Color.White;
            MeuTabbedPages.Children.Add(new MainPage());
            MeuTabbedPages.Children.Add(new ContentPageDias());
            MeuTabbedPages.Children.Add(new ContentPageIntegracoes());
            MainPage = MeuTabbedPages;
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
