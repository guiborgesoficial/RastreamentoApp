using PCLExt.FileStorage.Folders;
using RastreamentoApp.Classes;
using RastreamentoApp.UserInterfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RastreamentoApp
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<JsonRetorno.Encomendas> ObsColListEncomendas = new ObservableCollection<JsonRetorno.Encomendas>();
        private ObservableCollection<JsonRetorno.Evento> ObsColListEventos = new ObservableCollection<JsonRetorno.Evento>();
        public bool boolFoiEntregue { get; set; } = false;

        public MainPage()
        {
            InitializeComponent();
            ConsultandoEncomendasInTracks();
        }

        public void btnAdicionarEncomenda_Clicked(object sender, EventArgs e)
        {
            ContentPageAdicionarNovaEncomenda TelaAdicionarNovaEncomenda = new ContentPageAdicionarNovaEncomenda();
            Navigation.PushModalAsync(TelaAdicionarNovaEncomenda);
            ConsultandoEncomendasInTracks();
        }

        public async Task ConsultandoEncomendasInTracks()
        {
            ObsColListEncomendas.Clear();
            ObsColListEventos.Clear();

            bool existeDiretorio = Directory.Exists("/storage/emulated/0/Android/data/com.companyname.rastreamentoapp");

            if (existeDiretorio)
            {
                string[] tracks = Directory.GetFiles("/storage/emulated/0/Android/data/com.companyname.rastreamentoapp", "*.json", SearchOption.AllDirectories);

                for (int i = 0; i < tracks.Length; i++)
                {
                    FileInfo infoArquivo = new FileInfo(tracks[i]);
                    StreamReader leitorArquivo = new StreamReader(tracks[i]);
                    if (infoArquivo.Length > 10)
                    {
                        string response = leitorArquivo.ReadToEnd();
                        DeserializerJsonStringContent(response);
                    }
                }
            }
        }
        private void DeserializerJsonStringContent(string Iresponse)
        {
            var des = (JsonRetorno.Encomendas)Newtonsoft.Json.JsonConvert.DeserializeObject(Iresponse, typeof(JsonRetorno.Encomendas));
            ServiceEncomendas(des);
        }
        private void ServiceEncomendas(JsonRetorno.Encomendas IresponseDeserializada)
        {
            for (int i = 0; i < IresponseDeserializada.Eventos.Count; i++)
            {
                ObsColListEventos.Add(new JsonRetorno.Evento { Data = IresponseDeserializada.Eventos[i].Data, Hora = IresponseDeserializada.Eventos[i].Hora, Local = IresponseDeserializada.Eventos[i].Local, Status = IresponseDeserializada.Eventos[i].Status, SubStatus = IresponseDeserializada.Eventos[i].SubStatus, Imagem = ImagemStatusEncomenda(IresponseDeserializada, i)});
            }
            ObsColListEncomendas.Add(new JsonRetorno.Encomendas() { Codigo = IresponseDeserializada.Codigo, Descricao = IresponseDeserializada.Descricao, Preço = IresponseDeserializada.Preço, Telefone = IresponseDeserializada.Telefone, Servico = IresponseDeserializada.Servico, Quantidade = IresponseDeserializada.Quantidade, Eventos = IresponseDeserializada.Eventos, Entregue = boolFoiEntregue });

            listviewEncomendasAddGeral.ItemsSource = ObsColListEncomendas;
        }
        private string ImagemStatusEncomenda(JsonRetorno.Encomendas IresponseDeserializada, int x)
        {            
            for (int i = 0; i < IresponseDeserializada.Eventos.Count; i++)
            {
                if (IresponseDeserializada.Eventos[i].Status.ToUpper().Contains("ENTREGUE"))
                {
                    IresponseDeserializada.Eventos[i].Imagem = "entregue.png";
                    boolFoiEntregue = true;
                }
                else if (IresponseDeserializada.Eventos[i].Status.ToUpper().Contains("ENCAMINHADO"))
                {
                    IresponseDeserializada.Eventos[i].Imagem = "encaminhado.png";
                }
                else if (IresponseDeserializada.Eventos[i].Status.ToUpper().Contains("POSTADO"))
                {
                    IresponseDeserializada.Eventos[i].Imagem = "postado.png";
                }
                else if (IresponseDeserializada.Eventos[i].Status.ToUpper().Contains("ADUANEIRA"))
                {
                    IresponseDeserializada.Eventos[i].Imagem = "fiscalizacaoAduaneira.png";
                }
                else if (IresponseDeserializada.Eventos[i].Status.ToUpper().Contains("ENTREGA"))
                {
                    IresponseDeserializada.Eventos[i].Imagem = "saiuParaEntrega.png";
                }
                else
                {
                    IresponseDeserializada.Eventos[i].Imagem = "outros.png";
                }
            }
            return IresponseDeserializada.Eventos[x].Imagem;
        }
        public void MaisInfo(object sender, EventArgs e)
        {
            var itemSelect = ((MenuItem)sender);

            ContentPageEncomendaInfo TelaEncomendaInfo = new ContentPageEncomendaInfo();

            TelaEncomendaInfo.ListEventos = ObsColListEventos;
            TelaEncomendaInfo.ListEncomendas = ObsColListEncomendas;
            TelaEncomendaInfo.RetornandoInfo();

            Navigation.PushModalAsync(TelaEncomendaInfo);
        }

        public void DeletarItem(object sender, EventArgs e)
        {
            var itemSelect = ((MenuItem)sender);

            JsonRetorno.Encomendas item = ((JsonRetorno.Encomendas)itemSelect.CommandParameter);
            ObsColListEncomendas.Remove(item);

            File.Delete("/storage/emulated/0/Android/data/com.companyname.rastreamentoapp/TRACKS/" + item.Codigo + ".json");
        }

        private async void listviewEncomendasAddGeral_Refreshing(object sender, EventArgs e)
        {
            ContentPageAdicionarNovaEncomenda refreshList = new ContentPageAdicionarNovaEncomenda();

            for(int i = 0; i < ObsColListEncomendas.Count; i++)
            {
                await refreshList.GetJsonStringContent(ObsColListEncomendas[i].Codigo);
            }
            await ConsultandoEncomendasInTracks();
            listviewEncomendasAddGeral.EndRefresh();
        }
    }
}
