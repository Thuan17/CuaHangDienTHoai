using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using System.Net;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        // GET: Admin/Statistical

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index()
        {

            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            ViewBag.IsAdmin = isAdmin;
            var uniqueYears = (from order in db.OrderCustomer
                               select order.CreatedDate.Year).Distinct();
            SelectList yearList = new SelectList(uniqueYears);


            ViewBag.UniqueYears = yearList;


            ViewBag.SelectedYear = "";
            ViewBag.Year = DateTime.Now.Year.ToString();

            var uniqueMonth = (from order in db.OrderCustomer
                               select order.CreatedDate.Month).Distinct();
            var currentMonth = DateTime.Now.Month;
            SelectList MonthList = new SelectList(uniqueMonth, currentMonth);

            ViewBag.UniqueMonth = MonthList;
            return View();
            
        }

        public ActionResult Partial_StatitisYearAll()
        {

            var monthlyRevenue = GetMonthlyRevenueByOrder();
            return PartialView("_MonthlyRevenue", monthlyRevenue);
        }
        //start thông kê đơn hàng theo tất cả tháng
        public ActionResult Partial_StatisticalOrderAllMon()
        {

            return PartialView();
        }
        [HttpGet]
        public ActionResult GetStatisticalOrderAllMon()
        {
            int Year = DateTime.Now.Year;
            ViewBag.Year = DateTime.Now.Year;
            ViewBag.Year1 = Year;
            ViewBag.Day = DateTime.Now.Day;

            var salesData = from a in db.OrderCustomer
                            where a.CreatedDate.Year == Year
                            group a by a.CreatedDate.Month into g
                            select new
                            {
                                Month = g.Key,
                                DoanhThu = g.Sum(a => a.TotalAmount),
                                TienGoc = g.Sum(y => y.OrderDetail.Sum(b => b.Quantity * b.ProductDetail.OriginalPrice)),
                                LoiNhuan = g.Sum(a => a.TotalAmount) - g.Sum(y => y.OrderDetail.Sum(b => b.Quantity * b.ProductDetail.OriginalPrice)),
                                DonHuy = g.Count(a => a.OrderStatus != null && a.Confirm==true && a.OrderStatus != "Đã giao"),
                                DonHoanThanh = g.Count(a => a.OrderStatus == "Đã giao"),
                                Count = g.Count()
                            };

            var result = salesData.ToList();

            System.Diagnostics.Debug.WriteLine("Result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));


            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
        //end thông kê đơn hàng theo tất cả tháng
        //start thông kê đơn hàng theo  tháng
        public ActionResult Partial_StatisticalByOrderMon(int month)
        {
            ViewBag.Month = month;
            return PartialView();
        }
        [HttpGet]
        public ActionResult GetStatisticalByOrderMonSelect(int month)
        {

            var allOrders = db.OrderCustomer.ToList();

            var allOrderDetails = db.OrderDetail.ToList();

            var allProductDetails = db.ProductDetail.ToList();
            var salesData = from a in db.OrderCustomer
                            where a.CreatedDate.Month == month
                            group a by a.CreatedDate.Month into g
                            select new
                            {
                                Month = g.Key,
                                DoanhThu = g.Sum(a => a.TotalAmount),
                                TienGoc = g.Sum(y => y.OrderDetail.Sum(b => b.Quantity * b.ProductDetail.OriginalPrice)),
                                LoiNhuan = g.Sum(a => a.TotalAmount) - g.Sum(y => y.OrderDetail.Sum(b => b.Quantity * b.ProductDetail.OriginalPrice)),
                                DonHuy = g.Count(a => a.OrderStatus != null && a.Confirm==true && a.OrderStatus != "Đã giao"),
                                DonHoanThanh = g.Count(a => a.OrderStatus == "Đã giao"),
                                Count = g.Count()
                            };
            var result = salesData.ToList();




            System.Diagnostics.Debug.WriteLine("Result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }
        //end thông kê đơn hàng theo  tháng
        public ActionResult Partail_StatisOrderDay(int? month)
        {
            if (month == null)
            {
                month = DateTime.Now.Month;
            }

            ViewBag.SelectedMonth = month;

            return PartialView(); // Trả về partial view
        }


        [HttpGet]
        public ActionResult GetStatisticalByOrderDaySelect(int? month)
        {
            if (!month.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Month parameter is missing.");
            }

            var startDate = new DateTime(DateTime.Now.Year, month.Value, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var allBills = db.OrderCustomer
                .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate)
                .ToList(); // Chuyển dữ liệu vào bộ nhớ

            var salesData = allBills
                .GroupBy(a => a.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    DonHuy = g.Count(a => a.OrderStatus != null && a.OrderStatus != "Đã giao"),
                    DonHoanThanh = g.Count(a => a.OrderStatus == "Đã giao")
                })
                .ToList();

            var result = salesData.ToList();
            var chartData = new
            {
                Labels = result.Select(r => r.Date.ToString("yyyy-MM-dd")).ToArray(), // Định dạng ngày
                DonHuyData = result.Select(r => r.DonHuy).ToArray(),
                DonHoanThanhData = result.Select(r => r.DonHoanThanh).ToArray()
            };

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        private List<Admin_MonthlyRevenue> GetMonthlyRevenueByOrder()
        {


            int Year = DateTime.Now.Year;

            var data = db.OrderCustomer
                        .Where(b => b.CreatedDate.Year == Year)
                        .GroupBy(b => b.CreatedDate.Month)
                        .Select(g => new
                        {
                            MonthNumber = g.Key,
                            Amount = g.Sum(b => b.TotalAmount)

                        })
                .ToList();


            var monthlyRevenue = data
               .Select(d => new Admin_MonthlyRevenue
               {
                   Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.MonthNumber),
                   Amount = d.Amount
               })
               .ToList();
            return monthlyRevenue;
        }
    }
}