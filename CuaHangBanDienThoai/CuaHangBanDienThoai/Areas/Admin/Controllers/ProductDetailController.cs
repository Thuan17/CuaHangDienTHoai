using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ProductDetailController : Controller
    {
        // GET: Admin/ProductDetail

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.ProductDetail.OrderByDescending(x => x.ProductDetailId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.ProductDetail.ToList();

                ViewBag.Count = products.Count;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return View();
            }
        }



        [AuthorizeFunction("Quản trị viên")]
        public ActionResult Add()
        {

            if (db.Products.Any())
            {
                var productlist = db.Products.Where(pro => pro.ProductCategory != null).Select(pro => new ListProductId
                {
                    ProductsId = pro.ProductsId,
                    Image = pro.Image,
                    Title = pro.ProductCategory.Title.Trim()+" " + pro.Title.Trim(),
                }).ToList();
                ViewBag.ProductList = productlist;
                return View();
            }

          
                    
            return View();
        }


    }
}