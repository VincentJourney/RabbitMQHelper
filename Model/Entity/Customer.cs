using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Model.Entity
{
    public class Customer
    {
        public Guid CustomerID { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
    }
}
