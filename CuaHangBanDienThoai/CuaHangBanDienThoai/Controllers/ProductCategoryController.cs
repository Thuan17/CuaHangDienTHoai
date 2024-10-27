using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Controllers
{
    public class ProductCategoryController : Controller
    {
        // GET: ProductCategory

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Detail(string alias)
        {
            if (alias != null)
            {
                var productCategory = await db.ProductCategory.FirstOrDefaultAsync(x => x.Alias.Trim() == alias.Trim());
                if (productCategory == null)
                {
                    return View();
                }
                var productIds = db.Products.Where(x => x.ProductCategoryId == productCategory.ProductCategoryId).Select(x=>x.ProductsId).ToList() ;
            


                if (productIds != null)
                {
                    var productDetails =  db.ProductDetail
                                          .Where(x => productIds.Contains((int)x.ProductsId) && x.Products.IsActive == true)
                                          .GroupBy(x => x.ProductsId)
                                         .Select(g => g.FirstOrDefault())
                                            .AsQueryable();

                    var sortedItems = productDetails
                                     .OrderByDescending(x => x.Products.IsHot)
                                     .ThenBy(x => x.Products.ProductCategoryId)
                                     .ToList();


                    if (sortedItems.Count > 0&& sortedItems.Any())
                    {

                        ViewBag.CategoryName = productCategory.Title.Trim();
                        ViewBag.Count = sortedItems.Count;
                        ViewBag.Alias = alias;
                        return View(sortedItems);
                    }

                }
                return View();
            }
            else
            {
                ViewBag.CategoryName = "";
                return View();
            }
        }
        
        public async Task<ActionResult> ProductCategory(string alias)
        {
            if (alias != null )
            {
                var productCategory = await db.ProductCategory.FirstOrDefaultAsync(x => x.Alias.Trim() == alias.Trim()) ;
                if (productCategory == null)
                {
                    return View();
                }
                var product = await db.Products.FirstOrDefaultAsync(x => x.ProductCategoryId == productCategory.ProductCategoryId);
          
                if (product != null )
                {
                    var productdetails = await db.ProductDetail.Where(x => x.ProductsId == product.ProductsId).OrderByDescending(x => x.ProductDetailId).ToListAsync(); 
                    if(productdetails!=null&& productdetails.Count > 0)
                    {
                        ViewBag.CategoryName = productCategory.Title.Trim();
                        ViewBag.Count = productdetails.Count;
                        ViewBag.Alias = alias;
                        return View(product);
                    }
                   
                }
                return View();
            }
            else
            {
                ViewBag.CategoryName = "";
                return View();
            }
        }


    }
}