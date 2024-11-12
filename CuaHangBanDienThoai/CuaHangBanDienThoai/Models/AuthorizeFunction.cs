using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CuaHangBanDienThoai.Models;
using System.Web.Mvc;
namespace CuaHangBanDienThoai.Models
{
    public class AuthorizeFunction : AuthorizeAttribute
    {
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        private readonly List<string> requiredFunctions;


       
        public AuthorizeFunction(params string[] functions)
        {
            requiredFunctions = functions.ToList();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {


            int? staffId = (int?)httpContext.Session["EmployeeId"];
            if (staffId == null) return false;
            var role = GetTitleFunctionsByStaffId(staffId.Value);
            if (role != null && role.Equals("Quản trị viên"))
            {
                httpContext.Session["AdminRole"] = role;

            }
            else if (role != null && role.Equals("Quản lý"))
            {
                httpContext.Session["ManageRole"] = role;

            }
            var userFunctions = GetFunctionsByStaffId(staffId.Value);
            return requiredFunctions.Any(func => userFunctions.Contains(func));
        }
        public string GetTitleFunctionsByStaffId(int staffId)
        {
            
            var functions = (from e in db.Employee
                         where e.EmployeeId == staffId
                         select e.tb_Function.TitLe.Trim()).FirstOrDefault();
            return functions;
        }

        public List<string> GetFunctionsByStaffId(int staffId)
        {
            var functions = (from e in db.Employee
                             where e.EmployeeId == staffId
                             select e.tb_Function.TitLe.Trim()).ToList();

           

            return functions;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["EmployeeId"] == null)
            {
                filterContext.Result = new RedirectResult("/hethongnhanvien");
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Home/NoRole");
            }
        }



    }
}