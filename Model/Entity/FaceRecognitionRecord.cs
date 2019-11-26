using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Model.Entity
{
    [Table("FaceRecognitionRecord")]
    public class FaceRecognitionRecord
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string FaceId { get; set; }
        public Guid CustomerId { get; set; }
        public string RegionName { get; set; }
        public string DeviceName { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
