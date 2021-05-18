using RastreamentoApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RastreamentoApp.UserInterfaces
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentPageEncomendaInfo : ContentPage
    {
        public ObservableCollection<JsonRetorno.Encomendas> ListEncomendas = new ObservableCollection<JsonRetorno.Encomendas>();
        public ObservableCollection<JsonRetorno.Evento> ListEventos = new ObservableCollection<JsonRetorno.Evento>();
        public ContentPageEncomendaInfo()
        {
            InitializeComponent();
        }

        private void btnVoltar_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
        public void RetornandoInfo()
        {
            listviewEncomendasInfo.ItemsSource = ListEventos;

            lblCódigoRastreio.Text = ListEncomendas[0].Codigo;
            lblServiço.Text = ListEncomendas[0].Servico;
            if(ListEncomendas.First<JsonRetorno.Encomendas>().Entregue == true)
            {
                DateTime dataPostagem = Convert.ToDateTime(ListEventos.Last<JsonRetorno.Evento>().Data).Date;
                DateTime dataEntrega = Convert.ToDateTime(ListEventos.First<JsonRetorno.Evento>().Data).Date;
                TimeSpan qtdDiasEntrega = dataEntrega.Subtract(dataPostagem);
                lblDiasTrajeto.Text = qtdDiasEntrega.Days.ToString() + "\bdias";
            }
            else
            {
                var primeiraData = ListEventos.Last<JsonRetorno.Evento>();
                
                DateTime dataPostagem = Convert.ToDateTime(primeiraData.Data).Date;
                DateTime dataAtual = DateTime.Now;
                TimeSpan qtdDiasEntrega = dataAtual.Subtract(dataPostagem);
                lblDiasTrajeto.Text = qtdDiasEntrega.Days.ToString() + "\bdias";
            }
        }

        private void btnCompartilhar_Clicked(object sender, EventArgs e)
        {
            RetornandoInfo();
        }

        private void btnEditar_Clicked(object sender, EventArgs e)
        {
            ContentPageAdicionarNovaEncomenda editarInfo = new ContentPageAdicionarNovaEncomenda();
            editarInfo.entryPropCodigoRastreio = ListEncomendas[0].Codigo;
            editarInfo.entryPropDescriçãoEncomenda = ListEncomendas[0].Descricao;
            editarInfo.entryPropTelefone = ListEncomendas[0].Telefone;
            editarInfo.entryPropValorEncomenda = ListEncomendas[0].Preço;
            editarInfo.AtribuindoValorEntryParaEditarInfo();
            Navigation.PushModalAsync(editarInfo);
        }
    }
}