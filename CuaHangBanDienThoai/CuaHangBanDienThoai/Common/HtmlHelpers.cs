using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace CuaHangBanDienThoai.Common
{
    public static class HtmlHelpers
    {
            public static string EncodeCustomerId(this HtmlHelper htmlHelper, int customerId)
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(customerId.ToString());
                return System.Convert.ToBase64String(plainTextBytes);
            }
    }
}