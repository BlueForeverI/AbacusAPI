using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbacusAPI.Models
{
    [DataContract]
    public class OrderChangeLog
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public long ImageId { get; set; }

        [DataMember]
        public string OrderType { get; set; }

        [DataMember]
        public int OldQuantity { get; set; }

        [DataMember]
        public int NewQuantity { get; set; }

        [DataMember]
        public long OldSchoolCode { get; set; }

        [DataMember]
        public long NewSchoolCode { get; set; }

        [DataMember]
        public string OldRoom { get; set; }

        [DataMember]
        public string NewRoom { get; set; }
    }
}
