using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Admin/Employee
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.Employee.OrderByDescending(x => x.EmployeeId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var emp = db.Employee.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsManage = isManage;
                ViewBag.Count = emp.Count;
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
            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsManage = isManage;

            var functions = db.tb_Function.ToList();
            if (!isAdmin)
            {
               
                functions = functions.Where(f => f.TitLe.Trim() != "Quản trị viên"&& f.TitLe.Trim() != "Quản lý").ToList();
                if (!isManage)
                {
                    functions = functions.Where(f => f.TitLe.Trim() != "Quản lý"&& f.TitLe.Trim() != "Quản trị viên").ToList();
                }
            }
            ViewBag.Function = new SelectList(functions, "FunctionId", "Title");
            return View();
        }



    }
}