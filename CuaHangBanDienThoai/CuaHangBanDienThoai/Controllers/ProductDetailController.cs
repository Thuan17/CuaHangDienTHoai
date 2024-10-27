using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
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
            // Lấy danh sách sản phẩm từ ProductDetail và loại bỏ trùng bằng GroupBy
            var items = db.ProductDetail
                .Where(x => x.Products.IsActive == true)
                .GroupBy(x => x.ProductsId) // Nhóm theo ProductsId để tránh trùng lặp
                .Select(g => g.FirstOrDefault()) // Lấy sản phẩm đầu tiên trong mỗi nhóm
                .AsQueryable();

            // Lọc theo minPrice nếu có
            if (minPrice.HasValue)
            {
                items = items.Where(x => x.Price >= minPrice.Value);
            }

            // Lọc theo maxPrice nếu có
            if (maxPrice.HasValue)
            {
                items = items.Where(x => x.Price <= maxPrice.Value);
            }

            // Sắp xếp: Sản phẩm Hot trước, sau đó theo danh mục
            var sortedItems = items
                .OrderByDescending(x => x.Products.IsHot) // Sản phẩm Hot lên đầu
                .ThenBy(x => x.Products.ProductCategoryId) // Sau đó theo danh mục
                .ToList();

            if (sortedItems.Any())
            {
                var pageSize = 16;
                var pageIndex = page ?? 1;
                var pagedList = sortedItems.ToPagedList(pageIndex, pageSize);

                if (pageIndex > pagedList.PageCount)
                {
                    pageIndex = pagedList.PageCount;
                    pagedList = sortedItems.ToPagedList(pageIndex, pageSize);
                }

                // Gán thông tin phân trang vào ViewBag
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageIndex;
                ViewBag.PageCount = pagedList.PageCount;
                ViewBag.FirstItemOnPage = pagedList.FirstItemOnPage;
                ViewBag.LastItemOnPage = pagedList.LastItemOnPage;
                ViewBag.TotalItemCount = pagedList.TotalItemCount;
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
        public ActionResult Partial_DetailByCapactity(int? proId, int? productDetailId  , string capcity)
        {
            if(proId >0&&productDetailId > 0 && capcity != null)
            {
                var item = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == productDetailId&& x.ProductsId== proId && x.Capacity.Trim() == capcity.Trim());
                if (item!=null)
                {
                    ViewBag.ProductId = proId;
                    ViewBag.Capacity = capcity.Trim();
                    return PartialView(item);   
                }
            }
            return PartialView();
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

        public ActionResult Partail_ColorByProductsId(int productid, string capacity,string color )
        {
            if (productid != null&& capacity!=null&& color != null)
            {

                using (var dbContext = new CUAHANGDIENTHOAIEntities())
                {
                    var uniqueCapacitiesWithIdsAndImages = dbContext.ProductDetail
                    .Where(p => p.ProductsId == productid && p.Capacity == capacity)
                    .GroupBy(p => p.Color.Trim())
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
                    ViewBag.Color = color.Trim();
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