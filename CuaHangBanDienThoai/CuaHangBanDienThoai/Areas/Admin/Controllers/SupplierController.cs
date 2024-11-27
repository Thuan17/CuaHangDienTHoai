using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using PagedList;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Admin/Supplier
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.Supplier.OrderByDescending(x => x.SupplierId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.ProductDetail.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
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

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Add()
        {
            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Supplier model, Admin_AddSupplier req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đủ thông tin !!!" });
            }

            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var normalizedPhone = req.Phone.Trim();
                    var normalizedName = req.Name.Trim();

                
                    var existingSupplier = await db.Supplier
                        .FirstOrDefaultAsync(x => x.Phone == normalizedPhone || x.Name == normalizedName);

                    if (existingSupplier != null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Nhà cung cấp đã tồn tại !!!" });
                    }

                  
                    model.Location = req.Location.Trim();
                    model.Phone = normalizedPhone;
                    model.Name = normalizedName;

                    db.Supplier.Add(model);
                    await db.SaveChangesAsync();

                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Thêm nhà cung cấp thành công" });
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!", error = ex.Message });
                }
            }
        }

        public ActionResult Detail(int? id)
        {
            if(id!=null && id > 0)
            {
                var sup = db.Supplier.Find(id);
                if (sup != null)
                {
                    Admin_EditSupplier viewModel = new Admin_EditSupplier
                    {
                        SupplierId= sup.SupplierId,
                        Name = sup.Name.Trim(),
                        Phone = sup.Phone.Trim(),
                        Location = sup.Location.Trim(),

                    };
                    if (viewModel != null)
                    {
                        return PartialView(viewModel);  
                    }
                }
            }
            return PartialView();
        }
        public ActionResult Edit(int? id)
        {
            if (id != null && id > 0)
            {
                var sup = db.Supplier.Find(id);
                if (sup != null)
                {
                    Admin_EditSupplier viewModel = new Admin_EditSupplier
                    {
                        SupplierId = sup.SupplierId,
                        Name = sup.Name.Trim(),
                        Phone = sup.Phone.Trim(),
                        Location = sup.Location.Trim(),

                    };
                    if (viewModel != null)
                    {
                        return PartialView(viewModel);
                    }
                }
            }
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Admin_EditSupplier req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đủ thông tin !!!" });
            }

            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var normalizedPhone = req.Phone.Trim();
                    var normalizedName = req.Name.Trim();

                    var existingSupplier = await db.Supplier
                        .FirstOrDefaultAsync(x => (x.Phone == normalizedPhone || x.Name == normalizedName) && x.SupplierId != req.SupplierId);

                    if (existingSupplier != null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Nhà cung cấp đã tồn tại !!!" });
                    }

                  
                    var supplier = await db.Supplier
                        .FirstOrDefaultAsync(x => x.SupplierId == req.SupplierId);

                    if (supplier == null)
                    {
                        return Json(new { Success = false, Code = -3, msg = "Nhà cung cấp không tồn tại !!!" });
                    }

                   
                    bool isModified =
                        supplier.Name.Trim() != req.Name.Trim() ||
                        supplier.Phone.Trim() != req.Phone.Trim() ||
                        supplier.Location.Trim() != req.Location.Trim();

                    if (!isModified)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không có thay đổi nào !!!" });
                    }

                 
                    supplier.Name = normalizedName;
                    supplier.Phone = normalizedPhone;
                    supplier.Location = req.Location.Trim();

                    db.Entry(supplier).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Cập nhập nhà cung cấp thành công" });
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!", error = ex.Message });
                }
            }
        }



    }
}