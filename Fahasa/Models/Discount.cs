//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fahasa.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Discount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Discount()
        {
            this.Used = 0;
            this.Orders = new HashSet<Order>();
            this.Products = new HashSet<Product>();
        }
    
        public string Code { get; set; }
        public string Title { get; set; }
        public long Value { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> DateExpired { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public string ApplyField { get; set; }
        public string Content { get; set; }
        public Nullable<int> Used { get; set; }
        public int ConditionalPrice { get; set; }
        public string ConditionalOperator { get; set; }
        public Nullable<int> LimitUsed { get; set; }
        public string SubTitle { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
