using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class SellerController : Controller
    {
        // GET: Admin/Seller



        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Nhân viên bán hàng", "Quản lý", "Quản trị viên")]
        public ActionResult Index()
        {

            var product = db.Products.ToList();
            ViewBag.Product = new SelectList(product, "ProductsId", "Title");




            ViewBag.Date=DateTime.Now.Date;
            return View();
        }

        public async Task<ActionResult> Partail_ProductById(int productId)
        {
            if (productId>0&&db.ProductDetail.Any())
            {
                var pro =await db.ProductDetail.Where(x =>x.ProductsId==productId &&x.IsActive == true && x.Quantity > 0).ToListAsync();
                if (pro != null)
                {
                    ViewBag.Count = pro.Count();
                    return PartialView(pro);
                }
            }
            return PartialView();

        }

        public ActionResult Partail_Product()
        {
            if (db.ProductDetail.Any())
            {
                var pro =db.ProductDetail.Where(x=>x.IsActive==true&& x.Quantity>0).ToList();
                if (pro != null)
                {
                    ViewBag.Count=pro.Count();    
                    return PartialView(pro);    
                }
            }
            return PartialView();   

        }









        public ActionResult Partial_ListProductBill()
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null && cart.Items.Any())
            {

                
                return PartialView(cart.Items);
            }
            return PartialView();

        }

        [HttpGet]
        public async Task<JsonResult> GetTotalPrice()
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null && cart.Items.Any())
            {
                var totalPrice = cart.GetPriceTotal();
                var formattedPrice = CuaHangBanDienThoai.Common.Common.FormatNumber(totalPrice);
                return Json(new { TotalPrice = formattedPrice+"đ" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult>UpdateQuantity(int productid,int quantity)
        {
            if(productid <=0 && quantity<=0 )
            {
                return Json(new { Success = false, Code = -2, msg = "Không tìm thấy sản phầm" });
            }
            try {
                SellerCart cart = (SellerCart)Session["SellerCart"];
                if (cart != null && cart.Items.Any())
                {
                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        cart.UpSoLuong(productid, quantity);

                        return Json(new { Success = true, Code = 1, msg = "Cập nhập số lượng thành công" });
                    }    
                }
                return Json(new { Success = false, Code = -2, msg = "Không có sản phầm trong hóa đơn" });
            }
            catch(Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
            }
        }
        [HttpPost]
        public async Task<ActionResult> FindCustomer(string input)
        {

            if (!String.IsNullOrEmpty(input))
            {
                var customer = await db.Customer.FirstOrDefaultAsync(x => x.PhoneNumber.ToLower().Trim() == input.ToLower().Trim());
                  if(customer != null)
                {
                    ViewBag.txt = input.Trim();
                    return PartialView(customer);
                }
            }

            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult>DeleteItemCart(int productid)
        {
            try
            {
                if (productid <= 0)
                {
                    return Json(new { Success = false, Code = -2, msg = "Không tìm thấy sản phầm" });
                }
                SellerCart cart = (SellerCart)Session["SellerCart"];
                if (cart != null && cart.Items.Any())
                {
                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        var Name = (existingItem.ProductDetail.Products.Title.Trim() + " " + existingItem.ProductDetail.Capacity.Trim()) ?? "PadaMiniStore";
                        if (existingItem.SoLuong == 10)
                        {
                            return Json(new { Code = -2, Success = false, msg = "Số lượng " + Name + " tới giới hạn" });
                        }
                        cart.Remove(productid);
                        int count = cart.Items.Count();
                        return Json(new { Success = true, Code = 1, msg = "Xóa thành công", Count = count });

                    }
                
                }
                return Json(new { Success = false, Code = -2, msg = "Không có sản phầm trong hóa đơn" });
            }
              
            catch(Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
            }
        }

        [HttpPost]
        public async Task<ActionResult>AddBill(int productid, int quantity)
        {
            if (productid == 0)
            {
                return Json(new { Code = -2, Success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
           
            try
            {
                var checkSanPham =await db.ProductDetail.FirstOrDefaultAsync(row => row.ProductDetailId == productid);
                if (checkSanPham != null)
                {

                    var Name = (checkSanPham.Products.Title.Trim() + " " + checkSanPham.Ram.Trim() + "/" + checkSanPham.Capacity.Trim()) ?? "PadaMiniStore";
                  
                    SellerCart cart = (SellerCart)Session["SellerCart"];
                    if (cart == null)
                    {
                        cart = new SellerCart();
                    }

                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        if ( existingItem.SoLuong == 10)
                        {
                            return Json(new { Code = -2, Success = false, msg = "Số lượng " + Name+" tới giới hạn" });
                        }
                        existingItem.SoLuong += quantity;
                        existingItem.PriceTotal = existingItem.SoLuong * existingItem.Price;
                    }
                   
                    else
                    {
                        SellerCartItem item = new SellerCartItem
                        {
                            ProductDetailId = checkSanPham.ProductDetailId,
                          
                            SoLuong = quantity, 
                            ProductDetail = checkSanPham,
                           
                        };



                        item.Price = (decimal)checkSanPham.Price;

                        if (checkSanPham.PriceSale > 0)
                        {
                            item.Price = (decimal)checkSanPham.PriceSale;
                            item.PriceTotal = item.SoLuong * item.Price;
                        }
                        else
                        {
                            item.PriceTotal = item.SoLuong * item.Price;
                        }

                        cart.AddToCart(item, quantity);
                    }

                    int count = cart.Items.Count();
                    Session["SellerCart"] = cart;
                    return Json(new { Code = 1, Success=true, msg = "Thêm vào hoá đơn thành công" ,Count=count});
                }
                else
                {
                    return Json(new { Code = -2, Success = false, msg = "Không tìm thấy sản phẩm" });
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { Code = -99, Success = false, msg = "Hệ thống tạm ngưng" });
               
            }
            
        }
    }
}