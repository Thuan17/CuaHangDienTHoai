using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
     name: "Add_Products",
     url: "them-san-pham",
     defaults: new { controller = "Product", action = "Add" }

 );

            context.MapRoute(
      name: "Index_Products",
      url: "san-pham",
      defaults: new { controller = "Product", action = "Index" }

  );
            context.MapRoute(
         name: "Edit_ProductCategory",
         url: "sua-danhmuc-sanpham/{alias}",
         defaults: new { controller = "ProductCategory", action = "Edit" }

     );
            context.MapRoute(
name: "Add_ProductCategory",
url: "them-danh-muc-san-pham",
defaults: new { controller = "ProductCategory", action = "Add" }

);
            context.MapRoute(
   name: "Index_ProductCategory",
   url: "danh-muc-san-pham",
   defaults: new { controller = "ProductCategory", action = "Index" }

);

            context.MapRoute(
          name: "Edit_ProductCompany",
          url: "chinh-sua-hang/{alias}",
          defaults: new { controller = "ProductCompany", action = "Edit" }

      );
            context.MapRoute(
            name: "Detail_ProductCompany",
            url: "chitiethang/{alias}",
            defaults: new { controller = "ProductCompany", action = "Detail" }

        );

            context.MapRoute(
            name: "Add_ProductCompany",
            url: "them-hang-san-pham",
            defaults: new { controller = "ProductCompany", action = "Add" }

        );

            context.MapRoute(
             name: "Index_ProductCompany",
             url: "hang-san-pham",
             defaults: new { controller = "ProductCompany", action = "Index" }
               
         );



            context.MapRoute(
             name: "Login_Account",
             url: "hethongnhanvien",
             defaults: new { controller = "Account", action = "Login" }
         );
            context.MapRoute(
              name: "Logout_Account",
              url: "dangxuat",
              defaults: new { controller = "Account", action = "Logout", alias = UrlParameter.Optional }

          );
            context.MapRoute(
         name: "Index_Home",
         url: "hethongquanly",
         defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
          namespaces: new[] { "CuaHangBanDienThoai.Areas.Admin.Controllers" }

     );


            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",

                  defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            namespaces: new[] { "CuaHangBanDienThoai.Areas.Admin.Controllers" }
            );
        }
    }
}