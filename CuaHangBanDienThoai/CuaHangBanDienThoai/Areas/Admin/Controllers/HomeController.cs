using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        [AuthorizeFunction("Quản trị viên", "Quản lý")]
        public ActionResult Index()
        {
            return View();
        }
    }
}