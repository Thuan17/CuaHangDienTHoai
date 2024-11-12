using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;
using PagedList;
using PagedList.Mvc;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Admin/Customer
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]


        
        public ActionResult Index(int? page)
        {
            var items = db.Customer.OrderByDescending(x => x.CustomerId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.Customer.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.Count = products.Count;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                return View(items.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                ViewBag.txt = "Không tồn tại sản phẩm";
                return View();
            }
        }


        public ActionResult Partail_Detail(int customerid)
        {
            if(customerid> 0)
            {
                var customer = db.Customer.Find(customerid);
                if (customer != null)
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    ViewBag.IsAdmin = isAdmin;
                    return PartialView(customer);
                }
            }


            return PartialView();
        }

        public ActionResult Partail_OrderbyIdCustomer(int customerid)
        {
            if (customerid > 0)
            {
                var order = db.OrderCustomer.Where(x=>x.CustomerId==customerid).ToList().OrderByDescending(x=>x.OrderId);
                if (order != null)
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    ViewBag.IsAdmin = isAdmin;
                    return PartialView(order);
                }
            }


            return PartialView();
        }

        public ActionResult Partail_CartbyIdCustomer(int customerid)
        {
            if (customerid > 0)
            {
                var cart = db.Cart.FirstOrDefault(x => x.CustomerId == customerid);
                if (cart != null)
                {
                    var carItem = db.CartItem.Where(x => x.CartId == cart.CartId).OrderByDescending(x => x.CartItemId).ToList();
                    if (carItem != null)
                    {
                        bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                        ViewBag.IsAdmin = isAdmin;
                        return PartialView(carItem);
                    }
                }
                
            }


            return PartialView();
        }
        [HttpPost]
        public async Task<ActionResult> IsLock(int id)
        {
            try
            {
                var item = await db.Customer.FindAsync(id);

                if (item != null)
                {
                    item.IsLock = !item.IsLock;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Json(new { success = true, isLock = item.IsLock });
                }

                return Json(new { success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Lỗi cập nhập tag hiển thị trang chủ" });
            }
        }



        public JsonResult GetOrderStatistics(int customerId)
        {
            try
            {
                // Truy vấn dữ liệu từ cơ sở dữ liệu
                var totalOrders = db.OrderCustomer.Where(o => o.CustomerId == customerId).ToList();
                var successfulOrders = totalOrders.Count(o => o.Success == true && (o.OrderStatus?.Trim() != "Đơn huỷ" && o.OrderStatus?.Trim() != "Từ chối"));
                var canceledOrders = totalOrders.Count(o => o.OrderStatus?.Trim() == "Đơn huỷ");
                var nonConfirmOrders = totalOrders.Count(o => o.Confirm == false && (o.OrderStatus?.Trim() != "Đơn huỷ" && o.OrderStatus?.Trim() != "Từ chối"));
                var totalBill = db.Bill.Where(o => o.CustomerId == customerId).ToList();
                decimal successRate = 0;
                decimal cancelRate = 0;
                decimal nonConfirm = 0;

                if (totalOrders.Count > 0)
                {
                    successRate = (decimal)successfulOrders / totalOrders.Count * 100;
                    cancelRate = (decimal)canceledOrders / totalOrders.Count * 100;
                    nonConfirm = (decimal)nonConfirmOrders / totalOrders.Count * 100;
                }

                // Trả về kết quả dưới dạng JSON
                return Json(new
                {

                    SuccessRate = successRate,
                    SuccessCount = successfulOrders,
                    CancelRate = cancelRate,
                    CancelCount = canceledOrders,
                    ConFirmRate = nonConfirm,
                    ConFirmCount = nonConfirmOrders,
                    TotalOrder = totalOrders.Count(),
                    Totalbill = totalBill.Count(),
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về thông báo lỗi chi tiết
                Debug.WriteLine($"Lỗi: {ex.Message}");
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}