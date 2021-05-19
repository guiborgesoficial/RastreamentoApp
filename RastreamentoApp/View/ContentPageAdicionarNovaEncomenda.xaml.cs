using Newtonsoft.Json;
using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using RastreamentoApp.Model;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RastreamentoApp.View
{
    public partial class ContentPageAdicionarNovaEncomenda : ContentPage
    {
        public string entryPropCodigoRastreio { get; set; }
        public string entryPropDescriçãoEncomenda { get; set; }
        public string entryPropTelefone { get; set; }
        public string entryPropValorEncomenda { get; set; }
        public ContentPageAdicionarNovaEncomenda()
        {
            InitializeComponent();
        }

        private void btnAdicionarEncomenda_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(entryCodigoRastreio.Text) && entryCodigoRastreio.Text.Length == 13)
            {
                if(!string.IsNullOrEmpty(entryTelefone.Text) && entryTelefone.Text.Length == 13)
                {
                    GetJsonStringContent(entryCodigoRastreio.Text);
                }
                else
                {
                    entryTelefone.Text = string.Empty;
                    entryTelefone.Placeholder = "Preencha o telefone corretamente";
                    entryTelefone.PlaceholderColor = Color.Red;
                }
            }
            else
            {
                entryCodigoRastreio.Text = string.Empty;
                entryCodigoRastreio.Placeholder = "Preencha o código corretamente";
                entryCodigoRastreio.PlaceholderColor = Color.Red;
            }
        }
        public async Task GetJsonStringContent(string codigoRastreio)
        {
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
            des.Descricao = entryDescriçãoEncomenda.Text;
            des.Preço = entryValorEncomenda.Text;
            des.Telefone = entryTelefone.Text;
            des.Eventos.RemoveAt(des.Eventos.Count - 1);

            var contentJson = JsonConvert.SerializeObject(des);
            SalvandoJsonLocalAsync(codigoRastreio, contentJson);
        }
        public async System.Threading.Tasks.Task SalvandoJsonLocalAsync(string codigoRastreio, string Iresponse)
        {
            var localPasta = new LocalRootFolder();
            var pasta = localPasta.CreateFolder("TRACKS", CreationCollisionOption.OpenIfExists);
            var arquivo = pasta.CreateFile(codigoRastreio + ".json", CreationCollisionOption.ReplaceExisting);

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
        public void AtribuindoValorEntryParaEditarInfo()
        {
            entryCodigoRastreio.IsEnabled = false;
            entryCodigoRastreio.Text = entryPropCodigoRastreio;
            entryDescriçãoEncomenda.Text = entryPropDescriçãoEncomenda;
            entryTelefone.Text = entryPropTelefone;
            entryValorEncomenda.Text = entryPropValorEncomenda;
        }
    }
}