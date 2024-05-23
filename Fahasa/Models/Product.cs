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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Discount = 0;
            this.Carts = new HashSet<Cart>();
            this.Galleries = new HashSet<Gallery>();
            this.ImportProducts = new HashSet<ImportProduct>();
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Discounts = new HashSet<Discount>();
            this.Categories = new HashSet<Category>();
            this.Comments = new HashSet<Comment>();
        }
    
        public int Id { get; set; }
        public string ImageSrc { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public Nullable<bool> SoonRelease { get; set; }
        public string StockAvailable { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<int> AuthorId { get; set; }
        public Nullable<int> CoverId { get; set; }
        public string Desc { get; set; }
        public string Table { get; set; }
        public Nullable<int> Amount { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<int> PublisherId { get; set; }
        public string Brand { get; set; }
        public string Origin { get; set; }
        public Nullable<bool> IsGuarantee { get; set; }
    
        public virtual Author Author { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual Cover Cover { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gallery> Galleries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImportProduct> ImportProducts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual Supplier Supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Discount> Discounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
