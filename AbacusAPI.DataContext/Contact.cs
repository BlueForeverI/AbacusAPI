//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AbacusAPI.DataContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class Contact
    {
        public Contact()
        {
            this.Addresses = new HashSet<Address>();
            this.Conversations = new HashSet<Conversation>();
            this.MarketingJobs = new HashSet<MarketingJob>();
        }
    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> ContactTitleId { get; set; }
        public string PhoneDay { get; set; }
        public string PhoneAfterHours { get; set; }
        public string PhoneCell { get; set; }
        public string EmailWork { get; set; }
        public bool IsPrimaryEmailWork { get; set; }
        public string EmailPersonal { get; set; }
        public bool IsPrimaryEmailPersonal { get; set; }
        public Nullable<int> OrganisationId { get; set; }
        public Nullable<int> RoleId { get; set; }
    
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ContactTitle ContactTitle { get; set; }
        public virtual Organisation Organisation { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<MarketingJob> MarketingJobs { get; set; }
    }
}