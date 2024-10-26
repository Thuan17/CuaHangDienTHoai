using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CuaHangBanDienThoai
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
     name: "BillIndex",
     url: "tracuudonhang",
     defaults: new { controller = "Bill", action = "Index", id = UrlParameter.Optional },
     namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
 );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
          name: "Detail",
          url: "baiviet/{alias}",
          defaults: new { controller = "News", action = "Details", id = UrlParameter.Optional },
          namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
      );
            routes.MapRoute(
          name: "ChekOutSuccess",
          url: "muahangthanhcong/{code}",
          defaults: new { controller = "ShoppingCart", action = "CheckOutSucees", id = UrlParameter.Optional },
          namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
      );
            routes.MapRoute(
            name: "CategoryProduct",
            url: "danh-muc-san-pham/{alias}",
            defaults: new { controller = "ProductCategory", action = "Detail", id = UrlParameter.Optional },
            namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
        );
            routes.MapRoute(
               name: "Chitietsanpham",
              url: "chi-tiet/{alias}-p{id}",
               defaults: new { controller = "Product", action = "Details", id = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );
            routes.MapRoute(
                name: "DangKy",
                url: "dang-ky",
                defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional },
                 namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
            );
            routes.MapRoute(
                name: "DangNhap",
                url: "dang-nhap",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                   namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
            );
            routes.MapRoute(
         name: "Giohang",
         url: "gio-hang",
         defaults: new { controller = "ShoppingCart", action = "Index", id = UrlParameter.Optional },
            namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
     );
            routes.MapRoute(
             name: "ThanhToan",
             url: "thanh-toan",
             defaults: new { controller = "ShoppingCart", action = "Checkout", id = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
         );
            routes.MapRoute(
               name: "SanPham",
               url: "san-pham",
               defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );
            routes.MapRoute(
         name: "RedirectHome",
         url: "",
         defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional }
     );


            //// Route Trang chu
            routes.MapRoute(
             name: "indexHome",
             url: "trang-chu",
             defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }

         );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
            );
        }
    }
}
