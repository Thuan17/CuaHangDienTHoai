using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Admin/Review
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeFunction("Nhân viên tư vấn", "Nhân viên bán hàng", "Quản lý", "Quản trị viên")]
        public ActionResult Partail_ReviewByProductId(int? productid)
        {

            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý"); 
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsManage = isManage;


            ViewBag.Date = DateTime.Now;
            ViewBag.Count = 0;
            string name = "Cửa hàng điện thoại";
          
            if (productid != null && productid > 0)
            {
                var review = db.Review
                               .Where(x => x.ProductId == productid)
                               .OrderByDescending(x => x.ReviewId)
                               .ToList();
                var pro = db.Products.Find(productid);
                name = pro != null
                    ? $"{pro.ProductCategory.Title?.Trim()}  {pro.Title?.Trim()}"
                    : name;

                ViewBag.Name = name.Trim();

                if (review != null && review.Count > 0)
                {
                    ViewBag.Count = review.Count;
                    return PartialView(review);
                }
            }

            ViewBag.Name = name;
            return PartialView();
        }


    }
}