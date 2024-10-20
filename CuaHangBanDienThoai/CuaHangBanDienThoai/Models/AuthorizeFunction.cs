﻿using System;
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

            
            var userFunctions = GetFunctionsByStaffId(staffId.Value);

        
            return requiredFunctions.Any(func => userFunctions.Contains(func));
        }

        public List<string> GetFunctionsByStaffId(int staffId)
        {
            var functions = (from f in db.tb_Function
                             join r in db.Role on f.FunctionId equals r.FunctionId
                             where r.EmployeeId == staffId
                             select f.TitLe).ToList();

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