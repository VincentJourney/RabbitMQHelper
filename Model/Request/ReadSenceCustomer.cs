using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RabbitMQHelper.Model.Request
{
    public class ReadSenceEventRequest
    {
        [JsonProperty("event")]
        public EventInfo Info { get; set; }

        public object error { get; set; }
    }
    public class EventInfo
    {
        public string id { get; set; }
        public string face_id { get; set; }
        public string person_id { get; set; }
        public string device_id { get; set; }
        public string customer_id { get; set; }
        public string device_mac_address { get; set; }
        public string region_id { get; set; }
        public string region_name { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string device_name { get; set; }
        public CustomerInfo customer { get; set; }
    }

    public class CustomerInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string age { get; set; }
        public string person_id { get; set; }
        public string is_vip { get; set; }
        public string status { get; set; }
    }
}
