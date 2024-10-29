using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
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
        public ActionResult SetRedirectUrl(string redirectUrl)
        {
            Session["redirectUrl"] = redirectUrl; 
            return Json(new { success = true }); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string userInput, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    int failedAttempts = (int?)Session[$"FailedAttemptsCustomer_{userInput}"] ?? 0;


                    var f_password = MaHoaPass(password);


                    var account = db.Customer
                                    .FirstOrDefault(s => (s.Email == userInput || s.PhoneNumber == userInput) && s.Password == f_password);

                    if (account != null)
                    {

                        if (account.IsLock == true)
                        {
                            return Json(new { success = false, code = -2, msg = "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên!" });
                        }


                        if (failedAttempts >= 5)
                        {
                            account.IsLock = true;
                            await db.SaveChangesAsync();
                            failedAttempts = 0;
                            Session[$"FailedAttemptsCustomer_{userInput}"] = 0;
                            return Json(new { success = false, code = -2, message = "Tài khoản đã bị khóa sau nhiều lần đăng nhập sai." });
                        }
                        var fullName = account.CustomerName.Trim();
                        var lastName = fullName.Split(' ').Last();
                        Session["customer"] = account;
                        Session["userImage"] = account.Image ?? "/images/logo/logoweb.png";
                        Session["lastName"] = lastName.Trim();
                        Session["customerName"] = account.CustomerName.Trim();
                        Session["CustomerId"] = account.CustomerId;

                        if (account.Image != null)
                        {
                            Session["userimg"] = account.Image;
                        }
                        if (Session["redirectUrl"] != null)
                        {
                            string redirectUrl = Session["redirectUrl"].ToString();
                            Session.Remove("redirectUrl");
                          
                            return Json(new { success = true, code = 1, msg = "Chào mừng trở lại" , redirectUrl = redirectUrl.Trim(), customerId = account.CustomerId }); 
                        }
                        return Json(new { success = true, code = 1, msg = "Chào mừng trở lại", customerId= account.CustomerId }); ;


                    }
                    else
                    {
                        failedAttempts++;
                        Session[$"FailedAttemptsCustomer_{userInput}"] = failedAttempts;

                        return Json(new { success = false, code = -2, msg = "Tài khoản hoặc mật khẩu không đúng." });
                    }
                }
                else
                {

                    return Json(new { success = false, code = -2, msg = "Thông tin đăng nhập không hợp lệ." });
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
            if (Session["customer"] != null)
            {
              
                Session["customer"] = null;
                Session["userImage"] = null;
                Session["lastName"] = null;
                Session["customerName"] = null;
                Session["CustomerId"] = null;
               

              
                return Redirect("/dang-nhap");
            }

           
            return Redirect("/trang-chu");
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