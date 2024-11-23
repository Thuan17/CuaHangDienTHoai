using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace CuaHangBanDienThoai.Models
{
    public class Client_EditReview
    {
        public int ReviewId { get; set; }       
        public DateTime CreatedDate { get; set; }   
        public string Content { get;set; }
        public int ProductsId { get; set; }  
        public int CustomerId { get; set; }  
        public int Ratingscore { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }


    }
}