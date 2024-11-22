using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;

namespace CuaHangBanDienThoai.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Partial_AddReview(int? productid ,string name,string image)
        {
            if (productid != null && productid > 0&& name!=null&& image!=null)
            {
                var product = db.Products.Find(productid);
                if (product != null)
                {
                    Client_AddReview viewModel = new Client_AddReview
                    {
                        ProductsId = product.ProductsId,
                        Name = name.Trim(),
                        Image= image.Trim(),
                    };
                    return PartialView(viewModel);
                }
            }
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Comment(Review model ,Client_AddReview req)
        {
            if (Session["customer"] == null && Session["CustomerId"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập của bạn đã hết hạn !!!" });
            }
                if (!ModelState.IsValid) 
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn đầy đủ thông tin !!!" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try 
                {
                    model.ProductId = req.ProductsId;
                    model.Content=req.Content.Trim();
                    model.Ratingscore = req.Ratingscore;
                    model.CustomerId = (int)Session["CustomerId"];
                    model.CreatedDate = DateTime.Now;   
                    db.Review.Add(model);
                    await db.SaveChangesAsync();
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Càm ơn bạn đã để lại đánh giá " });
                }
                catch (Exception ex) 
                {
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
            }
        }







    }

}