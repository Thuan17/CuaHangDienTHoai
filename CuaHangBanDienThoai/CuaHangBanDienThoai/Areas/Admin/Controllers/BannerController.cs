using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Threading.Tasks;
using System.Data.Entity;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class BannerController : Controller
    {
        // GET: Admin/Banner
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            IEnumerable<Banner> items = db.Banner.OrderByDescending(x => x.BannerId);

            if (items != null)
            {
                var pageSize = 10;
                if (page == null)
                {
                    page = 1;
                }
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                items = items.ToPagedList(pageIndex, pageSize);
                var banner = db.Banner.ToList();

                ViewBag.Count = banner.Count;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return PartialView();
            }
        }

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Add()
        {
            return View();

        }
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Add(Banner model, Admin_AddBanner req, List<string> Images)
        {
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                   
                    if (Images == null)
                    {
                        return Json(new { success = false, msg = "Vui lòng chọn ảnh", Code = -3 });
                    }
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);

                    model.Link = req.Link ?? "";
                    model.CreatedBy = checkStaff.NameEmployee;
                    model.CreatedDate = DateTime.Now;
                    model.IsBackground = req.IsBackground;
                    model.IsActive = req.IsActive;

                    model.Image = Images[0];
                    db.Banner.Add(model);
                    db.SaveChanges();
                    dbContext.Commit();
                    return Json(new { success = true, Code = 1 });



                }
                catch (Exception e)
                {
                    dbContext.Rollback();
                    return Json(new { success = false, Code = -99 });
                }
            }
        }
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
    
        public ActionResult Edit(int? id)
        {
            try
            {
               
                var banner = db.Banner.Find(id);
                if (banner == null)
                {
                    ViewBag.ErrorMessage = "Lỗi hệ thống khôing tìm thấy";
                    return PartialView();
                }
                Admin_EditBanner viewModel = new Admin_EditBanner
                {
                    BannerId = banner.BannerId,

                    CreatedBy = banner.CreatedBy.Trim(),
                    CreatedDate = banner.CreatedDate,
                    IsActive = (bool)banner.IsActive,
                    Image = banner.Image,
                    Link = banner.Link,
                    IsBackground = (bool)banner.IsBackground,

                };
                return PartialView(viewModel);


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi hệ thống" + "" + ex.Message;


                return Json(new { success = false, Code = -99, msg = "Lỗi hệ thống" + "" + ex.Message });
            }

        }
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Edit(Admin_EditBanner req, List<string> Images)
        {
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {

                    if (Session["user"] == null)
                    {
                        return Json(new { success = false, Code = -2, msg = "Phiên đăng nhập hết hạn ", url = "/he-thong-nhan-vien" });
                    }
                    var banner = db.Banner.Find(req.BannerId);
                    if (banner == null)
                    {
                        ViewBag.ErrorMessage = "Lỗi hệ thống khôing tìm thấy";
                        return PartialView();
                    }
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);

                  
                    banner.BannerId = req.BannerId;

                    banner.CreatedBy = req.CreatedBy.Trim();
                    banner.CreatedDate = req.CreatedDate;
                    banner.IsActive = (bool)req.IsActive;
                    if (Images != null && Images.Count > 0)
                    {
                        banner.Image = Images[0];
                    }
                    else
                    {
                        banner.Image = req.Image.Trim();
                    }

                    banner.Link = req.Link ?? "";
                    banner.IsBackground = (bool)req.IsBackground;
                    db.Entry(banner).State = EntityState.Modified;
                    db.SaveChanges();
                    dbContext.Commit();
                    return Json(new { success = true, Code = 1, msg = "Sửa thành công Banner", url = "/quanly-banner" });



                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { success = false, Code = -99, msg = "Lỗi hệ thống" + "" + ex.Message });

                }

            }
        }
     









        [HttpPost]

        public async Task<ActionResult> Delete(int id)
        {
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    if (Session["user"] == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        if (id <= 0)
                        {
                            return Json(new { success = false, msg = "Lỗi lấy sản phẩm" });
                        }
                        var banner = db.Banner.Find(id);
                        if (banner == null)
                        {
                            return Json(new { success = false, msg = "Lỗi lấy sản phẩm" });
                        }
                        db.Banner.Remove(banner);
                        await db.SaveChangesAsync();


                        dbContext.Commit();


                        return Json(new { success = true, msg = "Xoá thành công" });

                    }
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { success = false, Code = -99, msg = "Lỗi hệ thống" + "" + ex.Message });

                }
            }


        }





        [HttpPost]
        public async Task<ActionResult> IsActive(int id)
        {
            var item = db.Banner.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.IsActive });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<ActionResult> IsBackground(int id)
        {
            var item = db.Banner.Find(id);
            if (item != null)
            {
                item.IsBackground = !item.IsBackground;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, isBackground = item.IsBackground });
            }

            return Json(new { success = false });
        }

    }
}