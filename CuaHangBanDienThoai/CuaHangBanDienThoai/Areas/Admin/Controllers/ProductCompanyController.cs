using CuaHangBanDienThoai.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ProductCompanyController : Controller
    {
        // GET: Admin/ProductCompany

    private     CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Quản lý", "Quản trị viên")]

        public ActionResult Index(int? page )
        {
            var items = db.ProductCompany.OrderByDescending(x => x.ProductCompanyId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
           
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                ViewBag.IsAdmin = isAdmin;

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var products = db.ProductCompany.ToList();

                ViewBag.Count = products.Count;
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

        [AuthorizeFunction( "Quản trị viên")]
        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(ProductCompany model, Admin_AddProductCompany req, List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (req.Title == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập tên hãng" });
                    }
                    string alias= CuaHangBanDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim());

                    var item = await db.ProductCompany.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
                    if (item != null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Hãng đã tồn tại" });
                    }

                    if (Images == null || Images.Count == 0)
                    {
                        return Json(new { Success = false,Code =-2,msg="Vui lòng chọn logo hãng"});
                    }
                   


                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    model.Icon = Images[0];
                    model.Title = req.Title;
                    model.CreatedDate=DateTime.Now;
                    model.CreatedBy = checkStaff.NameEmployee.Trim();
                    model.Alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(req.Title.Trim());
                    db.ProductCompany.Add(model);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    return Json(new { Success = true, Code = 1 });


                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    Console.WriteLine(ex.Message);
                    return Json(new { Success = false, Code = -100 });


                }
            }
        }

        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public async Task<ActionResult> Detail(string alias)
        {
            if (alias == null)
            {
                return View();
            }
            var item = await db.ProductCompany.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (item != null)
            {
                ViewBag.Name = item.Title.Trim();
                return View(item);
            }
            return View();
        }
        public ActionResult Partail_ProDetailByCompany(int? companyid)
        {
            if(companyid!=null && companyid > 0)
            {
                var product = from pd in db.ProductDetail
                              join p in db.Products on pd.ProductsId equals p.ProductsId 
                              join pcny in db.ProductCompany on p.ProductCompanyId equals pcny.ProductCompanyId    
                              where pcny.ProductCompanyId == companyid  
                              select pd;
                var title = db.ProductCompany.Find(companyid);
                if (title!=null && product != null)
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.Title= title.Title.Trim();
                    return PartialView(product);
                }
            }
            return PartialView();   
        }


        [AuthorizeFunction( "Quản trị viên")]
        public async Task<ActionResult> Edit(string alias)
        {
            if (alias == null)
            {
                return View();
            }
            var item = await db.ProductCompany.FirstOrDefaultAsync(x => x.Alias == alias.Trim());
            if (item != null)
            {
                ViewBag.Name = item.Title.Trim();
                return View(item);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCompany model,List<string> Images, List<int> rDefault)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
            try {
                    if (ModelState.IsValid)
                    {
                        var existingcompany = db.ProductCompany.Find(model.ProductCompanyId);
                        if (existingcompany == null)
                        {
                          


                            return Json(new { Success = false, Code = -2, msg = "Hãng đã tồn tại" });
                        }

                        existingcompany.Alias = Models.Common.Filter.FilterChar(model.Title.Trim());
                        AccountEmployee nvSession = (AccountEmployee)Session["user"];
                        var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);

                        existingcompany.Modifiedby = checkStaff.NameEmployee.Trim();

                        existingcompany.ModifiedDate = DateTime.Now;
            
                        existingcompany.Title = model.Title;

                        existingcompany.CreatedDate = model.CreatedDate;
                        existingcompany.CreatedBy = model.CreatedBy;
                        existingcompany.Alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(model.Title.Trim());

             
                        if (!string.IsNullOrEmpty(model.Icon))
                        {
                            existingcompany.Icon = model.Icon;
                        }

                        db.Entry(existingcompany).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        dbContextTransaction.Commit();

                        return Json(new { Success = true, Code = 1 });
                    }
                    else
                    {
                        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                        return Json(new { Success = false, Code = -2, msg = "Vui nhập đủ thông tin" });
                    }

                }
                catch(Exception ex) {

                    dbContextTransaction.Rollback();

                    Console.WriteLine(ex.Message);
                    return Json(new { Success = false, Code = -100 });

                }
            }
        }
       

    }
}