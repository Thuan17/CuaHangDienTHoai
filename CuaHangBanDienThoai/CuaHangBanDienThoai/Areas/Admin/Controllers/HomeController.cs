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
        [AuthorizeFunction("Quản lý", "Quản trị viên" )]
        public ActionResult Index()
        {
            return View();
        }
    }
}