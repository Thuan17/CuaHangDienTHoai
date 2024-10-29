using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CuaHangBanDienThoai.Models;
using System.Web.Mvc;

namespace CuaHangBanDienThoai.Models
{
    public class AuthorizeCustomer : AuthorizeAttribute
    {
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        private readonly List<string> requiredFunctions;



        public AuthorizeCustomer(params string[] functions)
        {
            requiredFunctions = functions.ToList();
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