# CuaHangDienTHoai

create database CUAHANGDIENTHOAI
go

use CUAHANGDIENTHOAI
go

----------------------Sản Phẩm
--Loại Sản Phẩm vd điện thoại & LapTop
create table ProductCategory(
ProductCategoryId int IDENTITY(1,1) NOT NULL primary key ,
Title nvarchar(150) NOT NULL,
Description nvarchar(max) NULL,
Icon varchar(max) NULL,
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate datetime NULL,
Modifiedby nvarchar(max) NULL,
Alias nvarchar(150) NOT NULL,
IsActive bit,
)
go

--Hang San Phấm
create table ProductCompany(
ProductCompanyId int IDENTITY(1,1) not null primary key ,
Title nvarchar(max),
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate datetime ,
Modifiedby nvarchar(max),
Alias nvarchar(250)Null,
Icon varchar(max) NULL
)
go

--Sản phẩm gì vd : iphone 11 & iphone 12
create table Products(
ProductsId int IDENTITY(1,1) not null primary key ,
Title nvarchar(max),
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate datetime ,
Modifiedby nvarchar(max),
Alias nvarchar(max)Null,
Screensize char(50) ,--KichCo
CPUspeed nvarchar(255),--TocDoCPU
OperatingSystem nvarchar(255),--HeDieuHanh
GPUspeed nvarchar(255),--TocDoGPU
MobileNetwork nvarchar(255) ,--MangDiDong
Sim char(255) ,
Wifi nvarchar(255) ,
Bluetooth char(255),
Connector nvarchar(255),--CongKetNoi
BatteryType char(255) ,--LoaiPin
ChargingSupport nvarchar(255) ,--HoTroSac
BatteryTechnology nvarchar(255) ,--CongNghePin
Image varchar(max),
CPU nvarchar(255),
GPU nvarchar (255),
BatteryCapacity nvarchar(255) ,--DungLuongPin
ProductCategoryId int,
ProductCompanyId int,
Description nvarchar(max),
IsHot bit ,
IsActive bit,
)
go

create table ProductDetail(
ProductDetailId int IDENTITY(1,1) not null primary key ,
Color nvarchar(40),
Price decimal(18,2),--giá bán
PriceSale decimal(18,2),--giá sale
OriginalPrice decimal(18,2),--giá nhập
Ram char(20) ,
Capacity char(35),--Dungluon san pham
ProductsId int ,
IsActive bit,-- hiển thị sản phẩm
IsHome bit ,
IsSale bit ,
IsHot bit ,
Alias nvarchar(max) Not Null,
Quantity int,
)
go

--Hình chi tiết sản phẩm
Create table ProductDetailImage(
ProductImageId int IDENTITY(1,1) NOT NULL primary key ,
ProductDetailId int NOT NULL,
Image varchar(max)NUll,
IsDefault bit NOT NULL,
)
go

-----------------------------------Nhan Viewn
create table AccountEmployee (
EmployeeId int NOT NULL primary key ,
Code varchar(10) Not null,--MSNV
Password varchar(100) not null,
IsLock bit Default 0 ,
);

--Thông tin nhân viên
create TABLE Employee(
EmployeeId int IDENTITY(1,1) NOT NULL primary key ,
PhoneNumber VARCHAR(15) Not null,--SDT
NameEmployee NVARCHAR(max) not null,--TenNhanVien
CitizenIdentificationCard char(12)not null,--CCCD
Email VARCHAR(100)not null,
Birthday Date not null ,
Location nvarchar(max)not null,--DiaChi
Wage decimal(18,2)NOT NULL,--Luong
Sex nvarchar(7),--GioiTinh
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedBy nvarchar(max) NULL,
ModifiedDate datetime NULL,
FunctionId int,--IDCHu chuc ngang
Image varchar(max)NUll,
ManagerId INT NULL,

);
go

ALTER TABLE Employee
ADD ManagerId INT NULL;

--Chuc Nang Nhân viên
create table tb_Function (
FunctionId int IDENTITY(1,1) NOT NULL primary key ,--FunctionId
TitLe nvarchar (max),--TenChucNang

)
go

--Quyen Hạng Nhân Viên
create table Role (
EmployeeId int Not null,
FunctionId int Not null,
Note nvarchar(max),--GhiChu
PRIMARY KEY(EmployeeId,FunctionId)
)
go
------------------------------------Khách hàng
create TABLE Customer (--tb_Customer
CustomerId int IDENTITY(1,1) NOT NULL primary key ,--CustomerId
PhoneNumber VARCHAR(15)null ,--SDT
CustomerName NVARCHAR(max) not null,--TenKhachHang
Email VARCHAR(100)null,
Password varchar(100) ,
Birthday Date null ,
NumberofPurchases int, --SoLanMua
Code char(10),
IsLock bit,--tài kkhoarn khoá
Image varchar(max)NUll,
CodeCreatedAt DATETIME NULL,
)
go

create table AddressCustomer(
AddressCusomerId int IDENTITY(1,1) NOT NULL primary key ,
CustomerId int,
Location nvarchar(max) not null,
IsDefault bit Default 0 ,
CustomerName NVARCHAR(max) ,
PhoneNumber VARCHAR(15),
);

------------------------------------Gio Hàng

create table Cart(
CartId int IDENTITY(1,1) NOT NULL primary key ,
CustomerId int,
)
go

create table CartItem (
CartItemId int IDENTITY(1,1) NOT NULL primary key ,
CartId int NOT NULL,
Quantity int NOT NULL,
ProductDetailId int NOT NULL,
);
go

------------------------------------Order bán tại của hàng

create table OrderCustomer(
OrderId int IDENTITY(1,1) NOT NULL primary key,
Code nvarchar(max) NOT NULL,
TotalAmount [decimal](18, 2) NOT NULL,
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate datetime NULL,
Modifiedby nvarchar(max) NULL,
TypePayment int NOT NULL,-- đã được thanh toán hay chưa
Confirm bit default 0,--Xác nhận đơn hàng
Success bit default 0,
SuccessDate datetime,--Nếu Success ==true thì sẽ đây là ngày nhận hàng
Note nvarchar (250),
Location nvarchar(max)NOt NULL,
CustomerId int ,
CustomerName nvarchar(max)NOt NULL,
Phone varchar(15)NOt NULL,
Email varchar(255)NOt NULL,
IsDelivery bit,
OrderStatus nvarchar(150) NULL,--trạng thái đơn hgafng
StatusDate datetime NULL,
ReturnDate datetime NULL, -- Ngày yêu cầu trả hàng
ReturnReason nvarchar(500) NULL,--Lý do trả hàng
)
go

create table OrderDetail (
OrderDetailId int IDENTITY(1,1) NOT NULL primary key ,
Price decimal(18, 2) NOT NULL,
Quantity int NOT NULL,
OrderId int,
ProductDetailId int
)
go

------------------------------------Order bán tại của hàng

create table Bill(
BillId int IDENTITY(1,1) NOT NULL primary key,
Code nvarchar(max) NOT NULL,
TotalAmount [decimal](18, 2) NOT NULL,
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate DATETIME NULL,
Modifiedby nvarchar(max) NULL,
Confirm bit,
Note nvarchar (250),
CustomerId int not null,
EmployeeId int not null,
)
go

create table BillDetail (
BillDetailId int IDENTITY(1,1) NOT NULL primary key ,
Price decimal(18, 2) NOT NULL,
Quantity int NOT NULL,
BillId int,
ProductDetailId int
)
go

------------------------------------Nhà Cung cấp
Create table Supplier(
SupplierId int IDENTITY(1,1) NOT NULL primary key ,
Name nvarchar(500),
Phone int ,
Location nvarchar(500),
)
go

--------------------------------------KHo nhập
Create table ImportWarehouse(
ImportWarehouseId int IDENTITY(1,1) NOT NULL primary key ,
CreatedBy nvarchar(250),
CreatedDate DateTime ,
ModifiedDate datetime ,
Modifiedby nvarchar(max),
EmployeeId int,
SupplierId int -----nha cung cap
)
go
Create table ImportWarehouseDetail(
ImportWarehouseDetailId int IDENTITY(1,1) NOT NULL primary key ,
ProductDetailId int ,
QuanTity int ,
ImportWarehouseId int ,
)
go

------------------------------------Voucher
create table Voucher (
VoucherId int IDENTITY(1,1) NOT NULL primary key ,
PercentPriceReduction int , --PhanTramGiam
Title nvarchar(250),
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate datetime ,
Modifiedby nvarchar(max),
Quantity int ,
StartDate datetime,
EndDate datetime,
)
go

CREATE TABLE VoucherDetail (
VoucherDetailId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    UsedDate DATETIME NULL,
    BillId INT NULL,  -- Foreign key reference to Bill, can be NULL
    OrderId INT NULL, -- Foreign key reference to OrderCustomer, can be NULL
    VoucherId INT NOT NULL,
    Code VARCHAR(60) NOT NULL,
    Status BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (BillId) REFERENCES Bill(BillId) ON DELETE SET NULL, -- Optional: set to NULL on Bill deletion
    FOREIGN KEY (OrderId) REFERENCES OrderCustomer(OrderId) ON DELETE SET NULL  -- Optional: set to NULL on Order deletion

);

------------------------------------Review

create table Review(
ReviewId int primary key IDENTITY(1,1) NOT NULL,
CreatedDate DATETIME NOT NULL,
ModifiedDate DATETIME NULL,
Modifiedby nvarchar null,
Content nvarchar(350)NOT NULL,
ProductId int NOT NULL ,
CustomerId int NOT NULL,
Ratingscore int NOT NULL ,--diem danh gia
);

    create table Provinces(
    	idProvinces int identity(1,1) primary key,
    	name nvarchar(255),
    );
    go

    create table Districts(
    	idDistricts int identity(1,1) primary key,
    	name nvarchar(255),
    	idProvinces int,
    	foreign key (idProvinces) references Provinces(idProvinces)
    );
    go

    create table Wards(
    	idWards int identity(1,1) primary key,
    	name nvarchar(255),
    	idDistricts int,
    	foreign key (idDistricts) references Districts(idDistricts)
    );
    go

--San Pham

create table InvoiceDetail (
InvoiceDetailId int IDENTITY(1,1) NOT NULL primary key ,
InvoiceId int NOT NULL,
Price decimal(18, 2) NOT NULL,
Quantity int NOT NULL,
ProductDetailId int Not null
)
go

create table Invoice ( -- Hoá đơn bán tại của hàng
InvoiceId int IDENTITY(1,1) NOT NULL primary key,
Code nvarchar(max) NOT NULL,
NameCustomer int NOT NULL,
Phone nvarchar(max) NOT NULL,
TotalAmount [decimal](18, 2) NOT NULL,
Quantity int NOT NULL,
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
ModifiedDate datetime NULL,
Modifiedby nvarchar(max) NULL,
TypePayment int NOT NULL,
EmployeeId int NOT NULL,
)
go

create table Banner(
BannerId int IDENTITY(1,1) NOT NULL primary key ,
CreatedBy nvarchar(max) NULL,
CreatedDate datetime NOT NULL,
IsActive bit NULL DEFAULT 0,
Image varchar (max) NOT NULL,
Link varchar (max)NULL,
IsBackground bit Null DEFAULT 0,
)
go
--CREATE TABLE Message (
-- MessageId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
-- Content NVARCHAR(max) NOT NULL,
-- Timestamp DATETIME NOT NULL,
-- IsRead bit NOT NULL DEFAULT 0
--);

---- Tạo bảng Detail cho nhân viên
--CREATE TABLE StaffMessageDetail (
-- DetailId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
-- MessageId int NOT NULL,
-- StaffId int NOT NULL,
-- FOREIGN KEY (MessageId) REFERENCES tb_Message(MessageId),
-- FOREIGN KEY (StaffId) REFERENCES tb_Staff(StaffId),
-- Content NVARCHAR(max) NOT NULL,
-- Timestamp DATETIME NOT NULL,
-- IsRead bit NOT NULL DEFAULT 0
--);

---- Tạo bảng Detail cho khách hàng
--CREATE TABLE CustomerMessageDetail (
-- DetailId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
-- MessageId int NOT NULL,
-- CustomerId int NOT NULL,
-- FOREIGN KEY (MessageId) REFERENCES Message(MessageId),
-- FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
-- Content NVARCHAR(max) NOT NULL,
-- Timestamp DATETIME NOT NULL,
-- IsRead bit NOT NULL DEFAULT 0
--);

--====================================================================================FK============================================================

--San Pham

ALTER TABLE Products
ADD CONSTRAINT FK_Products_ProductCategory
FOREIGN KEY (ProductCategoryId)
REFERENCES ProductCategory(ProductCategoryId);

alter table Products
add constraint FK_Product_ProductCompany
foreign key (ProductCompanyId)
references ProductCompany

alter table ProductDetail
add constraint FK_ProductDetail_Products
foreign key (ProductsId)
references Products

alter table ProductDetailImage
add constraint FK_ProductImage_ProductDetail  
foreign key (ProductDetailId)
references ProductDetail

------------------Nhan Vien

alter table Role
add constraint FK_Role_Employee
foreign key (EmployeeId)
references Employee

alter table Employee
add constraint FK_Staff_Employee
foreign key (FunctionId)
references tb_Function

alter table AccountEmployee
add constraint FK_AccountEmployee_Employee
foreign key (EmployeeId)
references Employee

ALTER TABLE Employee
ADD CONSTRAINT FK_Employee_Manager
FOREIGN KEY (ManagerId) REFERENCES Employee(EmployeeId);
------------------Gio Hang
alter table Cart
add constraint FK_Cart_Customer
foreign key (CustomerId)
references Customer

alter table CartItem
add constraint FK_CartItem_ProductDetail
foreign key (ProductDetailId)
references ProductDetail

alter table CartItem
add constraint FK_CartItem_Cart
foreign key (CartId)
references Cart

------------------Bill hoá đơn bán offline

alter table BillDetail
add constraint FK_BillDetail_Bill
foreign key (BillId)
references Bill

alter table Bill
add constraint FK_Bill_Employee
foreign key (EmployeeId)
references Employee

alter table Bill
add constraint FK_Bill_Customer
foreign key (CustomerId)
references Customer

alter table BillDetail
add constraint FK_BillDetail_ProductDetail
foreign key (ProductDetailId)
references ProductDetail

------------------Order hoá đơn bán online

alter table OrderDetail
add constraint FK_OrderDetail_Order
foreign key (OrderId)
references OrderCustomer

alter table OrderCustomer
add constraint FK_OrderCustomer_Customer
foreign key (CustomerId)
references Customer

alter table OrderDetail
add constraint FK_OrderDetail_ProductDetail
foreign key (ProductDetailId)
references ProductDetail

---Nhập hàng

alter table ImportWarehouse
add constraint FK_ImportWarehouse_Employee
foreign key (EmployeeId)
references Employee

ALTER TABLE ImportWareHouseDetail
ADD CONSTRAINT FK_ImportWareHouseDetail_ImportWareHouse
FOREIGN KEY (ImportWareHouseId)
REFERENCES ImportWareHouse;

alter table ImportWarehouseDetail
add constraint FK_ImportWarehouse_ProductDetail
foreign key (ProductDetailId)
references ProductDetail

alter table ImportWarehouse
add constraint FK_ImportWarehouse_Supplier
foreign key (SupplierId)
references Supplier
------------------ FK_Customer

alter table AddressCustomer
add constraint FK_AddressCusomer_Customer
foreign key (CustomerId)
references Customer

------------------ FK_Voucher

alter table VoucherDetail
add constraint FK_VoucherDetail_Voucher
foreign key (VoucherId)
references Voucher

------------------ FK_REview

alter table Review
add constraint FK_Review_Products
foreign key (ProductId)
references Products

alter table Review
add constraint FK_Review_Customer
foreign key (CustomerId)
references Customer

-------------------------------------Hóa đơn bán hàng

ALTER TABLE Invoice
ADD CONSTRAINT FK_Invoice_Employee
FOREIGN KEY (EmployeeId)
REFERENCES Employee;

ALTER TABLE InvoiceDetail
ADD CONSTRAINT FK_Invoice_Invoice
FOREIGN KEY (InvoiceId)
REFERENCES Invoice;

ALTER TABLE InvoiceDetail
ADD CONSTRAINT FK_Invoice_ProductDetail
FOREIGN KEY (ProductDetailId)
REFERENCES ProductDetail;
-------------------------------------------------------------------------TRIGGER

-- Tạo trigger khi khhách hàng đăng ký sẽ tạo luôn cart

CREATE TRIGGER CreateCartOnInsertKhachHang
ON Customer
AFTER INSERT
AS
BEGIN
SET NOCOUNT ON;

    -- Chèn dữ liệu mới vào bảng tb_Cart
    INSERT INTO Cart (CustomerId)
    SELECT CustomerId
    FROM inserted;

END;

-- Trigger cho INSERT để thiết lập CreatedDate và ModifiedDate
CREATE TRIGGER trg_ProductCategory_Insert
ON ProductCategory
AFTER INSERT
AS
BEGIN
-- Cập nhật CreatedDate và ModifiedDate bằng ngày hiện tại
UPDATE ProductCategory
SET CreatedDate = GETDATE()

    FROM ProductCategory pc
    INNER JOIN inserted i ON pc.ProductCategoryId = i.ProductCategoryId;

END
GO

----Dành cho bảnh ProductCategory
-- Trigger cho UPDATE để thiết lập ModifiedDate
CREATE TRIGGER trg_ProductCategory_Update
ON ProductCategory
AFTER UPDATE
AS
BEGIN
-- Chỉ cập nhật ModifiedDate cho bản ghi đã bị thay đổi
UPDATE ProductCategory
SET ModifiedDate = GETDATE()
FROM ProductCategory pc
INNER JOIN inserted i ON pc.ProductCategoryId = i.ProductCategoryId;
END
GO

----Dành cho bảnh Products
-- Trigger cho UPDATE để thiết lập ModifiedDate
CREATE TRIGGER trg_Product_Update
ON Products
AFTER UPDATE
AS
BEGIN

    UPDATE Products
    SET ModifiedDate = GETDATE()
    FROM Products pc
    INNER JOIN inserted i ON pc.ProductsId = i.ProductsId;

END
GO
-- Trigger cho INSERT để thiết lập CreatedDate và ModifiedDate
CREATE TRIGGER trg_Product_Insert
ON Products
AFTER INSERT
AS
BEGIN
-- Cập nhật CreatedDate và ModifiedDate bằng ngày hiện tại
UPDATE Products
SET CreatedDate = GETDATE()

    FROM Products pc
    INNER JOIN inserted i ON pc.ProductsId = i.ProductsId;

END
GO

----Dành cho bảnh ProductCompany
-- Trigger cho UPDATE để thiết lập ModifiedDate
CREATE TRIGGER trg_ProductCompany_Update
ON ProductCompany
AFTER UPDATE
AS
BEGIN

    UPDATE ProductCompany
    SET ModifiedDate = GETDATE()
    FROM ProductCompany pc
    INNER JOIN inserted i ON pc.ProductCompanyId = i.ProductCompanyId;

END
GO
-- Trigger cho INSERT để thiết lập CreatedDate và ModifiedDate
CREATE TRIGGER trg_ProductCompany_Insert
ON ProductCompany
AFTER INSERT
AS
BEGIN
-- Cập nhật CreatedDate và ModifiedDate bằng ngày hiện tại
UPDATE ProductCompany
SET CreatedDate = GETDATE()

    FROM ProductCompany pc
    INNER JOIN inserted i ON pc.ProductCompanyId = i.ProductCompanyId;

END
GO

----Dành cho bảnh ImportWarehouse nhập kho khi nhận từ bên đối tác
-- Trigger cho UPDATE để thiết lập ModifiedDate
CREATE TRIGGER trg_ImportWarehouse_Update
ON ImportWarehouse
AFTER UPDATE
AS
BEGIN

    UPDATE ImportWarehouse
    SET ModifiedDate = GETDATE()
    FROM ImportWarehouse import
    INNER JOIN inserted i ON import.ImportWarehouseId = i.ImportWarehouseId;

END
GO
-- Trigger cho INSERT để thiết lập CreatedDate và ModifiedDate
CREATE TRIGGER trg_ImportWarehouse_Insert
ON ImportWarehouse
AFTER INSERT
AS
BEGIN
-- Cập nhật CreatedDate và ModifiedDate bằng ngày hiện tại
UPDATE ImportWarehouse
SET CreatedDate = GETDATE()

    FROM ImportWarehouse import
    INNER JOIN inserted i ON import.ImportWarehouseId = i.ImportWarehouseId;

END
GO

---- Tạo trigger khi cửa hàng tạo luôn kho

--CREATE TRIGGER trg_CreateWarehouse
--ON tb_Store
--AFTER INSERT, UPDATE
--AS
--BEGIN
-- -- Kiểm tra xem bản ghi mới được thêm vào bảng tb_Store
-- IF EXISTS (SELECT \* FROM inserted)
-- BEGIN
-- -- Chèn các bản ghi mới vào bảng tb_Warehouse
-- INSERT INTO tb_Warehouse (CreatedBy, CreatedDate, StoreId)
-- SELECT i.CreatedBy, i.CreatedDate, i.StoreId
-- FROM inserted i
-- LEFT JOIN tb_Warehouse w ON i.StoreId = w.StoreId
-- WHERE w.StoreId IS NULL; -- Chỉ chèn nếu StoreId không tồn tại trong bảng tb_Warehouse
-- END
--END;
