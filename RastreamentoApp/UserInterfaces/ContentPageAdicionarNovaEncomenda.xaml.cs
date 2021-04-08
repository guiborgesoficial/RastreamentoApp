using System;
using Xamarin.Forms;
using RestSharp;
using RastreamentoApp.Classes;
using RestSharp.Serialization.Json;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.IO;
using PCLExt.FileStorage.Folders;
using PCLExt.FileStorage;
using RastreamentoApp.UserInterfaces;

namespace RastreamentoApp.UserInterfaces
{
    public partial class ContentPageAdicionarNovaEncomenda : ContentPage
    {
        public ContentPageAdicionarNovaEncomenda()
        {
            InitializeComponent();
        }
        public string jsonStringContent;

        private void btnAdicionarEncomenda_Clicked(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(entryCodigoRastreio.Text) && entryCodigoRastreio.Text.Length == 13)
            {
                GetJsonStringContent();
            }
            else
            {
                entryCodigoRastreio.Text = "";
                entryCodigoRastreio.Placeholder = "Preencha o código corretamente";
                entryCodigoRastreio.PlaceholderColor = Color.Red;
            }
        }
        public string GetJsonStringContent()
        {
            string codigoRastreio = entryCodigoRastreio.Text;
            var client = new RestClient("https://api.linketrack.com/track/json?user=teste&token=1abcd00b2731640e886fb41a8a9671ad1434c599dbaa0a0de9a5aa619f29a83f&codigo=" + codigoRastreio);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);
            jsonStringContent = response.Content.ToString();

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                DeserializerJsonStringContent(response, codigoRastreio);
            }
            return jsonStringContent;
        }
        public void DeserializerJsonStringContent(IRestResponse Iresponse, string codigoRastreio)
        {
            var des = (JsonRetorno.Encomendas)Newtonsoft.Json.JsonConvert.DeserializeObject(Iresponse.Content, typeof(JsonRetorno.Encomendas));
            JsonRetorno.Encomendas encomendasListView = new JsonRetorno.Encomendas();
            JsonRetorno.Evento eventoListView = new JsonRetorno.Evento();

            SalvandoJsonLocalAsync(codigoRastreio, Iresponse);

            encomendasListView.Codigo = des.Codigo;
            encomendasListView.Servico = des.Servico;
            encomendasListView.Quantidade = des.Quantidade;

            foreach (var infoEvento in des.Eventos)
            {
                eventoListView.Data = infoEvento.Data[0].ToString();
                eventoListView.Hora = infoEvento.Hora[0].ToString();
                eventoListView.Local = infoEvento.Local[0].ToString();
                eventoListView.Status = infoEvento.Status[0].ToString();
                eventoListView.SubStatus = infoEvento.SubStatus;
            }
        }
        /*public string SerializerJsonStringContent(IRestResponse Iresponse)
        {
            var dadosSerializados = new RestSharp.Serialization.Json.JsonSerializer().Serialize(Iresponse.Content);
            Iresponse.Content = dadosSerializados;
            return Iresponse.Content;
        }*/
        public async System.Threading.Tasks.Task SalvandoJsonLocalAsync(string codigoRastreio, IRestResponse Iresponse)
        {
            var localPasta = new LocalRootFolder();
            var pasta = localPasta.CreateFolder("TRACKS", CreationCollisionOption.OpenIfExists);
            var arquivo = pasta.CreateFile(codigoRastreio + ".json", CreationCollisionOption.OpenIfExists);

            if (arquivo.Exists)
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(arquivo.Path))
                {
                    sw.WriteLine("'Descrição do Produto': " + "'" + entryDescriçãoEncomenda.Text + "'" + ",");
                    sw.WriteLine("'Preço': " + "'" + entryValorEncomenda.Text + "'");
                    sw.Write(Iresponse.Content);
                }
                DisplayAlert("Concluído", "Sua encomenda foi adicionada", "OK");
                this.Navigation.PopModalAsync();
            }
        }
        private void checkboxValor_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(checkboxValor.IsChecked)
            {
                entryValorEncomenda.IsVisible = true;
            }
            else
            {
                entryValorEncomenda.IsVisible = false;
            }
        }

        private void entryCodigoRastreio_Focused(object sender, FocusEventArgs e)
        {
            entryCodigoRastreio.PlaceholderColor = Color.Gray;
            entryCodigoRastreio.Placeholder = "LB0123456789HK";
        }
    }
}