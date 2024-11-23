using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using CuaHangBanDienThoai.Models;
using DocumentFormat.OpenXml.Wordprocessing;

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

        public ActionResult Partail_ReviewByProductId(int? productid)
        {
            ViewBag.Date = DateTime.Now;
            ViewBag.Count = 0; 
            string name = "Cửa hàng điện thoại";
            if (Session["CustomerId"] != null)
            {
                ViewBag.CustomerId = (int)Session["CustomerId"];
            }
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


        public ActionResult Partial_EditReview(int reviewid , int customerid, string name, string image)
        {
            if (reviewid > 0 && customerid> 00 && name != null && image != null)
            {
                var review = db.Review.FirstOrDefault(x => x.ReviewId == reviewid && x.CustomerId == customerid);
                if (review != null)
                {
                    Client_EditReview viewModel = new Client_EditReview
                    {
                        ReviewId = review.ReviewId, 
                        CreatedDate = review.CreatedDate,
                        ProductsId = review.ProductId,
                        CustomerId = review.CustomerId,
                        Ratingscore = review.Ratingscore,
                        Content = review.Content?.Trim(),   
                        Name = name.Trim(),
                        Image = image.Trim(),
                    };
                    return PartialView(viewModel);
                }
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(Client_EditReview req)
        {
            if (Session["customer"] == null && Session["CustomerId"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập của bạn đã hết hạn !!!" });
            }
            if (req.ProductsId <= 0 )
            {
                return Json(new { Success = false, Code = -4, msg = "Lỗi lấy mã sản phẩm" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try 
                {
                    int customerId = (int)Session["CustomerId"];
                  
                    var customer=await db.Customer.FirstOrDefaultAsync(x=>x.CustomerId== customerId);
                    string Name = (customer!=null? customer.CustomerName?.Trim():req.Name?.Trim());
                    var review = await db.Review.FirstOrDefaultAsync(x => x.ReviewId == req.ReviewId && x.CustomerId == req.CustomerId);
                    if (review == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy bình luận !!!" });
                    }
                    review.Content = req.Content?.Trim();
                    review.Ratingscore=req.Ratingscore;
                    review.ModifiedDate=DateTime.Now;
                    db.Entry(review).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Cập nhập thành công , cảm ơn bạn " + Name });
                        }
                catch(Exception ex)
                {
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Comment(Review model ,Client_AddReview req)
        {
            if (Session["customer"] == null && Session["CustomerId"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập của bạn đã hết hạn !!!" });
            }
            if (req.ProductsId<=0 ) 
            {
                return Json(new { Success = false, Code = -4, msg = "Lỗi lấy mã sản phẩm" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    int customerId = (int)Session["CustomerId"];
                    var customer = await db.Customer.FirstOrDefaultAsync(x => x.CustomerId == customerId);
                    string Name = (customer != null ? customer.CustomerName?.Trim() : req.Name?.Trim());


                    model.ProductId = req.ProductsId;
                    model.Content=req.Content.Trim();
                    model.Ratingscore = req.Ratingscore;
                    model.CustomerId = (int)Session["CustomerId"];
                    model.CreatedDate = DateTime.Now;   
                    db.Review.Add(model);
                    await db.SaveChangesAsync();
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Càm ơn "+Name+" đã để lại đánh giá " });
                }
                catch (Exception ex) 
                {
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
            }
        }







    }

}