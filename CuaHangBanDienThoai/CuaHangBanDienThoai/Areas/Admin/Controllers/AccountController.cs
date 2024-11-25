using CuaHangBanDienThoai.Models;
using System;
using System.Collections;
using System.Collections.Generic;
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

                        Session["EmployeeId"] = employee.EmployeeId;
                        Session["AdminRole"] = employee.EmployeeId;


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
              
                return Url.Action("IndexEmployye", "Seller");
            }
            else if (roleEmployeeId == 4)
            {
                return Url.Action("StaffIndex", "WareHouseImport");
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