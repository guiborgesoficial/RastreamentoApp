using RastreamentoApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RastreamentoApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContentPageEncomendaInfo : ContentPage
    {
        public JsonRetorno.Encomendas ListEncomendas = new JsonRetorno.Encomendas();
        public ObservableCollection<JsonRetorno.Evento> ListEventos = new ObservableCollection<JsonRetorno.Evento>();
        public int propIndex { get; set; }
        public bool boolFoiEntregue { get; set; }
        public ContentPageEncomendaInfo()
        {
            InitializeComponent();
        }

        private void btnVoltar_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
        private void  buscandoEventos(JsonRetorno.Encomendas lista)
        {
            for (int i = 0; i < lista.Eventos.Count; i++)
            {
                ListEventos.Add(new JsonRetorno.Evento { Data = lista.Eventos[i].Data, Hora = lista.Eventos[i].Hora, Local = lista.Eventos[i].Local, Status = lista.Eventos[i].Status, SubStatus = lista.Eventos[i].SubStatus, Imagem = ImagemStatusEncomenda(lista, i) });
            }
        }
        private string ImagemStatusEncomenda(JsonRetorno.Encomendas lista, int x)
        {
            for (int i = 0; i < lista.Eventos.Count; i++)
            {
                if (lista.Eventos[i].Status.ToUpper().Contains("ENTREGUE"))
                {
                    lista.Eventos[i].Imagem = "entregue.png";
                    boolFoiEntregue = true;
                }
                else if (lista.Eventos[i].Status.ToUpper().Contains("ENCAMINHADO"))
                {
                    lista.Eventos[i].Imagem = "encaminhado.png";
                }
                else if (lista.Eventos[i].Status.ToUpper().Contains("POSTADO"))
                {
                    lista.Eventos[i].Imagem = "postado.png";
                }
                else if (lista.Eventos[i].Status.ToUpper().Contains("ADUANEIRA"))
                {
                    lista.Eventos[i].Imagem = "fiscalizacaoAduaneira.png";
                }
                else if (lista.Eventos[i].Status.ToUpper().Contains("ENTREGA"))
                {
                    lista.Eventos[i].Imagem = "saiuParaEntrega.png";
                }
                else
                {
                    lista.Eventos[i].Imagem = "outros.png";
                }
            }
            return lista.Eventos[x].Imagem;
        }
        public void RetornandoInfo()
        {
            buscandoEventos(ListEncomendas);
            listviewEncomendasInfo.ItemsSource = ListEventos;

            lblCódigoRastreio.Text = ListEncomendas.Codigo;
            lblServiço.Text = ListEncomendas.Servico;
            if(ListEncomendas.Entregue == true)
            {
                DateTime dataPostagem = Convert.ToDateTime(ListEventos.Last<JsonRetorno.Evento>().Data).Date;
                DateTime dataEntrega = Convert.ToDateTime(ListEventos.First<JsonRetorno.Evento>().Data).Date;
                TimeSpan qtdDiasEntrega = dataEntrega.Subtract(dataPostagem);
                lblDiasTrajeto.Text = qtdDiasEntrega.TotalDays.ToString() + "\bdias";
            }
            else
            {                
                DateTime dataPostagem = Convert.ToDateTime(ListEventos.Last<JsonRetorno.Evento>().Data);
                DateTime dataAtual = DateTime.Now;
                TimeSpan qtdDiasEntrega = dataAtual.Subtract(dataPostagem);
                lblDiasTrajeto.Text = qtdDiasEntrega.Days.ToString() + "\bdias";
            }
        }

        private async void btnCompartilhar_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri("whatsapp://send?phone=+" + ListEncomendas.Telefone));
            }
            catch (Exception erro)
            {
                await DisplayAlert("WhatsApp não instalado", erro.Message, "ok");
            }
            RetornandoInfo();
        }

        private void btnEditar_Clicked(object sender, EventArgs e)
        {
            ContentPageAdicionarNovaEncomenda editarInfo = new ContentPageAdicionarNovaEncomenda();
            editarInfo.entryPropCodigoRastreio = ListEncomendas.Codigo;
            editarInfo.entryPropDescriçãoEncomenda = ListEncomendas.Descricao;
            editarInfo.entryPropTelefone = ListEncomendas.Telefone;
            editarInfo.entryPropValorEncomenda = ListEncomendas.Preço;
            editarInfo.AtribuindoValorEntryParaEditarInfo();
            Navigation.PushModalAsync(editarInfo);
        }
    }
}