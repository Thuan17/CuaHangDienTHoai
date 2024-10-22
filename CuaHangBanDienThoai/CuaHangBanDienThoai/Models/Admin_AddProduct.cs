using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Models
{
    public class Admin_AddProduct
    {
        public int ProductCategoryId { get; set; }
        public int ProductCompanyId { get; set; }
        public string Title { get;set; }
        public string Screensize { get;set; }
        public string CPUspeed { get;set; }
        public string OperatingSystem { get;set; }
        public string GPUspeed { get;set; }
        public string MobileNetwork { get;set; }
        public string Sim { get;set; }
        public string Wifi { get;set; }
       
        public string Bluetooth { get;set; }
        public string Connector { get;set; }
      
        public string BatteryType { get;set; }
        public string ChargingSupport { get;set; }
        public string BatteryTechnology { get;set; }
        public string CPU { get;set; }
        public string GPU { get;set; }
        public string BatteryCapacity { get;set; }
        [AllowHtml]
        public string Description { get;set; }
     
    }
}