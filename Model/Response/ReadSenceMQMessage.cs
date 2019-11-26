using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Model.Response
{
    public class ReadSenceMQMessage
    {
        public string routing_key { get; set; }
        public string msg_type { get; set; }
        public MessageInfo data { get; set; }
        public object extra_data { get; set; }
    }

    public class MessageInfo
    {
        public string mac_address { get; set; }
        public string device_uuid { get; set; }
        public string person_uuid { get; set; }
        public string image_url { get; set; }
        public string device_name { get; set; }
        public string device_location { get; set; }
    }
}
