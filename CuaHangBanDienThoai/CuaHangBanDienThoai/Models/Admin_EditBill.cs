using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_EditBill
    {
        public List<Admin_EditBillItem> Items { get; set; }
        public int BillId { get; set; }
        public string Code { get; set; }
        public decimal TotalAmount { get; set; }
        public string CreatedBy { get; set; }
        public string Modifiedby { get; set; }
        public DateTime CreatedDate { get; set; }   
        public DateTime ModifiedDate { get; set;}
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public Customer customer { get; set; }


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

            Items.RemoveAll(item => item.BillDetailId == billtDetailId);
        }

    }
    public class Admin_EditBillItem
    {
        public int BillDetailId { get; set; }

        public int Quantity { get; set; }
        public int BillId { get; set; }
        public int ProductDetailId { get; set; }
        public decimal Price { get; set; }
        public ProductDetail productDetail { get; set; }
    }

}