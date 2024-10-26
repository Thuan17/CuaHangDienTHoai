using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuTop()
        {
            var Product = db.ProductDetail.Where(x => x.IsActive == true && x.IsHome == true ).ToList();
            if (Product.Count > 0)
            {


                return PartialView(Product);
            }
            return PartialView();
        }


        public ActionResult MenuArrivals()
        {
            var proCateActive = db.ProductCompany.OrderByDescending(x => x.Alias.Trim()).Take(4).ToList();
            if (proCateActive.Count > 0)
            {
                return PartialView(proCateActive);
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult MenuCategory()
        {
            var proCateActive = db.ProductCategory.Where(x => x.IsActive == true).OrderByDescending(x => x.Title.Trim()).Take(6).ToList();
            if (proCateActive.Count > 0)
            {
                return PartialView(proCateActive);
            }
            else
            {
                return PartialView();
            }
        }

        public ActionResult MenuLeft(string alias)
        {
            if (alias != null)
            {
                ViewBag.Alias = alias;
            }
            var productCategory = db.ProductCategory.Where(x => x.IsActive == true).OrderByDescending(x => x.Title.Trim()).ToList();
            if (productCategory != null)
            {
                return PartialView(productCategory);

            }
            return PartialView();
        }

    }
}