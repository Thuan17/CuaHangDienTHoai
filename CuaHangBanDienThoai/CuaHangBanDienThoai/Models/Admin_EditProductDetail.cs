using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_EditProductDetail
    {
        public List<ImageItemProduct> Items { get; set; }
        public int ProductDetailId { get; set; }        
        public string Color { get; set; }        
        public string ImageProduct { get; set; }        
        public decimal Price { get; set; }        
        public decimal PriceSale { get; set; }        
        public decimal OriginalPrice { get; set; }        
        public string Capacity { get; set; }        
        public string  Ram { get; set; }        
        public int ProductsId { get; set; }        
        public int Quantity { get; set; }        
        public string Alias { get; set; }        
        public bool IsActive { get; set; }        
        public bool IsHome { get; set; }        
        public bool IsSale { get; set; }
        public Products Products { get; set; }
        public Admin_EditProductDetail()
        {
            Items = new List<ImageItemProduct>();
        }
       
    }
    public class ImageItemProduct
    {
        public int ProductImageId { get; set; }
        public string Image { get; set; }
        public bool IsDefault { get; set; }
    }
}