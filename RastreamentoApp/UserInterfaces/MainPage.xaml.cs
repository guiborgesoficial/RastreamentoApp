using PCLExt.FileStorage.Folders;
using RastreamentoApp.Classes;
using RastreamentoApp.UserInterfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;

namespace RastreamentoApp
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<JsonRetorno.Encomendas> ObsColListEncomendas = new ObservableCollection<JsonRetorno.Encomendas>();
        private ObservableCollection<JsonRetorno.Evento> ObsColListEventos = new ObservableCollection<JsonRetorno.Evento>();
        public MainPage()
        {
            InitializeComponent();
            ConsultandoEncomendasInTracks();
        }

        public void btnAdicionarEncomenda_Clicked(object sender, EventArgs e)
        {
            ContentPageAdicionarNovaEncomenda TelaAdicionarNovaEncomenda = new ContentPageAdicionarNovaEncomenda();
            Navigation.PushModalAsync(TelaAdicionarNovaEncomenda);

            ObsColListEncomendas.Clear();
            ConsultandoEncomendasInTracks();
        }

        private void ConsultandoEncomendasInTracks()
        {
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
            ObsColListEncomendas.Add(new JsonRetorno.Encomendas() { Codigo = IresponseDeserializada.Codigo, Descricao = IresponseDeserializada.Descricao, Preço = IresponseDeserializada.Preço, Telefone = IresponseDeserializada.Telefone, Servico = IresponseDeserializada.Servico, Quantidade = IresponseDeserializada.Quantidade, Eventos = IresponseDeserializada.Eventos});
            listviewEncomendasAddGeral.ItemsSource = ObsColListEncomendas;

            for(int i = 0; i < IresponseDeserializada.Eventos.Count; i++)
            {
                ObsColListEventos.Add(new JsonRetorno.Evento { Data = IresponseDeserializada.Eventos[i].Data, Hora = IresponseDeserializada.Eventos[i].Hora, Local = IresponseDeserializada.Eventos[i].Local, Status = IresponseDeserializada.Eventos[i].Status, SubStatus = IresponseDeserializada.Eventos[i].SubStatus });
            }
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
    }
}
