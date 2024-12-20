﻿using System.Web.Mvc;

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
name: "Account_UpdatePass",
url: "cap-nhap-mat-khau/{encodedId}-name{name}",
defaults: new { controller = "Account", action = "UpdatePass", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "Account_Profile",
  url: "thong-tin-ca-nhan/{encodedId}-name{name}",
defaults: new { controller = "Account", action = "Profile", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "ImportWareHouse_Detail",
url: "chi-tiet-phieunhap/{code}",
defaults: new { controller = "ImportWarehouse", action = "Detail", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "ImportWareHouse_Import",
url: "lap-phieunhap",
defaults: new { controller = "ImportWarehouse", action = "Import", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "ImportWareHouse_Index",
url: "danh-sach-phieunhap",
defaults: new { controller = "ImportWarehouse", action = "Index", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "Supplier_Add",
url: "tao-moi-nhacungcap",
defaults: new { controller = "Supplier", action = "Add", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "Supplier_Index",
url: "danh-sach-nhacungcap",
defaults: new { controller = "Supplier", action = "Index", alias = UrlParameter.Optional }

);
            context.MapRoute(
name: "Seller_ALL",
url: "tat-ca-hoa-don",
defaults: new { controller = "Seller", action = "AllBill", alias = UrlParameter.Optional }

);

            context.MapRoute(
name: "Seller_index",
url: "ban-hang",
defaults: new { controller = "Seller", action = "Index", alias = UrlParameter.Optional }

);


            context.MapRoute(
name: "Emp_Add",
url: "tao-ho-so-nhan-vien",
defaults: new { controller = "Employee", action = "Add", alias = UrlParameter.Optional }

);

            context.MapRoute(
name: "Emp_Index",
url: "quan-ly-nhan-vien",
defaults: new { controller = "Employee", action = "Index", alias = UrlParameter.Optional }

);


            context.MapRoute(
 name: "statistical_Index",
 url: "thongkedoanhthu",
 defaults: new { controller = "Statistical", action = "Index", alias = UrlParameter.Optional }

);

            context.MapRoute(
 name: "Customer_Index",
 url: "quan-ly-tai-khoan-khach-hang",
 defaults: new { controller = "Customer", action = "Index", alias = UrlParameter.Optional }

);

            context.MapRoute(
  name: "Voucher_Add",
  url: "them-moi-voucher",
  defaults: new { controller = "Voucher", action = "Add", alias = UrlParameter.Optional }

);
            context.MapRoute(
    name: "Voucher_Index",
    url: "quan-ly-voucher",
    defaults: new { controller = "Voucher", action = "Index", alias = UrlParameter.Optional }

);


            context.MapRoute(
      name: "BillNonIsConfirm_Order",
      url: "don-chua-xu-ly",
      defaults: new { controller = "Order", action = "OrderNonIsConfirm", alias = UrlParameter.Optional }

  );

            context.MapRoute(
             name: "Index_Order",
             url: "quan-ly-don-hang",
             defaults: new { controller = "Order", action = "Index", alias = UrlParameter.Optional }

         );


            context.MapRoute(
       name: "Add_Banner",
       url: "them-moi-banner",
       defaults: new { controller = "Banner", action = "Add", alias = UrlParameter.Optional },
        namespaces: new[] { "CuaHangBanDienThoai.Areas.Admin.Controllers" }

   );


            context.MapRoute(
                name: "Index_Banner",
                url: "quanly-banner",
                defaults: new { controller = "Banner", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "CuaHangBanDienThoai.Areas.Admin.Controllers" }

            );


            context.MapRoute(
                name: "Detail_ProductDetail",
                url: "chitiet-san-pham-con/{alias}",
                defaults: new { controller = "ProductDetail", action = "Detail" }

                );
            context.MapRoute(
                name: "Edit_ProductDetail",
                url: "chinhsua-san-pham-con/{alias}",
                defaults: new { controller = "ProductDetail", action = "Edit" }

                );

            context.MapRoute(
  name: "Add_ForProduct_ProductDetail",
  url: "them-san-pham-con/ma-sp{productid}",
  defaults: new { controller = "ProductDetail", action = "Add" }

);
            context.MapRoute(
     name: "Add_ProductDetail",
     url: "them-san-pham-con",
     defaults: new { controller = "ProductDetail", action = "Add" }

 );

            context.MapRoute(
     name: "Index_ProductDetail",
     url: "san-pham-con",
     defaults: new { controller = "ProductDetail", action = "Index" }

 );
            context.MapRoute(
     name: "Edit_Products",
     url: "sua-sanpham/{alias}",
     defaults: new { controller = "Product", action = "Edit" }

 );
            context.MapRoute(
     name: "Details_Products",
     url: "chi-tiet/{alias}",
     defaults: new { controller = "Product", action = "Detail" }

 );
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
     name: "Detail_ProductCategory",
     url: "danhmuc/{alias}",
     defaults: new { controller = "ProductCategory", action = "Detail" }

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
     name: "Index_Home_ỉnexc",
     url: "he-thong-quan-ly",
     defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
      namespaces: new[] { "CuaHangBanDienThoai.Areas.Admin.Controllers" }

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