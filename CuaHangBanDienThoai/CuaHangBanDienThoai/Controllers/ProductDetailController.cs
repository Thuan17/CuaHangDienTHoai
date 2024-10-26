using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Controllers
{
    public class ProductDetailController : Controller
    {
        // GET: ProductDetail
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index(int? page, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<ProductDetail> items = db.ProductDetail.Where(x => x.IsActive == true);

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


        public async Task<ActionResult> Details(string alias)
        {
            if (alias == null )
            {
                return View();
            }
            var item = await db.ProductDetail.FirstOrDefaultAsync(x => x.Alias.Trim() == alias.Trim());
            if (item != null)
            { 

                var pro= await db.Products.FirstOrDefaultAsync(x => x.ProductsId==item.ProductsId);
                if (pro != null)
                {
                    ViewBag.Name = pro.ProductCategory.Title.Trim() + " " + pro.Title.Trim();
                }
                return View(item);
            } 
            else 
            {
                return View();
            }



        }


        public ActionResult Capacity(int productid, string DungLuong)
        {
            if (productid != null && DungLuong != null)
            {
                using (var dbContext = new CUAHANGDIENTHOAIEntities())
                {
                    var uniqueCapacitiesWithIdsAndImages = dbContext.ProductDetail
                    .Where(p => p.ProductsId == productid)
                    .GroupBy(p => p.Capacity.Trim())
                    .Select(g => new
                    {
                        Capacity = g.Key,
                        ProductDetailId = g.Min(p => p.ProductDetailId),

                    })
                    .ToList();

                    var viewModels = uniqueCapacitiesWithIdsAndImages.Select(item => new ProductColorViewModel
                    {

                        ProductDetailId = item.ProductDetailId,
                        ProductslId = productid,
                        Capacity = item.Capacity,

                    }).ToList();

                    ViewBag.ProductId = productid;
                    ViewBag.Capacity = DungLuong;
                    return PartialView(viewModels);
                }
            }
            else
            {
                return PartialView(null);
            }


        }



        //public ActionResult Capacity(int productid, string capacity)
        //{
        //    if (productid != 0 && !string.IsNullOrEmpty(capacity))
        //    {
        //        using (var dbContext = new CUAHANGDIENTHOAIEntities())
        //        {
        //            // Lấy tất cả dung lượng thuộc về productid cụ thể
        //            var uniqueCapacitiesWithIdsAndImages = dbContext.ProductDetail
        //                .Where(p => p.ProductsId == productid)
        //                .Select(p => new
        //                {
        //                    Capacity = p.Key,
        //                    ProductDetailId = p.ProductDetailId,
        //                })
        //                .Distinct() // Đảm bảo loại bỏ các bản sao nếu cần
        //                .ToList();

        //            // Lấy tất cả các bản ghi cho mỗi dung lượng
        //            var viewModels = uniqueCapacitiesWithIdsAndImages.Select(item => new ProductColorViewModel
        //            {
        //                ProductDetailId = item.ProductDetailId,
        //                ProductslId = productid,
        //                Capacity = item.Capacity,
        //            }).ToList();

        //            ViewBag.ProductId = productid;
        //            ViewBag.Capacity = capacity;

        //            return PartialView(viewModels);
        //        }
        //    }
        //    else
        //    {
        //        return PartialView(null);
        //    }
        //}

        public ActionResult Partail_ColorByProductsId(int productid, string capacity)
        {
            if (productid != null)
            {

                using (var dbContext = new CUAHANGDIENTHOAIEntities())
                {
                    var uniqueCapacitiesWithIdsAndImages = dbContext.ProductDetail
                    .Where(p => p.ProductsId == productid && p.Capacity == capacity)
                    .GroupBy(p => p.Color)
                    .Select(g => new
                    {
                        Color = g.Key,
                       
                        ProductDetailId = g.Min(p => p.ProductDetailId),
                    

                    })
                    .ToList();

                    var viewModels = uniqueCapacitiesWithIdsAndImages.Select(item => new ProductColorViewModel
                    {

                        ProductDetailId = item.ProductDetailId,
                        ProductslId = productid,
                        Color = item.Color,
                      

                    }).ToList();

                    ViewBag.ProductId = productid;
                  
                    return PartialView(viewModels);
                }
            }
            else
            {
                return PartialView(null);
            }


        }


    }
}