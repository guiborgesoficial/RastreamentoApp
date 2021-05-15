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
            if(ListEncomendas[0].Entregue == true)
            {
                DateTime dataPostagem = Convert.ToDateTime(ListEventos[ListEventos.Count - 2].Data).Date;
                DateTime dataEntrega = Convert.ToDateTime(ListEventos[0].Data).Date;
                TimeSpan qtdDiasEntrega = dataEntrega.Subtract(dataPostagem);
                lblDiasTrajeto.Text = qtdDiasEntrega.TotalDays.ToString() + "\bdias";
            }
            else
            {
                int index = 0; ;
                if(ListEventos.Count >= 0 && ListEventos.Count <= 2)
                {
                    index = 0;
                }
                else
                {
                    index = ListEventos.Count - 2;
                }
                DateTime dataPostagem = Convert.ToDateTime(ListEventos[index].Data).Date;
                DateTime dataAtual = DateTime.Now;
                TimeSpan qtdDiasEntrega = dataAtual.Subtract(dataPostagem);
                lblDiasTrajeto.Text = qtdDiasEntrega.TotalDays.ToString() + "\bdias";
            }
        }

        private void btnCompartilhar_Clicked(object sender, EventArgs e)
        {

        }

        private void btnEditar_Clicked(object sender, EventArgs e)
        {
            RetornandoInfo();
        }
    }
}