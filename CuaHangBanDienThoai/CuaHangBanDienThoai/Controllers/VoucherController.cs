using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;
namespace CuaHangBanDienThoai.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Voucher
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            var currentDate = DateTime.Now;

            var validVouchers = db.Voucher
                .Where(v => v.StartDate.HasValue && v.ModifiedDate.HasValue &&
                            v.StartDate <= currentDate &&
                            v.EndDate >= currentDate)
                .ToList();

            return View(validVouchers);
        }
        public ActionResult Partial_VoucherDetail(int id)
        {
            if (id <= 0)
            {
                ViewBag.error = "Không tìm thấy bảng ghi nào !!!";
                return PartialView();
            }
            var voucherDetail = db.VoucherDetail.Where(x => x.VoucherId == id).Take(5);
            if (voucherDetail != null)
            {
                return PartialView(voucherDetail);
            }
            else
            {
                ViewBag.error = "Không tìm thấy bảng ghi nào !!!";
                return PartialView();
            }
        }
    }
}