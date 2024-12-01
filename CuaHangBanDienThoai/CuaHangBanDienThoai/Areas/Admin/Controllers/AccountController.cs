using CuaHangBanDienThoai.Common;
using CuaHangBanDienThoai.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string Code, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int failedAttempts = (int?)Session[$"FailedAttempts_{Code}"] ?? 0;


                    var f_password = MaHoaPass(password);


                    var account = db.AccountEmployee
                                    .FirstOrDefault(s => s.Code == Code && s.Password == f_password);

                    if (account != null)
                    {

                        if (account.IsLock == true)
                        {
                            return Json(new { success = false, code = -4, msg = "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên!" });
                        }


                        if (failedAttempts >= 5)
                        {
                            account.IsLock = true;
                            await db.SaveChangesAsync();
                            failedAttempts = 0;
                            Session[$"FailedAttempts_{Code}"] = 0;
                            return Json(new { success = false, code = -5, message = "Tài khoản đã bị khóa sau nhiều lần đăng nhập sai." });
                        }


                        var employee = db.Employee.FirstOrDefault(s => s.EmployeeId == account.EmployeeId);
                        if (employee == null)
                        {
                            return Json(new { success = false, code = -2, msg = "Không tìm thấy thông tin nhân viên." });
                        }

                        var Funtion = db.Employee.FirstOrDefault(s => s.EmployeeId == account.EmployeeId);
                      

                        var fullName = employee.NameEmployee.Trim();
                        var lastName = fullName.Split(' ').Last();
                        Session["EmployeeId"] = employee.EmployeeId;
                        Session["AdminRole"] = employee.EmployeeId;
                      
                        Session["lastName"] = lastName.Trim();
                        Session["customerName"] = employee.NameEmployee.Trim();
                        Session["user"] = account;
                        Session["nameuser"] = employee.NameEmployee;
                        Session["userName"] = employee;
                        if (employee.Image != null)
                        {
                            Session["userimg"] = employee.Image;
                        }


                        string redirectUrl = DetermineRedirectUrl((int)employee.FunctionId, employee.tb_Function.TitLe);
                        if (redirectUrl != null)
                        {
                            return Json(new { success = true, redirectUrl = redirectUrl, msg = "Chào mừng trở lại!" });
                        }
                        else
                        {
                            failedAttempts++;
                            Session[$"FailedAttempts_{Code}"] = failedAttempts;
                            return Json(new { success = false, code = -1, msg = "Chức năng không hợp lệ." });
                        }
                    }
                    else
                    {
                        failedAttempts++;
                        Session[$"FailedAttempts_{Code}"] = failedAttempts;

                        return Json(new { success = false, code = -4, msg = "Tài khoản hoặc mật khẩu không đúng." });
                    }
                }
                else
                {

                    return Json(new { success = false, code = -5, msg = "Thông tin đăng nhập không hợp lệ." });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ngoại lệ
                return Json(new { success = false, code = -100, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        public async Task<ActionResult> Profile(string encodedId, string name)
        {
            if (string.IsNullOrEmpty(encodedId) && string.IsNullOrEmpty(name))
            {
                return View();
            }

            try
            {

                int customerId = DecodeCustomerId(encodedId);


                var customer = await db.Employee.FirstOrDefaultAsync(c => c.EmployeeId == customerId);
                if (customer == null)
                {
                    return View();
                }
                ViewBag.Email = customer.Email.Trim();
                ViewBag.Name=customer.NameEmployee.Trim();
                ViewBag.MaskedEmail = Helper.MaskEmail(customer.Email.Trim());
                return View(customer);
            }
            catch
            {

                return View();
            }
        }
        public async Task<ActionResult> UpdatePass(string encodedId, string name)
        {
            if (string.IsNullOrEmpty(encodedId) && string.IsNullOrEmpty(name))
            {
                return View();
            }

            try
            {

                int customerId = DecodeCustomerId(encodedId);


                var customer = await db.Employee.FirstOrDefaultAsync(c => c.EmployeeId == customerId);
                if (customer == null)
                {
                    return View();
                }
                Admin_UpdatePassEmployee viewModel = new Admin_UpdatePassEmployee {
                    EmployeeId= customer.EmployeeId,
                    Code=customer.AccountEmployee.Code,
                    IsLock=(bool)customer.AccountEmployee.IsLock,
                    Password = customer.AccountEmployee.Password,
                    employee=customer,  

                };
                ViewBag.Email = customer.Email.Trim();
                ViewBag.Name = customer.NameEmployee.Trim();
                ViewBag.MaskedEmail = Helper.MaskEmail(customer.Email.Trim());


              
                return PartialView(viewModel);
            }
            catch
            {

                return PartialView();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePass(Admin_UpdatePassEmployee req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đầy đủ thông tin !!!" });

            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var f_passold = MaHoaPass(req.Password);
                    var checkLock = await db.AccountEmployee.FirstOrDefaultAsync(x => x.EmployeeId == req.EmployeeId && x.Password== f_passold.Trim());
                    if (checkLock == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Tài khoản không tồn tại !!!" });
                    }
                    if (checkLock.IsLock == true)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Tài khoản đã bị khoá !!!" });
                    }
                    var f_pass = MaHoaPass(req.PasswordNew);
                    if(checkLock.Password.Trim() == f_pass.Trim())
                    {
                        return Json(new { Success = false, Code = -2, msg = "Mật khẩu trùng với trước !!!" });
                    }
                    checkLock.Password = f_pass.Trim(); 
                    db.Entry(checkLock).State = EntityState.Modified; 
                    await db.SaveChangesAsync();    
                    dbContext.Commit();
                    return Json(new { Success = true, Code =1 , msg = "Mật khẩu thay đổi thành công !!!" });

                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
            }
        }
        public int DecodeCustomerId(string encodedId)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(encodedId);
            return int.Parse(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
        }

        public ActionResult Logout()
        {
            if (Session["user"] != null || Session["userimg"] != null)
            {
                Session["user"] = null;
                Session["nameuser"] = null;
                Session["userName"] = null;
                Session["userimg"] = null;
                Session["EmployeeId"] = null;
                Session["AdminRole"] = null;
                Session["ManageRole"] = null;
                Session["lastName"] = null;
                Session["customerName"] = null;
               
               
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Login", "Account");
        }

        private string DetermineRedirectUrl(int roleEmployeeId, string functionTitle)
        {
            if (roleEmployeeId == 3)
            {
              
              
                return "/ban-hang";
            }
            else if (roleEmployeeId == 4)
            {
              
                return "/danh-sach-phieunhap";
            }
            else if (roleEmployeeId == 1 || roleEmployeeId == 2 || functionTitle.Contains("Quản trị viên") || functionTitle.Contains("Quản Lý"))
            {
                return "/hethongquanly";
            }
            return null;
        }

        public static string MaHoaPass(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromaccount = Encoding.UTF8.GetBytes(str);
            byte[] targetaccount = md5.ComputeHash(fromaccount);
            string byte2String = null;

            for (int i = 0; i < targetaccount.Length; i++)
            {
                byte2String += targetaccount[i].ToString("x2");

            }
            return byte2String;

        }
    }
}