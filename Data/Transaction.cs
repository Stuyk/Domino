using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domino.Data
{
    public partial class Transaction
    {
        [JsonProperty("Value")]
        public long Value { get; set; }

        [JsonProperty("TargetAddress")]
        public string TargetAddress { get; set; }

        [JsonProperty("FromAddress")]
        public string FromAddress { get; set; }

        public Transaction(long value = -1, string fromAddress = "", string targetAddress = "")
        {
            if (value != -1)
                Value = value;

            if (fromAddress != "")
                FromAddress = fromAddress;

            if (targetAddress != "")
                TargetAddress = targetAddress;
        }
    }

    public partial class DeserializeTransaction
    {
        public static Transaction FromJson(string json) => JsonConvert.DeserializeObject<Transaction>(json, TransactionConverter.Settings);
    }

    public static class SerializeTransaction
    {
        public static string ToJson(this DeserializeTransaction self) => JsonConvert.SerializeObject(self, TransactionConverter.Settings);
    }

    public class TransactionConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
