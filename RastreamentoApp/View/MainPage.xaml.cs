using PCLExt.FileStorage.Folders;
using RastreamentoApp.Model;
using RastreamentoApp.View;
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
            ImagemStatusEncomenda(IresponseDeserializada);
            ObsColListEncomendas.Add(new JsonRetorno.Encomendas() { Codigo = IresponseDeserializada.Codigo, Descricao = IresponseDeserializada.Descricao, Preço = IresponseDeserializada.Preço, Telefone = IresponseDeserializada.Telefone, Servico = IresponseDeserializada.Servico, Quantidade = IresponseDeserializada.Quantidade, Eventos = IresponseDeserializada.Eventos, Entregue = boolFoiEntregue });
            listviewEncomendasAddGeral.ItemsSource = ObsColListEncomendas;
        }
        private string ImagemStatusEncomenda(JsonRetorno.Encomendas IresponseDeserializada)
        {            
            for (int i = 0; i < IresponseDeserializada.Eventos.Count; i++)
            {
                if (IresponseDeserializada.Eventos[i].Status.ToUpper().Contains("ENTREGUE"))
                {
                    IresponseDeserializada.Eventos[i].Imagem = "entregue.png";
                    boolFoiEntregue = true;
                }
                else
                {
                    IresponseDeserializada.Eventos[i].Imagem = "outros.png";
                    boolFoiEntregue = false;
                }
            }
            return IresponseDeserializada.Eventos[0].Imagem;
        }
        public void MaisInfo(object sender, EventArgs e)
        {
            var itemSelect = ((MenuItem)sender);
            JsonRetorno.Encomendas item = (JsonRetorno.Encomendas)itemSelect.CommandParameter;

            int index = ObsColListEncomendas.IndexOf(item);

            ContentPageEncomendaInfo TelaEncomendaInfo = new ContentPageEncomendaInfo();
            TelaEncomendaInfo.ListEncomendas = ObsColListEncomendas[index];
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
                refreshList.entryPropDescriçãoEncomenda = ObsColListEncomendas[0].Descricao;
                refreshList.entryPropTelefone = ObsColListEncomendas[0].Telefone;
                refreshList.entryPropValorEncomenda = ObsColListEncomendas[0].Preço;
                refreshList.AtribuindoValorEntryParaEditarInfo();
                await refreshList.GetJsonStringContent(ObsColListEncomendas[i].Codigo);
            }
            await ConsultandoEncomendasInTracks();
            listviewEncomendasAddGeral.EndRefresh();
        }
    }
}
