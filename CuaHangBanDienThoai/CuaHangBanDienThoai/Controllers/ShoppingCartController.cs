using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models.Payment;
using System.Text;

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
            if (Session["CustomerId"] == null && Session["customer"] == null)
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

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task < ActionResult> CheckOut(int customerid  ,OrderView req)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {

                if (customerid < 0 && Session["CustomerId"] == null)
                {
                    return Json(new { Success = false, Code = -3, msg = "Phiên đặng nhập đã hết hạn" });
                }


                try
                {
                    if (ModelState.IsValid)
                    {
                        if (Session["CustomerId"] != null)
                        {
                            int idKhach = (int)Session["CustomerId"];
                            if(idKhach!= customerid)
                            {
                                return Json(new { Success = false, Code = -2, msg = "Phiên đặng nhập đã hết hạn" ,Url="dang-nhap"});
                            }


                            var inforKhachHang =await db.Customer.FirstOrDefaultAsync(x => x.CustomerId == idKhach);
                            ShoppingCart cart = (ShoppingCart)Session["Cart"];

                            if (cart != null)
                            {
                                var insufficientItems = ProcessCartItems(cart, idKhach);
                                if (insufficientItems.Any())
                                {
                                    return Json(new { Success = false, Code = -4, InsufficientItems = insufficientItems.Select(i => new { i.ProductDetailId, i.ProductName }) });
                                }

                                var order = CreateOrder(cart, inforKhachHang, req.TypePaymentVN);

                                if (order == null)
                                {
                                    return Json(new { Success = false, Code = -2, msg="Đơn hàng thất bại" });
                                }


                                db.OrderCustomer.Add(order);
                                db.SaveChanges();
                                int OrderId = order.OrderId;
                                inforKhachHang.NumberofPurchases += 1;
                                db.Entry(inforKhachHang).State = EntityState.Modified;
                                db.SaveChanges();



                                if (cart.Code != null && cart.PercentPriceReduction > 0)
                                {

                                    if (! await UpdateVoucherDetail(cart.Code, order))
                                    {
                                        return Json(new { Success = false, Code = -2, msg = "Áp dụng voucher thất bại" });
                                    }
                                }


                                if (req.TypePaymentVN == 2)
                                {
                                    dbContextTransaction.Commit();
                                    var url = UrlPayment(req.TypePaymentVN, order.Code);
                                   

                                    return Json(new { Success = true, Code = req.TypePaymentVN, Url = url });
                                }
                                else
                                {
                                    SendConfirmationEmails(cart, order, inforKhachHang);


                                    cart.ClearCart();

                                    dbContextTransaction.Commit();
                                    string orderCode = order.Code;
                                    return Json(new { Success = true, Code = 1, Url = $"muahangthanhcong/{orderCode}", OrderCode = orderCode });
                                }
                            }
                            else
                            {
                                return Json(new {Success=false , Code =-2,msg="Vui lòng chọn sản phẩm",Url="gio-hang" });
                            }
                        }
                        else
                        {
                            return Json(new { Success = false, Code = -2, msg = "Phiên đặng nhập đã hết hạn", Url = "dang-nhap" });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, Code = -2 ,msg="Vui lòng điền đủ thông tin"});
                    }
                  
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { Success = false, Code = -99 });
                }
            }
        }
        #region/* Thanh toán vnpay*/

        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.OrderCustomer.FirstOrDefault(x => x.Code == orderCode);

            if (order == null) // Kiểm tra xem đơn hàng có tồn tại không
            {
                return ""; // Trả về chuỗi rỗng nếu không tìm thấy đơn hàng
            }

            // Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; // URL nhận kết quả trả về 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; // URL thanh toán của VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; // Mã định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; // Secret Key

            // Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString());
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code);

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return urlPayment;
        }

        #endregion

        public ActionResult VnpayReturn()
        {


        
            try
            {
                if (Request.QueryString.Count > 0)
                {
                    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; // Chuỗi bí mật
                    var vnpayData = Request.QueryString;
                    VnPayLibrary vnpay = new VnPayLibrary();

                    foreach (string s in vnpayData)
                    {
                        // Lấy tất cả dữ liệu truy vấn querystring
                        if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(s, vnpayData[s]);
                        }
                    }

                    string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                    string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                    string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                    String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                    String TerminalID = Request.QueryString["vnp_TmnCode"];
                    long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                    String bankCode = Request.QueryString["vnp_BankCode"];

                    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                    if (checkSignature)
                    {
                        var itemOrder = db.OrderCustomer.FirstOrDefault(x => x.Code == orderCode);
                        var customer = db.Customer.FirstOrDefault(x => x.CustomerId == itemOrder.CustomerId);
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            if (itemOrder != null && customer != null)
                            {
                                itemOrder.TypePayment = 2; // Đã thanh toán
                                db.OrderCustomer.Attach(itemOrder);
                                db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                                SendConfirmationEmails(cart, itemOrder, customer);

                                ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                                ViewBag.Code= orderCode;

                                return RedirectToAction("CheckOutSuccess", "ShoppingCart", new
                                {
                                    orderCode = orderCode,
                                    Amount=vnp_Amount.ToString()
                                });
                            }
                            else
                            {

                                UpdateWarehouseQuantity(itemOrder);
                                return RedirectToAction("CheckOutFailVnpay", "ShoppingCart");

                                //ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                            }
                        }
                        else
                        {
                            if (itemOrder != null)
                            {
                                UpdateWarehouseQuantity(itemOrder);
                                return RedirectToAction("CheckOutFailVnpay", "ShoppingCart");
                            }
                            ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                        }
                        ViewBag.ThanhToanThanhCong = "Số tiền thanh toán (VND): " + vnp_Amount.ToString();
                        return View();
                    }
                }
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "Error");
            }
        }


        public ActionResult CheckOutSuccess(string orderCode)
        {
            if (orderCode != null )
            {
                var order = db.OrderCustomer.FirstOrDefault(x => x.Code == orderCode);
                if (order != null)
                {
                    return View(order);
                }
            }
            return View();
        }

        public ActionResult CheckOutFailVnpay()
        {
            return View();
        }
        private void SendConfirmationEmails(ShoppingCart cart, OrderCustomer order, Customer customerInfo)
        {
            var itemsTable = GenerateItemsTable(cart);
            var totalAmount = cart.Items.Sum(item => item.Price * item.Quantity);

            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
            contentCustomer = ReplaceOrderPlaceholders(contentCustomer, order, customerInfo, itemsTable, totalAmount);
            CuaHangBanDienThoai.Common.Common.SendMail("PadaMiniStore", "Đơn hàng #" + order.Code, contentCustomer, customerInfo.Email);

            string contentAdmin = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
            contentAdmin = ReplaceOrderPlaceholders(contentAdmin, order, customerInfo, itemsTable, totalAmount);
            CuaHangBanDienThoai.Common.Common.SendMail("PadaMiniStore", "Đơn hàng mới #" + order.Code, contentAdmin, ConfigurationManager.AppSettings["EmailAdmin"]);
        }
        private string GenerateItemsTable(ShoppingCart cart)
        {
            var sb = new StringBuilder();
            foreach (var item in cart.Items)
            {
                sb.Append("<tr>")
                    .Append("<td>").Append(item.ProductName).Append("</td>")
                    .Append("<td>").Append(item.Quantity).Append("</td>")
                    .Append("<td>").Append(CuaHangBanDienThoai.Common.Common.FormatNumber(item.PriceTotal, 0)).Append("</td>")
                    .Append("</tr>");
            }
            return sb.ToString();
        }

        private string ReplaceOrderPlaceholders(string template, OrderCustomer order, Customer customerInfo, string itemsTable, decimal totalAmount)
        {
            return template
                .Replace("{{MaDon}}", order.Code)
                .Replace("{{SanPham}}", itemsTable)
                .Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"))
                .Replace("{{TenKhachHang}}", order.CustomerName)
                .Replace("{{Phone}}", order.Phone)
                .Replace("{{Email}}", customerInfo.Email)
                .Replace("{{DiaChiNhanHang}}", order.Location)
                .Replace("{{ThanhTien}}", CuaHangBanDienThoai.Common.Common.FormatNumber(totalAmount, 0))
                .Replace("{{TongTien}}", CuaHangBanDienThoai.Common.Common.FormatNumber(totalAmount, 0));
        }


        private async Task<bool> UpdateVoucherDetail(string codeVoucher, OrderCustomer order)
        {
            try
            {
                var voucherDetail = await db.VoucherDetail.FirstOrDefaultAsync(x => x.Code == codeVoucher && x.Status == false);
                if (voucherDetail != null)
                {
                    var existingOrder =await db.OrderCustomer.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
                    if (existingOrder != null)
                    {

                        if (voucherDetail.VoucherId>0)
                        {
                            var existingVoucher = await db.Voucher.FirstOrDefaultAsync(v => v.VoucherId == voucherDetail.VoucherId);
                            if (existingVoucher != null)
                            {
                                voucherDetail.OrderId = order.OrderId;
                                voucherDetail.BillId = null;
                                voucherDetail.UsedDate = DateTime.Now;
                                voucherDetail.Status = true;
                               await db.SaveChangesAsync();     
                                return true; 
                            }
                            else
                            {
                                return false; 
                            }
                        }
                        else
                        {
                            return false; 
                        }
                    }
                    else
                    {
                        return false; 
                    }
                }
                else
                {
                    UpdateWarehouseQuantity(order);
                    return false; // Không tìm thấy voucher hoặc đã được sử dụng
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu cần
                return false; // Có lỗi khi cập nhật voucher
            }
        }
        private void UpdateWarehouseQuantity(OrderCustomer itemOrder)
        {
            var orderDetails = db.OrderDetail.Where(od => od.OrderId == itemOrder.OrderId).ToList();

            foreach (var orderDetail in orderDetails)
            {
                // Lấy thông tin sản phẩm chi tiết từ đơn hàng
                int productDetailId = (int)orderDetail.ProductDetailId;
                int quantity = orderDetail.Quantity;

                // Cập nhật số lượng sản phẩm trong kho
                var warehouseDetail = db.ProductDetail.FirstOrDefault(w => w.ProductDetailId == productDetailId);
                if (warehouseDetail != null)
                {
                    // Trả lại số lượng sản phẩm vào kho
                    warehouseDetail.Quantity += quantity;
                    db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    // Xử lý khi không tìm thấy sản phẩm trong kho (nếu cần)
                }
            }
            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();

            RestoreCartItems(itemOrder);

            DeleteOrderAndOrderDetails(itemOrder);


        }


        private void DeleteOrderAndOrderDetails(OrderCustomer itemOrder)
        {
            var orderDetails = db.OrderCustomer.Where(od => od.OrderId == itemOrder.OrderId).ToList();
            foreach (var orderDetail in orderDetails)
            {
                db.OrderCustomer.Remove(orderDetail);
            }
            db.OrderCustomer.Remove(itemOrder);
            db.SaveChanges();
        }
        private void RestoreCartItems(OrderCustomer itemOrder)
        {
            var orderDetails = db.OrderDetail.Where(od => od.OrderId == itemOrder.OrderId).ToList();
            int customerId = (int)itemOrder.CustomerId;


            var cart = db.Cart.FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null)
            {
                // Tạo mới giỏ hàng nếu chưa tồn tại
                cart = new Cart { CustomerId = customerId };
                db.Cart.Add(cart);
                db.SaveChanges();
            }
            foreach (var orderDetail in orderDetails)
            {
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductDetailId = (int)orderDetail.ProductDetailId,
                    Quantity = orderDetail.Quantity,
                   
                };

                db.CartItem.Add(cartItem);
            }
            db.SaveChanges();
        }
        private string GenerateOrderCode()
        {
            Random ran = new Random();
            return "DH" + string.Concat(Enumerable.Range(0, 5).Select(_ => ran.Next(0, 10)));
        }
        private OrderCustomer CreateOrder(ShoppingCart cart, Customer customerInfo, int typePayment)
        {
            var addresscustomer = db.AddressCustomer.FirstOrDefault(x => x.CustomerId == customerInfo.CustomerId && x.IsDefault == true);
            if (addresscustomer != null)

            {
                var order = new OrderCustomer
                {
                    CustomerName = customerInfo.CustomerName.Trim(),
                    Phone = addresscustomer.PhoneNumber.Trim(),
                    Location = addresscustomer.Location.Trim(),
                    Email = customerInfo.Email,

                    TypePayment = typePayment,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Confirm = false,
                  
                    IsDelivery = false,
                    Success = false,
                    Code = GenerateOrderCode(),
                    CustomerId = customerInfo.CustomerId,
                    TotalAmount =cart.GetTotalPrice()
            };
               


             
            

                // Thêm các chi tiết đơn hàng
                cart.Items.ForEach(row => order.OrderDetail.Add(new OrderDetail
                {
                    ProductDetailId = row.ProductDetailId,
                    Quantity = row.Quantity,
                    Price = row.PriceTotal
                }));

                return order;

            }
            return null;    

          
        }
        private List<ShoppingCartItem> ProcessCartItems(ShoppingCart cart, int customerId)
        {
            List<ShoppingCartItem> insufficientItems = new List<ShoppingCartItem>();
            List<int> rollbackQuantities = new List<int>();
            List<ShoppingCartItem> removedItems = new List<ShoppingCartItem>();

            try
            {
                var itemsCopy = new List<ShoppingCartItem>(cart.Items);

                foreach (var item in itemsCopy)
                {
                    var warehouseDetail = db.ProductDetail.FirstOrDefault(w => w.ProductDetailId == item.ProductDetailId);

                    if (warehouseDetail != null && warehouseDetail.Quantity >= item.Quantity)
                    {
                        rollbackQuantities.Add((int)warehouseDetail.Quantity);

                        warehouseDetail.Quantity -= item.Quantity;
                        db.Entry(warehouseDetail).State = System.Data.Entity.EntityState.Modified;

                        DeleteCartSucces(customerId, item.ProductDetailId);

                        removedItems.Add(item);
                    }
                    else
                    {
                        insufficientItems.Add(item);
                    }
                }

                // Kiểm tra nếu có sản phẩm không đủ hàng
                if (insufficientItems.Any())
                {
                    // Rollback lại số lượng sản phẩm đã giảm
                    foreach (var removedItem in removedItems)
                    {
                        var originalQuantity = rollbackQuantities[removedItems.IndexOf(removedItem)];
                        var currentWarehouseDetail = db.ProductDetail.FirstOrDefault(w => w.ProductDetailId == removedItem.ProductDetailId);
                        if (currentWarehouseDetail != null)
                        {
                            currentWarehouseDetail.Quantity += removedItem.Quantity;
                            db.Entry(currentWarehouseDetail).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                 
                    db.SaveChanges();
                }
                else
                {
                    // Nếu tất cả sản phẩm đều đủ hàng, lưu thay đổi và commit giao dịch
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                // Rollback lại số lượng sản phẩm đã giảm
                foreach (var removedItem in removedItems)
                {
                    var originalQuantity = rollbackQuantities[removedItems.IndexOf(removedItem)];
                    var currentWarehouseDetail = db.ProductDetail.FirstOrDefault(w => w.ProductDetailId == removedItem.ProductDetailId);
                    if (currentWarehouseDetail != null)
                    {
                        currentWarehouseDetail.Quantity += removedItem.Quantity;
                        db.Entry(currentWarehouseDetail).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }

            return insufficientItems;
        }
        private void DeleteCartSucces(int idKhachHang, int productId)
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var checkCart = db.Cart.FirstOrDefault(x => x.CustomerId == idKhach);
                if (checkCart != null)
                {
                    var checkItemCart = db.CartItem.SingleOrDefault(x => x.CartId == checkCart.CartId && x.ProductDetailId == productId);
                    if (checkItemCart != null)
                    {
                        db.CartItem.Remove(checkItemCart);
                        db.SaveChanges();
                    }
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteFromCartSession(int productdetaili) // ko có tìa khoản 
        {
            try
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    var checkProduct = cart.Items.FirstOrDefault(x => x.ProductDetailId == productdetaili);
                    if (checkProduct != null)
                    {
                        cart.Remove(productdetaili);
                        return Json(new { Success = true, msg = "Đã xoá thành công", Code = 1 });
                    }
                    else
                    {
                        return Json(new { Success = false, msg = "Lỗi mã sản phẩm", Code = -1 });
                    }
                }
                else
                {
                    return Json(new { Success = false, msg = "Không tồn tại sản phẩm", Code = -2 });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, msg = "Lỗi máy chủ !", Code = -100 });
            }


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

        public ActionResult Partail_ChangeAddress()
        {
            if (Session["CustomerId"] == null && Session["customer"] == null)
            {
                return PartialView();
            }

            int customerId = (int)Session["CustomerId"];
            if (customerId > 0)
            {
                var customer = db.Customer.Find(customerId);
                var checkAddress = db.AddressCustomer.Where(a => a.CustomerId == customerId).ToList();
                if (customer != null && checkAddress != null)
                {
                    ViewBag.CustomerId = customerId;
                    return PartialView(checkAddress);
                }

            }
            return PartialView();
        }
        public ActionResult Partail_EditAddress(int addressid)
        {
            if (addressid > 0)
            {
                var address = db.AddressCustomer.Find(addressid);
                return PartialView(address);
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress(AddressCustomer model)
        {
            if (ModelState.IsValid)
            {


                var existingAddress = db.AddressCustomer
                              .FirstOrDefault(a =>
                                  a.CustomerId == model.CustomerId &&
                                  a.Location.Trim().ToLower() == model.Location.Trim().ToLower() &&
                                  a.PhoneNumber.Trim() == model.PhoneNumber.Trim());

                if (existingAddress != null)
                {
                    return Json(new { Success = false, Code = -2, msg = "Địa chỉ này đã tồn tại trong hệ thống." });
                }
                db.AddressCustomer.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { Success = true });
            }
            return Json(new { Success = false, msg = "Vui lòng điền đủ thông tin" });
        }


        public ActionResult Partial_AddNewAddress()
        {
            if (Session["CustomerId"] == null && Session["customer"] == null)
            {
                return PartialView();
            }
            else
            {
                int customerId = (int)Session["CustomerId"];
                return PartialView();
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemAddressCustomer(int customerId, CLient_AddressCustomer req, AddressCustomer model)
        {
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    if (customerId > 0)
                    {
                        var customer = db.Customer.Find(customerId);
                        if (customer != null)
                        {
                            if (req.Location == null && req.CustomerName == null && req.PhoneNumber == null)
                            {
                                return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đầy đủ !!!" });
                            }
                            string Address = "";
                            if (req.Location != null || req.NameWard != null || req.NameDistrict != null || req.NameProvince != null)
                            {
                                Address = $"{req.Location}, {req.NameWard}, {req.NameDistrict}, {req.NameProvince}";
                            }

                            // Kiểm tra xem địa chỉ đã tồn tại hay chưa
                            var existingAddress = db.AddressCustomer
                                .FirstOrDefault(a =>
                                    a.CustomerId == customerId &&
                                    a.Location.Trim().ToLower() == req.Location.Trim().ToLower() &&
                                    a.PhoneNumber.Trim() == req.PhoneNumber.Trim());

                            if (existingAddress != null)
                            {
                                return Json(new { Success = false, Code = -2, msg = "Địa chỉ này đã tồn tại trong hệ thống." });
                            }

                            model.CustomerId = customerId;
                            model.CustomerName = req.CustomerName.Trim();
                            model.PhoneNumber = req.PhoneNumber.Trim();
                            model.Location = Address?.Trim()?? req.Location?.Trim();
                            if (Address != null)
                            {
                                model.IsDefault = true;

                            }
                            else
                            {
                                model.IsDefault = false;
                            }

                            db.AddressCustomer.Add(model);
                            db.SaveChanges();

                            dbContext.Commit();
                            return Json(new { Success = true, Code = 1, msg = "Thêm thành công" });
                        }
                        else
                        {
                            return Json(new { Success = false, Code = -2, msg = "Phiên đăng nhập đã hết hạn" });
                        }
                    }
                    return Json(new { Success = false, Code = -2, msg = "Phiên đăng nhập đã hết hạn" });
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Lỗi hệ thống" });
                }
            }
        }


        public ActionResult ThemAddress()
        {
            if (Session["customer"]!=null&& Session["CustomerId"] != null)
            {
                int id =(int) Session["CustomerId"];
                if (id > 0)
                {
                    ViewBag.Provinces = new SelectList(db.Provinces.ToList(), "idProvinces", "name");
                    ViewBag.CustomerId = id;
                    return PartialView();
                }
            }
            return PartialView();
        }


        [HttpPost]
        public ActionResult ChangeAddressDefault(int? addressid , int ? customerid)
        {
            if (Session["CustomerId"] == null && Session["customer"] == null)
            {
                return Json(new { Code = -3, msg = "Phiên đăng nhập hết hạn " });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    if (addressid > 0 && customerid > 0)
                    {
                        var address = db.AddressCustomer.FirstOrDefault(x => x.CustomerId == customerid && x.AddressCusomerId == addressid);
                        if (address != null)
                        {

                            var change = db.AddressCustomer.Where(x => x.CustomerId == customerid).ToList();
                            foreach(var item in change)
                            {
                                item.IsDefault = false;
                            }

                            address.IsDefault = true;   
                            db.Entry(address).State = EntityState.Modified;
                             db.SaveChanges();
                            dbContext.Commit();
                            return Json(new { Success = true, Code = 1, msg = "Đã thay đổi địa chỉ mặc định" });
                        }
                    }
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy mã" });
                    }

                }
                catch(Exception ex) 
                {
                    dbContext.Rollback();   
                    return Json(new { Success = false,Code = -99, msg = "Lỗi hệ thống" });
                
                }
            }
              
        }

        public ActionResult Partial_ThongTinKhachHang()
        {
            if (Session["CustomerId"] == null && Session["customer"] == null)
            {
                return View();
            }

            int customerId = (int)Session["CustomerId"];
            if (customerId > 0)
            {
                var customer = db.Customer.Find(customerId);
                var checkAddress = db.AddressCustomer.FirstOrDefault(a => a.CustomerId == customerId && a.IsDefault == true);

                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {
                    ViewBag.TotalPrice = cart.GetTotalPrice();

                  
                    var viewModel = new OrderView
                    {
                        Email = customer?.Email
                    };

                    if (checkAddress != null && checkAddress.IsDefault == true)
                    {
                        viewModel.NameCustomer = checkAddress.CustomerName;
                        viewModel.PhoneCustomer = checkAddress.PhoneNumber;
                        viewModel.Location = checkAddress.Location;
                    }

                    return PartialView(viewModel); 
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
        [HttpGet]
        public JsonResult GetVoucher(string Code)
        {
            if (Code == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var voucher = (from chiTiet in db.VoucherDetail
                               join voucherDetail in db.Voucher on chiTiet.VoucherId equals voucherDetail.VoucherId
                               where chiTiet.Code.Trim() == Code.Trim() && voucherDetail.EndDate >= DateTime.Now // Kiểm tra ngày hết hạn
                               select new
                               {
                                   voucherDetail.PercentPriceReduction,  // PhanTramGiam
                                   voucherDetail.Title,
                                   voucherDetail.CreatedBy,
                                   voucherDetail.CreatedDate,
                                   voucherDetail.ModifiedDate,
                                   voucherDetail.StartDate,
                                   voucherDetail.EndDate,
                                   voucherDetail.Quantity,
                                   chiTiet.Status
                               }).FirstOrDefault();

                if (voucher != null)
                {
                    // Kiểm tra số lượng voucher còn lại
                    if (voucher.Quantity <= 0)
                    {
                        return Json(new { error = "Voucher đã hết số lượng sử dụng" }, JsonRequestBehavior.AllowGet);
                    }

                    
                    if (voucher.Status == true) // Giả sử 1 là trạng thái "đã sử dụng"
                    {
                        return Json(new { error = "Voucher đã được sử dụng" }, JsonRequestBehavior.AllowGet);
                    }

                    var formattedVoucher = new
                    {
                        PercentPriceReduction = voucher.PercentPriceReduction,
                        Title = voucher.Title,
                        CreatedBy = voucher.CreatedBy,
                        StartDate = voucher.StartDate?.ToString("yyyy-MM-ddTHH:mm:ss"),

                        EndDate = voucher.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        voucher.Quantity,
                        voucher.Status
                    };

                    return Json(formattedVoucher, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "Không tìm thấy mã giảm giá hoặc mã đã hết hạn" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và ghi log nếu cần
                Console.WriteLine("Lỗi trong quá trình lấy thông tin mã giảm giá: " + ex.Message);
                return Json(new { error = "Đã xảy ra lỗi khi lấy thông tin mã giảm giá" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UseVoucher (int percentPriceReduction ,string code )
        {
            if (percentPriceReduction > 0)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {

                    cart.PercentPriceReduction = percentPriceReduction;
                    cart.Code = code;
                    Session["Cart"] = cart;
                    return Json(new { Success = true });
                }

            }
            else
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null && cart.Items.Any())
                {

                    cart.PercentPriceReduction = 0;
                    cart.Code = null;
                    Session["Cart"] = cart;
                    return Json(new { Success = true });
                }
            }
            return Json(new { Success = false });
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
                                var Name = cartItem.ProductDetail.Products.Title.Trim() + " " + cartItem.ProductDetail.Ram.Trim() + "/" + cartItem.ProductDetail.Capacity.Trim();
                                ShoppingCartItem item = new ShoppingCartItem
                                {
                                    ProductDetailId = (int)cartItem.ProductDetailId,
                                    ProductName = Name ?? "",
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

        public ActionResult CheckOutSuceess(string Code)
        {
         
            if (Code != null)
            {
                var order = db.OrderCustomer.FirstOrDefault(b => b.Code == Code);
                if (order != null)
                {
                    ViewBag.Code = order.Code;
                    ViewBag.item = order;
                    return View(order);
                }
                return View();
            }
            else
            {
                ViewBag.Code = "";
                return View();
            }
        }
        public ActionResult Partial_BillDetail(int id)
        {
            if (id > 0)
            {
                var orderDetail = db.OrderDetail.Where(x => x.OrderId == id).ToList();
                if (orderDetail != null)
                {
                    ViewBag.BillDetail = orderDetail;
                    return PartialView(orderDetail);
                }
            }
            return View();
        }
    }
}