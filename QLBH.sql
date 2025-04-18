-- Tạo cơ sở dữ liệu
CREATE DATABASE QuanLyBanHang2;
GO
USE QuanLyBanHang2;
GO

-- Tạo bảng Categories
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

-- Tạo bảng Products
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    ProductName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    ImageUrl NVARCHAR(255),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- Tạo bảng Users (Thêm cột Role)
CREATE TABLE Users (
    UserId NVARCHAR(128) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(15),
    Address NVARCHAR(255),
    Role NVARCHAR(20) NOT NULL DEFAULT 'User' CHECK (Role IN ('Admin', 'User')),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Cart
CREATE TABLE Cart (
    CartId INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(128) NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    AddedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- Tạo bảng Orders
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(128) NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    ShippingAddress NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(15) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Tạo bảng OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- Tạo bảng Payments
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);

-- Chèn dữ liệu vào Categories
INSERT INTO Categories (CategoryName, Description)
VALUES 
    (N'Laptop', N'Máy tính xách tay'),
    (N'Bàn phím', N'Bàn phím cơ và thường'),
    (N'Chuột', N'Chuột gaming và văn phòng'),
    (N'Màn hình', N'Màn hình máy tính'),
    (N'Ổ cứng SSD', N'Ổ cứng thể rắn'),
    (N'Ổ cứng HDD', N'Ổ cứng truyền thống'),
    (N'Tai nghe', N'Tai nghe không dây và có dây'),
    (N'Card đồ họa', N'Card đồ họa GPU'),
    (N'Bộ vi xử lý', N'CPU máy tính'),
    (N'Bo mạch chủ', N'Mainboard máy tính'),
    (N'Nguồn máy tính', N'PSU cho PC'),
    (N'Router Wi-Fi', N'Thiết bị mạng');

-- Chèn dữ liệu vào Products
INSERT INTO Products (CategoryId, ProductName, Description, Price, Stock, ImageUrl)
VALUES 
    (1, N'Laptop Dell XPS 13', N'Laptop cao cấp, Intel Core i7, 16GB RAM, SSD 512GB', 25000000.00, 10, N'images/laptop_dell_xps_13.jpg'),
    (1, N'Laptop HP Pavilion 15', N'Laptop đa dụng, Intel Core i5, 8GB RAM, SSD 256GB', 19000000.00, 15, N'images/laptop_hp_pavilion_15.jpg'),
    (1, N'Laptop Lenovo ThinkPad X1', N'Laptop doanh nhân, Intel Core i7, 16GB RAM, SSD 1TB', 27000000.00, 8, N'images/laptop_lenovo_thinkpad_x1.jpg'),
    (1, N'Laptop Asus ROG Strix', N'Laptop gaming, AMD Ryzen 9, 32GB RAM, RTX 3060', 32000000.00, 5, N'images/laptop_asus_rog_strix.jpg'),
    (1, N'Laptop Acer Nitro 5', N'Laptop gaming tầm trung, Intel Core i5, 16GB RAM, GTX 1650', 22000000.00, 12, N'images/laptop_acer_nitro_5.jpg'),
    (2, N'Bàn phím cơ Razer BlackWidow', N'Bàn phím cơ RGB, switch Green, chống nước', 2800000.00, 20, N'images/ban_phim_razer_blackwidow.jpg'),
    (2, N'Bàn phím cơ Corsair K70', N'Bàn phím cơ Cherry MX Red, đèn RGB', 3500000.00, 18, N'images/ban_phim_corsair_k70.jpg'),
    (3, N'Chuột gaming Razer DeathAdder', N'Chuột gaming 16,000 DPI, đèn RGB', 1600000.00, 25, N'images/chuot_razer_deathadder.jpg'),
    (3, N'Chuột gaming Logitech G502', N'Chuột gaming 25,600 DPI, 11 nút tùy chỉnh', 1800000.00, 22, N'images/chuot_logitech_g502.jpg'),
    (3, N'Chuột không dây Microsoft Surface', N'Chuột không dây mỏng nhẹ, pin AA', 900000.00, 30, N'images/chuot_microsoft_surface.jpg'),
    (4, N'Màn hình LG UltraGear 32 inch', N'Màn hình gaming 144Hz, QHD, IPS', 7200000.00, 10, N'images/man_hinh_lg_ultragear_32.jpg'),
    (4, N'Màn hình Dell UltraSharp 27 inch', N'Màn hình 4K, màu sắc chính xác, USB-C', 8800000.00, 8, N'images/man_hinh_dell_ultrasharp_27.jpg'),
    (4, N'Màn hình Asus TUF Gaming 32 inch', N'Màn hình gaming 165Hz, Full HD, FreeSync', 6500000.00, 15, N'images/man_hinh_asus_tuf_32.jpg'),
    (5, N'Ổ cứng SSD Samsung 980 Pro 1TB', N'SSD NVMe M.2, tốc độ đọc 7000MB/s', 3800000.00, 25, N'images/ssd_samsung_980_pro.jpg'),
    (5, N'Ổ cứng SSD WD Black SN850 1TB', N'SSD NVMe M.2, tốc độ đọc 7000MB/s, PS5 compatible', 3600000.00, 20, N'images/ssd_wd_black_sn850.jpg'),
    (6, N'Ổ cứng HDD Seagate 2TB', N'HDD 7200RPM, dung lượng lớn, bền bỉ', 1800000.00, 30, N'images/hdd_seagate_2tb.jpg'),
    (6, N'Ổ cứng HDD Western Digital 1TB', N'HDD 5400RPM, phù hợp lưu trữ', 1200000.00, 35, N'images/hdd_wd_1tb.jpg'),
    (7, N'Tai nghe Sony WH-1000XM5', N'Tai nghe chống ồn, âm thanh Hi-Res', 7800000.00, 12, N'images/tai_nghe_sony_xm5.jpg'),
    (7, N'Tai nghe Apple AirPods Pro', N'Tai nghe không dây, chống ồn chủ động', 5500000.00, 18, N'images/tai_nghe_airpods_pro.jpg'),
    (7, N'Tai nghe Bose QuietComfort 45', N'Tai nghe over-ear, chống ồn cao cấp', 7900000.00, 10, N'images/tai_nghe_bose_qc45.jpg'),
    (8, N'Card đồ họa NVIDIA RTX 4060', N'GPU 8GB GDDR6, hỗ trợ ray tracing', 12000000.00, 8, N'images/gpu_nvidia_rtx_4060.jpg'),
    (8, N'Card đồ họa AMD RX 6700 XT', N'GPU 12GB GDDR6, hiệu năng cao', 10500000.00, 10, N'images/gpu_amd_rx_6700_xt.jpg'),
    (9, N'Bộ vi xử lý Intel Core i7-13700K', N'CPU 16 nhân, xung nhịp 5.4GHz', 12000000.00, 15, N'images/cpu_intel_i7_13700k.jpg'),
    (9, N'Bộ vi xử lý AMD Ryzen 7 7700X', N'CPU 8 nhân, xung nhịp 5.4GHz', 11000000.00, 12, N'images/cpu_amd_ryzen_7_7700x.jpg'),
    (10, N'Bo mạch chủ ASUS ROG Strix B660', N'Mainboard Intel B660, hỗ trợ DDR5', 6000000.00, 10, N'images/mainboard_asus_b660.jpg'),
    (10, N'Bo mạch chủ MSI MAG B550', N'Mainboard AMD B550, hỗ trợ Ryzen 5000', 5500000.00, 12, N'images/mainboard_msi_b550.jpg'),
    (11, N'Nguồn máy tính Corsair RM750', N'PSU 750W, 80+ Gold, full modular', 2800000.00, 20, N'images/psu_corsair_rm750.jpg'),
    (11, N'Nguồn máy tính Cooler Master 650W', N'PSU 650W, 80+ Bronze', 2400000.00, 25, N'images/psu_coolermaster_650w.jpg'),
    (12, N'Router Wi-Fi TP-Link AX6000', N'Router Wi-Fi 6, tốc độ 6000Mbps', 4800000.00, 15, N'images/router_tplink_ax6000.jpg'),
    (12, N'Router Wi-Fi Asus RT-AX86U', N'Router Wi-Fi 6, tốc độ 5700Mbps, gaming', 5200000.00, 10, N'images/router_asus_rt_ax86u.jpg');

-- Chèn dữ liệu vào Users (Cập nhật với Role)
INSERT INTO Users (UserId, Username, PasswordHash, FullName, Email, Phone, Address, Role)
VALUES 
    ('user1', 'khachhang1', 'user123', 'Nguyen Van A', 'nva@gmail.com', '0909123456', '123 Đường ABC, TP.HCM', 'User'),
    ('admin1', 'admin', 'admin123', 'Admin User', 'admin@gmail.com', '0909123457', '456 Đường XYZ, TP.HCM', 'Admin');


