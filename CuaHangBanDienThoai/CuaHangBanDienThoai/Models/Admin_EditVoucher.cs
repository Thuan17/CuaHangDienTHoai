using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_EditVoucher
    {
        public List<Admin_EditVoucherDetail> Items { get; set; }

        public int VoucherId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Modifiedby { get; set; }
        public int PercentPriceReduction { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Admin_EditVoucher()
        {
            Items = new List<Admin_EditVoucherDetail>();
        }

    }
    public class Admin_EditVoucherDetail
    {
        public int VoucherDetailId { get; set; }
        public DateTime? UsedDate { get; set; } 
        public int? BillId { get; set; }     
        public int? OrderId { get; set; }
        public int VoucherId { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }
        public VoucherDetail voucherDetail { get; set; }
        public OrderCustomer orderCustomer { get; set; }
        public Bill bill { get; set; }

    }
}