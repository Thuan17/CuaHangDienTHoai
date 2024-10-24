using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_AddProductDetail
    {
        public string Color { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public decimal OriginalPrice { get; set; }

        public string Ram { get; set; }
        public string Capacity { get; set; }
        public int ProductsId { get; set; }
        public int Quantity { get; set; }
        //public string IsActive  { get; set; }
        //public string IsHome { get; set; }
        //public string IsSale { get; set; }
        //public string IsHot { get; set; }
   
        //public List<ImageItem> Items { get; set; }
    }
    //public class ImageItem
    //{
    //    public string Image { get; set; }
    //    public bool IsDefault { get; set; }
    //}
}