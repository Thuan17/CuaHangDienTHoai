using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ProductCompanyController : Controller
    {
        // GET: Admin/ProductCompany

    private     CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]

        public ActionResult Index(int? page )
        {
            var items = db.ProductCompany.OrderByDescending(x => x.ProductCompanyId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.ProductCompany.ToList();

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
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(ProductCompany model, Admin_AddProductCompany req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Images == null || Images.Count == 0)
                    {
                        return Json(new { Success = false,Code =-2,msg="Vui lòng chọn logo hãng"});
                    }
                    if (req.Title == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập tên hãng" });
                    }


                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    model.Icon = Images[0];
                    model.Title = req.Title;
                    model.CreatedDate=DateTime.Now;
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

    }
}