using Newtonsoft.Json;
using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using RastreamentoApp.Classes;
using RastreamentoApp.UserInterfaces;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

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
            //entry setado para não precisar digitar ao fazer testes

            if (!string.IsNullOrEmpty(entryCodigoRastreio.Text) && entryCodigoRastreio.Text.Length == 13)
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
        public async void GetJsonStringContent()
        {
            string codigoRastreio = entryCodigoRastreio.Text;
            var client = new RestClient("https://api.linketrack.com/track/json?user=teste&token=1abcd00b2731640e886fb41a8a9671ad1434c599dbaa0a0de9a5aa619f29a83f&codigo=" + codigoRastreio);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                DeserializerJsonStringContent(response, codigoRastreio);
            }
        }
        public void DeserializerJsonStringContent(IRestResponse Iresponse, string codigoRastreio)
        {
            var des = (JsonRetorno.Encomendas)Newtonsoft.Json.JsonConvert.DeserializeObject(Iresponse.Content, typeof(JsonRetorno.Encomendas));
            JsonRetorno.Encomendas encomendasListView = new JsonRetorno.Encomendas();
            JsonRetorno.Evento eventoListView = new JsonRetorno.Evento();

            des.Descricao = entryDescriçãoEncomenda.Text;
            des.Preço = entryValorEncomenda.Text;
            des.Telefone = entryTelefone.Text;

            var contentJson = JsonConvert.SerializeObject(des);
            SalvandoJsonLocalAsync(codigoRastreio, contentJson);
        }
        public async System.Threading.Tasks.Task SalvandoJsonLocalAsync(string codigoRastreio, string Iresponse)
        {
            var localPasta = new LocalRootFolder();
            var pasta = localPasta.CreateFolder("TRACKS", CreationCollisionOption.OpenIfExists);
            var arquivo = pasta.CreateFile(codigoRastreio + ".json", CreationCollisionOption.OpenIfExists);

            if (arquivo.Exists)
            {
                StreamWriter sw = File.CreateText(arquivo.Path);
                await sw.WriteAsync(Iresponse);
                sw.Close();
                
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