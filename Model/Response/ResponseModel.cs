using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RabbitMQHelper.Model.Response
{
    public class ResponseModel<T>
    {
        [JsonProperty("Result")]
        public Result Result { get; set; }
        [JsonProperty("Data")]
        public T Data { get; set; }
    }

    public class Result
    {
        [JsonProperty("HasError")]
        public bool HasError { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}
