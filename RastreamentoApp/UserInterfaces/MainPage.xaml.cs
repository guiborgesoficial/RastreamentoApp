using RastreamentoApp.UserInterfaces;
using System;
using Xamarin.Forms;

namespace RastreamentoApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        public void btnAdicionarEncomenda_Clicked(object sender, EventArgs e)
        {
            ContentPageAdicionarNovaEncomenda TelaAdicionarNovaEncomenda = new ContentPageAdicionarNovaEncomenda();
            Navigation.PushModalAsync(TelaAdicionarNovaEncomenda);
        }
    }
}
