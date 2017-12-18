using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domino.Data
{
    public partial class Block
    {
        [JsonProperty("PreviousHash")]
        public string PreviousHash { get; set; }

        [JsonProperty("Transactions")]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty("BlockHash")]
        public string BlockHash { get; set; }

        public Block()
        {

        }
    }

    public partial class DeserializeBlock
    {
        public static Block FromJson(string json) => JsonConvert.DeserializeObject<Block>(json, BlockConverter.Settings);
    }

    public static class SerializeBlock
    {
        public static string ToJson(this DeserializeBlock self) => JsonConvert.SerializeObject(self, BlockConverter.Settings);
    }

    public class BlockConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
