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
     defaults: new { controller = "Cart", action = "Search", id = UrlParameter.Optional },
     namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
 );


            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
          name: "Voucher",
          url: "voucher",
          defaults: new { controller = "Voucher", action = "Index", id = UrlParameter.Optional },
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
          defaults: new { controller = "ShoppingCart", action = "CheckOutSuceess", id = UrlParameter.Optional },
          namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
      );
            routes.MapRoute(
            name: "CategoryProduct",
            url: "loai-san-pham/{alias}",
            defaults: new { controller = "ProductCategory", action = "Detail", id = UrlParameter.Optional },
            namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
        );

            routes.MapRoute(
          name: "ChitietsanphamTheodungluongVaMau",
         url: "san-pham/{alias}-{capacity}-color{color}",
          defaults: new { controller = "ProductDetail", action = "DetailByColor", id = UrlParameter.Optional },
           namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
      );
            routes.MapRoute(
             name: "ChitietsanphamTheodungluong",
            url: "san-pham/{alias}-dl{capacity}",
             defaults: new { controller = "ProductDetail", action = "DetailByOption", id = UrlParameter.Optional },
              namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
         );
            routes.MapRoute(
               name: "Chitietsanpham",
              url: "san-pham/{alias}",
               defaults: new { controller = "ProductDetail", action = "Details", id = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );
            
            
            routes.MapRoute(
               name: "DonHangCuaTui",
              url: "tat-ca-don-hang/{id}",
               defaults: new { controller = "Cart", action = "MyOrder", id = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );

            routes.MapRoute(
               name: "ThongTinCaNhan",
              url: "tai-khoan/{encodedId}-name{name}",
               defaults: new { controller = "Account", action = "Profile", id = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );
            routes.MapRoute(
              name: "KHoipuc",
              url: "khoi-phuc-mat-khau/{id}-{email}",
              defaults: new { controller = "Account", action = "UpadatePass", id = UrlParameter.Optional, email = UrlParameter.Optional },
              namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
          );

            routes.MapRoute(
               name: "Quenmatkhau",
               url: "quen-mat-khau",
               defaults: new { controller = "Account", action = "ForgotPassword", id = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );
            routes.MapRoute(
                name: "DangKy",
                url: "dang-ky",
                defaults: new { controller = "Account", action = "Register", id = UrlParameter.Optional },
                 namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
            );
            routes.MapRoute(
               name: "DangNxuat",
               url: "dang-xuat",
               defaults: new { controller = "Account", action = "Logout", id = UrlParameter.Optional },
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
         defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
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
               url: "cua-hang",
               defaults: new { controller = "ProductDetail", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
           );

           



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


            // Route cho trang lỗi 404
            routes.MapRoute(
                name: "404-PageNotFound",
                url: "khong-tim-thay-trang",
                defaults: new { controller = "Error", action = "NotFound" },
                   namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
            );

            // Route cho trang lỗi 500
            routes.MapRoute(
                name: "500-InternalServerError",
                url: "Loi-he-thong-may-chu",
                defaults: new { controller = "Error", action = "ServerError" },
                   namespaces: new[] { "CuaHangBanDienThoai.Controllers" }
            );
        }
    }
}
