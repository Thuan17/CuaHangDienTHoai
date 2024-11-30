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
using PagedList;


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
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using System.Web.Services.Description;
namespace CuaHangBanDienThoai.Areas.Admin.Controllers
{
    public class ImportWarehouseController : Controller
    {
        // GET: Admin/ImportWarehouse
        private CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        [AuthorizeFunction("Nhân viên kho","Quản lý", "Quản trị viên")]
        public ActionResult Index(int? page)
        {
            var items = db.ImportWarehouse.OrderByDescending(x => x.ImportWarehouseId).ToList();

            if (items != null)
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


                bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsMange = isManage;
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
        [AuthorizeFunction("Nhân viên kho", "Quản lý", "Quản trị viên")]
        public ActionResult Import()
        {
            var product = db.Products.ToList();
            ViewBag.Product = new SelectList(product, "ProductsId", "Title");


            var supplier = db.Supplier.ToList();
            ViewBag.Supplier = new SelectList(supplier, "SupplierId", "Name");
            ViewBag.Date = DateTime.Now.Date;
            return View() ;
        }
        [HttpPost]
   
        public async Task<ActionResult>Import(int? supplierId)
        {
            if (supplierId == null && supplierId <= 0)
            {
                return Json(new { Suceess = false, Code = -2, msg = "Vui lòng chọn nhà cung cấp !!!" });
            }
            ImportCart cart = (ImportCart)Session["ImportCart"];
            if (cart == null)
            {
                return Json(new { Suceess = false, Code = -2, msg = "Chưa có sản phẩm trong phiếu nhập !!!" });
            }
            if (Session["user"] == null)
            {
                return Json(new { Suceess = false, Code = -3, msg = "Phiên đăng nhập của bạn đã hết hạn !!!" });
            }
            using (var dbContext = db.Database.BeginTransaction())
            {
                try
                {
                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff =await db.Employee.FirstOrDefaultAsync(row => row.EmployeeId == nvSession.EmployeeId);
                    int ImportWarehouseId = 0;
                    try
                    {
                        string code;
                        ImportWarehouse checkCode; 

                        do
                        {
                            code = new Random().Next(0, 999999).ToString("D6");
                            checkCode = await db.ImportWarehouse.FirstOrDefaultAsync(x => x.Code == code); 
                        } while (checkCode != null);




                        ImportWarehouse ImportWarehouse = new ImportWarehouse();
                        ImportWarehouse.SupplierId = supplierId;
                        ImportWarehouse.CreatedBy = checkStaff.NameEmployee?.Trim();
                       
                        ImportWarehouse.CreatedDate = DateTime.Now;
                  
                        ImportWarehouse.Code = code.Trim();
                        ImportWarehouse.EmployeeId = (int)nvSession.EmployeeId;

                        cart.Items.ForEach(x => ImportWarehouse.ImportWarehouseDetail.Add(new ImportWarehouseDetail
                        {
                            ProductDetailId = x.ProductDetailId,
                            QuanTity = x.SoLuong,
                            ImportWarehouseId = (int)ImportWarehouse.ImportWarehouseId,

                    }));
                    
                        db.ImportWarehouse.Add(ImportWarehouse);
                      await db.SaveChangesAsync();  
                        ImportWarehouseId = (int)ImportWarehouse.ImportWarehouseId;
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Phiếu nhập bị lỗi" }); // Phiếu nhậ bị lỗi

                    }

                    try
                    {
                        foreach (var item in cart.Items)
                        {
                            if (item.SoLuong > 0)
                            {

                                var updateQuantityProduct = db.ProductDetail
                                    .FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId);


                                updateQuantityProduct.Quantity += item.SoLuong;
                                db.Entry(updateQuantityProduct).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {

                                return Json(new { Success = false, Code = -2, msg= "Số lượng không hợp lệ" }); // Số lượng không hợp lệ

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = false, Code = -2, msg = "Lưu kho bị lỗi" }); // Lưu kho bị lỗi

                    }


                    if (ImportWarehouseId < 0)
                    {
                        return Json(new { Success = false, Code = -7, Url = "" });//Lỗi xuất phiéue nhập
                    }
                    string invoicePath = ExportInvoice(ImportWarehouseId);
                    if (!string.IsNullOrEmpty(invoicePath))
                    {
                        cart.ClearCart();
                        dbContext.Commit();
                        return Json(new { Success = true, Code = 1, FilePath = invoicePath });
                    }
                    else
                    {
                        return Json(new { Success = false, Code = -7, FilePath = invoicePath });

                    }

                }
                catch (Exception ex)
                {
                    dbContext.Rollback();
                    return Json(new { Suceess = false, Code = -99, msg = "Hệ thống tạm ngưng !!!" });

                }
            }
        }
        [AuthorizeFunction("Nhân viên kho", "Quản lý", "Quản trị viên")]
        public ActionResult Partail_Detail(int? id)
        {
            if(id !=null && id> 0)
            {
                var import = db.ImportWarehouse.Find(id);
                if (import != null)
                {
                    Admin_EditImport viewModel = new Admin_EditImport {
                        ImportWarehouseId = import.ImportWarehouseId,
                        CreatedBy = import.CreatedBy,
                        CreatedDate = (DateTime)import.CreatedDate,
                        ModifiedDate = import .ModifiedDate ?? DateTime.MinValue,
                        Modifiedby = import.Modifiedby ?? "",
                        EmployeeId=(int)import.EmployeeId,
                        SupplierId = (int)import.SupplierId,
                        Code=import.Code,
                        supplier=db.Supplier.FirstOrDefault(x=>x.SupplierId==import.SupplierId),
                        employee = db.Employee.FirstOrDefault(x=>x.EmployeeId==import.EmployeeId),
                    };
                    var ImportDetail = db.ImportWarehouseDetail
                                 .Where(x => x.ImportWarehouseId == id)
                                 .Select(detail => new Admin_EditImportItem
                                 {
                                     ImportWarehouseDetailId = detail.ImportWarehouseDetailId,
                                     Quantity =(int) detail.QuanTity,
                                     ImportWarehouseId = (int)detail.ImportWarehouseId,
                                     ProductDetailId = (int)detail.ProductDetailId,
                                     productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId),


                                 }).ToList();
                    viewModel.Items = ImportDetail;
                    Session["Admin_DetailOrder_" + id] = viewModel;


                    ViewBag.Count = viewModel.Items.Count;
                    return PartialView(viewModel);
                }
            }

            return PartialView();
        }

        public ActionResult Detail(string code)
        {
            if (code != null )
            {
                var import = db.ImportWarehouse.FirstOrDefault(x=>x.Code==code.Trim());
                if (import != null)
                {

                    ViewBag.NameSupplier=import.Supplier.Name.Trim();   
                    ViewBag.Code=import.Code;   
                    Admin_EditImport viewModel = new Admin_EditImport
                    {
                        ImportWarehouseId = import.ImportWarehouseId,
                        CreatedBy = import.CreatedBy,
                        CreatedDate = (DateTime)import.CreatedDate,
                        ModifiedDate = import.ModifiedDate ?? DateTime.MinValue,
                        Modifiedby = import.Modifiedby ?? "",
                        EmployeeId = (int)import.EmployeeId,
                        SupplierId = (int)import.SupplierId,
                        Code = import.Code,
                        supplier = db.Supplier.FirstOrDefault(x => x.SupplierId == import.SupplierId),
                        employee = db.Employee.FirstOrDefault(x => x.EmployeeId == import.EmployeeId),
                    };
                    var ImportDetail = db.ImportWarehouseDetail
                                 .Where(x => x.ImportWarehouseId == import.ImportWarehouseId)
                                 .Select(detail => new Admin_EditImportItem
                                 {
                                     ImportWarehouseDetailId = detail.ImportWarehouseDetailId,
                                     Quantity = (int)detail.QuanTity,
                                     ImportWarehouseId = (int)detail.ImportWarehouseId,
                                     ProductDetailId = (int)detail.ProductDetailId,
                                     productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId),


                                 }).ToList();
                    viewModel.Items = ImportDetail;
                    Session["Admin_DetailOrder_" + import.ImportWarehouseId] = viewModel;


                    ViewBag.Count = viewModel.Items.Count;
                    return View(viewModel);
                }
            }

            return View();
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
        private string ExportInvoice(int orderId)
        {
            var order = db.ImportWarehouse.Find(orderId);
            if (order != null)
            {
                var Supplier = db.Supplier.Find(order.SupplierId);
                if (Supplier != null)
                {
                    try
                    {
                        string templatePath = Server.MapPath("~/Content/templates/PhieuNhap.html");
                        if (templatePath == null)
                        {
                            return null;
                        }
                        string htmlContent = System.IO.File.ReadAllText(templatePath);

                        string ImportWarehouseId = order.ImportWarehouseId.ToString();
                        string PhoneSupplier = Supplier.Phone.ToString();
                        string CreateDate = order.CreatedDate.ToString();

                        htmlContent = htmlContent.Replace("#{{Day}}", DateTime.Now.ToString("dd/MM/yyyy"));
                        htmlContent = htmlContent.Replace("#{{ImportWarehouseId}}", order.Code.Trim());

                        htmlContent = htmlContent.Replace("#{{SupplierName}}", Supplier.Name);
                        htmlContent = htmlContent.Replace("#{{Location}}", Supplier.Location);
                        htmlContent = htmlContent.Replace("#{{Phone}}", PhoneSupplier);
                        htmlContent = htmlContent.Replace("#{{CreatedBy}}", order.CreatedBy);
                        htmlContent = htmlContent.Replace("#{{CreateDate}}", CreateDate);
                        var orderDetails = db.ImportWarehouseDetail
                            .Where(od => od.ImportWarehouseId == order.ImportWarehouseId)
                            .ToList();




                        string TotalQuantity = orderDetails.Count().ToString();
                        htmlContent = htmlContent.Replace("#{{TotalQuantity}}", TotalQuantity);
                        string productRows = "";
                        int index = 1;

                        foreach (var detail in orderDetails)
                        {
                            var productTitle = (detail.ProductDetail.Products.ProductCategory.Title.Trim() + " " + detail.ProductDetail.Products.Title.Trim()) ?? "PadaMiniStore";
                            var GetCapacity = (detail.ProductDetail.Ram.Trim() +"-"+ detail.ProductDetail.Capacity.Trim()+"/" + detail.ProductDetail.Color.Trim()) ?? "PadaMiniStore";
                            var productDetail = db.ProductDetail.FirstOrDefault(pd => pd.ProductDetailId == detail.ProductDetailId);
                            

                        
                            string productRow = $@"
                            <tr>
                                <td>{index}</td>
                                <td>{productTitle}</td>
                                <td>{GetCapacity}</td>
                                <td>{detail.QuanTity}</td>
                               </tr>";
                            productRows += productRow;
                            index++;
                        }

                        htmlContent = htmlContent.Replace("#{{ProductRows}}", productRows);
                        string fileName = $"PhieuNhap_{order.Code.Trim()}_{order.Supplier.Name}_{DateTime.Now.Date.ToString("dd-MM-yyyy")}.docx";
                        string folderPath = Server.MapPath("~/Order");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }


                        string filePath = Path.Combine(folderPath, fileName);

                        string logoPath = Server.MapPath("~/images/logo/logo80.png");
                        ConvertHTMLToWord(htmlContent, filePath, logoPath);

                        return filePath;
                    }
                    catch (Exception ex)
                    {
                        
                        return null;
                    }
                }
                return null;

            }
            return null;
        }

        private void ConvertHTMLToWord(string htmlContent, string wordFilePath, string logoPath)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                // Tạo một tài liệu Word mới
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    // Tạo phần main của tài liệu
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                   
                    if (System.IO.File.Exists(logoPath))
                    {
                        AddImageToBody(mainPart, logoPath);
                    }

                    
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
        }

        // Hàm thêm hình ảnh vào tài liệu Word
        private void AddImageToBody(MainDocumentPart mainPart, string imagePath)
        {
            string imagePartId = "imageId";
            ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png, imagePartId);

            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                imagePart.FeedData(stream);
            }

            var element = new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = 990000L, Cy = 792000L },
                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture 1"
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "New Bitmap Image.png"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()
                                ),
                                new PIC.BlipFill(
                                    new A.Blip()
                                    {
                                        Embed = imagePartId,
                                        CompressionState = A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle()
                                    )
                                ),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = 990000L, Cy = 792000L }
                                    ),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                                )
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                    )
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = "50D07946"
                });

            mainPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }

        [HttpGet]
        public ActionResult Partail_SupplierbyId(int? supplierId)
        {
            if (supplierId > 0)
            {
                var supplier=db.Supplier.Find(supplierId);  
                if (supplier != null) 
                {
                    return PartialView(supplier);   
                }
            }
            return PartialView();   
        }



        public ActionResult Partial_ListProductImport()
        {
            ImportCart cart = (ImportCart)Session["ImportCart"];
            if (cart != null && cart.Items.Any())
            {


                return PartialView(cart.Items);
            }
            return PartialView();

        }


        [HttpPost]
        public async Task<ActionResult> UpdateQuantity(int productid, int quantity)
        {
            if (productid <= 0 && quantity <= 0)
            {
                return Json(new { Success = false, Code = -2, msg = "Không tìm thấy sản phầm" });
            }
            try
            {
                ImportCart cart = (ImportCart)Session["ImportCart"];
                if (cart != null && cart.Items.Any())
                {
                    ImportCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        cart.UpSoLuong(productid, quantity);

                        return Json(new { Success = true, Code = 1, msg = "" });
                    }
                }
                return Json(new { Success = false, Code = -2, msg = "Không có sản phầm trong hóa đơn" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteItemCart(int productid)
        {
            try
            {
                if (productid <= 0)
                {
                    return Json(new { Success = false, Code = -2, msg = "Không tìm thấy sản phầm" });
                }
                ImportCart cart = (ImportCart)Session["ImportCart"];
                if (cart != null && cart.Items.Any())
                {
                    ImportCartItem existingItem = cart.Items.FirstOrDefault(x => x.ProductDetailId == productid);
                    if (existingItem != null)
                    {
                        var Name = (existingItem.ProductDetail.Products.Title.Trim() + " " + existingItem.ProductDetail.Capacity.Trim()) ?? "PadaMiniStore";
                     
                        cart.Remove(productid);
                        int count = cart.Items.Count();
                        return Json(new { Success = true, Code = 1, msg = "Xóa thành công", Count = count });

                    }

                }
                return Json(new { Success = false, Code = -2, msg = "Không có sản phầm trong hóa đơn" });
            }

            catch (Exception ex)
            {
                return Json(new { Success = false, Code = -99, msg = "Hệ thống tạm ngưng" });
            }
        }














        //start sua phieu nhap done
        [AuthorizeFunction("Quản lý", "Quản trị viên")]
        public ActionResult Partail_Edit(int? id)
        {
            if (id != null && id > 0)
            {
                var import = db.ImportWarehouse.Find(id);
                if (import != null)
                {
                    Admin_EditImport viewModel = new Admin_EditImport
                    {
                        ImportWarehouseId = import.ImportWarehouseId,
                        CreatedBy = import.CreatedBy,
                        CreatedDate = (DateTime)import.CreatedDate,
                        ModifiedDate = import.ModifiedDate ?? DateTime.MinValue,
                        Modifiedby = import.Modifiedby ?? "",
                        EmployeeId = (int)import.EmployeeId,
                        SupplierId = (int)import.SupplierId,
                        Code = import.Code,
                        supplier = db.Supplier.FirstOrDefault(x => x.SupplierId == import.SupplierId),
                        employee = db.Employee.FirstOrDefault(x => x.EmployeeId == import.EmployeeId),
                    };
                    var ImportDetail = db.ImportWarehouseDetail
                                 .Where(x => x.ImportWarehouseId == id)
                                 .Select(detail => new Admin_EditImportItem
                                 {
                                     ImportWarehouseDetailId = detail.ImportWarehouseDetailId,
                                     Quantity = (int)detail.QuanTity,
                                     ImportWarehouseId = (int)detail.ImportWarehouseId,
                                     ProductDetailId = (int)detail.ProductDetailId,
                                     productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductDetailId == detail.ProductDetailId),


                                 }).ToList();
                    viewModel.Items = ImportDetail;
                    var supplier = db.Supplier.ToList();
                    ViewBag.Supplier = new SelectList(supplier, "SupplierId", "Name", import.SupplierId);
                    Session["Admin_ImportWareHouse_" + id] = viewModel;


                    ViewBag.Count = viewModel.Items.Count;
                    return PartialView(viewModel);
                }
            }

            return PartialView();
        }
        [HttpPost]
        public ActionResult DeleteItem(int importwarehousedetailid, int importwarehouseid)
        {
            try
            {
           
                if (Session["Admin_ImportWareHouse_" + importwarehouseid] == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                }
                    Admin_EditImport viewModel = (Admin_EditImport)Session["Admin_ImportWareHouse_" + importwarehouseid];
                    if (viewModel == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy hóa đơn trong session" });
                    }
                    if (viewModel.Items.Count > 1)
                    {
                        viewModel.Items.RemoveAll(x => x.ImportWarehouseDetailId == importwarehousedetailid);

                        Session["Admin_ImportWareHouse_" + importwarehouseid] = viewModel;

                        return Json(new { success = true, code = 1, message = "Xóa thành công" });
                    }
                    else { return Json(new { success = false, message = "Bắt buộc 1 sản phẩm" }); }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateQuantityForEditNew(int productdetailId, int importwarehouseid, int importwarehousedetailid, int newQuantity)
        {
            try
            {

                if (Session["user"] == null)
                {
                    return RedirectToAction("DangNhap", "Account");
                }
                if (Session["Admin_ImportWareHouse_" + importwarehouseid] == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để cập nhật số lượng." });

                }
               Admin_EditImport viewModel = (Admin_EditImport)Session["Admin_ImportWareHouse_" + importwarehouseid];

                if (viewModel == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để cập nhật số lượng." });
                }
                if (newQuantity < 5 || newQuantity > 150 )
                {
                    return Json(new { success = false, message = "Số lượng không hợp lý." });
                }
                var itemToUpdate = viewModel.Items.FirstOrDefault(item =>
                    item.ProductDetailId == productdetailId &&
                    item.ImportWarehouseId == importwarehouseid && item.ImportWarehouseDetailId == importwarehousedetailid);

                if (itemToUpdate != null)
                {
                    int oldQuantity = itemToUpdate.Quantity;
                    itemToUpdate.Quantity = newQuantity;
                    Session["Admin_ImportWareHouse_" + importwarehouseid] = viewModel;
                    return Json(new { success = true });

                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm để cập nhật số lượng." });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Admin_EditImport model)
        {
          
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, code = -1, message = "Dữ liệu không hợp lệ." });
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                 
                    if (Session["user"] == null)
                    {
                        return RedirectToAction("DangNhap", "Account");
                    }

                    AccountEmployee nvSession = (AccountEmployee)Session["user"];
                    var checkStaff = db.Employee.SingleOrDefault(row => row.EmployeeId == nvSession.EmployeeId);
                    if (checkStaff == null)
                    {
                        return Json(new { Success = false, Code = -7, msg = "Phiên đăng nhập hết hạn" });
                    }


                  
                    var importWarehouse =await db.ImportWarehouse.FindAsync(model.ImportWarehouseId);
                    if (importWarehouse == null)
                    {
                        return Json(new { success = false, code = -2, message = "Không tìm thấy hóa đơn nhập kho." });
                    }

                    // Cập nhật thông tin hóa đơn nhập kho
                    importWarehouse.ImportWarehouseId = model.ImportWarehouseId;
                    importWarehouse.SupplierId = model.SupplierId;
                    importWarehouse.CreatedBy = model.CreatedBy;
                    importWarehouse.CreatedDate = model.CreatedDate;
                    importWarehouse.EmployeeId = model.EmployeeId;
                    importWarehouse.ModifiedDate = DateTime.Now;
                    importWarehouse.Modifiedby = checkStaff.NameEmployee?.Trim();
                    importWarehouse.Code = model.Code.Trim();

                    db.Entry(importWarehouse).State = EntityState.Modified;

                    // Lưu thông tin hóa đơn nhập kho vào database
                  await  db.SaveChangesAsync();

                    // Xử lý chi tiết nhập kho
                    var sessionKey = "Admin_ImportWareHouse_" + model.ImportWarehouseId;
                    var viewModel = Session[sessionKey] as Admin_EditImport;
                    if (viewModel == null || viewModel.Items.Count < 1)
                    {
                        return Json(new { success = false, code = -2, message = "Hóa đơn phải có ít nhất 1 sản phẩm." });
                    }

                    
                    var existingDetails =await db.ImportWarehouseDetail.Where(d => d.ImportWarehouseId == importWarehouse.ImportWarehouseId).ToListAsync();

      
                    foreach (var detail in existingDetails.ToList())
                    {
                       
                        var viewModelDetail = viewModel.Items.FirstOrDefault(item => item.ImportWarehouseDetailId == detail.ImportWarehouseDetailId);
                        if (viewModelDetail != null)
                        {
                          
                            if (detail.QuanTity != viewModelDetail.Quantity)
                            {
                         
                                int oldQuantity = (int)detail.QuanTity; 
                                int newQuantity = viewModelDetail.Quantity;
                                int quantityChange = newQuantity - oldQuantity; 

                              
                                detail.QuanTity = newQuantity;
                                db.Entry(detail).State = EntityState.Modified;

                               
                                if (detail.ProductDetailId == null)
                                {
                                    return Json(new { success = false, code = -2, message = "Chi tiết nhập kho không có ProductDetailId." });
                                }
                                var warehouseDetail = await db.ProductDetail.FirstOrDefaultAsync(w => w.ProductDetailId == detail.ProductDetailId);
                                if (warehouseDetail == null)
                                {
                                 return Json(new { success = false, code = -2, message = $"Không tìm thấy sản phẩm  = {detail.ProductDetailId} trong kho." });
                                }
                                warehouseDetail.Quantity += quantityChange; // Cập nhật số lượng tồn kho
                                db.Entry(warehouseDetail).State = EntityState.Modified;
                              
                            }
                        }
                        else
                        {
                           
                            if (detail.ProductDetailId == null)
                            {
                                return Json(new { success = false, code = -2, message = "Chi tiết nhập kho không có ProductDetailId." });
                            }
                            var warehouseDetail = await db.ProductDetail.FirstOrDefaultAsync(w => w.ProductDetailId == detail.ProductDetailId);
                            if (warehouseDetail == null)
                            {
                                return Json(new { success = false, code = -2, message = $"Không tìm thấy sản phẩm với ProductDetailId = {detail.ProductDetailId} trong kho." });
                            }
                                 warehouseDetail.Quantity -= detail.QuanTity; // Giảm số lượng tồn kho
                                db.Entry(warehouseDetail).State = EntityState.Modified;
                                db.ImportWarehouseDetail.Remove(detail);
                            
                             
                        }
                    }

               
                   await db.SaveChangesAsync();
                    dbContextTransaction.Commit();


                    string invoicePath = ExportInvoice(model.ImportWarehouseId);
                    if (!string.IsNullOrEmpty(invoicePath))
                    {
                        return Json(new { success = true, code = 1, filePath = invoicePath });
                    }
                    else
                    {
                        return Json(new { success = false, code = -2, message = "Không thể xuất hóa đơn." });
                    }
                }
                catch (Exception ex)
                {
          
                    dbContextTransaction.Rollback();
                    return Json(new { success = false, code = -99, message = $"Lỗi ngoại lệ: {ex.Message}" });
                }
            }
        }
        public ActionResult GetUpdatedOrderRow(int importid)
        {
            bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
            bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsManage = isManage;
            var import = db.ImportWarehouse.FirstOrDefault(b => b.ImportWarehouseId == importid);
            if (import != null)
            {
                return PartialView(import);
            }
            return HttpNotFound();
        }
        public ActionResult Partail_ItemEditImport(int importwarehouseid)
        {
            Admin_EditImport viewModel = (Admin_EditImport)Session["Admin_ImportWareHouse_" + importwarehouseid];

            if (viewModel != null && viewModel.Items.Any())
            {
                int count = viewModel.Items.Count;
                ViewBag.Count = count;
                return PartialView(viewModel);
            }
            return PartialView();
        }

        //End sua phieu nhap
        public ActionResult SuggestImport(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {

                ViewBag.Content = search;
                var query = from im in db.ImportWarehouse
                            join sup in db.Supplier on im.SupplierId equals sup.SupplierId into pcJoin
                            from sup in pcJoin.DefaultIfEmpty()
                            join emp in db.Employee on im.EmployeeId equals emp.EmployeeId into empJoin
                            from emp in empJoin.DefaultIfEmpty()
                            where im.Code.ToLower().Trim().Contains(search.Trim().ToLower()) || emp.NameEmployee.ToLower().Trim().Contains(search.Trim()) ||
                                sup.Name.ToLower().Trim().Contains(search.Trim())
                      select im;
                var import = query.OrderByDescending(x => x.ImportWarehouseId).ToList();
                if (import!=null&&import.Any())
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                    int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.IsMange = isManage;
                    var count = import.Count();
                    ViewBag.Count = count;
                   
                    return PartialView(import);
                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                return PartialView();
            }
        }

        public ActionResult GetImportByDate(DateTime ngaynhap)
        {
            try
            {
                DateTime selectedDate = ngaynhap;

                DateTime startDate = selectedDate.Date;
                DateTime endDate = startDate.AddDays(1);

                var orders = db.ImportWarehouse
                    .Where(o => o.CreatedDate >= startDate && o.CreatedDate < endDate)
                    .ToList();

                if (orders != null && orders.Count > 0)
                {
                    bool isAdmin = Session["AdminRole"] != null && Session["AdminRole"].ToString().Equals("Quản trị viên");
                    bool isManage = Session["ManageRole"] != null && Session["ManageRole"].ToString().Equals("Quản lý");
                    int? employeeId = ((AccountEmployee)Session["user"])?.EmployeeId;
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.IsMange = isManage;
                    ViewBag.Count = orders.Count;
                    ViewBag.Date = ngaynhap;
                    ViewBag.Content = ngaynhap.ToString("dd/MM/yyyy");
                    return PartialView(orders); 
                }
                else
                {
                    return PartialView();  // Nếu không có đơn hàng nào
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { Success = false, Code = -99, msg = "Có lỗi xảy ra!" });
            }
        }



        [HttpPost]
        public async Task<ActionResult> EntryForm(int productid, int quantity)
        {
            if (productid == 0)
            {
                return Json(new { Code = -2, Success = false, msg = "Không tìm thấy mã sản phẩm" });
            }

            try
            {
                var checkSanPham = await db.ProductDetail.FirstOrDefaultAsync(row => row.ProductDetailId == productid);
                if (checkSanPham != null)
                {

                    var Name = (checkSanPham.Products.Title.Trim() + " " + checkSanPham.Ram.Trim() + "/" + checkSanPham.Capacity.Trim()) ?? "PadaMiniStore";

                    ImportCart cart = (ImportCart)Session["ImportCart"];
                    if (cart == null)
                    {
                        cart = new ImportCart();
                    }
                    ImportCartItem item = new ImportCartItem
                    {
                        ProductDetailId = checkSanPham.ProductDetailId,

                        SoLuong = quantity,
                        ProductDetail = checkSanPham,

                    };

                    cart.AddToCart(item, quantity);
                    int count = cart.Items.Count();
                    Session["ImportCart"] = cart;
                    return Json(new { Code = 1, Success = true, msg = "", Count = count });
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