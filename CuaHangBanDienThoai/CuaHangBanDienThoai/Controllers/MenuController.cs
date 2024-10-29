using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.ViewModels;
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
        public ActionResult Banner()
        {
            var banners = db.Banner.Where(x => x.IsActive == true && x.IsBackground == false).ToList();

            var bannerPairs = new List<BannerPairViewModel>();

            for (int i = 0; i < banners.Count; i += 2)
            {
                var pair = new BannerPairViewModel
                {
                    LeftBanner = banners[i],
                    RightBanner = (i + 1 < banners.Count) ? banners[i + 1] : null
                };
                bannerPairs.Add(pair);
            }

            return PartialView("_BannerPartial", bannerPairs);
        }
        public ActionResult BigBanner()
        {
            var banners = db.Banner.FirstOrDefault(x => x.IsActive == true && x.IsBackground == true);
            if (banners != null)
            {
                return PartialView("_BigBannerPartial", banners);
            }
            return PartialView("_BigBannerPartial");

        }
        public ActionResult BannerMobile()
        {
            var banners = db.Banner.Where(x => x.IsActive == true).ToList();
            if (banners != null)
            {
                return PartialView("_BanneMobilerPartial", banners);
            }
            return PartialView("_BanneMobilerPartial");


        }
        public ActionResult MenuTop()
        {
            var Product = db.ProductDetail.Where(x => x.IsActive == true && x.IsHome == true ).OrderByDescending(x=>x.Products.IsHot==true).ToList();   
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