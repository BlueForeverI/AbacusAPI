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
    
    public partial class Role
    {
        public Role()
        {
            this.ContactRoles = new HashSet<ContactRole>();
            this.Contacts = new HashSet<Contact>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<ContactRole> ContactRoles { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
