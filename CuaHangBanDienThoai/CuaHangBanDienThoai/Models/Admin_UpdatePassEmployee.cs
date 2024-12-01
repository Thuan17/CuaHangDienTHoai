using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_UpdatePassEmployee
    {
        public int EmployeeId { get;set; }
        public string Code { get;set; } 
        public string Password { get;set; } 
        public string PasswordNew { get;set; } 
        public bool IsLock { get;set; } 
        public Employee employee { get;set; }   

    }
}