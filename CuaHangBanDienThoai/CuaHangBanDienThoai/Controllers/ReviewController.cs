using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuaHangBanDienThoai.Models;

namespace CuaHangBanDienThoai.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Partial_AddReview(int? productid ,string name)
        {
            if (productid != null && productid > 0&& name!=null)
            {
                var product = db.Products.Find(productid);
                if (product != null)
                {
                    Client_AddReview viewModel = new Client_AddReview
                    {
                        ProductsId = product.ProductsId,
                        Name = name.Trim(),
                    };
                    return PartialView(viewModel);
                }
            }
            return PartialView();
        }
    }

}