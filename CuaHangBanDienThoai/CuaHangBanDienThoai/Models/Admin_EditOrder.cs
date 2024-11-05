using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_EditOrder
    {
        public List<Admin_EditOrderItem> Items { get; set; }
        public int OrderId { get;set; }
        public string  Code { get;set; }
        public decimal TotalAmount { get;set; }
        public string CreatedBy { get;set; }
        public DateTime CreatedDate { get;set; }
        public DateTime ModifiedDate { get;set; }
        public string Modifiedby { get;set; }
        public int TypePayment { get;set; }
        public bool Confirm { get;set; }
        public bool Success { get;set; }
        public DateTime SuccessDate { get;set; }
        public string Note { get;set; }
        public string Location { get;set; }
        public int CustomerId { get;set; }
        public string CustomerName { get;set; }
        public string Phone { get;set; }
        public string Email { get;set; }
        public bool IsDelivery { get;set; }
        public Admin_EditOrder()
        {
            Items = new List<Admin_EditOrderItem>();
        }




        public decimal GetPriceTotal()
        {
            return Items.Sum(item =>
            {
                if (item.ProductDetailId > 0)
                {
                    return (decimal)item.Price * item.Quantity;
                }
                else
                {
                    return (decimal)item.Price * item.Quantity;
                }
            });
        }




        public int GetTongSoLuong()
        {
            return Items.Sum(x => x.Quantity);
        }
        public void RemoveItem(int billtDetailId)
        {

            Items.RemoveAll(item => item.OrderDetailId == billtDetailId);
        }







    }
    public class Admin_EditOrderItem
    {
        public int OrderDetailId { get; set; }
        public decimal Price { get; set; }  
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail productDetail { get; set; }    
       
    }
}