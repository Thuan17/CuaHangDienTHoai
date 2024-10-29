using CuaHangBanDienThoai.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PartialCartLayout(int? id)
        {
            if (id <= 0)
            {
                return PartialView();  
            }
            var cart = db.Cart.FirstOrDefault(x => x.CustomerId == id);
            if (cart==null)
            {
                return PartialView();
            }
            var cartDetail = db.CartItem.Where(x => x.CartId == cart.CartId).ToList();
            if (cartDetail == null)
            {
                return PartialView(cartDetail);
                
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddtoCart(int id, int quantity)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Session["CustomerId"] != null)
                    {

                        int idKhach = (int)Session["CustomerId"];

                        var checkIdCart = db.Cart.FirstOrDefault(x => x.CustomerId == idKhach);

                        if (checkIdCart != null)
                        {

                            int checkId = checkIdCart.CartId;
                            var checkIdCartItem = db.CartItem.SingleOrDefault(ci => ci.CartId == checkId && ci.ProductDetailId == id);

                            if (checkIdCartItem != null)
                            {
                                checkIdCartItem.Quantity += quantity;
                                checkIdCartItem.TemPrice = checkIdCartItem.Price * checkIdCartItem.Quantity;
                                db.SaveChanges();
                            }
                            else
                            {
                                var productDetail = db.ProductDetail.Find(id);
                                if (productDetail != null)
                                {

                                    CartItem cartitem = new CartItem
                                    {
                                        CartId = checkId,
                                        ProductDetailId = productDetail.ProductDetailId,
                                        Quantity = quantity,

                                        Price = (decimal)productDetail.Price,
                                        TemPrice = (decimal)productDetail.PriceSale,
                                        PriceTotal = (decimal)productDetail.Price * quantity
                                    };
                                    db.CartItem.Add(cartitem);
                                    db.SaveChanges();

                                }
                                else
                                {
                                return Json( new { Success = false, msg = "Sản phẩm không tồn tại!", code = -2 });
                                    
                                }
                            }
                            dbContextTransaction.Commit();  
                            return Json(new { Success = true, msg = "Đã thêm vào giỏ hàng", code = 1 });

                        }
                        else
                        {
                            return PartialView("Cartnull");
                        }

                    }
                    else
                    {
                        Session["redirectUrl"] = Request.Url.ToString();
                        return Json(new { Success = false, msg = "Vui lòng đăng nhập để tiếp tục!", code = -1 });
                    }
                  
                }
                catch(Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { Success = false, msg = "Hệ thống tạm đóng", code = -99 });
                }
                
              }
        }
        public ActionResult ShowCount()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];

                var checkIdCart = db.Cart.FirstOrDefault(x => x.CustomerId == idKhach);

                if (checkIdCart != null)
                {
                    int checkId = checkIdCart.CartId;

                    var cartItem = db.CartItem.Where(row => row.CartId == checkId).Count();
                    return Json(new { Count = cartItem }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }

    }
}