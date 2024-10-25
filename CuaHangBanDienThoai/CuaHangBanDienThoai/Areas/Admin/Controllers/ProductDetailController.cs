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
using Antlr.Runtime.Misc;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

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
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
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
                
                        string rawAlias = $"{product.Alias+"-"}{req.Color + "-"} {req.Ram + "-"}{req.Capacity + "-"}".Trim();
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
                            model.Quantity = req.Quantity;


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


        [AuthorizeFunction("Quản trị viên")]
        public async Task<ActionResult> Edit(string alias)
        {

            if (alias!=null&& db.Products.Any())
            {
                var proDetails=await db. ProductDetail.FirstOrDefaultAsync(p => p.Alias == alias);

                if (proDetails == null) 
                {
                    return View();
                }

                var Name = proDetails.Products.ProductCategory.Title.Trim() + " " + proDetails.Products.Title.Trim() + " " + proDetails.Capacity.Trim();
                ViewBag.Name = Name;


                var productlist = db.Products
                    .Where(pro => pro.ProductCategory != null)
                    .Select(pro => new ListProductId
                    {
                        ProductsId = pro.ProductsId,
                        Image = pro.Image,
                        Title = pro.ProductCategory.Title.Trim() + " " + pro.Title.Trim(),
                        IsCheck = proDetails.ProductsId.HasValue && proDetails.ProductsId > 0 && proDetails.ProductsId == pro.ProductsId
                    })
                    .ToList();
                var ramOptions = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "4GB", Text = "4GB" },
                                new SelectListItem { Value = "8GB", Text = "8GB" },
                                new SelectListItem { Value = "16GB", Text = "16GB" },
                               new SelectListItem { Value = "32GB", Text = "32GB" },
                                new SelectListItem { Value = "64GB", Text = "64GB" },
                            };
                ViewBag.RamOptions = new SelectList(ramOptions, "Value", "Text", proDetails.Ram.Trim());

                var capacityOptions = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "64GB", Text = "64GB" },
                                new SelectListItem { Value = "128GB", Text = "128GB" },
                                new SelectListItem { Value = "256GB", Text = "256GB" },
                               new SelectListItem { Value = "512GB", Text = "512GB" },
                                new SelectListItem { Value = "1TB", Text = "1TB" },
                                new SelectListItem { Value = "2TB", Text = "2TB" },
                            };
                ViewBag.CapacityOptions = new SelectList(capacityOptions, "Value", "Text", proDetails.Capacity.Trim());

                Admin_EditProductDetail viewModel = new Admin_EditProductDetail {
                ProductDetailId = proDetails.ProductDetailId,
                    ImageProduct = proDetails.Products.Image.Trim(),
                    Color=proDetails.Color.Trim(),
                    Alias = proDetails.Alias.Trim(),
                    Price = proDetails.Price??0,
                    PriceSale = proDetails.PriceSale ?? 0,
                    OriginalPrice = proDetails.OriginalPrice ?? 0,
                    Ram = proDetails.Ram.Trim(),
                    Capacity = proDetails.Capacity.Trim(),
                    ProductsId = (int)proDetails.ProductsId,
                    Quantity = proDetails.Quantity??0,
                    IsActive =(bool) proDetails.IsActive,
                    IsHome = (bool) proDetails.IsHome,
                    IsSale = (bool)proDetails.IsSale,
                    Products= proDetails.Products,
                    Items = db.ProductDetailImage
                                .Where(x => x.ProductDetailId == proDetails.ProductDetailId)
                                .Select(image => new ImageItemProduct
                                {
                                    ProductImageId = image.ProductImageId,
                                    Image = image.Image,
                                    IsDefault = image.IsDefault
                                }).ToList()

                };
              
                ViewBag.ProductList = productlist;
          


                Session["Admin_EditProductDetails" + alias] = viewModel;
                return View(viewModel);
            }
            return View();
        }

        [AuthorizeFunction("Quản trị viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Admin_EditProductDetail req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid&& req.Alias!=null)
                    {

                      
                        var proDetails = await db.ProductDetail.FirstOrDefaultAsync(p => p.Alias == req.Alias);
                        var sessionKey = "Admin_EditProductDetails" + req.Alias;



                        Admin_EditProductDetail viewModel = (Admin_EditProductDetail)Session[sessionKey];


                        if (viewModel == null || db.Products.Find(viewModel.ProductsId) == null /*|| product == null*/)
                        {
                            return Json(new { Success = false, Code = -2, msg = "Danh mục hoặc hãng" });
                        }

                        if (proDetails == null)
                        {

                            return Json(new { Success = false, Code = -2, msg = "Danh mục hoặc hãng" });

                        }

                        var product = await db.Products.FirstOrDefaultAsync(x => x.ProductsId == req.ProductsId);



                        if (product == null)
                        {

                            return Json(new { Success = false, Code = -2, msg = "Danh mục hoặc hãng" });

                        }

                        string rawAlias = $"{product.Alias + "-"}{req.Color + "-"} {req.Ram + "-"}{req.Capacity}".Trim();
                        string alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(rawAlias).Trim();





                        if (alias != proDetails.Alias)
                        {
                            var checkproduct = await db.ProductDetail.FirstOrDefaultAsync(x => x.Alias == alias.Trim() && x.ProductDetailId != req.ProductDetailId);
                            if (checkproduct != null)
                            {
                                return Json(new { Success = false, Code = -2, msg = "Sản phẩm đã tồn tại bảng ghi trên !!!" });

                            }
                        }


                        bool isModified =
                              proDetails.ProductDetailId != req.ProductDetailId ||
                              proDetails.Color != req.Color ||
                              proDetails.Price != req.Price ||
                              proDetails.PriceSale != req.PriceSale ||
                              proDetails.OriginalPrice != req.OriginalPrice ||
                              proDetails.Ram != req.Ram ||
                              proDetails.Capacity != req.Capacity ||
                              proDetails.ProductsId != req.ProductsId ||
                              proDetails.IsActive != req.IsActive ||
                              proDetails.IsHome != req.IsHome ||
                              proDetails.IsSale != req.IsSale ||
                              proDetails.Capacity != req.Capacity;
                              

                     

                        if (!isModified)
                        {
                            return Json(new { Success = false, Code = -1, msg = "Không có thay đổi nào !!!" });
                        }
                        if (alias != proDetails.Alias)
                        {
                            proDetails.Alias = alias.Trim();
                        }
                        proDetails.ProductDetailId = req.ProductDetailId;
                        proDetails.Color = req.Color.Trim();
                    
                        proDetails.Price = req.Price;
                        proDetails.PriceSale = req.PriceSale;
                        proDetails.OriginalPrice = req.OriginalPrice;
                        proDetails.Ram = req.Ram.Trim();
                        proDetails.Capacity = req.Capacity.Trim();
                        proDetails.ProductsId = (int)req.ProductsId;
                        proDetails.Quantity = req.Quantity;
                        proDetails.IsActive = (bool)req.IsActive;
                        proDetails.IsHome = (bool)req.IsHome;
                        proDetails.IsSale = (bool)req.IsSale;
                        db.Entry(proDetails).State = EntityState.Modified;
                        db.SaveChanges();



                        var checkimageProduct =await db.ProductDetailImage.Where(x => x.ProductDetailId == req.ProductDetailId).ToListAsync();

                        if (viewModel.Items != null && viewModel.Items.Count > 0 )
                        {

                            foreach (var image in checkimageProduct.ToList())
                            {
                                var viewImage = viewModel.Items.FirstOrDefault(x => x.ProductImageId == image.ProductImageId);

                                if (viewImage == null)
                                {
                                    db.ProductDetailImage.Remove(image);
                                }
                                else
                                {
                                    image.Image = viewImage.Image;
                                    image.IsDefault = viewImage.IsDefault;
                                    db.Entry(image).State = EntityState.Modified;
                                }
                                db.SaveChanges();
                            }
                        }

                        if (Images != null && Images.Count > 0 )
                        {
                            for (int i = 0; i < Images.Count; i++)
                            {
                                try
                                {
                                    bool isDefault = false;


                                    if (rDefault != null && rDefault.Count > 0)
                                    {
                                        isDefault = (i + 1 == rDefault[0]);
                                    }
                                    if(!string.IsNullOrEmpty(Images[i]))
                                    {
                                        proDetails.ProductDetailImage.Add(new ProductDetailImage
                                        {
                                            ProductDetailId = req.ProductDetailId,
                                            Image = Images[i],
                                            IsDefault = isDefault
                                        });
                                        db.SaveChanges();
                                    }
                                  
                                }
                                catch (Exception ex)
                                {
                                    return Json(new { Success = false, Code = -2,msg="Lỗi trong quá trình thêm ảnh mới" }); 
                                }

                            }
                        }

                        Session["Admin_EditProductDetails" + req.Alias] = null;

                        dbContextTransaction.Commit();

                        return Json(new { Success = true, Code = 1 });

                    }
                    else
                    {


                        return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đủ dữ liệu" });



                    }
                }
                catch (Exception ex)
                {
                    var proDetails = await db.ProductDetail.FirstOrDefaultAsync(p => p.Alias == req.Alias);

                    if (proDetails == null)
                    {
                        return View();
                    }
                    dbContextTransaction.Rollback();

                    var productlist = db.Products
                     .Where(pro => pro.ProductCategory != null)
                     .Select(pro => new ListProductId
                     {
                         ProductsId = pro.ProductsId,
                         Image = pro.Image,
                         Title = pro.ProductCategory.Title.Trim() + " " + pro.Title.Trim(),
                         IsCheck = proDetails.ProductsId.HasValue && proDetails.ProductsId > 0 && proDetails.ProductsId == pro.ProductsId
                     })
                     .ToList();
                    var ramOptions = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "4GB", Text = "4GB" },
                                new SelectListItem { Value = "8GB", Text = "8GB" },
                                new SelectListItem { Value = "16GB", Text = "16GB" },
                               new SelectListItem { Value = "32GB", Text = "32GB" },
                                new SelectListItem { Value = "64GB", Text = "64GB" },
                            };
                    ViewBag.RamOptions = new SelectList(ramOptions, "Value", "Text", proDetails.Ram.Trim());

                    var capacityOptions = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "64GB", Text = "64GB" },
                                new SelectListItem { Value = "128GB", Text = "128GB" },
                                new SelectListItem { Value = "256GB", Text = "256GB" },
                               new SelectListItem { Value = "512GB", Text = "512GB" },
                                new SelectListItem { Value = "1TB", Text = "1TB" },
                                new SelectListItem { Value = "2TB", Text = "2TB" },
                            };
                    ViewBag.CapacityOptions = new SelectList(capacityOptions, "Value", "Text", proDetails.Capacity.Trim());
                    return Json(new { Success = false, Code = -100 });
                }

            }

        }



        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public async Task<ActionResult> Detail(string alias)
        {
            if (alias == null)
            {
                ViewBag.Name = "Không tìm thấy sản phẩm";
                return View();
            }

            var item = await db.ProductDetail.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (item != null)
            {
                AccountEmployee nvSession = (AccountEmployee)Session["user"];
                var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);

                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;

         
                ViewBag.Name = item.Products.Title.Trim() + "  " + item.Color.Trim() + " " + item.Ram.Trim() + " " + item.Capacity.Trim();
                return View(item);
            }
            return View();

        }



        [HttpPost]
        public JsonResult DeleteImage(string alias,   int producdetailid, int productImageId)
        {

            if (alias == null || producdetailid < 0 || productImageId < 0)
            {
                return Json(new { success = false, message = "Mã lỗi hình ảnh ." });
            }
            var sessionKey = "Admin_EditProductDetails" + alias;

      
            Admin_EditProductDetail viewModel = (Admin_EditProductDetail)Session[sessionKey];
            if (viewModel == null)
            {
                return Json(new { success = false, message = "Product not found in session." });
            }
            if(viewModel.Items.Count < 3)
            {

                return Json(new { success = false, message = "Sản phẩm cần ít nhất 3 ảnh " });
            }
            var imageItem = viewModel.Items.FirstOrDefault(img => img.ProductImageId == productImageId);
            if (imageItem != null)
            {

                if (imageItem.IsDefault)
                {

                    var nextImage = viewModel.Items.FirstOrDefault(img => img.ProductImageId != productImageId);
                    if (nextImage != null)
                    {
                        nextImage.IsDefault = true;
                    }
                    viewModel.Items.Remove(imageItem);

                    Session[sessionKey] = viewModel;
                    return Json(new { success = true, code = 1 });
                }
                else
                {
                    viewModel.Items.Remove(imageItem);

                    Session[sessionKey] = viewModel;
                    return Json(new { success = true, code = 2 });
                }


            }
            else
            {
                return Json(new { success = false, message = "Image not found in session." });
            }
        }

        




        [HttpPost]
        public ActionResult UpdateImage(string alias, int producdetailid, int productImageId, string url)
        {
            if (alias == null || producdetailid < 0 || productImageId < 0 || url == null)
            {
                return Json(new { success = false, message = "Mã lỗi hình ảnh ." });
            }
            var sessionKey = "Admin_EditProductDetails" + alias;
            Admin_EditProductDetail viewModel = (Admin_EditProductDetail)Session[sessionKey];
            if (viewModel == null)
            {
                return Json(new { success = false, message = "Product not found in session." });
            }
            var imageItem = viewModel.Items.FirstOrDefault(img => img.ProductImageId == productImageId);
            if (imageItem != null)
            {
                imageItem.Image = url;
                if (imageItem.IsDefault)
                {
                    var nextImage = viewModel.Items.FirstOrDefault(img => img.ProductImageId != productImageId);
                    if (nextImage != null)
                    {
                        nextImage.IsDefault = true;
                    }
                }
                Session[sessionKey] = viewModel;
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Image not found in session." });
            }
        }

  


        [HttpPost]
        public ActionResult UpdateImageisDefault(string alias,int producdetailid, int productImageId, string url, bool isDefault)
        {
            if (alias == null || producdetailid < 0 || productImageId < 0|| url == null)
            {
                return Json(new { success = false, message = "Mã lỗi hình ảnh ." });
            }
            var sessionKey = "Admin_EditProductDetails" + alias;
            Admin_EditProductDetail viewModel = (Admin_EditProductDetail)Session[sessionKey];
            if (viewModel == null)
            {
                return Json(new { success = false, message = "Product not found in session." });
            }

            var imageItem = viewModel.Items.FirstOrDefault(img => img.ProductImageId == productImageId);
            if (imageItem != null)
            {
                imageItem.Image = url;
                imageItem.IsDefault = isDefault;

                if (isDefault)
                {
                    // Đặt tất cả các ảnh khác thành không phải mặc định
                    foreach (var img in viewModel.Items.Where(img => img.ProductImageId != productImageId))
                    {
                        img.IsDefault = false;
                    }
                }

                Session[sessionKey] = viewModel;
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Image not found in session." });
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