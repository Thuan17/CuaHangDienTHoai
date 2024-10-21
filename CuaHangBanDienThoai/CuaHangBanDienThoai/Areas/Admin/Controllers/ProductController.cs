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
        public async Task<ActionResult> Add(ProductCompany model, Admin_AddProduct req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (req.Title == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập tên hãng" });
                    }
                    string alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim());

                    var item = await db.ProductCompany.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
                    if (item != null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Hãng đã tồn tại" });
                    }

                    if (Images == null || Images.Count == 0)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn logo hãng" });
                    }



                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    model.Icon = Images[0];
                    model.Title = req.Title;
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = checkStaff.NameEmployee.Trim();
                    model.Alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim());
                    db.ProductCompany.Add(model);
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

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public async Task<ActionResult> Detail(string alias)
        {
            var item = await db.ProductCompany.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (item != null)
            {
                ViewBag.Name = item.Title.Trim();
                return View(item);
            }
            return View();

        }

    }
}