using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.Threading.Tasks;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ProductCategoryController : Controller
    {
        // GET: Admin/ProductCategory
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.ProductCategory.OrderByDescending(x => x.ProductCategoryId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.ProductCategory.ToList();

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
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(ProductCategory model, string Title, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Title == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập tên danh mục" });
                    }
                    string alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(Title.Trim());

                    var item = await db.ProductCategory.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
                    if (item != null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Danh mục đã tồn tại" });
                    }

                    if (Images == null || Images.Count == 0)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn Icon danh mục" });
                    }



                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    model.Icon = Images[0];
                    model.Title = Title;
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = checkStaff.NameEmployee.Trim();
                    model.Alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(Title.Trim());
                    db.ProductCategory.Add(model);
                    db.SaveChanges();
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




        [AuthorizeFunction("Quản trị viên")]
        public async Task<ActionResult> Edit(string alias)
        {
            if (alias == null)
            {
                return View();
            }
            var item = await db.ProductCategory.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (item != null)
            {
                ViewBag.Name = item.Title.Trim();
                return View(item);
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategory model, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var existingcategory = db.ProductCategory.Find(model.ProductCategoryId);
                        if (existingcategory == null)
                        {



                            return Json(new { Success = false, Code = -2, msg = "Danh mục đã tồn tại" });
                        }

                        existingcategory.Alias = Models.Common.Filter.FilterChar(model.Title.Trim());
                        AccountEmployee nvSession = (AccountEmployee)Session["user"];
                        var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);

                        existingcategory.Modifiedby = checkStaff.NameEmployee.Trim();

                        existingcategory.ModifiedDate = DateTime.Now;

                        existingcategory.Title = model.Title;

                        existingcategory.CreatedDate = model.CreatedDate;
                        existingcategory.CreatedBy = model.CreatedBy;
                        existingcategory.Alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(model.Title.Trim());


                        if (!string.IsNullOrEmpty(model.Icon))
                        {
                            existingcategory.Icon = model.Icon;
                        }

                        db.Entry(existingcategory).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();

                        return Json(new { Success = true, Code = 1 });
                    }
                    else
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { Success = false, Code = -2, msg = "Vui nhập đủ thông tin" });
                    }

                }
                catch (Exception ex)
                {

                    dbContextTransaction.Rollback();

                    Console.WriteLine(ex.Message);
                    return Json(new { Success = false, Code = -100 });

                }
            }
        }


    }
}