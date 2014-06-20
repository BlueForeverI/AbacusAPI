using System;
using System.Runtime.Serialization;

namespace AbacusAPI.Models
{
    [DataContract]
    public class OrderInfo
    {
        [DataMember]
        public long ImageId { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string School { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string KidsName { get; set; }

        [DataMember]
        public string Room { get; set; }

        [DataMember]
        public decimal Calcs { get; set; }

        [DataMember]
        public decimal Cards { get; set; }

        [DataMember]
        public decimal Diary { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string PrintedId { get; set; }

        [DataMember]
        public string ReOrder { get; set; }

        [DataMember]
        public string BatchBy { get; set; }

        [DataMember]
        public string Batch { get; set; }

        [DataMember]
        public string AuthCode { get; set; }

        [DataMember]
        public DateTime? BatchDate { get; set; }

        [DataMember]
        public string CustomerOrderStatus { get; set; }

        [DataMember]
        public bool Vertical { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string Log { get; set; }

        [DataMember]
        public string DiaryCrop { get; set; }

        [DataMember]
        public DateTime? InvoicedDate { get; set; }
    }
}
