using CuaHangBanDienThoai.Common;
using CuaHangBanDienThoai.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using static CuaHangBanDienThoai.Controllers.AccountController;

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
                        return Json(new { success = true, code = 1, msg = "Chào mừng trở lại", customerId= account.CustomerId }); 


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

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(CLient_Register req , Customer _khachhang)
        {

            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        int failedAttempts = (int?)Session[$"FailedAttemptsCustomer_{req.Email}"] ?? 0;


                        var f_password = MaHoaPass(req.password);


                        var checkEmail = await db.Customer.SingleOrDefaultAsync(x => x.Email == req.Email.Trim());
                        if (checkEmail != null)
                        {
                            return Json(new { success = false, code = -2, msg = "Email đã có trên hệ thống !!!" });
                        }





                        var checkPhone = await db.Customer.FirstOrDefaultAsync(x => x.PhoneNumber == req.phone.Trim() && x.Email == null);
                        if (checkPhone != null)
                        {
                         
                            checkPhone.Email = req.Email.Trim();
                            checkPhone.PhoneNumber = req.phone.Trim();
                            checkPhone.Password = f_password.Trim();
                            checkPhone.Birthday = req.Birthday;
                            checkPhone.IsLock = false;
                            db.Entry(checkPhone).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            dbContext.Commit(); 
                            return Json(new { success = true, code = 1, msg = "Đăng ký thành công", Url="/dang-nhap" });
                        }
                        else
                        {
                            _khachhang.Email = req.Email;
                            _khachhang.PhoneNumber = req.phone.Trim();
                            _khachhang.Password = f_password.Trim();
                            _khachhang.Birthday = req.Birthday;
                            _khachhang.IsLock = false;
                            _khachhang.CustomerName = req.fullName.Trim();
                            _khachhang.NumberofPurchases = 0;
                            db.Customer.Add(_khachhang);

                            await db.SaveChangesAsync();

                            dbContext.Commit();
                            return Json(new { success = true, code = 1, msg = "Đăng ký thành công", Url = "/dang-nhap" });
                        }

                    }
                    else
                    {

                        return Json(new { success = false, code = -2, msg = "Thông tin đăng nhập không hợp lệ." });
                    }
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();   
                    return Json(new { success = false, code = -100, message = "Lỗi hệ thống: " + ex.Message });
                }
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


        public ActionResult ForgotPassword()
        {

            return View();
        }
        [HttpPost]
      
        public async Task<ActionResult> ForgotPassword(string userInput)
        {
            if (userInput == null)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập số điện thoại hoặc số điện thoại" });
            }
            using (var dbConText = db.Database.BeginTransaction())
            {
                try {
                    var customer = await db.Customer.FirstOrDefaultAsync(s => (s.Email == userInput || s.PhoneNumber == userInput));
                    if (customer == null) 
                    {
                        return Json(new { Success = false, Code = -2, msg = "Tài khoản không hợp lệ" });
                    }


                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var random = new Random();
                    var randomString = new StringBuilder(3);

                    for (int x = 0; x < 6; x++)
                    {
                        randomString.Append(chars[random.Next(chars.Length)]);
                    }

                    string randomString123 = randomString.ToString(); ;

                    customer.Code = randomString123.Trim();
                    customer .CodeCreatedAt = DateTime.Now;
                    db.Entry(customer).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/SendForgotPass.html"));
                    contentCustomer = contentCustomer.Replace("{{Code}}", customer.Code);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", customer.CustomerName.Trim());
                    contentCustomer = contentCustomer.Replace("{{Email}}", customer.Email.Trim());

                    CuaHangBanDienThoai.Common.Common.SendMail("PadaMiniStore", "Mã khôi phục tài khoản" , contentCustomer.ToString(), customer.Email);

                    
                    dbConText.Commit();

                    return Json(new { Success = true, Code = 1, msg = "Mã đã được gửi qua mail",CustomerId=customer.CustomerId , Email=customer.Email?.Trim() });
                }
                catch(Exception ex)
                {
                    dbConText.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
                }
            }
        }


        public ActionResult RessetPass(int id, string email)
        {
            if(id>0 && email!=null)
            {
                string decodedEmail = HttpUtility.UrlDecode(email);

                var customer = db.Customer.FirstOrDefault(s => s.CustomerId == id && s.Email.Trim() == decodedEmail.Trim());
                if (customer != null)
                {

                    UpdatePass viewModel = new UpdatePass
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.CustomerName,
                        Code = null,
                        Password = null,

                    };

                    return View(viewModel);
                }
            }
            return View();
        }

        public ActionResult ChangePass(int customerid)
        {

            customerid = 3;
            if (customerid > 0)
            {


                var customer = db.Customer.FirstOrDefault(s => s.CustomerId == customerid);
                if (customer != null)
                {

                    UpdatePass viewModel = new UpdatePass
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.CustomerName,
                        Code = null,
                        Password = null,

                    };

                    return PartialView(viewModel);
                }
            }
            return PartialView();
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePass(UpdatePass req )
        {
            if(req.CustomerId <= 0 && req.Password == null)
            {
                return Json(new { Success = false, Code = 2, msg = "Vui lòng nhập đầy đủ thông tin !!!" });
            }
            using (var dbConText = db.Database.BeginTransaction())
            {
                try {
                    var customer = await db.Customer.FirstOrDefaultAsync(x => x.CustomerId == req.CustomerId);
                    if (customer == null)
                    {
                        return Json(new { Success = false, Code = -3, msg = "Không tìm thấy tài khoản !!!" });
                    }
                    var f_password = MaHoaPass(req.Password.Trim());
                    var f_passwordNew = MaHoaPass(req.PasswordNew.Trim());
                    if (customer.Password.Trim()!= f_password.Trim())
                    {
                        return Json(new { Success = false, Code = -2, msg = "Sai mật khẩu !!!" });
                    }
                    if (customer.Password.Trim() == f_passwordNew.Trim())
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng để trùng mật khẩu cũ!!!" });
                    }

                 




                    customer.Password = f_passwordNew.Trim(); 
                    db.Entry(customer).State = EntityState.Modified;    
                    await db.SaveChangesAsync();
                   dbConText.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Cập nhập thành công " });
                }
                catch(Exception ex) 
                {
                    dbConText.Rollback();   
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                }
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
                var nonConfirmOrders = totalOrders.Count(o => o.Confirm == false&& (o.OrderStatus?.Trim() != "Đơn huỷ"&&o.OrderStatus?.Trim() != "Từ chối"));

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
                }, JsonRequestBehavior.AllowGet) ;
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về thông báo lỗi chi tiết
                Debug.WriteLine($"Lỗi: {ex.Message}");
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> Profile(string encodedId,string name )
        {
            if (string.IsNullOrEmpty(encodedId)&& string.IsNullOrEmpty(name))
            {
                return View();
            }

            try
            {
           
                int customerId = DecodeCustomerId(encodedId);

             
                var customer = await db.Customer.FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    return View();
                }
                ViewBag.Email = customer.Email.Trim();

                ViewBag.MaskedEmail= Helper.MaskEmail(customer.Email.Trim());
                return View(customer);
            }
            catch
            {
              
                return View();
            }
        }



        public ActionResult UpdateProFile(int customerid)
        {
            if (customerid > 0)
            {
                var customer = db.Customer.Find(customerid);
                if (customer != null)
                {
                    Client_UpdateProFile viewModel = new Client_UpdateProFile { 
                    CustomerId = customer.CustomerId, 
                    CustomerName=customer.CustomerName.Trim(),
                        PhoneNumber = customer.PhoneNumber.Trim(),
                    Email=customer.Email.Trim(),
                        Birthday = customer.Birthday,
                    
                    };
                    if (viewModel != null)
                    {
                        return PartialView(viewModel);
                    }
                }
            }


            return PartialView();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> UpdateProFile(Client_UpdateProFile req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập đầy đủ thông tin" });

            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    if(req.CustomerId < 0)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập đầy đủ thông tin" });
                    }
                    else 
                    {
                        var customer=await db.Customer.FindAsync(req.CustomerId);
                        if (customer == null)
                        {
                            return Json(new { Success = false, Code = -2, msg = "Không tìm thấy dữ liệu trên" });
                        }
                        var f_password = MaHoaPass(req.PassWord.Trim());
                         if (customer.Password != f_password)
                        {
                            return Json(new { Success = false, Code = -2, msg = "Sai mật khẩu !!!" });
                        }
                        customer.Email=req.Email.Trim();
                        customer.CustomerName=req.CustomerName.Trim();
                        customer.Birthday = req.Birthday;
                        db.Entry(customer).State= EntityState.Modified;
                        await db.SaveChangesAsync();
                        dbContext.Commit(); 
                        return Json(new { Success = true, Code = 1, msg = "Cập nhập tài khoản thành công" });
                    }
                } 
                catch(Exception ex)
                {
                    dbContext.Rollback   ()   ;
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
                }
            }
        }




        public string EncodeCustomerId(int customerId)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(customerId.ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public int DecodeCustomerId(string encodedId)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(encodedId);
            return int.Parse(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
        }


        public ActionResult UpadatePass(int id , string email)
        {
           
            if (id>0 && email != null)
            {
                string decodedEmail = HttpUtility.UrlDecode(email);

                var customer =  db.Customer.FirstOrDefault(s => s.CustomerId== id && s.Email.Trim()== decodedEmail.Trim());
                if (customer != null)
                {

                    UpdatePass viewModel = new UpdatePass{ 
                        CustomerId = customer.CustomerId,       
                        CustomerName = customer.CustomerName,   
                        Code = null,
                        Password = null,

                    };

                    return View(viewModel);
                }
            }
            return View();
        }


        public async Task<ActionResult> UpPassForget(UpdatePass req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập thông tin" });
            }

            using (var dbConText = db.Database.BeginTransaction())
            {
                try
                {
                    var customer = await db.Customer.FirstOrDefaultAsync(s => s.CustomerId == req.CustomerId);
                    if (customer == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Tài khoản không hợp lệ" });
                    }

                    var checkCode = await db.Customer.FirstOrDefaultAsync(s => s.CustomerId == customer.CustomerId && s.Code.Trim() == req.Code.Trim());
                    if (checkCode == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Mã không hợp lệ vui lòng thử lại !!!" });
                    }

                    DateTime dateTime = DateTime.Now;

                    var currentTimeUtc = DateTime.UtcNow; 
                    var codeCreatedAtUtc = checkCode.CodeCreatedAt.GetValueOrDefault().ToUniversalTime(); 

                    var timeSinceCodeCreated = currentTimeUtc - codeCreatedAtUtc;

                 


                    if (timeSinceCodeCreated.TotalMinutes >=5)
                    {
                        customer.Code = null;
                        

                        db.Entry(checkCode).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        dbConText.Commit();
                        return Json(new { Success = false, Code = -3, msg = "Mã đã hết hạn.", Url = "/quen-mat-khau" });
                    }

                    var f_password = MaHoaPass(req.Password);
                    checkCode.Password = f_password.Trim();
                    customer.Code = null;
                    db.Entry(checkCode).State = EntityState.Modified;
                  
                    await db.SaveChangesAsync();
                    dbConText.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Cập nhập thành công ,hãy đăng nhập lại", Url = "/dang-nhap" });
                }
                catch (Exception ex)
                {
                    dbConText.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
                }
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