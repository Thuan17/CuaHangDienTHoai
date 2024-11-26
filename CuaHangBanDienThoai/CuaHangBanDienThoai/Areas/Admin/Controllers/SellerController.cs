using CuaHangBanDienThoai.Common;
using CuaHangBanDienThoai.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using ZXing;
using ZXing.QrCode;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;



using ImageProcessor.Processors;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using ZXing.QrCode.Internal;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class SellerController : Controller
    {
        // GET: Admin/Seller



        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Nhân viên bán hàng", "Quản lý", "Quản trị viên")]
        public ActionResult Index()
        {

            var product = db.Products.ToList();
            ViewBag.Product = new SelectList(product, "ProductsId", "Title");
            ViewBag.Date=DateTime.Now.Date;
            return View();
        }
        [AuthorizeFunction("Nhân viên bán hàng", "Quản lý", "Quản trị viên")]
        public ActionResult IndexEmployye()
        {

            var product = db.Products.ToList();
            ViewBag.Product = new SelectList(product, "ProductsId", "Title");
            ViewBag.Date = DateTime.Now.Date;
            return View();
        }
        public async Task<ActionResult> Partail_ProductById(int productId)
        {
            if (productId>0&&db.ProductDetail.Any())
            {
                var pro =await db.ProductDetail.Where(x =>x.ProductsId==productId &&x.IsActive == true && x.Quantity > 0).ToListAsync();
                if (pro != null)
                {
                    ViewBag.Count = pro.Count();
                    return PartialView(pro);
                }
            }
            return PartialView();

        }
        [AuthorizeFunction( "Quản lý", "Quản trị viên")]
        public ActionResult AllBill()
        {
            DateTime selectedDate = DateTime.Now.Date;
            var bill =db.Bill.ToList();
            if (bill != null)
            {

                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;

                ViewBag.Date = DateTime.Now.Date;
                ViewBag.IsAdmin = isAdmin;

                ViewBag.CurrentEmployeeId = employeeId;
                ViewBag.Today = selectedDate.Date;
                ViewBag.IsManage = isManage;
                ViewBag.Count=bill.Count;
                return View(bill);
            }
            return View();
        }
        public ActionResult Partail_BillToday()
        {
            DateTime selectedDate = DateTime.Now.Date;



            var bill = db.Bill
                         .Where(x => DbFunctions.TruncateTime(x.CreatedDate) == selectedDate).OrderBy(x => x.BillId)
                         .ToList();
            if (bill != null &bill.Count > 0)
            {

                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;

               
                ViewBag.IsAdmin = isAdmin;

                ViewBag.CurrentEmployeeId = employeeId;

                ViewBag.IsManage = isManage;
                ViewBag.Count = bill.Count;
                ViewBag.Today = selectedDate.Date;
                return PartialView(bill);
            }
            return PartialView();
        }


        public ActionResult GetBillByDate(DateTime ngayxuat)
        {
            try
            {
                DateTime selectedDate = ngayxuat;

                DateTime startDate = selectedDate.Date;
                DateTime endDate = startDate.AddDays(1);

                var orders = db.Bill
                    .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                    .ToList();

                if (orders != null && orders.Count > 0)
                {



                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                    int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;

                    // Truyền thông tin qua ViewBag
                    ViewBag.IsAdmin = isAdmin;

                    ViewBag.CurrentEmployeeId = employeeId;

                    ViewBag.IsManage = isManage;
                    ViewBag.Count = orders.Count;
                    ViewBag.Date = ngayxuat.ToString("dd/MM/yyyy");
                    ViewBag.Content = ngayxuat.ToString("dd/MM/yyyy");
                    return PartialView(orders);  
                }
                else
                {
                    ViewBag.Count = 0;
                    ViewBag.Date = ngayxuat.ToString("dd/MM/yyyy"); 
                    return PartialView(); 
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { Success = false, Code = -99, msg = "Có lỗi xảy ra!" });
            }
        }







        [HttpGet]
        public async Task<ActionResult> SearchBill(string search)
        {

            if (Session["user"] == null)
            {
                return RedirectToAction("Account", "Login");

            }

            if (!String.IsNullOrEmpty(search))
            {

                var keyword = search.Trim().ToLower();
                var alias = CuaHangBanDienThoai.Models.Common.Filter.FilterChar(keyword);


                var querry=await (from customer in db.Customer
                                  join bill in db.Bill on customer.CustomerId equals bill.CustomerId  
                                  where customer.PhoneNumber.ToLower().Trim().Contains(keyword)||
                                       bill.Code.ToLower().Trim().Contains(keyword)  
                                       select bill  ).OrderByDescending(x=>x.CreatedDate).ToListAsync();
                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;

                // Truyền thông tin qua ViewBag
                ViewBag.IsAdmin = isAdmin;
              
                ViewBag.CurrentEmployeeId = employeeId;
               
                ViewBag.IsManage = isManage;
                if (querry.Count > 0)
                {
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];


                    ViewBag.CurrentEmployeeId = employeeId;
                    var totalCount = querry.Count;
                    ViewBag.Key = search.Trim();
                    ViewBag.Count = totalCount;
                 
                    return PartialView(querry);

                }  
               
            }
            ViewBag.Key = search.Trim();
            return PartialView();
        }



        public ActionResult GetUpdatedBillRow(int billid)
        {

            var bill = db.Bill.FirstOrDefault(b => b.BillId == billid);
            if (bill != null)
            {
                return PartialView(bill);
            }
            return HttpNotFound();
        }




        public ActionResult Partial_BillByEmployee(int? employeeid)
        {

            if (Session["user"] != null&& employeeid ==null)
            {
                AccountEmployee nvSession = (AccountEmployee)Session["user"];
                employeeid=(int)nvSession.EmployeeId;   

            }
            if(employeeid>0)
            {
                var bill=db.Bill.Where(x=>x.EmployeeId==employeeid).OrderByDescending(x=>x.CreatedDate).ToList();   
                if(bill != null)
                {
                    ViewBag.EmployeeId = employeeid;    
                    ViewBag.Count=bill.Count(); 
                    return PartialView(bill);   
                }   
            }
            return PartialView();
        }
        public ActionResult Partail_BillDetail(int billid)
        {
            if (billid > 0)
            {
                var billdetail=db.BillDetail.Where(x=>x.BillId==billid).ToList();
                if (billdetail != null)
                {
                    return  PartialView(billdetail);        
                }
            }
            return PartialView();
        }


        public ActionResult Partail_Product()
        {
            if (db.ProductDetail.Any())
            {
                var pro =db.ProductDetail.Where(x=>x.IsActive==true&& x.Quantity>0).ToList();
                if (pro != null)
                {
                    ViewBag.Count=pro.Count();    
                    return PartialView(pro);    
                }
            }
            return PartialView();   

        }
        public ActionResult Partial_ListProductBill()
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null && cart.Items.Any())
            {

                
                return PartialView(cart.Items);
            }
            return PartialView();

        }

        [HttpGet]
        public async Task<JsonResult> GetTotalPrice()
        {
            SellerCart cart = (SellerCart)Session["SellerCart"];
            if (cart != null && cart.Items.Any())
            {
                var totalPrice = cart.GetPriceTotal();
                var formattedPrice = CuaHangBanDienThoai.Common.Common.FormatNumber(totalPrice);
                return Json(new { TotalPrice = formattedPrice+"đ" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { TotalPrice = "0 đ" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult>UpdateQuantity(int productid,int quantity)
        {
            if(productid <=0 && quantity<=0 )
            {
                return Json(new { Success = false, Code = -2, msg = "Không tìm thấy sản phầm" });
            }
            try {
                SellerCart cart = (SellerCart)Session["SellerCart"];
                if (cart != null && cart.Items.Any())
                {
                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        cart.UpSoLuong(productid, quantity);

                        return Json(new { Success = true, Code = 1, msg = "Cập nhập số lượng thành công" });
                    }    
                }
                return Json(new { Success = false, Code = -2, msg = "Không có sản phầm trong hóa đơn" });
            }
            catch(Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
            }
        }




        [HttpPost]
        public async Task<ActionResult>CheckOut( int customerid)
        {
           
            if ( customerid <= 0)
            {
                return Json(new {Success=false, Code = -2, msg = "Không tìm thấy khách hàng" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try 
                {
                    if (Session["user"] == null)
                    {
                        return Json(new { Success = false, code = -3,msg="Phiên đăng nhập của bạn đa hết hạn" });
                    }
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    var customer = await db.Customer.FirstOrDefaultAsync(x => x.CustomerId == customerid);
                    if (customer == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy khách hàng" });
                    }

                    SellerCart cart = (SellerCart)Session["SellerCart"];
                    if (cart == null ||cart.Items.Count<=0)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không có sản phẩm trong hoá đơn" });
                    }

                    foreach (var item in cart.Items)
                    {
                        var checkQuantityPro = db.ProductDetail.Find(item.ProductDetailId);
                      
                        if (checkQuantityPro != null)
                        {
                            if (checkQuantityPro.Quantity >= item.SoLuong)
                            {
                                checkQuantityPro.Quantity -= item.SoLuong;
                                db.Entry(checkQuantityPro).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                var Name = (checkQuantityPro.Products.Title.Trim() + " " + checkQuantityPro.Ram.Trim() + "/" + checkQuantityPro.Capacity.Trim()) ?? "PadaMiniStore";
                                return Json(new { Success = false, Code = -2,msg="Khong không đủ số lượng  " +Name }); // Kho không đủ số lượng
                            }
                        }
                        else
                        {
                            var Name = (checkQuantityPro.Products.Title.Trim() + " " + checkQuantityPro.Ram.Trim() + "/" + checkQuantityPro.Capacity.Trim()) ?? "PadaMiniStore";
                            return Json(new { Success = false, Code = -2, msg = "Khong không đủ số lượng  " + Name }); // Kho không đủ số lượng
                        }
                    }
                    string code = new Random().Next(0, 99999).ToString("D4");
                    Bill bill = new Bill
                    {
                        CustomerId = customer.CustomerId,
                        //Phone = customer.PhoneNumber,
                        EmployeeId= checkStaff.EmployeeId,


                        CreatedDate = DateTime.Now,
                        CreatedBy = checkStaff.NameEmployee?.Trim(),
                        Code = code.Trim(), 
                    };
                    cart.Items.ForEach(x => bill.BillDetail.Add(new BillDetail
                    {
                        ProductDetailId = x.ProductDetailId,
                        Quantity = x.SoLuong,
                        Price = x.Price
                    }));
            
                    customer.NumberofPurchases=(customer.NumberofPurchases==null)? 1 : customer.NumberofPurchases+=1;
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    bill.TotalAmount = cart.GetPriceTotal();
                    db.Bill.Add(bill);
                    db.SaveChanges();

                    cart.ClearCart();
                    Session["SellerCart"] = cart;
                    string invoicePath = ExportInvoice(bill.BillId);
                    if (!string.IsNullOrEmpty(invoicePath))
                    {
                        dbContext.Commit();
                        return Json(new { Success = true, Code = 1, filePath = invoicePath ,employeeid=bill.EmployeeId});
                    }else 
                    {
                      
                        return Json(new { Success = false, Code = -2, filePath = invoicePath });
                    }
                }
                catch (Exception ex) 
                { 
                dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });

                } 
            }





        }

        public ActionResult DownloadWord(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }


        public ActionResult DownloadInvoice(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string fileName = Path.GetFileName(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi khi tải hóa đơn: " + ex.Message);
            }
        }


        public string ExportInvoice(int sellerId)
        {

            if (sellerId < 0)
            {
                return null;
            }

            var seller = db.Bill.Find(sellerId);
            if (seller != null)
            {
                var customer = db.Customer.Find(seller.CustomerId);
                if (customer == null)
                {
                    return null;
                }
              
                try
                {
                    string templatePath = Server.MapPath("~/Content/templates/HoaDon.html");
                    string htmlContent = System.IO.File.ReadAllText(templatePath);

                    // Thay thế các biến trong template HTML bằng giá trị thực tế
                    htmlContent = htmlContent.Replace("#{{Code}}", seller.Code);
                    htmlContent = htmlContent.Replace("#{{CreatedDate}}", seller.CreatedDate.ToString("dd/MM/yyyy"));
                    htmlContent = htmlContent.Replace("#{{CustomerName}}", customer.CustomerName);
                    htmlContent = htmlContent.Replace("#{{Phone}}", customer.PhoneNumber.Trim());
                    htmlContent = htmlContent.Replace("#{{CreatedBy}}", seller.CreatedBy.Trim());
                    // Lấy chi tiết đơn hàng
                    var sellerDetail = db.BillDetail
                        .Where(od => od.BillId == seller.BillId)
                        .ToList();

                    string productRows = "";
                    int index = 1;
                   
                    foreach (var detail in sellerDetail)
                    {
                        var Name = (detail.ProductDetail.Products.ProductCategory.Title.Trim() + " " + detail.ProductDetail.Products.Title.Trim()) ?? "PadaMiniStore";
                        var GetCapacity = (detail.ProductDetail.Capacity.Trim() + "/" + detail.ProductDetail.Color.Trim()) ?? "PadaMiniStore";
                      
                        string productRow = $@"
                    <tr>
                        <td>{index}</td>
                        <td>{Name?.Trim()}</td>
                        <td>{GetCapacity.Trim()}</td>
                        <td>{detail.Quantity}</td>
                        <td>{CuaHangBanDienThoai.Common.Common.FormatNumber(detail.ProductDetail.PriceSale > 0 ? detail.ProductDetail.PriceSale : detail.ProductDetail.Price)}</td>
                        <td>{CuaHangBanDienThoai.Common.Common.FormatNumber(detail.Quantity * (detail.ProductDetail.PriceSale > 0 ? detail.ProductDetail.PriceSale : detail.ProductDetail.Price))} đ</td>
                    </tr>";
                        productRows += productRow;
                        index++;
                    }

                    htmlContent = htmlContent.Replace("#{{ProductRows}}", productRows);

                    // Tính tổng tiền thanh toán
                    decimal total = seller.TotalAmount;
                    htmlContent = htmlContent.Replace("#{{TotalAmountSeller}}", CuaHangBanDienThoai.Common.Common.FormatNumber(seller.TotalAmount) + " đ");

                    string fileName = $"PADA_{seller.Code}.docx";
                    string folderPath = Server.MapPath("~/Order");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);

                   
                    string logoPath = Server.MapPath("~/images/logo/logo80.png");

                    ConvertHTMLToWord(htmlContent, filePath, logoPath, seller.Code);

                    return filePath;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        public void ConvertHTMLToWord(string htmlContent, string wordFilePath, string logoPath, string qrCodeContent)
        {
            // Thay thế placeholder với hình ảnh logo và mã QR code
            string logoPlaceholder = "#{{Image}}";
            string qrCodePlaceholder = "#{{QRCode}}";

            if (System.IO.File.Exists(logoPath))
            {
                string logoBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(logoPath));
                string imgTag = $"<img src='data:image/png;base64,{logoBase64}' style='width: 100px; height: 100px;' />";
                htmlContent = htmlContent.Replace(logoPlaceholder, imgTag);
            }

            if (!string.IsNullOrEmpty(qrCodeContent))
            {
                using (MemoryStream qrCodeStream = GenerateQRCode(qrCodeContent))
                {
                    string qrCodeBase64 = Convert.ToBase64String(qrCodeStream.ToArray());
                    string imgTag = $"<img src='data:image/png;base64,{qrCodeBase64}' style='width: 50px; height: 50px;' />";
                    htmlContent = htmlContent.Replace(qrCodePlaceholder, imgTag);
                }
            }

            using (MemoryStream memStream = new MemoryStream())
            {
                // Tạo một tài liệu Word mới
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memStream, WordprocessingDocumentType.Document))
                {
                    // Tạo phần main của tài liệu
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Chuyển đổi HTML thành Open XML và thêm vào tài liệu Word
                    string altChunkId = "AltChunkId1";
                    AlternativeFormatImportPart chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, altChunkId);
                    using (StreamWriter streamWriter = new StreamWriter(chunk.GetStream()))
                    {
                        streamWriter.Write(htmlContent);
                    }

                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;
                    body.Append(altChunk);

                    // Lưu tài liệu Word xuống tệp
                    mainPart.Document.Save();
                }

                // Lưu tài liệu Word xuống tệp
                using (FileStream fileStream = new FileStream(wordFilePath, FileMode.Create))
                {
                    memStream.Position = 0;
                    memStream.CopyTo(fileStream);
                }
            }

            // Chỉnh sửa tài liệu Word để đảm bảo hình ảnh có kích thước đúng
            FixImageSizes(wordFilePath);
        }
        private MemoryStream GenerateQRCode(string qrCodeContent)
        {
            // Khởi tạo đối tượng BarcodeWriter để tạo mã QR
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new QrCodeEncodingOptions
            {
                Height = 50,
                Width = 50,
                Margin = 0,
                ErrorCorrection = ErrorCorrectionLevel.H
            };

            // Tạo mã QR từ qrCodeContent
            var qrCodeBitmap = barcodeWriter.Write(qrCodeContent);

            // Chuyển đổi mã QR thành MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return new MemoryStream(ms.ToArray());
            }
        }

        private void FixImageSizes(string wordFilePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(wordFilePath, true))
            {
                foreach (var image in wordDocument.MainDocumentPart.Document.Descendants<DocumentFormat.OpenXml.Drawing.Blip>())
                {
                    if (image.Embed != null)
                    {
                        var imagePart = wordDocument.MainDocumentPart.GetPartById(image.Embed.Value) as ImagePart;
                        if (imagePart != null)
                        {
                            using (var imageStream = imagePart.GetStream())
                            {
                                using (var bitmap = new System.Drawing.Bitmap(imageStream))
                                {
                                    var width = bitmap.Width * 9525; // 1 pixel = 9525 EMUs
                                    var height = bitmap.Height * 9525;

                                    var drawing = image.Ancestors<DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline>().FirstOrDefault();
                                    if (drawing != null)
                                    {
                                        drawing.Extent.Cx = width;
                                        drawing.Extent.Cy = height;

                                        var extents = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Extents>().FirstOrDefault();
                                        if (extents != null)
                                        {
                                            extents.Cx = width;
                                            extents.Cy = height;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                wordDocument.MainDocumentPart.Document.Save();
            }
        }


        public ActionResult Partail_AddCustomer()
        {
            return PartialView();
        }



        [HttpPost]
        public ActionResult DeleteItem(int billid, int billdetailid)
        {
            try
            {
                if (Session["Admin_EditBill_" + billid] != null)
                {
                    Admin_EditBill viewModel = (Admin_EditBill)Session["Admin_EditBill_" + billid];
                    if (viewModel != null)
                    {
                        if (viewModel.Items.Count > 1)
                        {
                            viewModel.Items.RemoveAll(x => x.BillDetailId == billdetailid);

                            Session["Admin_EditBill_" + billid] = viewModel;

                            return Json(new { success = true, code = 1, message = "Xóa thành công" });
                        }
                        else { return Json(new { success = false, message = "Bắt buộc 1 sản phẩm" }); }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                    }
                }
                else
                {

                    return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult UpdateQuantityForEditNew(int productdetailId, int billid, int billdetailid, int newQuantity)
        {
            try
            {
                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }
                if (Session["Admin_EditBill_" + billid] == null)
                {
                    return Json(new { Success = false, msg = "Không tìm thấy sản phẩm để cập nhật số lượng." });

                }
                Admin_EditBill viewModel = (Admin_EditBill)Session["Admin_EditBill_" + billid];
                if (viewModel == null)
                {
                    return Json(new { Success = false, msg = "Không tìm thấy sản phẩm để cập nhật số lượng." });
                }
                var itemToUpdate = viewModel.Items.FirstOrDefault(item =>
                    item.ProductDetailId == productdetailId &&
                    item.BillId == billid && item.BillDetailId == billdetailid);

                if (itemToUpdate != null)
                {
                    int oldQuantity = itemToUpdate.Quantity;
                    itemToUpdate.Quantity = newQuantity;
                    Session["Admin_EditBill_" + billid] = viewModel;
                    return Json(new { Success = true });

                }
                return Json(new { Success = false, msg = "Không tìm thấy sản phẩm để cập nhật số lượng." });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, msg = ex });
            }
        }


        public ActionResult Partail_ItemEditBill(int billid)
        {
            Admin_EditBill order = (Admin_EditBill)Session["Admin_EditBill_" + billid];
            if (order != null && order.Items.Any())
            {
                int count = order.Items.Count;
                ViewBag.Count = count;
                return PartialView(order);
            }
            return PartialView();
        }
        public ActionResult GetTotalPriceItem(int billid)
        {
            try
            {
                if (Session["Admin_EditBill_" + billid] != null)
                {
                    var viewModel = (Admin_EditBill)Session["Admin_EditBill_" + billid];
                    if (viewModel != null)
                    {
                        return Json(CuaHangBanDienThoai.Common.Common.FormatNumber(viewModel.GetPriceTotal(), 0) + " đ", JsonRequestBehavior.AllowGet);
                    }
                }
                return Json("0 đ", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0 đ", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Partail_EditBill(int? billid)
        {
            if(billid!=null&& billid> 0)
            {
                var bill = db.Bill.FirstOrDefault(x=>x.BillId== billid);
                if(bill != null) 
                {
                    Admin_EditBill viewModel = new Admin_EditBill {
                        BillId = bill.BillId,
                        Code = bill.Code,
                        TotalAmount = bill.TotalAmount,
                        CreatedBy = bill.CreatedBy,
                        CreatedDate = bill.CreatedDate,
                        ModifiedDate = bill.ModifiedDate ?? DateTime.MinValue,
                        Modifiedby = bill.Modifiedby ?? "",
                        CustomerId = bill.CustomerId,
                        EmployeeId = bill.EmployeeId,
                        customer = db.Customer.FirstOrDefault(x => x.CustomerId == bill.CustomerId),
                    };
                    var billDetail = db.BillDetail.Where(x => x.BillId == billid)
                        .Select(detail => new Admin_EditBillItem
                            {
                            BillDetailId= detail.BillDetailId,
                            Quantity=detail.Quantity,
                            Price=detail.Price,
                            BillId= (int)detail.BillId,
                            ProductDetailId=(int)detail.ProductDetailId,
                            productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId),

                        }).ToList();
                    viewModel.Items = billDetail;


                    Session["Admin_EditBill_" + billid] = viewModel;

                    ViewBag.Count = viewModel.Items.Count;
                    return PartialView(viewModel);
                }
            }
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>Edit(Admin_EditBill model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập đầy đủ " });
            }
            using (var dbConText = db.Database.BeginTransaction())
            {
                try 
                {
                   if(Session["Admin_EditBill_" + model.BillId] == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Vui lòng nhập đầy đủ " });
                    }

                    if (Session["user"] == null)
                    {
                        return Json(new { Success = false, Code = -3, msg = "Phiên đăng nhập của bạn đã hết hạn" });
                    }
                    var sessionKey = "Admin_EditBill_" + model.BillId;
                    var viewModel = Session[sessionKey] as Admin_EditBill;
                    if (viewModel == null || viewModel.Items.Count < 1)
                    {
                        return Json(new { Success = false, Code = viewModel == null ? -3 : -4 });
                    }






                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff =await db.Employee.FirstOrDefaultAsync(row => row.EmployeeId == nvSession.EmployeeId);
                    var bill = await db.Bill.FirstOrDefaultAsync(x => x.BillId == model.BillId);
                    if(bill == null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Không tìm thấy đơn hàng " });
                    }
                    decimal totalAmount = viewModel.Items.Sum(detail => detail.Price * detail.Quantity);
                    bill.Modifiedby = checkStaff.NameEmployee.Trim();
                    bill.ModifiedDate=DateTime.Now;
                    bill.TotalAmount= totalAmount;  
                    db.Entry(bill).State = EntityState.Modified; 
                    







                    await db.SaveChangesAsync();
                    var existingDetails = db.BillDetail.Where(d => d.BillId == bill.BillId).ToList();
                    var newDetails = viewModel.Items;
                    foreach (var detail in existingDetails.ToList())
                    {
                        var viewModelDetail = viewModel.Items.FirstOrDefault(x => x.BillDetailId == detail.BillDetailId);
                        if (viewModelDetail != null)
                        {
                            int oldQuantity = detail.Quantity;
                            int newQuantity = viewModelDetail.Quantity;
                            int quantityChange = newQuantity - oldQuantity;

                            // Cập nhật số lượng trong chi tiết hóa đơn bán hàng
                            detail.Quantity = newQuantity;
                            db.Entry(detail).State = EntityState.Modified;

                            // Kiểm tra và cập nhật số lượng trong kho
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = await db.ProductDetail.FirstOrDefaultAsync(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    string Name = warehouseDetail.Products.Title.Trim() + " " + warehouseDetail.Ram.Trim() + "/" + warehouseDetail.Capacity.Trim();
                                    if (quantityChange > 0) // Nếu tăng số lượng
                                    {
                                        // Kiểm tra kho đủ số lượng không
                                        if (warehouseDetail.Quantity < quantityChange)
                                        {
                                            dbConText.Rollback();
                                            return Json(new { success = false, code = -2, msg = $"Số lượng sản phẩm '{Name}' trong kho không đủ." });
                                        }


                                        warehouseDetail.Quantity -= quantityChange;
                                    }
                                    else if (quantityChange < 0) // Nếu giảm số lượng
                                    {

                                        warehouseDetail.Quantity += -quantityChange;
                                    }

                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                }
                                else
                                {
                                    return Json(new { success = false, code = -2, msg = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -2, msg = "Chi tiết hóa đơn không có ProductsId." });
                            }
                        }
                        else
                        {
                            // Xóa chi tiết hóa đơn không có trong viewModel
                            if (detail.ProductDetailId != null)
                            {
                                var warehouseDetail = await db.ProductDetail.FirstOrDefaultAsync(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail != null)
                                {
                                    warehouseDetail.Quantity += detail.Quantity; // Cộng lại số lượng tồn kho
                                    db.Entry(warehouseDetail).State = EntityState.Modified;
                                    db.BillDetail.Remove(detail);
                                }
                                else
                                {
                                    return Json(new { success = false, code = -2, msg = $"Không tìm thấy sản phẩm với ProductsId = {detail.ProductDetailId} trong kho." });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, code = -2, msg = "Chi tiết hóa đơn không có ProductsId." });
                            }
                        }
                    }
                    await db.SaveChangesAsync();
                    string invoicePath = ExportInvoice(bill.BillId);
                    if (!string.IsNullOrEmpty(invoicePath))
                    {
                        sessionKey = null;
                        dbConText.Commit();
                        return Json(new { Success = true, Code = 1, msg = "Cập nhập thành công", filePath = invoicePath, billCode = bill.Code, BillId = bill.BillId });
                    }
                    else
                    {

                        return Json(new { Success = false, Code = -2, msg="Lỗi không thể in hoá đơn"});
                    }

                }
                catch (Exception ex)
                {
                    dbConText.Rollback();   
                    return Json(new { Success = false, Code = -99, msg = "Hê thống tạm ngưng "+ ex });
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>AddCustomer(Customer model , Admin_AddCustomer req)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Code = -2, msg = "Vui lòng điền đầy đủ thông tin" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    var customer = await db.Customer.FirstOrDefaultAsync(x => x.PhoneNumber.Trim() == req.PhoneNumber.Trim());
                    if (customer != null)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Số điện thoại đã tồn tại !!!"});
                    }
                    model.PhoneNumber = req.PhoneNumber.Trim();
                    model.CustomerName = req.CustomerName.Trim();
                    db.Customer.Add(model);
                    await db.SaveChangesAsync();    
                    dbContext.Commit(); 
                    return Json(new { Success = true,Code=1 , msg="Đăng ký thành công",PhoneNumber=req.PhoneNumber.Trim()});
                }
                catch (Exception ex) 
                {
                    dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
                }   
            }
        }





        [HttpPost]
        public async Task<ActionResult> FindCustomer(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var customer = await db.Customer
                    .FirstOrDefaultAsync(x => x.PhoneNumber.ToLower().Trim().Contains(input.ToLower().Trim()));

                if (customer != null)
                {
                    ViewBag.txt = input.Trim();
                    return PartialView(customer);
                }
            }

            return PartialView();
        }


        [HttpPost]
        public async Task<ActionResult>DeleteItemCart(int productid)
        {
            try
            {
                if (productid <= 0)
                {
                    return Json(new { Success = false, Code = -2, msg = "Không tìm thấy sản phầm" });
                }
                SellerCart cart = (SellerCart)Session["SellerCart"];
                if (cart != null && cart.Items.Any())
                {
                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        var Name = (existingItem.ProductDetail.Products.Title.Trim() + " " + existingItem.ProductDetail.Capacity.Trim()) ?? "PadaMiniStore";
                        if (existingItem.SoLuong == 10)
                        {
                            return Json(new { Code = -2, Success = false, msg = "Số lượng " + Name + " tới giới hạn" });
                        }
                        cart.Remove(productid);
                        int count = cart.Items.Count();
                        return Json(new { Success = true, Code = 1, msg = "Xóa thành công", Count = count });

                    }
                
                }
                return Json(new { Success = false, Code = -2, msg = "Không có sản phầm trong hóa đơn" });
            }
              
            catch(Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
            }
        }

        [HttpPost]
        public async Task<ActionResult>AddBill(int productid, int quantity)
        {
            if (productid == 0)
            {
                return Json(new { Code = -2, Success = false, msg = "Không tìm thấy mã sản phẩm" });
            }
           
            try
            {
                var checkSanPham =await db.ProductDetail.FirstOrDefaultAsync(row => row.ProductDetailId == productid);
                if (checkSanPham != null)
                {

                    var Name = (checkSanPham.Products.Title.Trim() + " " + checkSanPham.Ram.Trim() + "/" + checkSanPham.Capacity.Trim()) ?? "PadaMiniStore";
                  
                    SellerCart cart = (SellerCart)Session["SellerCart"];
                    if (cart == null)
                    {
                        cart = new SellerCart();
                    }

                    SellerCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        if ( existingItem.SoLuong == 10)
                        {
                            return Json(new { Code = -2, Success = false, msg = "Số lượng " + Name+" tới giới hạn" });
                        }
                        existingItem.SoLuong += quantity;
                        existingItem.PriceTotal = existingItem.SoLuong * existingItem.Price;
                    }
                   
                    else
                    {
                        SellerCartItem item = new SellerCartItem
                        {
                            ProductDetailId = checkSanPham.ProductDetailId,
                          
                            SoLuong = quantity, 
                            ProductDetail = checkSanPham,
                           
                        };



                        item.Price = (decimal)checkSanPham.Price;

                        if (checkSanPham.PriceSale > 0)
                        {
                            item.Price = (decimal)checkSanPham.PriceSale;
                            item.PriceTotal = item.SoLuong * item.Price;
                        }
                        else
                        {
                            item.PriceTotal = item.SoLuong * item.Price;
                        }

                        cart.AddToCart(item, quantity);
                    }

                    int count = cart.Items.Count();
                    Session["SellerCart"] = cart;
                    return Json(new { Code = 1, Success=true, msg = "Thêm vào hoá đơn thành công" ,Count=count});
                }
                else
                {
                    return Json(new { Code = -2, Success = false, msg = "Không tìm thấy sản phẩm" });
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { Code = -99, Success = false, msg = "Hệ thống tạm ngưng" });
               
            }
            
        }
    }
}