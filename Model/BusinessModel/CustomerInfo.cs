using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Model.BusinessModel
{
    public class CRMMessageInfo
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string RegionName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
