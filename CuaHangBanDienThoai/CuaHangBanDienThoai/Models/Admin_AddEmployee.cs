using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_AddEmployee
    {
        public string PhoneNumber { get;set; }
        public string NameEmployee { get;set; }
        public string CitizenIdentificationCard { get;set; }
        public string Email { get;set; }
        public DateTime Birthday { get;set; }
        public string Location { get;set; }
        public decimal Wage { get;set; }
        public string Sex { get;set; }
        public string Image { get;set; }
        public int FunctionId { get;set; }

    }
}