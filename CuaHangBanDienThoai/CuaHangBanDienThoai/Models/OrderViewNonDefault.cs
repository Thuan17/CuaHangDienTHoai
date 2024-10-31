using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class OrderViewNonDefault
    {

        public int AddressCusomerId { get;set; }
        public int CustomerId { get;set; }
        public string Location { get;set; }
        public string CustomerName { get;set; }
        public string PhoneNumber { get;set; }
    }
}