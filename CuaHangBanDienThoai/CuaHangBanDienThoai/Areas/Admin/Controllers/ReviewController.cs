using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpPost]
        public async Task<ActionResult> Delete(int reviewid, int productid)
        {
            if(reviewid <=0 && productid <= 0)
            {
                return Json(new { Success = false, msg = "Không tìm thấy mã ", Code = -2 });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var review = await db.Review.FirstOrDefaultAsync(x => x.ReviewId == reviewid && x.ProductId == productid);
                    if (review == null)
                    {
                        return Json(new { Success = false, msg = "Không tìm thấy đánh giá này ", Code = -2 });
                    }
                    db.Review.Remove(review);
                    await db.SaveChangesAsync();
dbContext.Commit();
                    return Json(new { Success = true, msg = "Xoá thành công đánh giá", Code = 1 });
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, msg = "Hệ thống tạm ngưng ", Code = -99 });
                }
             
            }
           
        }
    }

   



}