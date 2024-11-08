using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class UpdatePass
    {

        public int CustomerId { get; set; } 
        public string CustomerName { get; set; }    
        public string Password { get; set; }    
        public string Code { get; set; }    

    }
}