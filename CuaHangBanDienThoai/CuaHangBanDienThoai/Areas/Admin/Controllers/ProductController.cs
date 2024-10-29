using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.Threading.Tasks;
using CKFinder.Settings;
using CKFinder.Connector;
using Microsoft.AspNet.SignalR.Hubs;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities  ();

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.Products.OrderByDescending(x => x.ProductsId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.Products.ToList();
              
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
        [AuthorizeFunction( "Quản trị viên")]
        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
            ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Products model, Admin_AddProduct req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                   
                    if (req.Title == null )
                    {
                        ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
                        ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập tên sản phẩm" });
                    }
                   
                    if (req.ProductCategoryId <=0|| req.ProductCompanyId <= 0)
                    {
                        ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
                        ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
                        return Json(new { Success = false, Code = -2, msg = "Danh mục hoặc hãng" });
                    }

                   
                    if (Images == null || Images.Count == 0)
                    {
                        ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
                        ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn logo hãng" });
                    }
                  
                    var productCompany = await db.ProductCompany.FindAsync(req.ProductCompanyId);
                    var productCategory = await db.ProductCategory.FindAsync(req.ProductCategoryId);
                    if (productCompany == null && productCategory == null)
                    {
                        ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
                        ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng danh mục hoặc hãng " });

                    }

                    string alias;
                    string rawAlias = $"{productCategory.Title} {productCompany.Title} {req.Title.Trim()}".Trim();
                    alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(rawAlias).Trim();
                    string description = Request.Form["Description"];
                    var checkProduct = await db.Products.FirstOrDefaultAsync(x => x.Alias == alias);
                    if (checkProduct != null)
                    {
                        ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
                        ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
                        return Json(new { Success = false, Code = -2, msg = "Sản phẩm đã tồn tại" });

                    }


                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    model.Image = Images[0];
                    model.Title = req.Title;
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = checkStaff.NameEmployee.Trim();
                    model.Alias = alias;
                    model.Screensize = req.Screensize;
                    model.CPUspeed = req.CPUspeed.Trim();
                    model.OperatingSystem = req.OperatingSystem.Trim();
                    model.GPUspeed = req.GPUspeed.Trim();
                    model.MobileNetwork = req.MobileNetwork.Trim();
                    model.Sim = req.Sim.Trim();
                    model.Wifi = req.Wifi.Trim();
                    model.Bluetooth = req.Bluetooth.Trim();
                    model.BatteryType = req.BatteryType.Trim();
                    model.ChargingSupport = req.ChargingSupport.Trim();
                    model.BatteryTechnology = req.BatteryTechnology.Trim();
                    model.CPU = req.CPU.Trim();
                    model.GPU = req.GPU.Trim();
                    model.BatteryCapacity = req.BatteryCapacity.Trim();
                    model.IsHot = false;
                    model.IsActive = false;
                model.ProductCategoryId=req.ProductCategoryId;
                    model.ProductCompanyId = req.ProductCompanyId;



                    db.Products.Add(model);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return Json(new { Success = true, Code = 1 });


                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    Console.WriteLine(ex.Message);
                    ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title");
                    ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title");
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

            var item = await db.Products.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (item != null)
            {

                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                string rawAlias = $"{item.ProductCategory.Title}  {item.Title.Trim()}".Trim().ToLower();
                ViewBag.Name = rawAlias;
                return View(item);
            }
            return View();

        }

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Partail_ProDetailById(int? id)
        {
            if (!id.HasValue || id <= 0)
            {
                return PartialView();
            }

            var item = db.ProductDetail
                .Where(x => x.ProductsId == id)
                .OrderByDescending(x => x.ProductDetailId)
                .ToList();

            if (item.Any())
            {
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                return PartialView(item);
            }

            return PartialView();
        }

        //[AuthorizeFunction("Quản lý", "Quản trị viên")]
        //public async Task<ActionResult> Partail_ProDetailById(int? id)
        //{
        //    if (!id.HasValue || id <= 0)
        //    {
        //        return PartialView();
        //    }

        //    var item = await db.ProductDetail
        //        .Where(x => x.ProductsId == id)
        //        .OrderByDescending(x => x.ProductDetailId)
        //        .ToListAsync();

        //    if (item.Any())
        //    {
        //        return PartialView(item);
        //    }

        //    return PartialView();
        //}



        [AuthorizeFunction( "Quản trị viên")]
        public async Task<ActionResult> Edit(string alias)
        {
            if(alias==null)
            {
                ViewBag.Name = "Không tìm thấy sản phẩm";
                return View();
            }


            var product = await db.Products.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (product != null)
            {
                string rawAlias = $"{product.ProductCategory.Title}  {product.Title.Trim()}".Trim().ToLower();
                ViewBag.Name = rawAlias;

                ViewBag.ProductCategory = new SelectList(db.ProductCategory.ToList(), "ProductCategoryId", "Title", product.ProductCategoryId);
                ViewBag.ProductCompany = new SelectList(db.ProductCompany.ToList(), "ProductCompanyId", "Title", product.ProductCompanyId);
                Admin_EditProduct ViewModel = new Admin_EditProduct
                {
                    ProductId = product.ProductsId,
                    ProductCategoryId =(int) product.ProductCategoryId,
                    ProductCompanyId = (int) product.ProductCompanyId,
                    Title = product.Title.Trim(),
                    CreatedBy = product.CreatedBy.Trim(),
                    CreatedDate = product.CreatedDate,
                    Screensize = product.Screensize.Trim(),
                    CPUspeed = product.CPUspeed,
                    OperatingSystem = product.OperatingSystem.Trim(),
                    GPUspeed = product.GPUspeed.Trim(),
                    MobileNetwork = product.MobileNetwork.Trim(),
                    Sim = product.Sim.Trim(),
                    Wifi = product.Wifi.Trim(),
                    Bluetooth = product.Bluetooth.Trim(),
                    Connector = product.Connector.Trim(),
                    BatteryType = product.BatteryType.Trim(),
                    ChargingSupport = product.ChargingSupport.Trim(),
                    BatteryTechnology = product.BatteryTechnology.Trim(),
                    CPU = product.CPU.Trim(),
                    GPU = product.GPU.Trim(),
                    BatteryCapacity = product.BatteryCapacity.Trim(),
                    Alias=product.Alias.Trim(),     
                    IsActive=(bool)product.IsActive,
                    IsHot=(bool)product.IsHot,
                  


                    Description = product.Description,

                    Image = product.Image.Trim(),

                };
                Session["Admin_EditProduct" + product.ProductsId] = ViewModel;





                return View(ViewModel);
            }
            return View();

        }



        [AuthorizeFunction("Quản trị viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Admin_EditProduct req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    if (checkStaff == null)
                    {
                        return Json(new { Success = false, Code = -7, msg = "Phiên đăng nhập hết hạn" });
                    }

                    if (req.Alias == null || req.ProductId <= 0)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy mã sản phẩm" });
                    }

                    var product = await db.Products.FindAsync(req.ProductId);
                    if (product == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Lỗi lấy sản phẩm" });
                    }
                    var productCompany = await db.ProductCompany.FindAsync(req.ProductCompanyId);
                    var productCategory = await db.ProductCategory.FindAsync(req.ProductCategoryId);
                    if (productCompany == null|| productCategory==null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Lỗi lấy danh mục và hãng sản phẩm" });
                    }


             
                    string rawAlias = $"{productCategory.Title} {productCompany.Title} {req.Title.Trim()}".Trim();
                    string  alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(rawAlias).Trim();
                   
                    if(alias != product.Alias)
                    {
                        var checkproduct = await db.Products.FirstOrDefaultAsync(x => x.Alias == alias.Trim()&&x.ProductsId!=req.ProductId);
                        if(checkproduct!=null)
                        {
                            return Json(new { Success = false, Code = -2, msg = "Sản phẩm đã tồn tại bảng ghi trên !!!" });

                        }
                    }

                    // Kiểm tra xem dữ liệu có thay đổi không
                    bool isModified =
                        product.ProductCategoryId != req.ProductCategoryId ||
                        product.ProductCompanyId != req.ProductCompanyId ||
                        product.Title.Trim() != req.Title.Trim() ||
                        product.Screensize.Trim() != req.Screensize.Trim() ||
                        product.CPUspeed != req.CPUspeed ||
                        product.OperatingSystem.Trim() != req.OperatingSystem.Trim() ||
                        product.GPUspeed.Trim() != req.GPUspeed.Trim() ||
                        product.MobileNetwork.Trim() != req.MobileNetwork.Trim() ||
                        product.Sim.Trim() != req.Sim.Trim() ||
                        product.Wifi.Trim() != req.Wifi.Trim() ||
                        product.Bluetooth.Trim() != req.Bluetooth.Trim() ||
                        product.Connector.Trim() != req.Connector.Trim() ||
                        product.BatteryType.Trim() != req.BatteryType.Trim() ||
                        product.ChargingSupport.Trim() != req.ChargingSupport.Trim() ||
                        product.BatteryTechnology.Trim() != req.BatteryTechnology.Trim() ||
                        product.CPU.Trim() != req.CPU.Trim() ||
                        product.GPU.Trim() != req.GPU.Trim() ||
                        product.BatteryCapacity.Trim() != req.BatteryCapacity.Trim() ||
                        product.Alias.Trim() != req.Alias.Trim() ||
                        product.Description != req.Description ||
                          
                        (Images != null && Images.Count > 0 && !string.IsNullOrEmpty(Images[0]) && product.Image != Images[0]);

                    if (!isModified)
                    {
                        return Json(new { Success = false, Code = -1, msg = "Không có thay đổi nào !!!" });
                    }
                    if (alias != product.Alias)
                    {
                        product.Alias = alias.Trim();
                    }

                    product.ProductCategoryId = req.ProductCategoryId;
                    product.ProductCompanyId = req.ProductCompanyId;
                    product.Title = req.Title.Trim();
                    product.Screensize = req.Screensize.Trim();
                    product.CPUspeed = req.CPUspeed;
                    product.OperatingSystem = req.OperatingSystem.Trim();
                    product.GPUspeed = req.GPUspeed.Trim();
                    product.MobileNetwork = req.MobileNetwork.Trim();
                    product.Sim = req.Sim.Trim();
                    product.Wifi = req.Wifi.Trim();
                    product.Bluetooth = req.Bluetooth.Trim();
                    product.Connector = req.Connector.Trim();
                    product.BatteryType = req.BatteryType.Trim();
                    product.ChargingSupport = req.ChargingSupport.Trim();
                    product.BatteryTechnology = req.BatteryTechnology.Trim();
                    product.CPU = req.CPU.Trim();
                    product.GPU = req.GPU.Trim();
                    product.BatteryCapacity = req.BatteryCapacity.Trim();
                    product.Alias = alias.Trim();
                    product.Description = req.Description;
                    product.Modifiedby = checkStaff.NameEmployee.Trim();
                    product.ModifiedDate = DateTime.Now;

                    if (Images!=null&& Images.Count > 0&&! string.IsNullOrEmpty(Images[0]))
                    {
                        product.Image = Images[0];
                    }
                    else
                    {
                        product.Image = req.Image.Trim();
                    }

                    db.Entry(product).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    return Json(new { Success = true, Code = 1 });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    Console.WriteLine(ex.Message);
                    return Json(new { Success = false, Code = -100 });
                }
            }
        }



        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        [HttpGet]
        public async Task<ActionResult> SearchProduct(string key, int? page)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(key.Trim().ToLower());
                var keyLower = key.Trim().ToLower();

                var productCategoryid = await db.ProductCategory
                    .Where(x => x.Title.Contains(keyLower) || x.Alias.Contains(alias))
                    .Select(x => x.ProductCategoryId)
                    .ToListAsync();

                var productCompanyid = await db.ProductCompany
                    .Where(x => x.Title.Contains(keyLower) || x.Alias.Contains(alias))
                    .Select(x => x.ProductCompanyId)
                    .ToListAsync();

               
                var query = db.Products.AsQueryable()
                    .Where(x =>
                        (!productCategoryid.Any() || productCategoryid.Contains((int)x.ProductCategoryId)) ||
                        (!productCompanyid.Any() || productCompanyid.Contains((int)x.ProductCompanyId))||
                        x.Title.Contains(keyLower));

               
                var totalCount = await query.CountAsync();

                
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var products = await query
                    .OrderByDescending(pd => pd.ProductsId)
                    .Skip((pageNumber - 1) * pageSize) 
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.Key = key.Trim();
                ViewBag.Count = totalCount; 

                return View(products.ToPagedList(pageNumber, pageSize)); 
            }

            return View();
        }




        [HttpPost]
        public async Task<ActionResult> IsActive(int id)
        {
            try
            {
                var item = await db.Products.FindAsync(id);
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
        public async Task<ActionResult> IsHot(int id)
        {
            try
            {
                var item = await db.Products.FindAsync(id);

                if (item != null)
                {
                    item.IsHot = !item.IsHot;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Json(new { success = true, isHot = item.IsHot });
                }

                return Json(new { success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Lỗi cập nhập tag hiển thị trang chủ" });
            }
        }





    }
}