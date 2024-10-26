using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace CuaHangBanDienThoai.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<ProductDetail> items = db.ProductDetail.Where(x => x.IsActive == true );

            if (minPrice.HasValue)
            {
                items = items.Where(x => x.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                items = items.Where(x => x.Price <= maxPrice.Value);
            }

            items = items.OrderByDescending(x => x.Products.ProductCategory).ToList();

            if (items != null)
            {
                var pageSize = 16;
                var pageIndex = page ?? 1;
                var pagedList = items.ToPagedList(pageIndex, pageSize);

                if (pageIndex > pagedList.PageCount)
                {
                    pageIndex = pagedList.PageCount;
                    pagedList = items.ToPagedList(pageIndex, pageSize);
                }

                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageIndex; // Số trang hiện tại
                ViewBag.PageCount = pagedList.PageCount; // Tổng số trang
                ViewBag.FirstItemOnPage = pagedList.FirstItemOnPage; // Số sản phẩm đầu tiên trên trang
                ViewBag.LastItemOnPage = pagedList.LastItemOnPage; // Số sản phẩm cuối cùng trên trang
                ViewBag.TotalItemCount = pagedList.TotalItemCount; // Tổng số sản phẩm
                ViewBag.MinPrice = items.Min(x => x.Price);
                ViewBag.MaxPrice = items.Max(x => x.Price);
                return View(pagedList);
            }

            return View();
        }



        public ActionResult Partial_ItemsByCateId()
        {
            var items = (from pd in db.ProductDetail
                         join p in db.Products on pd.ProductsId equals p.ProductsId
                         where pd.IsHome == true && pd.IsActive == true 
                         group new { p, pd } by p.ProductCompanyId into grouped
                         orderby grouped.Key descending
                         select grouped)
              .Take(4)
              .SelectMany(g => g.Select(x => x.pd).OrderBy(r => Guid.NewGuid()).Take(3))
              .ToList();

            var randomizedItems = items.OrderBy(x => Guid.NewGuid()).ToList();


            return PartialView(randomizedItems);
        }
    }
}