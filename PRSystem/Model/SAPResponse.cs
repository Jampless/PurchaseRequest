using Newtonsoft.Json;

namespace PRSystem.Model
{
    public class SAPResponse
    {
    }
    public class ItemsResponse
    {
        [JsonProperty("value")]
        public List<Items>? Value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string? OdataNextLink { get; set; }
    }

    public class SuppliersResponse
    {
        [JsonProperty("value")]
        public List<Suppliers>? Value { get; set; }

        [JsonProperty("odata.nextLink")]
        public string? OdataNextLink { get; set; }
    }
}
