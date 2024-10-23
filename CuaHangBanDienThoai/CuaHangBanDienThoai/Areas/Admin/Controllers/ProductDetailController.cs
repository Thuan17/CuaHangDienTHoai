using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Threading.Tasks;
using System.Data.Entity;
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
        public  ActionResult Add(int? productid)
        {

            if (db.Products.Any())
            {
                var productlist = db.Products
                    .Where(pro => pro.ProductCategory != null)
                    .Select(pro => new ListProductId
                    {
                        ProductsId = pro.ProductsId,
                        Image = pro.Image,
                        Title = pro.ProductCategory.Title.Trim() + " " + pro.Title.Trim(),
                        IsCheck = productid.HasValue && productid > 0 && productid == pro.ProductsId 
                    })
                    .ToList();

                ViewBag.ProductList = productlist;
                return View();
            }
      
            return View();
        }
        [AuthorizeFunction("Quản trị viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Add(ProductDetail model ,Admin_AddProductDetail req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try {
                    if (ModelState.IsValid)
                    {
                        if (Images == null  )
                        {
                            return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn ít nhất 3 ảnh sản phẩm" });
                           
                        }


                            if (req.ProductsId <=0)
                        {

                            return Json(new { Success = false, Code = -2, msg = "Danh mục hoặc hãng" });

                        }
                        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductsId == req.ProductsId);


                       
                        if (product == null)
                        {
                           
                            return Json(new { Success = false, Code = -2, msg = "Danh mục hoặc hãng" });

                        }
                
                        string rawAlias = $"{product.Alias}{req.Color} {req.Ram}{req.Capacity}".Trim();
                        string  alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(rawAlias).Trim();
                        var checkDetails = await db.ProductDetail.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
                        if (checkDetails != null)
                        {
                            return Json(new { Success = false, Code = -3, msg = "Sản phẩm đã tồn tại" });
                        }
                        else
                        {
                            model.Color = req.Color.Trim();
                            model.Price = req.Price;
                            model.PriceSale = req.PriceSale;
                            model.OriginalPrice = req.OriginalPrice;
                            model.Ram = req.Ram.Trim();
                            model.Capacity = req.Capacity.Trim();
                            model.ProductsId = req.ProductsId;
                            model.Ram = req.Ram.Trim();
                            model.IsActive = false;
                            model.IsHome = false;
                            model.IsSale = false;
                            model.IsHot = false;
                            model.Alias = alias.Trim();


                            db.ProductDetail.Add(model);
                            db.SaveChanges();


                            if (Images != null )
                            {
                                if (Images.Count < 2 && rDefault == null || rDefault.Count == 0)
                                {
                                    return Json(new { Success = false, Code =-2 , msg = "Vui lòng chọn ảnh đại diện sản phẩm" });
                                }

                                for (int i = 0; i < Images.Count; i++)
                                {
                                    bool isDefault = (i + 1 == rDefault[0]);

                                    model.ProductDetailImage.Add(new ProductDetailImage
                                    {
                                        ProductDetailId = model.ProductDetailId,
                                        Image = Images[i],
                                        IsDefault = isDefault
                                    });
                                    db.SaveChanges();
                                }

                            }
                            else
                            {
                                return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn ảnh sản phẩm" });
                            }


                            dbContextTransaction.Commit();
                            return Json(new { Success = true, Code = 1, msg = "Sản phẩm con được tạo thành công" });
                          
                        }
                    }
                    else
                    {
                        dbContextTransaction.Rollback();

                       
                        var productlist = db.Products.Where(pro => pro.ProductCategory != null).Select(pro => new ListProductId
                        {
                            ProductsId = pro.ProductsId,
                            Image = pro.Image,
                            Title = pro.ProductCategory.Title.Trim() + " " + pro.Title.Trim(),
                        }).ToList();
                        ViewBag.ProductList = productlist;
                        return Json(new { Success = false, Code = -2 ,msg="Vui lòng điền đủ dữ liệu"});
                    }
                
                
                
                }catch(Exception ex) 
                {
                    dbContextTransaction.Rollback();

                    Console.WriteLine(ex.Message);
                    var productlist = db.Products.Where(pro => pro.ProductCategory != null).Select(pro => new ListProductId
                    {
                        ProductsId = pro.ProductsId,
                        Image = pro.Image,
                        Title = pro.ProductCategory.Title.Trim() + " " + pro.Title.Trim(),
                    }).ToList();
                    ViewBag.ProductList = productlist;
                    return Json(new { Success = false, Code = -100 });
                }
            }


        }






        [HttpPost]
        public async Task<ActionResult> IsActive(int id)
        {
            try
            {
                var item = await db.ProductDetail.FindAsync(id);
                if (item != null)
                {
                    item.IsActive = !item.IsActive;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new { success = true, isActive = item.IsActive });
                }
                return Json(new { success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Lỗi cập nhập tag hiển thị " });
            }
        }
        [HttpPost]
        public async Task<ActionResult> IsHome(int id)
        {
            try
            {
                var item = await db.ProductDetail.FindAsync(id);

                if (item != null)
                {
                    item.IsHome = !item.IsHome;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Json(new { success = true, isActive = item.IsHome });
                }

                return Json(new { success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Lỗi cập nhập tag hiển thị trang chủ" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> IsSale(int id)
        {
            try
            {
                var item = await db.ProductDetail.FindAsync(id);
                if (item != null)
                {
                    item.IsSale = !item.IsSale;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json(new { success = true, isActive = item.IsSale });
                }
                return Json(new { success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Lỗi cập nhập tag Sale" });
            }


        }
    }
}