using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Client_UpdateProFile
    {
        public int CustomerId { get; set; } 
        public string CustomerName { get; set; }    
        public string Email{get; set; } 
        public string PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }



    }
}