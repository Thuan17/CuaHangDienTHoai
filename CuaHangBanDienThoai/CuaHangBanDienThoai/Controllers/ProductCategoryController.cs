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
                var product = await db.Products.FirstOrDefaultAsync(x => x.ProductCategoryId == productCategory.ProductCategoryId);

                if (product != null)
                {
                    var productdetails = await db.ProductDetail.Where(x => x.ProductsId == product.ProductsId).OrderByDescending(x => x.ProductDetailId).ToListAsync();
                    if (productdetails != null && productdetails.Count > 0)
                    {
                        ViewBag.CategoryName = productCategory.Title.Trim();
                        ViewBag.Count = productdetails.Count;
                        ViewBag.Alias = alias;
                        return View(productdetails);
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