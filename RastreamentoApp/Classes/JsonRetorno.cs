using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RastreamentoApp.Classes
{
    public class JsonRetorno
    {
        public partial class Encomendas
        {
            [JsonProperty("Descrição do Produto")]
            public string Descricao { get; set; }
            [JsonProperty("Preço")]
            public string Preço { get; set; }
            [JsonProperty("Telefone")]
            public string Telefone { get; set; }
            [JsonProperty("codigo")]
            public string Codigo { get; set; }

            [JsonProperty("servico")]
            public string Servico { get; set; }

            [JsonProperty("quantidade")]
            public int Quantidade { get; set; }

            [JsonProperty("eventos")]
            public List<Evento> Eventos { get; set; }
            [JsonProperty("Entregue")]
            public bool Entregue { get; set; }
        }
        public partial class Evento
        {
            [JsonProperty("data")]
            public string Data { get; set; }

            [JsonProperty("hora")]
            public string Hora { get; set; }

            [JsonProperty("local")]
            public string Local { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("subStatus")]
            public List<string> SubStatus { get; set; }
            [JsonProperty("Imagem")]
            public string Imagem { get; set; }
        }
    }
}
