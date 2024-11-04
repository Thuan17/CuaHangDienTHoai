using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order


        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
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

    }



    


}