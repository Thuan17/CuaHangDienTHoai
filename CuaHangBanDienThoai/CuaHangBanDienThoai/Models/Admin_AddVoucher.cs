using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_AddVoucher
    {
        public int PercentPriceReduction { get;set; }
        public string Title { get;set; }
        public int Quantity { get;set; }
        public DateTime StartDate { get;set; }
        public DateTime EndDate { get;set; }
  
    }
}