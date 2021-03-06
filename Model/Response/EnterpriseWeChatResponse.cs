﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Model.Response
{
    public class GetTokenResponse
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class SendMessageResponse
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
