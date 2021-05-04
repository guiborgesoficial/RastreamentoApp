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
        private ObservableCollection<JsonRetorno.Encomendas> encomendas = new ObservableCollection<JsonRetorno.Encomendas>();
        private ObservableCollection<JsonRetorno.Evento> eventos = new ObservableCollection<JsonRetorno.Evento>();


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
            encomendas.Add(new JsonRetorno.Encomendas() { Codigo = IresponseDeserializada.Codigo, Descricao = IresponseDeserializada.Descricao, Preço = IresponseDeserializada.Preço, Telefone = IresponseDeserializada.Telefone });
            listviewEncomendasAddGeral.ItemsSource = encomendas;

            /*foreach (var infoEvento in IresponseDeserializada.Eventos)
            {
                eventoListView.Data = infoEvento.Data[0].ToString();
                eventoListView.Hora = infoEvento.Hora[0].ToString();
                eventoListView.Local = infoEvento.Local[0].ToString();
                eventoListView.Status = infoEvento.Status[0].ToString();
                eventoListView.SubStatus = infoEvento.SubStatus;
            }*/
        }
    }
}
