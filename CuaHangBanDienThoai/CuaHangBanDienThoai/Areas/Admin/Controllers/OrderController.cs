using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order


        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        [AuthorizeFunction("Nhân viên kho", "Quản lý", "Quản trị viên")]
        public async Task<ActionResult> Index(int? page)
        {
            IEnumerable<OrderCustomer> items = db.OrderCustomer.OrderByDescending(x => x.OrderId);
            if (items != null)
            {
                var pageSize = 10;
                if (page == null)
                {
                    page = 1;
                }
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                items = items.ToPagedList(pageIndex, pageSize);
                var OrderCustomer = db.OrderCustomer.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsManage = isManage;

                ViewBag.Count = OrderCustomer.Count;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return PartialView();
            }
        }
        [AuthorizeFunction("Nhân viên kho", "Quản lý", "Quản trị viên")]
        public async Task<ActionResult> EmployeeIndex(int? page)
        {
            IEnumerable<OrderCustomer> items = db.OrderCustomer.OrderByDescending(x => x.OrderId);
            if (items != null)
            {
                var pageSize = 10;
                if (page == null)
                {
                    page = 1;
                }
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                items = items.ToPagedList(pageIndex, pageSize);
                var OrderCustomer = db.OrderCustomer.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.Count = OrderCustomer.Count;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return PartialView();
            }
        }
        [AuthorizeFunction("Nhân viên kho","Quản lý", "Quản trị viên")]
        public ActionResult OrderNonIsConfirm(int? page)
        {

            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            ViewBag.IsAdmin = isAdmin;

            IEnumerable<OrderCustomer> items = db.OrderCustomer.Where(x => x.Confirm == false && x.OrderStatus == null && x.ReturnDate == null && x.ReturnReason == null).OrderByDescending(x => x.OrderId);
            if (items != null)
            {
                var pageSize = 10;
                if (page == null)
                {
                    page = 1;
                }
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                items = items.ToPagedList(pageIndex, pageSize);
                var order = db.OrderCustomer.Where(x => x.Success == false).Count();
               
                ViewBag.Count = order>0?order   : 0;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items);
            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return PartialView();
            }
        }
        public ActionResult Partail_ProductOrder(int id)
        {
            if (id <= 0)
            {
                return PartialView();
            }
            var ProductBillDetail = db.OrderDetail.Where(x => x.OrderId == id);
            if (ProductBillDetail != null)
            {
                return PartialView(ProductBillDetail);
            }
            else
            {
                return PartialView();
            }
        }


        public ActionResult Partial_DetailOrder(int id)
        {

            try
            {
                if (id > 0)
                {

                    var Order = db.OrderCustomer.FirstOrDefault(x => x.OrderId == id);
                    if (Order == null)
                    {
                        return PartialView();
                    }
                    Admin_EditOrder ViewModelOrder = new Admin_EditOrder
                    {
                        OrderId = Order.OrderId,
                        Code = Order.Code,
                       
                        TotalAmount = Order.TotalAmount,
                        CreatedDate = Order.CreatedDate,
                        ModifiedDate = Order.ModifiedDate ?? DateTime.MinValue,

                        CustomerName = Order.CustomerName.Trim(),

                        Modifiedby = Order.Modifiedby,
                        TypePayment = Order.TypePayment ,
                        Success = (bool)Order.Success,
                        Note = Order.Note,
                        Phone = Order.Phone,
                        Location = Order.Location,
                        Email = Order.Email,
                        CustomerId =(int) Order.CustomerId,
                        IsDelivery = Order.IsDelivery ?? false,
                       Confirm = (bool)Order.Confirm,
                        OrderStatus = Order.OrderStatus,
                        StatusDate = Order.StatusDate ?? DateTime.MinValue,

                        SuccessDate = Order.SuccessDate ?? DateTime.MinValue,
                    };


                    var OrderDetail = db.OrderDetail
                                    .Where(x => x.OrderId == id)
                                    .Select(detail => new Admin_EditOrderItem
                                    {
                                        OrderDetailId = detail.OrderDetailId,
                                        Quantity = detail.Quantity,
                                        OrderId = (int)detail.OrderId,
                                        ProductDetailId = (int)detail.ProductDetailId,
                                        Price = detail.Price,
                                        productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId),


                                    }).ToList();
                    ViewModelOrder.Items = OrderDetail;


                    Session["Admin_DetailOrder_" + id] = ViewModelOrder;


                    ViewBag.Count = ViewModelOrder.Items.Count;
                    return PartialView(ViewModelOrder);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView();
            }
        }
        public ActionResult Partial_EditOrder(int id)
        {
            try
            {
                if (id > 0)
                {

                    var Order = db.OrderCustomer.FirstOrDefault(x => x.OrderId == id);
                    if (Order == null)
                    {
                        return PartialView();
                    }
                    Admin_EditOrder ViewModelOrder = new Admin_EditOrder
                    {
                        OrderId = Order.OrderId,
                        Code = Order.Code,

                        TotalAmount = Order.TotalAmount,
                        CreatedDate = Order.CreatedDate,
                        ModifiedDate = Order.ModifiedDate ?? DateTime.MinValue,

                        CustomerName = Order.CustomerName.Trim(),  

                        Modifiedby = Order.Modifiedby,
                        TypePayment = Order.TypePayment,
                        Success = (bool)Order.Success,
                        Note = Order.Note,
                        Phone = Order.Phone,
                        Location = Order.Location,
                        Email = Order.Email,
                        CustomerId = (int)Order.CustomerId,
                        IsDelivery = Order.IsDelivery ?? false,
                        OrderStatus = Order.OrderStatus,
                        StatusDate = Order.StatusDate ?? DateTime.MinValue,

                        SuccessDate = Order.SuccessDate ?? DateTime.MinValue,
                    };


                    var OrderDetail = db.OrderDetail
                                    .Where(x => x.OrderId == id)
                                    .Select(detail => new Admin_EditOrderItem
                                    {
                                        OrderDetailId = detail.OrderDetailId,
                                        Quantity = detail.Quantity,
                                        OrderId = (int)detail.OrderId,
                                        ProductDetailId = (int)detail.ProductDetailId,
                                        Price = detail.Price,
                                        productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId),


                                    }).ToList();
                    ViewModelOrder.Items = OrderDetail;


                    Session["Admin_EditOrder_" + id] = ViewModelOrder;


                    ViewBag.Count = ViewModelOrder.Items.Count;
                    return PartialView(ViewModelOrder);
                }
                return PartialView();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView();
            }
        }

        public ActionResult GetUpdatedOrderRow(int OrderId)
        {
            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsManage = isManage;
            var Order = db.OrderCustomer.FirstOrDefault(b => b.OrderId == OrderId);
            if (Order != null)
            {
                return PartialView(Order);
            }
            return HttpNotFound();
        }

        public ActionResult SuggestOrder(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var order = db.OrderCustomer.Where(c => c.CustomerName.Contains(search) || c.Code.Contains(search) || c.Phone.Contains(search)).ToList();


                if (order.Any())
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    ViewBag.IsAdmin = isAdmin;
                    var count = order.Count();
                    ViewBag.Count = count;
                    ViewBag.Content = search;
                    return PartialView(order);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return PartialView();
            }
        }
        public ActionResult GetOrderByDate(DateTime ngayxuat)
        {
            try
            {
                DateTime selectedDate = ngayxuat;

                DateTime startDate = selectedDate.Date;
                DateTime endDate = startDate.AddDays(1);

                var orders = db.OrderCustomer
                    .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                    .ToList();

                if (orders != null && orders.Count > 0)
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.Count = orders.Count;
                    ViewBag.Date = ngayxuat;
                    ViewBag.Content = ngayxuat.ToString("dd/MM/yyyy");
                    return PartialView(orders);  // Trả về PartialView chứa danh sách đơn hàng
                }
                else
                {
                    return PartialView();  // Nếu không có đơn hàng nào
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { Success = false, Code = -99, msg = "Có lỗi xảy ra!" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Admin_EditOrder model)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (Session["user"] == null)
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    var sessionKey = "Admin_EditOrder_" + model.OrderId;
                    var viewModel = Session[sessionKey] as Admin_EditOrder;
                    if (viewModel == null || viewModel.Items.Count < 1)
                    {
                        return Json(new { success = false, code = viewModel == null ? -3 : -4 });
                    }
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff =await db.Employee.FirstOrDefaultAsync(row => row.EmployeeId == nvSession.EmployeeId);

                    var seller = await db.OrderCustomer.FindAsync(model.OrderId);
                   


                    if (seller == null)
                    {
                        return Json(new { success = false, code = -2, message = "Lỗi không tìm thấy mã đơn hàng" });
                    }
                    decimal totalAmount = viewModel.Items.Sum(detail => detail.Price * detail.Quantity);
                    bool isModified =
                          seller.OrderId != model.OrderId ||
                          seller.Code != model.Code ||
                          seller.CreatedBy != model.CreatedBy ||
                          seller.TypePayment != model.TypePayment ||
                          seller.Note != model.Note ||
                          seller.Location != model.Location ||
                          seller.CustomerName != model.CustomerName ||
                          seller.Phone != model.Phone ||
                          seller.TotalAmount != totalAmount;



                    if (!isModified)
                    {
                        return Json(new { success = false, code = -2, message = "Không có thay đổi nào !!!" });
                    }

                    seller.OrderId = model.OrderId;
                        seller.Code = model.Code;

                        seller.TotalAmount = totalAmount;
                        seller.CreatedDate = model.CreatedDate;
                        seller.ModifiedDate = DateTime.Now;
                    seller.CustomerId=model.CustomerId; 
                        seller.CustomerName = model.CustomerName.Trim();  

                       seller.Modifiedby = nvSession.Employee.NameEmployee.Trim();
                         seller.TypePayment = model.TypePayment;
                         seller.Success = (bool)model.Success;
                         seller.Note = model.Note;
                         seller.Phone = model.Phone;
                         seller.Location = model.Location;
                         seller.Email = model.Email;
                      
                         seller.IsDelivery = (bool)model.IsDelivery ;
                    if (model.StatusDate != default(DateTime))
                    {
                        seller.StatusDate = model.StatusDate;
                    }


                    db.Entry(seller).State = EntityState.Modified;
                  await db.SaveChangesAsync();
                    var existingDetails = db.OrderDetail.Where(d => d.OrderId == seller.OrderId).ToList();
                    var newDetails = viewModel.Items;
                    foreach (var detail in existingDetails.ToList())
                    {
                        var viewModelDetail = viewModel.Items.FirstOrDefault(x => x.OrderDetailId == detail.OrderDetailId);
                        if (viewModelDetail != null)
                        {
                            int oldQuantity = detail.Quantity;
                            int newQuantity = viewModelDetail.Quantity;
                            int quantityChange = newQuantity - oldQuantity;

                            // Cập nhật số lượng trong chi tiết hóa đơn bán hàng
                            detail.Quantity = newQuantity;
                            db.Entry(detail).State = EntityState.Modified;
                           
                            // Kiểm tra và cập nhật số lượng trong kho
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = await db.ProductDetail.FirstOrDefaultAsync(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    string Name = warehouseDetail.Products.Title.Trim() + " " + warehouseDetail.Ram.Trim() + "/" + warehouseDetail.Capacity.Trim();
                                    if (quantityChange > 0) // Nếu tăng số lượng
                                    {
                                        // Kiểm tra kho đủ số lượng không
                                        if (warehouseDetail.Quantity < quantityChange)
                                        {
                                            dbContextTransaction.Rollback();
                                            return Json(new { success = false, code = -2, message = $"Số lượng sản phẩm '{Name}' trong kho không đủ." });
                                        }


                                        warehouseDetail.Quantity -= quantityChange;
                                    }
                                    else if (quantityChange < 0) // Nếu giảm số lượng
                                    {

                                        warehouseDetail.Quantity += -quantityChange;
                                    }

                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                }
                                else
                                {
                                    return Json(new { success = false, code = -2, message = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -2, message = "Chi tiết hóa đơn không có ProductsId." });
                            }
                        }
                        else
                        {
                            // Xóa chi tiết hóa đơn không có trong viewModel
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = await db.ProductDetail.FirstOrDefaultAsync(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    warehouseDetail.Quantity += detail.Quantity; // Cộng lại số lượng tồn kho
                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                    db.OrderDetail.Remove(detail);
                                }
                                else
                                {
                                    return Json(new { success = false, code = -2, message = $"Không tìm thấy sản phẩm với ProductsId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -2, message = "Chi tiết hóa đơn không có ProductsId." });
                            }
                        }
                    }

                    sessionKey = null;
                  await db.SaveChangesAsync();  
                    dbContextTransaction.Commit();
                    return Json(new { success = true, Code = 1, mess = "Cập nhập thành công", orderCode = seller.Code, OrderId = seller.OrderId });






                }
                catch (Exception ex) { dbContextTransaction.Rollback(); return Json(new { success = false, code = -5, message = $"Lỗi ngoại lệ: {ex.Message}" }); }
            }
        }


        [HttpPost]
        public ActionResult UpdateOrderStatus(int OrderId, string status)
        {

            var order = db.OrderCustomer.SingleOrDefault(b => b.OrderId == OrderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Hoá đơn không tồn tại.", code = OrderId });
            }

            order.OrderStatus = status;
            order.StatusDate = DateTime.Now;

            switch (status)
            {
                case "Chưa giao":
                    order.Confirm = false;
                    order.StatusDate = null;
                    break;

                case "Đã xuất kho":
                    order.Confirm = true;
                    break;

                default:
                   
                    break;
            }

            try
            {
                db.SaveChanges();
                return Json(new { success = true, code = order.Code, Confirm = order.Confirm });
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, message = "Lỗi khi cập nhật hoá đơn.", code = order.Code, error = ex.Message });
            }
        }





        public ActionResult Partail_ItemEditorder(int orderId)
        {
            Admin_EditOrder order = (Admin_EditOrder)Session["Admin_EditOrder_" + orderId];
            if (order != null && order.Items.Any())
            {
                int count = order.Items.Count;
                ViewBag.Count = count;
                return PartialView(order);
            }
            return PartialView();
        }


        [HttpPost]
        public ActionResult UpdateQuantityForEditNew(int productdetailId, int orderId, int orderDetailId, int newQuantity)
        {
            try
            {

                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }
                if (Session["Admin_EditOrder_" + orderId] == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để cập nhật số lượng." });

                }
                Admin_EditOrder viewModel = (Admin_EditOrder)Session["Admin_EditOrder_" + orderId];

                if (viewModel == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để cập nhật số lượng." });
                }

                var itemToUpdate = viewModel.Items.FirstOrDefault(item =>
                    item.ProductDetailId == productdetailId &&
                    item.OrderId == orderId && item.OrderDetailId == orderDetailId);

                if (itemToUpdate != null)
                {

                    int oldQuantity = itemToUpdate.Quantity;


                    itemToUpdate.Quantity = newQuantity;

                  
                    Session["Admin_EditOrder_" + orderId] = viewModel;

                  
                    return Json(new { success = true });



                }


                return Json(new { success = false, message = "Không tìm thấy sản phẩm để cập nhật số lượng." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteItem(int orderId, int orderdetailId)
        {
            try
            {
                if (Session["Admin_EditOrder_" + orderId] != null)
                {
                    Admin_EditOrder viewModel = (Admin_EditOrder)Session["Admin_EditOrder_" + orderId];
                    if (viewModel != null)
                    {
                        if (viewModel.Items.Count > 1)
                        {
                            viewModel.Items.RemoveAll(x => x.OrderDetailId == orderdetailId);

                            Session["Admin_EditOrder_" + orderId] = viewModel;
                       
                            return Json(new { success = true, code = 1, message = "Xóa thành công" });
                        }
                        else { return Json(new { success = false, message = "Bắt buộc 1 sản phẩm" }); }
                    }
                    else
                    {




                        return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                    }
                }
                else
                {

                    return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }


        public ActionResult GetTotalPrice(int orderId)
        {
            try
            {
                if (Session["Admin_EditOrder_" + orderId] != null)
                {
                    var viewModel = (Admin_EditOrder)Session["Admin_EditOrder_" + orderId];
                    if (viewModel != null)
                    {
                        return Json(CuaHangBanDienThoai.Common.Common.FormatNumber(viewModel.GetPriceTotal(), 0) + " đ", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("0 đ", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0 đ", JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult IsConfirm(int id)
        {
            try
            {
                var items = db.OrderCustomer.Find(id);
                if (items != null)
                {
                    items.Confirm = !items.Confirm;
                 
                    
                    db.Entry(items).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, isConfirm = items.Confirm });
                }
                return Json(new { success = false, message = "Lỗi mã đơn hàng" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e });

            }


        }


        public JsonResult CountOrderNew()
        {
            var orderNew = db.OrderCustomer.Where(x => x.Confirm == false && x.OrderStatus==null&& x.ReturnDate == null && x.ReturnReason == null).Count();
            return Json(new { Count = orderNew }, JsonRequestBehavior.AllowGet);
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






