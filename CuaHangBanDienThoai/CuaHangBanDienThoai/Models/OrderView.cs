using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class OrderView
    {
        public string PhoneCustomer { get; set; }
        public string NameCustomer { get; set; }
        public string Note { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public bool delivery { get; set; }
        public int TypePaymentVN { get; set; }

    }
}