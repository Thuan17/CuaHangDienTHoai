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
using System.Web.Helpers;
using System.Web.UI;
using System.Web.Razor.Tokenizer.Symbols;
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

                var functions = db.tb_Function.ToList();
               
                ViewBag.Function = new SelectList(functions, "FunctionId", "Title");
               

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
            var manager =db.Employee.ToList();      
            if (!isAdmin)
            {
               
                functions = functions.Where(f => f.TitLe.Trim() != "Quản trị viên"&& f.TitLe.Trim() != "Quản lý").ToList();
                manager = manager.Where(f => f.tb_Function.TitLe?.Trim() == "Quản lý" && f.tb_Function.TitLe?.Trim() == "Quản lý").ToList();
                if (!isManage)
                {
                    functions = functions.Where(f => f.TitLe.Trim() != "Quản lý"&& f.TitLe.Trim() != "Quản trị viên").ToList();
                    manager = manager.Where(f => f.tb_Function.TitLe?.Trim() == "Quản lý" ).ToList();
                }
            }
            ViewBag.Function = new SelectList(functions, "FunctionId", "Title");
            ViewBag.Manager = new SelectList(manager, "EmployeeId", "NameEmployee");
          
            return View();
        }
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Add(Admin_AddEmployee req , Employee emp , AccountEmployee acc, HttpPostedFileBase newImage)
        {
            if (newImage == null)
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
                        return Json(new { Success = false, Code = -2, msg = "Tên hình ảnh không quá 20 ky tu!" });
                       
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
                    emp.ManagerId = req.ManagerId == 0 ? null : (int?)req.ManagerId;
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
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Patial_Detial(int employeeid)
        {
            if (employeeid > 0)
            {
                var emp = db.Employee.FirstOrDefault(x => x.EmployeeId == employeeid);
                if (emp != null)
                {
                 bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.IsManage = isManage;
                    ViewBag.TargetRole = emp.tb_Function?.TitLe?.Trim(); // Vai trò của người đang xem
                    if (emp.ManagerId != null)
                    {
                      var Manager= db.Employee.FirstOrDefault(x => x.EmployeeId == emp.ManagerId);
                        ViewBag.Manager = Manager.NameEmployee?.Trim();  
                    }
                    return PartialView(emp); 
                }  
            }
            return PartialView();   
        }
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Partial_Edit(int employeeid)
        {
            if (employeeid > 0)
            {
                var emp = db.Employee.FirstOrDefault(x => x.EmployeeId == employeeid);
                if (emp != null)
                {
                   
                    Admin_EditEmployee viewModel = new Admin_EditEmployee
                    {
                        EmployeeId=emp.EmployeeId,
                        PhoneNumber = emp.PhoneNumber,
                        NameEmployee=emp.NameEmployee.Trim(),
                        CitizenIdentificationCard = emp.CitizenIdentificationCard.Trim(),
                        Email = emp.Email.Trim(),
                        Birthday = emp.Birthday,
                        Location = emp.Location.Trim(),
                        Wage = emp.Wage,
                        Sex = emp.Sex.Trim(),
                        TitleFuntion = emp.tb_Function.TitLe.Trim(),
                        Code  = emp.AccountEmployee.Code.Trim(),
                        Image = emp.Image??"",
                        CreatedBy = emp.CreatedBy.Trim(),
                        CreatedDate = (DateTime)emp.CreatedDate,
                        FunctionId =(int)emp.FunctionId,
                        ManagerId = emp.ManagerId.HasValue ? (int?)emp.ManagerId : null,

                        IsLock = (bool)emp.AccountEmployee.IsLock,  
                    };
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.IsManage = isManage;

                    var functions = db.tb_Function.ToList();
                    var manager = db.Employee.ToList();
                    if (!isAdmin)
                    {

                        functions = functions.Where(f => f.TitLe.Trim() != "Quản trị viên" && f.TitLe.Trim() != "Quản lý").ToList();
                        manager = manager.Where(f => f.tb_Function.TitLe?.Trim() == "Quản lý" && f.tb_Function.TitLe?.Trim() == "Quản lý").ToList();
                        if (!isManage)
                        {
                            functions = functions.Where(f => f.TitLe.Trim() != "Quản lý" && f.TitLe.Trim() != "Quản trị viên").ToList();
                            manager = manager.Where(f => f.tb_Function.TitLe?.Trim() == "Quản lý").ToList();
                        }
                    }

                    var sexOptions = new List<SelectListItem>
                            {
                                new SelectListItem { Value = "Nam", Text = "Nam" },
                                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                            };
                    ViewBag.SexOptions = new SelectList(sexOptions, "Value", "Text", emp.Sex.Trim());


                    ViewBag.Function = new SelectList(functions, "FunctionId", "Title",emp.FunctionId);
                    ViewBag.Manager = new SelectList(manager, "EmployeeId", "NameEmployee", emp.ManagerId ?? 0); 


                    Session["Admin_EditEmp" + emp.EmployeeId] = viewModel;
                    return PartialView(viewModel);
                }
               
            }
            return PartialView();
        }


        [AuthorizeFunction("Quản trị viên")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(Admin_EditEmployee req , HttpPostedFileBase newImage)
        {
            if (!ModelState.IsValid) {
                return Json(new { Success = false, Code = -2,msg="Vui lòng nhập đầy đủ thông tin !!!" }); 
            }
            if (Session["user"] == null)
            {
                return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập hết hạn!!!" });
            }
            AccountEmployee nvSession = (AccountEmployee)Session["user"];
            var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);

            using (var dbContext = db.Database.BeginTransaction())
            {

                try
                {
                    var emp = await db.Employee.FirstOrDefaultAsync(x => x.EmployeeId == req.EmployeeId);
                    if (emp == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy nhân viên !!!" });
                    }
                    var accemp = await db.AccountEmployee.FirstOrDefaultAsync(x => x.EmployeeId == req.EmployeeId);
                    if (accemp == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy nhân viên !!!" });
                    }

                    if(newImage==null)
                    {
                        bool isModified =
                        emp.NameEmployee.Trim() != req.NameEmployee.Trim() ||
                        emp.PhoneNumber.Trim() != req.PhoneNumber.Trim() ||
                        emp.CitizenIdentificationCard.Trim() != req.CitizenIdentificationCard.Trim() ||
                        emp.Email.Trim() != req.Email.Trim() ||
                        emp.Location.Trim() != req.Location.Trim() ||
                        emp.Wage != req.Wage ||
                        emp.Sex != req.Sex ||
                        emp.FunctionId != req.FunctionId ||
                         emp.ManagerId != req.ManagerId ||
                        emp.Birthday != req.Birthday ||
                        emp.AccountEmployee.IsLock != req.IsLock;
                        if (!isModified)
                        {
                            return Json(new { Success = false, Code = -4, msg = "Không có thay đổi gì !!!" });
                        }

                    }
   




                    if (emp.PhoneNumber.Trim() != req.PhoneNumber.Trim())
                    {
                        var isPhoneNumberDuplicate = await db.Employee
                                              .AnyAsync(x => x.PhoneNumber.Trim() == req.PhoneNumber.Trim());
                        if (isPhoneNumberDuplicate)
                        {
                            return Json(new { Success = false, Code = -2, msg = "Số điện thoại mới trùng !!!" });
                        }
                       
                    }
                    if (emp.Email.Trim() != req.Email.Trim() || emp.CitizenIdentificationCard.Trim() != req.CitizenIdentificationCard.Trim())
                    {
                        var isDuplicate = await db.Employee.AnyAsync(x =>
                                                                     (x.Email == req.Email.Trim() || x.CitizenIdentificationCard.Trim() == req.CitizenIdentificationCard.Trim())
                                                                     && x.EmployeeId != req.EmployeeId);
                        if (isDuplicate)
                        {
                            if (await db.Employee.AnyAsync(x => x.Email.Trim() == req.Email.Trim()))
                            {
                                return Json(new { Success = false, Code = -2, msg = "Email mới đã trùng !!!" });
                            }
                            else
                            {
                                return Json(new { Success = false, Code = -2, msg = "Số căn cước mới đã trùng !!!" });
                            }
                        }
                    }




                    emp.EmployeeId = req.EmployeeId;
                    emp.PhoneNumber = req.PhoneNumber?.Trim();
                    emp.NameEmployee = req.NameEmployee?.Trim();

                    emp.CitizenIdentificationCard = req.CitizenIdentificationCard?.Trim();
                    emp.Email = req.Email?.Trim();
                    emp.Birthday = req.Birthday;
                    emp.Location = req.Location?.Trim();
                    emp.Wage = req.Wage;
                    emp.Sex = req.Sex?.Trim();


                    emp.Image = req.Image?.Trim();
                    emp.CreatedBy = req.CreatedBy?.Trim();
                    emp.CreatedDate = (DateTime)req.CreatedDate;
                    emp.FunctionId = (int)req.FunctionId;
                    emp.ModifiedBy = checkStaff.NameEmployee?.Trim();
                    emp.ModifiedDate = DateTime.Now;    
                    emp.FunctionId = (int)req.FunctionId;
                    emp.ManagerId = req.ManagerId == 0 ? null : (int?)req.ManagerId;

                    accemp.IsLock = (bool)req.IsLock;
                  
                    if (newImage == null)
                    {
                        emp.Image = req.Image?.Trim();
                    }
                    else
                    {
                        string fileName = Path.GetFileName(newImage.FileName);
                        if (fileName.Length > 20)
                        {
                            return Json(new { Success = false, Code = -2, msg = "Tên hình ảnh không quá 20 ky tu!" });

                        }
                        string path = Path.Combine(Server.MapPath("~/UploadsEmp/Image"), fileName);
                        newImage.SaveAs(path);
                        emp.Image = fileName?.Trim();

                    }
                    db.Entry(emp).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    db.Entry(accemp).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Câp nhập thành công !!!" });
                }
                catch (Exception ex) 
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });
                } 
              

            }
        }
        [HttpGet]
        public ActionResult EmployeeById(int? functionChoseid)
        {
            if (functionChoseid > 0)
            {
                var fun = db.tb_Function.Find(functionChoseid);
                if (fun != null) 
                {
                    var emp = db.Employee.Where(x => x.FunctionId == functionChoseid).OrderByDescending(x => x.FunctionId).ToList();
                    if (emp != null)
                    {
                        ViewBag.Key = fun.TitLe.Trim();
                        ViewBag.Count = emp.Count();

                        var functions = db.tb_Function.ToList();

                        ViewBag.Function = new SelectList(functions, "FunctionId", "Title", fun.FunctionId);
                        bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                        bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                        ViewBag.IsAdmin = isAdmin;
                        ViewBag.IsManage = isManage;

                        return PartialView(emp);
                    }
                }  
            }
            return PartialView();
        }


        [HttpGet]
        public async Task<ActionResult> Search(string search)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
               
                var keyword = search.Trim().ToLower();
                var alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(keyword);   
                var employees = await (from acc in db.AccountEmployee
                                       join emp in db.Employee on acc.EmployeeId equals emp.EmployeeId
                                       where acc.Code.ToLower().Contains(keyword) ||
                                             emp.NameEmployee.ToLower().Contains(keyword) || 
                                             emp.PhoneNumber.Contains(keyword) 
                                       select emp)
                                       .OrderBy(x => x.NameEmployee) 
                                       .ToListAsync();

          
                var totalCount = employees.Count;

              
                ViewBag.Key = search.Trim();
                ViewBag.Count = totalCount;
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsManage = isManage;

                var functions = db.tb_Function.ToList();

                ViewBag.Function = new SelectList(functions, "FunctionId", "Title");
                return PartialView(employees); 
            }
            return PartialView(new List<Employee>());
        }




        [HttpPost]
        public async Task<ActionResult> ResetPass(int employeeId)
        {
            if (employeeId <= 0)
            {
                return Json(new { Success = false, Code = -2, msg = "Không tìm thấy nhân viên !!! " });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var acc = await db.AccountEmployee.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
                    if (acc == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy nhân viên !!! " });
                    }
                    var f_pass = MaHoaPass("123");
                    acc.Password = f_pass.Trim();
                    db.Entry(acc).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    dbContext.Commit();
                    return Json(new { Success = true, Code = 1, msg = "Đổi mật khẩu mặt định thành công " });
                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng !!! " });
                }
            }



        }



   

        [HttpPost]
        public async Task<ActionResult> IsLock(int id)
        {
            try
            {
                var item = await db.AccountEmployee.FirstOrDefaultAsync(x=>x.EmployeeId==id);

                if (item != null)
                {
                    item.IsLock = !item.IsLock;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Json(new { success = true, isLock = item.IsLock });
                }

                return Json(new { success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, msg = "Lỗi cập nhập tag hiển thị trang chủ" });
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