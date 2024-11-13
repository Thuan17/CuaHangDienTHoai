using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Admin/Employee
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.Employee.OrderByDescending(x => x.EmployeeId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var emp = db.Employee.ToList();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsManage = isManage;
                ViewBag.Count = emp.Count;
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
            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsManage = isManage;

            var functions = db.tb_Function.ToList();
            if (!isAdmin)
            {
               
                functions = functions.Where(f => f.TitLe.Trim() != "Quản trị viên"&& f.TitLe.Trim() != "Quản lý").ToList();
                if (!isManage)
                {
                    functions = functions.Where(f => f.TitLe.Trim() != "Quản lý"&& f.TitLe.Trim() != "Quản trị viên").ToList();
                }
            }
            ViewBag.Function = new SelectList(functions, "FunctionId", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Add(Admin_AddEmployee req , Employee emp , AccountEmployee acc, HttpPostedFileBase newImage)
        {
            if (newImage == null && newImage.ContentLength < 0)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng chọn ảnh nhân viên !!!" });
            }
            if (Session["user"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập hết hạn!!!" });
            }
            AccountEmployee nvSession = (AccountEmployee)Session["user"];
            var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
            if (!ModelState.IsValid) 
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đầy đủ thông tin !!!" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try 
                {
                    var isPhoneNumberDuplicate = await db.Employee
                                                 .AnyAsync(x => x.PhoneNumber.Trim() == req.PhoneNumber.Trim());
                    if (isPhoneNumberDuplicate)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Số điện thoại trùng !!!" });
                    }

                
                    var isDuplicate = await db.Employee.AnyAsync(x =>
                        x.Email == req.Email.Trim() ||
                        x.CitizenIdentificationCard.Trim() == req.CitizenIdentificationCard.Trim());

                    if (isDuplicate)
                    {
                        if (await db.Employee.AnyAsync(x => x.Email.Trim() == req.Email.Trim()))
                        {
                            return Json(new { Success = false, Code = -2, msg = "Email đã trùng !!!" });
                        }
                        else
                        {
                            return Json(new { Success = false, Code = -2, msg = "Số căn cước đã trùng !!!" });
                        }
                    }


                    emp.PhoneNumber = req.PhoneNumber?.Trim();  
                    emp.NameEmployee = req.NameEmployee?.Trim();  
                    emp.CitizenIdentificationCard = req.CitizenIdentificationCard?.Trim();  
                    emp.Email = req.Email?.Trim();
                 

                    DateTime birthday;
                    if (DateTime.TryParse(req.Birthday, out birthday))
                    {
                        emp.Birthday = birthday.Date;  // Chỉ lấy phần ngày
                    }
                    else
                    {
                        return Json(new { Success = false, Code = -2, msg = "Ngày sinh không hợp lệ!" });
                    }

                    string fileName = Path.GetFileName(newImage.FileName);
                    if (fileName.Length > 20)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Tên hình ảnh không 20 ky tu!" });
                       
                    }
                    string path = Path.Combine(Server.MapPath("~/UploadsEmp/Image"), fileName);
                    newImage.SaveAs(path);
                    emp.Image = fileName?.Trim() ?? "/images/logo/logo2.png";


                    emp.Location = req.Location?.Trim();  
                    emp.Wage = req.Wage;  
                    emp.Sex = req.Sex?.Trim();  
                    emp.CreatedDate = DateTime.Now;
                    emp.CreatedBy = checkStaff.NameEmployee?.Trim();
                    emp.FunctionId = req.FunctionId;
                 
                    //emp.Image = req.Image?.Trim()?? "/images/logo/logo2.png";
                     db.Employee.Add(emp);
                    await db.SaveChangesAsync();


                    const string chars = "0123456789";
                    var random = new Random();
                    string randomString;

                    do
                    {
                        var randomStringBuilder = new StringBuilder(6);
                        for (int x = 0; x < 6; x++)
                        {
                            randomStringBuilder.Append(chars[random.Next(chars.Length)]);
                        }
                        randomString = randomStringBuilder.ToString();
                    }
                    while (await db.AccountEmployee.AnyAsync(x => x.Code == randomString.Trim()));


                    var f_Pass = MaHoaPass("123");
                    acc.Code = randomString.Trim();
                    acc.EmployeeId = emp.EmployeeId;
                     acc.Password = f_Pass;    
                     acc.IsLock = false;    
                     db.AccountEmployee.Add(acc);
                    await db.SaveChangesAsync();
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Tạo hồ sơ thành công" });
                }
                catch (Exception ex) 
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
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