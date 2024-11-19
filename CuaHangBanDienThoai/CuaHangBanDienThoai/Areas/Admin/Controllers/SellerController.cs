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
                        return Json(new { success = false, code = -3,msg="Phiên đăng nhập của bạn đa hết hạn" });
                    }
                    Employee nvSession = (Employee)Session["user"];
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
                                return Json(new { success = false, Code = -2,msg="Khong không đủ số lượng  " +Name }); // Kho không đủ số lượng
                            }
                        }
                        else
                        {
                            var Name = (checkQuantityPro.Products.Title.Trim() + " " + checkQuantityPro.Ram.Trim() + "/" + checkQuantityPro.Capacity.Trim()) ?? "PadaMiniStore";
                            return Json(new { success = false, Code = -2, msg = "Khong không đủ số lượng  " + Name }); // Kho không đủ số lượng
                        }
                    }
                    Bill bill = new Bill
                    {
                        CustomerId = customer.CustomerId,
                        //Phone = customer.PhoneNumber,
                        //StaffId = nvSession.StaffId,
                        TypePayment = 0,
                        CreatedDate = DateTime.Now,
                        CreatedBy = nvSession.NameEmployee?.Trim(),
                        Code = "HD" + new Random().Next(0, 99999).ToString("D4")
                    };
                    cart.Items.ForEach(x => bill.BillDetail.Add(new BillDetail
                    {
                        ProductDetailId = x.ProductDetailId,
                        Quantity = x.SoLuong,
                        Price = x.Price
                    }));
            
                    customer.NumberofPurchases += 1;
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
                        dbContextTransaction.Commit();
                        return Json(new { success = true, code = 1, filePath = invoicePath });
                    }
                }
                catch (Exception ex) 
                { 
                dbContext.Rollback();
                    return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });

                } 
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
                var staff = db.Employee.Find(seller.Employee);
                if (staff == null)
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
                    //htmlContent = htmlContent.Replace("#{{Phone}}", seller.Phone);
                    htmlContent = htmlContent.Replace("#{{CreatedBy}}", staff.NameEmployee.Trim());
                    // Lấy chi tiết đơn hàng
                    var sellerDetail = db.BillDetail
                        .Where(od => od.BillId == seller.BillId)
                        .ToList();

                    string productRows = "";
                    int index = 1;
                   
                    foreach (var detail in sellerDetail)
                    {
                        var Name = (detail.ProductDetail.Products.ProductCategory.Title.Trim() + " " + detail.ProductDetail.Products.Title.Trim()) ?? "PadaMiniStore";
                        var GetCapacity = (detail.ProductDetail.Ram.Trim() + "/" + detail.ProductDetail.Capacity.Trim()) ?? "PadaMiniStore";
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

                   
                    string logoPath = Server.MapPath("~/images/logo/logoweb.png");

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