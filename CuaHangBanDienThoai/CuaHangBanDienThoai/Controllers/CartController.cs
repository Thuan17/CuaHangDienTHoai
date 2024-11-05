using CuaHangBanDienThoai.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CuaHangBanDienThoai.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        public ActionResult Index(int? id)
        {
            if (Session["CustomerId" ]==null&& id == null && id <= 0)
            {
                return PartialView();
            }
            if(Session["CustomerId"] != null&&id == null)
            {
                id = (int)Session["CustomerId"];
            }
            
            var cart = db.Cart.FirstOrDefault(x => x.CustomerId == id);
            if (cart == null)
            {
                return PartialView();
            }
            var cartDetail = db.CartItem.Where(x => x.CartId == cart.CartId).ToList();
            if (cartDetail != null)
            {
                return PartialView(cartDetail);

            }
            return PartialView();
        }
        public ActionResult Partial_Index(int? id)
        {


            if (Session["CustomerId"] == null && id == null && id <= 0)
            {
                return PartialView();
            }
            if (Session["CustomerId"] != null && id == null)
            {
                id = (int)Session["CustomerId"];
            }

            var cart = db.Cart.FirstOrDefault(x => x.CustomerId == id);
            if (cart == null)
            {
                return PartialView();
            }
            if (id != null && id > 0)
            {
                var cartDetail = db.CartItem.Where(x => x.CartId == cart.CartId).ToList();
                if (cartDetail != null)
                {
                    return PartialView(cartDetail);

                }
                return PartialView();
            }
            return PartialView();
        }
     
        [HttpGet]
        public JsonResult GetPrice(int cartitemid)
        {

            if (cartitemid <= 0)
            {
                return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
            }
            var cartId = db.CartItem.FirstOrDefault(x => x.CartItemId == cartitemid);
            if (cartId == null)
            {
                return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
            }
            var cart = db.Cart.FirstOrDefault(x => x.CartId == cartId.CartId);
            if (cartId == null)
            {
                return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
            }

            decimal totalPrice = 0;
            decimal price = 0;

            var cartItems = db.CartItem.Where(x => x.CartId == cartId.CartId).ToList();

            if ( cart.CartItem.Any())
            {

               foreach ( var item in cartItems)
                {
                    price = ((decimal)item.ProductDetail.PriceSale > 0 ? (decimal)item.ProductDetail.PriceSale : (decimal)item.ProductDetail.Price);
                    totalPrice += price * item.Quantity;
                }
               
                var formattedPrice = CuaHangBanDienThoai.Common.Common.FormatNumber(totalPrice);
                return Json(new { TotalPrice = formattedPrice }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Partial_Item_Cart_Small(int? id)
        {
            if (id==null&&id <= 0)
            {
                return PartialView();
            } 
            var cart = db.Cart.FirstOrDefault(x => x.CustomerId == id);
            if (cart == null)
            {
                return PartialView();
            }
            var cartDetail = db.CartItem.Where(x => x.CartId == cart.CartId).ToList();
            if (cartDetail != null)
            {
                return PartialView(cartDetail);

            }
            return PartialView();
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
            if (cartDetail != null)
            {
                return PartialView(cartDetail);
                
            }
            return PartialView();
        }


        [HttpPost]
        public async Task<ActionResult> UpdateQuantity(int cartitemid, int productdetailid, int quantity)
        {
            if (Session["customer"] == null || Session["CustomerId"] == null)
            {
                return Json(new { Code = -2, msg = "Vui lòng đăng nhập", url = "/dang-nhap" });
            }

            if (cartitemid <= 0 || productdetailid <= 0 || quantity <= 0)
            {
                return Json(new { Success = false, Code = -2, msg = "Thông tin không hợp lệ" });
            }

            try
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    int customerId = (int)Session["CustomerId"];

                    var cart = await db.Cart.FirstOrDefaultAsync(x => x.CustomerId == customerId);
                    if (cart == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy giỏ hàng của bạn" });
                    }

                    var cartItem = await db.CartItem.FirstOrDefaultAsync(
                        x => x.CartItemId == cartitemid && x.ProductDetailId == productdetailid);

                    if (cartItem == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Sản phẩm không tồn tại trong giỏ hàng" });
                    }

                   
                    int newQuantity =  quantity;
                    if (newQuantity <= 0)
                    {
                        db.CartItem.Remove(cartItem); 
                    }
                    else
                    {
                        cartItem.Quantity = newQuantity;
                        db.Entry(cartItem).State = EntityState.Modified;
                    }

                    await db.SaveChangesAsync();
                    dbContextTransaction.Commit();

                    return Json(new { Success = true, Code = 1, newQuantity });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Lỗi hệ thống: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult>DeleteCartItem(int cartitemid, int productdetailid)
        {
            if (Session["customer"] == null || Session["CustomerId"] == null)
            {
                return Json(new { Code = -2, msg = "Vui lòng đăng nhập", url = "/dang-nhap" });
            }

            if (cartitemid <= 0 || productdetailid <= 0 )
            {
                return Json(new { Success = false, Code = -2, msg = "Thông tin không hợp lệ" });
            }
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try 
                {
                    int customerId = (int)Session["CustomerId"];

                    var cart = await db.Cart.FirstOrDefaultAsync(x => x.CustomerId == customerId);
                    if (cart == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy giỏ hàng của bạn" });
                    }

                    var cartItem = await db.CartItem.FirstOrDefaultAsync(
                        x => x.CartItemId == cartitemid && x.ProductDetailId == productdetailid);

                    if (cartItem == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Sản phẩm không tồn tại trong giỏ hàng" });
                    }
                    db.CartItem.Remove(cartItem);
                    await db.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Xoá sản phẩm thành công" });

                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Lỗi hệ thống: " + ex.Message });


                }     
            }
        }


        public ActionResult MyOrder(int? id)
        {
            if (Session["CustomerId"] == null && id == null && id <= 0)
            {
                return View();
            }
            if (Session["CustomerId"] != null && id == null)
            {
                id = (int)Session["CustomerId"];
            }

            var order = db.OrderCustomer.FirstOrDefault(x => x.CustomerId == id);
            if (order != null)
            {
                return View(order);
            }
           
            return View();
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