//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CuaHangBanDienThoai.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductDetail()
        {
            this.BillDetail = new HashSet<BillDetail>();
            this.CartItem = new HashSet<CartItem>();
            this.ImportWarehouseDetail = new HashSet<ImportWarehouseDetail>();
            this.InvoiceDetail = new HashSet<InvoiceDetail>();
            this.ProductDetailImage = new HashSet<ProductDetailImage>();
        }
    
        public int ProductDetailId { get; set; }
        public string Color { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PriceSale { get; set; }
        public Nullable<decimal> OriginalPrice { get; set; }
        public string Ram { get; set; }
        public string Capacity { get; set; }
        public Nullable<int> ProductsId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsHome { get; set; }
        public Nullable<bool> IsSale { get; set; }
        public Nullable<bool> IsHot { get; set; }
        public string Alias { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillDetail> BillDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartItem> CartItem { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImportWarehouseDetail> ImportWarehouseDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetail { get; set; }
        public virtual Products Products { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductDetailImage> ProductDetailImage { get; set; }
    }
}
