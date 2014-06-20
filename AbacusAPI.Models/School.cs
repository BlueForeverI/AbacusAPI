using System;
using System.Runtime.Serialization;

namespace AbacusAPI.Models
{
    [DataContract]
    public class School
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public long Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Suburb { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string Town { get; set; }
    }
}
