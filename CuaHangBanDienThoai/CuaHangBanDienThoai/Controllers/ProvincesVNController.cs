using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;
namespace CuaHangBanDienThoai.Controllers
{
    public class ProvincesVNController : Controller
    {
        // GET: ProvincesVN
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();   
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetDistrictsByProvince(int idProvinces)
        {
            var districts = db.Districts
                .Where(d => d.idProvinces == idProvinces)
                .Select(d => new
                {
                    d.idDistricts,
                    d.name
                })
                .ToList();
            return Json(districts, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetWardsByDistrict(int idDistricts)
        {
            var wards = db.Wards
                .Where(w => w.idDistricts == idDistricts)
                .Select(w => new
                {
                    w.idWards,
                    w.name
                })
                .ToList();
            return Json(wards, JsonRequestBehavior.AllowGet);
        }
    }
}