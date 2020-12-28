namespace MasterAppStore.Models
{
    using Newtonsoft.Json;
    using MasterAppStore.Models.Enums;
    using System.Collections.Generic;

    public class Product
    {
       [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName ="category")]
        public Category Category { get; set; }

        [JsonProperty(PropertyName ="description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName ="quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName ="color")]
        public Color Color { get; set; }

        [JsonProperty(PropertyName ="size")]
        public Size Size { get; set; }

        [JsonProperty(PropertyName ="price")]
        public double Price { get; set; }

        [JsonProperty(PropertyName ="images")]
        public List<string>? Images { get; set; }

    }
}
