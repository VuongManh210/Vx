<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="Vx.ChiTietSanPham" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Chi Tiết Sản Phẩm</title>
    <style>
        /* Reset cơ bản */
        html, body {
            margin: 0;
            padding: 0;
            height: 100%;
            font-family: 'Arial', sans-serif;
        }

        /* Wrapper để đảm bảo footer ở dưới cùng */
        #form1 {
            display: flex;
            flex-direction: column;
            min-height: 100vh; /* Chiều cao tối thiểu bằng viewport */
        }

        /* Thanh menu ngang (giữ nguyên từ Home.aspx) */
        .navbar {
            background: linear-gradient(90deg, #007bff, #00c4cc);
            padding: 0 20px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 70px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            z-index: 1000;
            transition: background 0.3s ease;
        }

        .navbar:hover {
            background: linear-gradient(90deg, #0056b3, #009faf);
        }

        .logo {
            font-size: 24px;
            font-weight: 700;
            color: #fff;
            text-decoration: none;
            text-transform: uppercase;
            letter-spacing: 1px;
            padding: 10px 15px;
            border-radius: 5px;
            transition: transform 0.3s ease, background 0.3s ease;
        }

        .logo:hover {
            transform: scale(1.05);
            background: rgba(255, 255, 255, 0.1);
        }

        .search-container {
            display: flex;
            align-items: center;
            width: 45%;
            background: #fff;
            padding: 5px;
            border-radius: 25px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            margin: 0 20px;
            transition: box-shadow 0.3s ease;
        }

        .search-container:hover {
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
        }

        .search-container select,
        .search-container input {
            border: none;
            padding: 10px;
            font-size: 15px;
            outline: none;
            background: transparent;
        }

        .search-container select {
            width: 120px;
            border-right: 1px solid #ddd;
            border-radius: 25px 0 0 25px;
            color: #555;
        }

        .search-container input {
            flex-grow: 1;
            color: #333;
        }

        .search-container button {
            background: #007bff;
            color: white;
            border: none;
            padding: 10px 20px;
            cursor: pointer;
            border-radius: 0 25px 25px 0;
            font-size: 16px;
            transition: background 0.3s ease;
        }

        .search-container button:hover {
            background: #0056b3;
        }

        .user-info {
            display: flex;
            align-items: center;
            color: #fff;
            font-size: 16px;
            font-weight: 500;
            margin-right: 20px;
        }

        .user-info span {
            margin-right: 10px;
        }

        .logout-btn {
            background: #dc3545;
            color: white;
            border: none;
            padding: 8px 15px;
            border-radius: 20px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: transform 0.3s ease, background 0.3s ease;
        }

        .logout-btn:hover {
            background: #c82333;
            transform: scale(1.05);
        }

        .cart {
            font-size: 16px;
            font-weight: 600;
            color: #fff;
            display: flex;
            align-items: center;
            cursor: pointer;
            padding: 8px 15px;
            border-radius: 20px;
            transition: background 0.3s ease, transform 0.3s ease;
        }

        .cart:hover {
            background: rgba(255, 255, 255, 0.1);
            transform: scale(1.05);
        }

        .cart i {
            font-size: 24px;
            margin-left: 10px;
        }

        @media (max-width: 768px) {
            .navbar {
                flex-wrap: wrap;
                height: auto;
                padding: 10px;
            }

            .search-container {
                width: 100%;
                margin: 10px 0;
            }

            .user-info, .cart {
                margin: 5px 10px;
            }

            .logo {
                font-size: 20px;
            }
        }

        /* Nội dung chi tiết sản phẩm */
        .container {
            padding-top: 100px;
            display: flex;
            justify-content: center;
            flex-grow: 1;
        }

        .product-details {
            max-width: 80%;
            background: #fff;
            border-radius: 15px;
            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);
            padding: 20px;
            margin-bottom: 20px;
            display: flex;
        }

        .product-image img {
            max-width: 300px;
            border-radius: 10px;
            margin-right: 30px;
        }

        .product-info {
            flex-grow: 1;
        }

        .product-info h2 {
            font-size: 28px;
            font-weight: 700;
            color: #333;
            margin-bottom: 15px;
        }

        .product-info p {
            font-size: 16px;
            color: #555;
            margin-bottom: 10px;
        }

        .product-info .price {
            font-size: 24px;
            font-weight: 600;
            color: #e74c3c;
            margin-bottom: 15px;
        }

        .quantity-control {
            display: flex;
            align-items: center;
            margin-bottom: 20px;
        }

        .quantity-control input {
            width: 60px;
            text-align: center;
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 5px;
            margin: 0 10px;
        }

        .quantity-control button {
            padding: 8px 15px;
            border: none;
            background: #007bff;
            color: #fff;
            border-radius: 5px;
            cursor: pointer;
            transition: background 0.3s ease;
        }

        .quantity-control button:hover {
            background: #0056b3;
        }

        .action-buttons {
            display: flex;
            gap: 15px;
        }

        .btn-add-to-cart, .btn-buy-now {
            padding: 12px 25px;
            border: none;
            border-radius: 20px;
            font-weight: 600;
            cursor: pointer;
            color: #fff;
            transition: background 0.3s ease, transform 0.3s ease, box-shadow 0.3s ease;
        }

        .btn-add-to-cart {
            background: linear-gradient(90deg, #28a745, #34c759);
        }

        .btn-add-to-cart:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2);
        }

        .btn-buy-now {
            background: linear-gradient(90deg, #ff6f61, #ff8a65);
        }

        .btn-buy-now:hover {
            background: linear-gradient(90deg, #ff5a4d, #ff7050);
            transform: scale(1.05);
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2);
        }

        .btn-add-to-cart.disabled, .btn-buy-now.disabled {
            background: #ccc;
            cursor: not-allowed;
        }

        @media (max-width: 768px) {
            .product-details {
                flex-direction: column;
            }

            .product-image img {
                max-width: 100%;
                margin-right: 0;
                margin-bottom: 20px;
            }

            .action-buttons {
                flex-direction: column;
                gap: 10px;
            }

            .btn-add-to-cart, .btn-buy-now {
                width: 100%;
                padding: 12px;
            }
        }

        /* Footer (giữ nguyên từ Home.aspx) */
        .footer {
            background: linear-gradient(135deg, #0056b3, #00c4cc);
            color: #fff;
            padding: 30px 20px;
            text-align: center;
            box-shadow: 0 -4px 15px rgba(0, 0, 0, 0.2);
            width: 100%;
        }

        .footer-content {
            display: flex;
            justify-content: space-around;
            flex-wrap: wrap;
            max-width: 1200px;
            margin: 0 auto 20px auto;
        }

        .footer-section {
            flex: 1;
            min-width: 200px;
            margin: 10px;
        }

        .footer-section h4 {
            font-size: 18px;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .footer-section p, .footer-section a {
            font-size: 14px;
            color: #fff;
            margin: 5px 0;
            text-decoration: none;
        }

        .footer-section a:hover {
            color: #ffdd57;
            text-decoration: underline;
        }

        .footer-bottom {
            border-top: 1px solid rgba(255, 255, 255, 0.2);
            padding-top: 10px;
            font-size: 14px;
            letter-spacing: 0.5px;
        }

        .footer-bottom a {
            color: #ffdd57;
            text-decoration: none;
            font-weight: 600;
        }

        .footer-bottom a:hover {
            text-decoration: underline;
        }

        .footer i {
            margin-right: 8px;
        }

        @media (max-width: 768px) {
            .footer-content {
                flex-direction: column;
                text-align: center;
            }

            .footer-section {
                margin: 15px 0;
            }

            .footer-section h4 {
                font-size: 16px;
            }

            .footer-section p, .footer-section a {
                font-size: 12px;
            }
        }

        /* Popup nhập thông tin giao hàng */
        .modal {
            display: none;
            position: fixed;
            z-index: 1001;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            justify-content: center;
            align-items: center;
        }

        .modal-content {
            background-color: #fff;
            padding: 20px;
            border-radius: 10px;
            width: 90%;
            max-width: 500px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            position: relative;
        }

        .modal-content h3 {
            font-size: 22px;
            margin-bottom: 20px;
            text-align: center;
            color: #333;
        }

        .modal-content label {
            display: block;
            font-size: 14px;
            font-weight: 600;
            margin-bottom: 5px;
            color: #555;
        }

        .modal-content input {
            width: 100%;
            padding: 10px;
            margin-bottom: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
            outline: none;
            box-sizing: border-box;
        }

        .modal-content input:focus {
            border-color: #007bff;
            box-shadow: 0 0 5px rgba(0, 123, 255, 0.3);
        }

        .modal-buttons {
            display: flex;
            justify-content: space-between;
            gap: 10px;
        }

        .modal-buttons button {
            padding: 12px 20px; /* Tăng padding trên/dưới từ 10px lên 12px để nút cao hơn một chút */
            border: none;
            border-radius: 5px;
            font-weight: 600;
            font-size: 14px; /* Đảm bảo kích thước chữ phù hợp */
            cursor: pointer;
            transition: background 0.3s ease, transform 0.3s ease;
            flex: 1;
            line-height: 1; /* Đảm bảo chiều cao không bị ảnh hưởng bởi line-height mặc định */
            display: flex; /* Sử dụng flex để căn giữa nội dung */
            justify-content: center;
            align-items: center;
            min-height: 44px; /* Đặt chiều cao tối thiểu để đảm bảo dễ bấm trên di động */
        }

        .btn-confirm {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: #fff;
        }

        .btn-confirm:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
        }

        .btn-cancel {
            background: linear-gradient(90deg, #dc3545, #e4606d);
            color: #fff;
        }

        .btn-cancel:hover {
            background: linear-gradient(90deg, #c82333, #d6384e);
            transform: scale(1.05);
        }

        @media (max-width: 768px) {
            .modal-buttons {
                flex-direction: column; /* Xếp dọc các nút trên thiết bị di động */
                gap: 8px;
            }

            .modal-buttons button {
                padding: 12px 15px; /* Giảm padding ngang một chút trên di động */
                min-height: 48px; /* Tăng chiều cao tối thiểu trên di động để dễ bấm */
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Thanh Menu -->
        <div class="navbar">
            <a href="Home.aspx" class="logo">HOME</a>
            <div class="search-container">
                <asp:DropDownList ID="ddlCategory" runat="server">
                    <asp:ListItem Text="Tất cả" Value="all"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Nhập thông tin cần tìm..."></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="🔍" OnClick="btnSearch_Click" />
            </div>
            <div class="user-info">
                <span>Xin chào, <a href="Profile.aspx" style="color: #fff; text-decoration: none;" onmouseover="this.style.textDecoration='underline';" onmouseout="this.style.textDecoration='none';"><asp:Label ID="lblUsername" runat="server" Text="Khách"></asp:Label></a></span>
                <asp:Button ID="btnLogout" runat="server" Text="Đăng Xuất" CssClass="logout-btn" OnClick="btnLogout_Click" />
            </div>
            <div class="cart" onclick="window.location.href='Cart.aspx';">
                GIỎ HÀNG
                <i class="fa fa-shopping-cart"></i>
            </div>
        </div>

        <!-- Nội dung chi tiết sản phẩm -->
        <div class="container">
            <div class="product-details">
                <div class="product-image">
                    <img id="imgSanPham" runat="server" alt="Hình ảnh sản phẩm" />
                </div>
                <div class="product-info">
                    <h2 id="lblTenSanPham" runat="server"></h2>
                    <p class="price" id="lblGia" runat="server"></p>
                    <p id="lblMoTa" runat="server"></p>
                    <p id="lblTonKho" runat="server"></p>
                    <div class="quantity-control">
                        <asp:Button ID="btnGiam" runat="server" Text="-" OnClick="btnGiam_Click" />
                        <asp:TextBox ID="txtSoLuong" runat="server" Text="1"></asp:TextBox>
                        <asp:Button ID="btnTang" runat="server" Text="+" OnClick="btnTang_Click" />
                    </div>
                    <div class="action-buttons">
                        <asp:Button ID="btnThemVaoGioHang" runat="server" Text="Thêm vào giỏ hàng" CssClass="btn-add-to-cart" OnClick="btnThemVaoGioHang_Click" />
                        <asp:Button ID="btnMuaNgay" runat="server" Text="Mua ngay" CssClass="btn-buy-now" OnClientClick="showModal(); return false;" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Popup nhập thông tin giao hàng -->
        <div id="shippingModal" class="modal">
            <div class="modal-content">
                <h3>Thông tin giao hàng</h3>
                <label for="txtModalHoTen">Họ và tên:</label>
                <asp:TextBox ID="txtModalHoTen" runat="server" placeholder="Nhập họ và tên"></asp:TextBox>
                <label for="txtModalSoDienThoai">Số điện thoại:</label>
                <asp:TextBox ID="txtModalSoDienThoai" runat="server" placeholder="Nhập số điện thoại"></asp:TextBox>
                <label for="txtModalDiaChi">Địa chỉ giao hàng:</label>
                <asp:TextBox ID="txtModalDiaChi" runat="server" placeholder="Nhập địa chỉ"></asp:TextBox>
                <div class="modal-buttons">
                    <asp:Button ID="btnConfirmOrder" runat="server" Text="Xác nhận" CssClass="btn-confirm" OnClick="btnConfirmOrder_Click" />
                    <button type="button" class="btn-cancel" onclick="closeModal()">Hủy</button>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <div class="footer">
            <div class="footer-content">
                <div class="footer-section">
                    <h4>Liên hệ</h4>
                    <p><i class="fa fa-map-marker-alt"></i> 123 Đường ABC, TP.HCM</p>
                    <p><i class="fa fa-phone"></i> 0909 123 456</p>
                    <p><i class="fa fa-envelope"></i> support@manhdzstore.com</p>
                </div>
                <div class="footer-section">
                    <h4>Dịch vụ</h4>
                    <a href="Home.aspx"><i class="fa fa-shopping-bag"></i> Mua sắm</a>
                    <a href="Cart.aspx"><i class="fa fa-shopping-cart"></i> Giỏ hàng</a>
                    <a href="Contact.aspx"><i class="fa fa-headset"></i> Hỗ trợ</a>
                </div>
                <div class="footer-section">
                    <h4>Thông tin</h4>
                    <a href="About.aspx"><i class="fa fa-info-circle"></i> Giới thiệu</a>
                    <a href="Policy.aspx"><i class="fa fa-file-alt"></i> Chính sách</a>
                </div>
            </div>
            <div class="footer-bottom">
                <span>© 2025 Manhdz Store. All rights reserved. | <a href="Contact.aspx">Liên hệ chúng tôi</a></span>
            </div>
        </div>
    </form>

    <script>
        function showModal() {
            document.getElementById('shippingModal').style.display = 'flex';
        }

        function closeModal() {
            document.getElementById('shippingModal').style.display = 'none';
        }
    </script>
</body>
</html>