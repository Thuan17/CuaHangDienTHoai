using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_EditImport
    {
        public List<Admin_EditImportItem> Items { get; set; }
        public Admin_EditImport()
        {
            Items = new List<Admin_EditImportItem>();
        }
        public int ImportWarehouseId { get;set; }
        public string CreatedBy { get;set; }
        public DateTime CreatedDate { get;set; }
        public DateTime ModifiedDate { get;set; }
        public string Modifiedby { get;set; }
        public int EmployeeId { get;set; }
        public int SupplierId { get;set; }
        public string Code { get;set; }
        public Supplier supplier { get;set; }       
        public Employee employee { get;set; }       
        public int GetTongSoLuong()
        {
            return Items.Sum(x => x.Quantity);
        }
        public void RemoveItem(int billtDetailId)
        {

            Items.RemoveAll(item => item.ImportWarehouseDetailId == billtDetailId);
        }




    }
    public class Admin_EditImportItem
    {
        public int ImportWarehouseDetailId { get; set; }
    
        public int Quantity { get; set; }
        public int ImportWarehouseId { get; set; }
        public int ProductDetailId { get; set; }
        public ProductDetail productDetail { get; set; }

    }
}