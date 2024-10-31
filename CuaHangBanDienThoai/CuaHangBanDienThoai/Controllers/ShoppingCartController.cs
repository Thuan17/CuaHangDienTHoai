using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckOut()
        {
            if(Session["CustomerId"]==null&& Session["customer"] == null)
            {
                return View();
            }

            int customerId = (int)Session["CustomerId"];

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
                ViewBag.CustomerId = customerId;
                return View(cart);
            }
            return View();
          

        }

        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        //public ActionResult Partial_ThongTinKhachHangData()
        //{
        //    if (Session["CustomerId"] == null && Session["customer"] == null)
        //    {
        //        return View();
        //    }

        //    int customerId = (int)Session["CustomerId"];
        //    if (customerId > 0)
        //    {
        //        var checkAddress = (from ci in db.AddressCusomer
        //                            join pd in db.Customer on ci.CustomerId equals pd.CustomerId
        //                            where ci.CustomerId == customerId
        //                            select ci).FirstOrDefault();
        //        ShoppingCart cart = (ShoppingCart)Session["Cart"];
        //        if (cart != null && cart.Items.Any())
        //        {
        //            if (checkAddress != null)
        //            {
        //                ViewBag.TotalPrice = cart.GetTotalPrice();
        //                return PartialView(checkAddress);
        //            }
        //            else
        //            {
        //                ViewBag.Provinces = new SelectList(db.Provinces.ToList(), "idProvinces", "name");
        //                return PartialView();
        //            }
        //        }
        //    }




        //    return PartialView();
        //}
        public ActionResult Partial_ThongTinKhachHang(int? addressid , int? customerid)
        {
            if (Session["CustomerId"] == null && Session["customer"] == null)
            {
                return View(); // No logged-in user
            }

            int customerId = (int)Session["CustomerId"];
            if (customerId > 0)
            {
                // Fetch customer and address data
                var customer = db.Customer.Find(customerId);
                var checkAddress = db.AddressCustomer.FirstOrDefault(a => a.CustomerId == customerId);
             
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {



                    ViewBag.TotalPrice = cart.GetTotalPrice();
                    if (checkAddress != null)
                    {
                        OrderView viewModel;

                        if (checkAddress.IsDefault==true)
                        {
                            viewModel = new OrderView
                            {
                                NameCustomer = checkAddress.CustomerName,
                                PhoneCustomer = checkAddress.PhoneNumber,
                                Email = customer.Email,
                                Location = checkAddress.Location
                            };

                            var CustomerAddresses = db.AddressCustomer
                             .Select(address => new OrderViewNonDefault
                             {
                                 AddressCusomerId = address.AddressCusomerId,
                                 CustomerId = (int)address.CustomerId,
                                 Location = address.Location,
                                 CustomerName = address.CustomerName,
                                 PhoneNumber = address.PhoneNumber,
                             }).ToList();
                            ViewBag.AddressCustomerNonDefault = CustomerAddresses;


                            return PartialView(viewModel);
                        }
                      

                       
                    }
                    else
                    {
                        ViewBag.Provinces = new SelectList(db.Provinces.ToList(), "idProvinces", "name");
                     return PartialView();
                    }

                }
            }
            ViewBag.Provinces = new SelectList(db.Provinces.ToList(), "idProvinces", "name");
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetPrice()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                var totalPrice = cart.GetTotalPrice();
                var formattedPrice = CuaHangBanDienThoai.Common.Common.FormatNumber(totalPrice);
                return Json(new { TotalPrice = formattedPrice }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult UpdateQuantitySession(int productdetailid, int quantity)// ko có tìa khoản 
        {
            try
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (productdetailid < 0 || quantity < 1)
                {
                    return Json(new { Success = false, msg = "Yêu cầu không hợp lệ !", Code = -2 });
                }

                if (cart != null)
                {
                    var checkProduct = cart.Items.FirstOrDefault(x => x.ProductDetailId == productdetailid);
                    if (checkProduct != null)
                    {
                        //checkProduct.Quantity= quantity;
                        cart.UpdateQuantity(productdetailid, quantity);
                        return Json(new { Success = true, msg = "Cập nhập số lượng thành công", Code = 1 });
                    }
                    else
                    {
                        return Json(new { Success = false, msg = "Không tồn tại sản phẩm", Code = -2 });
                    }
                }
                else
                {
                    return Json(new { Success = false, msg = "Không tồn tại sản phẩm", Code = -2 });
                }
            }
            catch (Exception ex) { return Json(new { Success = false, msg = "Không thể cập nhập số lượng !", Code = -99}); }

        }


        [HttpPost]
        public async Task< ActionResult> CheckOutProduct ( List <int> producdetailid)
        {

            try
            {
                if (Session["customer"] == null && Session["CustomerId"] == null)
                {
                    return Json(new { Success = false, Code = -3, msg = "Phiên đang nhập hết hạn" });
                }
                if (producdetailid.Count() <= 0)
                {
                    return Json(new { Success = false, Code = -2, msg = "Danh sách sản phẩm rỗng" });
                }
                if (producdetailid != null && producdetailid.Any())
                {
                    int idKhach = (int)Session["CustomerId"];
                    var checkIdCart = await db.Cart.FirstOrDefaultAsync(x => x.CustomerId == idKhach);
                    if (checkIdCart != null)
                    {
                        int checkId = checkIdCart.CartId;

                        ShoppingCart cart = (ShoppingCart)Session[""];
                        if (cart == null)
                        {
                            cart = new ShoppingCart();
                        }

                        foreach (var productId in producdetailid)
                        {
                            var cartItem = (from ci in db.CartItem
                                            join pd in db.ProductDetail on ci.ProductDetailId equals pd.ProductDetailId
                                            where ci.CartId == checkId && ci.ProductDetailId == productId
                                            select ci).FirstOrDefault();

                            if (cartItem != null)
                            {

                                ShoppingCartItem item = new ShoppingCartItem
                                {
                                    ProductDetailId = (int)cartItem.ProductDetailId,

                                    Quantity = cartItem.Quantity,
                                    ProductDetail = cartItem.ProductDetail,
                                };

                                if (cartItem.ProductDetail.ProductDetailImage.FirstOrDefault(x => x.IsDefault) != null)
                                {
                                    item.ProductImg = cartItem.ProductDetail.ProductDetailImage.FirstOrDefault(row => row.IsDefault).Image;
                                }

                                item.Price = (decimal)cartItem.ProductDetail.Price;
                                item.PriceSale = (decimal)cartItem.ProductDetail.PriceSale;
                                if (cartItem.ProductDetail.PriceSale > 0)
                                {
                                    item.PriceSale = (decimal)cartItem.ProductDetail.PriceSale;
                                    item.PriceTotal = item.Quantity * item.PriceSale;
                                }
                                else
                                {
                                    item.PriceTotal = item.Quantity * item.Price;
                                }

                                Session["Cart"] = cart;
                                cart.AddToCart(item, cartItem.Quantity);
                            }
                        }
                        return Json(new { Success = true,Code = 1, url = "/thanh-toan" });

                    }
                }
                return Json(new { Success = false, Code = -2, msg = "Danh sách sản phẩm rỗng" });

            }
            catch(Exception ex) 
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });

            }   
            


        }



    }
}