using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Admin/Voucher

        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.Voucher.OrderByDescending(x => x.VoucherId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var Voucher = db.Voucher.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.Count = Voucher.Count;
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

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Add()
        {
            return View();  
        }

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Add(Voucher model ,Admin_AddVoucher req)
         {
            if (Session["user"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập hết hạn" });
            }
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điển đầy đủ thông tin" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    model.Title = req.Title.Trim();
                    model.PercentPriceReduction = req.PercentPriceReduction;
                    model.Quantity = req.Quantity;
                    model.StartDate = Convert.ToDateTime(req.StartDate);
                    model.EndDate = Convert.ToDateTime(req.EndDate); 
                 
                    model.CreatedBy = checkStaff.NameEmployee.Trim();
                    model.CreatedDate = DateTime.Now;


                    db.Voucher.Add(model);
                    await db.SaveChangesAsync();

                    dbContext.Commit();
                    var result = await AddVoucherDetails(model, req);
                    if (result)
                    {
                        return Json(new { Success = true, Code = 1,msg="Tạo Voucher thành công " });
                    }
                    else
                    {
                        
                        db.Voucher.Remove(model);
                        await db.SaveChangesAsync();


                        return Json(new { Success = false, Code = -2, msg = "Có lỗi xảy ra khi lưu voucher details. Vui lòng thử lại sau." });
                      
                    }

                }
                catch (Exception ex) 
                {
                    dbContext.Rollback();   
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
            }
              
        }
        private async Task<bool> AddVoucherDetails(Voucher model, Admin_AddVoucher req)
        {
            try
            {
                for (int i = 0; i < model.Quantity; i++)
                {


                    while (true)
                    {
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        var random = new Random();
                        var randomString = new StringBuilder(3);

                        for (int x = 0; x < 3; x++)
                        {
                            randomString.Append(chars[random.Next(chars.Length)]);
                        }

                        string randomString123 = randomString.ToString(); ;
                        string combinedCode = req.Title.Trim() + "-"+randomString123;
                        string filteredCode = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(combinedCode);

                        var checkCode =await db.VoucherDetail.FirstOrDefaultAsync(x => x.Code == filteredCode);

                        if (checkCode == null)
                        {
                            var voucherDetail = new VoucherDetail();
                            voucherDetail.Code = filteredCode;
                        

                        voucherDetail.OrderId=null;
                        voucherDetail.BillId = null;
                            voucherDetail.VoucherId = model.VoucherId;
                            voucherDetail.Status = false;

                            db.VoucherDetail.Add(voucherDetail);
                            await db.SaveChangesAsync();

                            break;
                        }
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is set up)
                return false;
            }
        }
        public ActionResult Partail_Detail(int? id)
        {
            if(id!=null && id > 0)
            {
                var voucher = db.Voucher.Find(id);
                if (voucher == null)
                {
                    return PartialView();

                }
                Admin_EditVoucher viewModel = new Admin_EditVoucher {


                    VoucherId = voucher.VoucherId,
                    CreatedDate = voucher.CreatedDate,
                    Createdby = voucher.CreatedBy,
                    ModifiedDate =voucher.ModifiedDate??null,
                    Modifiedby = voucher.Modifiedby??null,
                    PercentPriceReduction =(int) voucher.PercentPriceReduction, 
                   Title = voucher.Title,
                  Quantity =(int) voucher.Quantity,
                 StartDate = (DateTime)voucher.StartDate,
                     EndDate = (DateTime)voucher.EndDate,
                };

                var OrderDetail = db.VoucherDetail
                                    .Where(x => x.VoucherId == id)
                                    .Select(detail => new Admin_EditVoucherDetail
                                    {
                                        VoucherDetailId = detail.VoucherDetailId,
                                        UsedDate = detail.UsedDate ?? null,
                                        OrderId = detail.OrderId ?? 0,     
                                        BillId = detail.BillId ?? 0,
                                        VoucherId = detail.VoucherId,
                                        Code = detail.Code,
                                        Status = detail.Status,
                                        voucherDetail = db.VoucherDetail.FirstOrDefault(x => x.VoucherId == detail.VoucherId),
                                        orderCustomer = (detail.OrderId.HasValue && detail.OrderId.Value != 0)
                                        ? db.OrderCustomer.FirstOrDefault(x => x.OrderId == detail.OrderId)
                                        : null,
                                      
                                        bill = (detail.BillId.HasValue && detail.BillId.Value != 0)
                                           ? db.Bill.FirstOrDefault(x => x.BillId == detail.BillId)
                                           : null

                                    }).ToList();
                viewModel.Items = OrderDetail;
             

                Session["Admin_DetailVoucher_" + id] = viewModel;

                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.Count = viewModel.Items.Count;
                return PartialView(viewModel);

            }

            return PartialView();
        }
        public ActionResult Partail_Edit(int? id)
        {
            if (id != null && id > 0)
            {
                var voucher = db.Voucher.Find(id);
                if (voucher == null)
                {
                    return PartialView();

                }
                Admin_EditVoucher viewModel = new Admin_EditVoucher
                {


                    VoucherId = voucher.VoucherId,
                    CreatedDate = voucher.CreatedDate,
                    Createdby = voucher.CreatedBy,
                    ModifiedDate = voucher.ModifiedDate ?? null,
                    Modifiedby = voucher.Modifiedby ?? null,
                    PercentPriceReduction = (int)voucher.PercentPriceReduction,
                    Title = voucher.Title,
                    Quantity = (int)voucher.Quantity,
                    StartDate = (DateTime)voucher.StartDate,
                    EndDate = (DateTime)voucher.EndDate,
                };

                var OrderDetail = db.VoucherDetail
                                    .Where(x => x.VoucherId == id)
                                    .Select(detail => new Admin_EditVoucherDetail
                                    {
                                        VoucherDetailId = detail.VoucherDetailId,
                                        UsedDate = detail.UsedDate ?? null,
                                        OrderId = detail.OrderId ?? 0,
                                        BillId = detail.BillId ?? 0,
                                        VoucherId = detail.VoucherId,
                                        Code = detail.Code,
                                        Status = detail.Status,
                                        voucherDetail = db.VoucherDetail.FirstOrDefault(x => x.VoucherId == detail.VoucherId),
                                        orderCustomer = (detail.OrderId.HasValue && detail.OrderId.Value != 0)
                                        ? db.OrderCustomer.FirstOrDefault(x => x.OrderId == detail.OrderId)
                                        : null,

                                        bill = (detail.BillId.HasValue && detail.BillId.Value != 0)
                                           ? db.Bill.FirstOrDefault(x => x.BillId == detail.BillId)
                                           : null

                                    }).ToList();
                viewModel.Items = OrderDetail;


                Session["Admin_EditVoucher_" + id] = viewModel;

                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.Count = viewModel.Items.Count;
                return PartialView(viewModel);

            }

            return PartialView();
        }









        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Admin_EditVoucher req)
        {
            if (Session["user"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập hết hạn" });
            }
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điển đầy đủ thông tin" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var vocher = await db.Voucher.FirstOrDefaultAsync(x => x.VoucherId == req.VoucherId);
                    if (vocher == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy voucher !!!" });

                    }

                    int quantityNew = (int)req.Quantity - (int)vocher.Quantity;

                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    vocher.VoucherId = req.VoucherId;
                    vocher.CreatedDate = req.CreatedDate;
                    vocher.CreatedBy = req.Createdby;
                    vocher.ModifiedDate = req.ModifiedDate ?? DateTime.Now;
                    vocher.Modifiedby = checkStaff.NameEmployee.Trim();
                    vocher.PercentPriceReduction = (int)req.PercentPriceReduction;
                    vocher.Title = req.Title;
                    vocher.Quantity = (int)req.Quantity;
                    vocher.StartDate = (DateTime)req.StartDate;
                    vocher.EndDate = (DateTime)req.EndDate;
                    db.Entry(vocher).State = EntityState.Modified;
                   await db.SaveChangesAsync();

                        
                        if(quantityNew > 0)
                        {
                            for (int i = 0; i < quantityNew; i++)
                            {
                                while (true)
                                {
                                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                                    var random = new Random();
                                    var randomString = new StringBuilder(3);

                                    for (int x = 0; x < 3; x++)
                                    {
                                        randomString.Append(chars[random.Next(chars.Length)]);
                                    }

                                    string randomString123 = randomString.ToString(); ;
                                    string combinedCode = req.Title.Trim() + "-" + randomString123;
                                    string filteredCode = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(combinedCode);

                                    var checkCode = db.VoucherDetail.FirstOrDefault(x => x.Code == filteredCode);

                                    if (checkCode == null)
                                    {
                                        var voucherDetail = new VoucherDetail();
                                        voucherDetail.Code = filteredCode;


                                        voucherDetail.OrderId = null;
                                        voucherDetail.BillId = null;
                                        voucherDetail.VoucherId = vocher.VoucherId;
                                        voucherDetail.Status = false;

                                        db.VoucherDetail.Add(voucherDetail);
                                        await db.SaveChangesAsync();
                                        break;
                                    }
                                }
                            }
                           
                        }
                    else if (quantityNew < 0)
                    {
                            var voucherDetailsToDelete = db.VoucherDetail
                                  .Where(x => x.Status == false && x.VoucherId == vocher.VoucherId)
                                  .Take(-quantityNew)  
                                  .ToList();
                            int availableToDelete = voucherDetailsToDelete.Count;
                            if (availableToDelete < -quantityNew)
                            {
                                return Json(new { Success = false, Code = -2, msg = "Số lượng voucher không đủ để xóa" });
                            }
                            else
                            {
                                if (voucherDetailsToDelete.Any())
                                {
                                    db.VoucherDetail.RemoveRange(voucherDetailsToDelete);
                                    await db.SaveChangesAsync();
                                }
                            }
                        }


                
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Cập nhập thành công" });

                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
            }

        }



            public JsonResult ConutVoucherDetailnotYetUsed(int? id)
        {
            if (id > 0 && id != null)
            {
              
                var totalVoucherDetailCount = db.VoucherDetail.Where(x => x.VoucherId == id).Count();
                var usedOrerCount = db.VoucherDetail
                 .Where(x => x.VoucherId == id && x.UsedDate != null && (x.OrderId != null && x.BillId == null))
                 .Count();
                var usedBillCount = db.VoucherDetail
                .Where(x => x.VoucherId == id && x.UsedDate != null && (x.OrderId == null && x.BillId != null))
                .Count();


                var notYetUsedCount = db.VoucherDetail
                    .Where(x => x.VoucherId == id && x.UsedDate == null && (x.OrderId == null || x.BillId == null))
                    .Count();
                return Json(new { NotYetUsed = notYetUsedCount, Total = totalVoucherDetailCount , UsedOrerCount = usedOrerCount, UsedBillCount= usedBillCount }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { NotYetUsed = 0, Total = 0 }, JsonRequestBehavior.AllowGet);
        }


    }
}