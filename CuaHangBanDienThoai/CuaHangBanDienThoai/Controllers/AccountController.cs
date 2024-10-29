using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
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
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int failedAttempts = (int?)Session[$"FailedAttempts_{email}"] ?? 0;


                    var f_password = MaHoaPass(password);


                    var account = db.Customer
                                    .FirstOrDefault(s => s.Email == email && s.Password == f_password);

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