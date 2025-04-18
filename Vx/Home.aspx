<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Vx.Home" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">
    <!-- Thêm Swiper CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/swiper@11/swiper-bundle.min.css" />
    <meta charset="utf-8" />
    <title>Trang Home</title>
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

        /* Thanh menu ngang (giữ nguyên) */
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

        /* Banner ngang với Swiper */
        .banner-container {
            height: 300px; /* Chiều cao banner */
            margin-top: 70px; /* Đặt margin-top bằng chiều cao của navbar */
            position: relative;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            width: 80%; /* Khớp với max-width của .sanpham-container */
            max-width: 80%; /* Giới hạn tối đa giống .sanpham-container */
            margin-left: auto;
            margin-right: auto; /* Căn giữa banner */
        }

        .swiper-slide {
            position: relative;
        }

        .swiper-slide img {
            width: 100%;
            height: 100%;
            object-fit: cover; /* Đảm bảo hình ảnh không bị méo */
            transition: transform 10s ease;
        }

        .swiper-slide:hover img {
            transform: scale(1.1); /* Hiệu ứng zoom nhẹ khi hover */
        }

        .banner-overlay {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.3); /* Lớp phủ tối để làm nổi bật chữ */
            display: flex;
            justify-content: center;
            align-items: center;
            flex-direction: column;
            color: #fff;
            text-align: center;
            opacity: 0;
            transition: opacity 0.5s ease;
        }

        .swiper-slide:hover .banner-overlay {
            opacity: 1; /* Hiển thị lớp phủ khi hover */
        }

        .banner-overlay h2 {
            font-size: 36px;
            margin-bottom: 10px;
            text-transform: uppercase;
            letter-spacing: 2px;
        }

        .banner-overlay p {
            font-size: 18px;
            margin-bottom: 20px;
        }

        .banner-overlay a {
            background: #ffdd57;
            color: #333;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 25px;
            font-weight: 600;
            transition: background 0.3s ease, transform 0.3s ease;
        }

        .banner-overlay a:hover {
            background: #ffd700;
            transform: scale(1.05);
        }

        /* Nút điều hướng của Swiper - Chỉ giữ biểu tượng < > */
        .swiper-button-prev, .swiper-button-next {
            color: #fff; /* Màu biểu tượng */
            background: transparent; /* Xóa nền */
            width: 30px;
            height: 30px;
            border: none; /* Xóa viền */
            display: flex;
            align-items: center;
            justify-content: center;
            transition: color 0.3s ease; /* Hiệu ứng đổi màu biểu tượng khi hover */
            margin-top: 0;
            top: 50%;
            transform: translateY(-50%);
        }

        .swiper-button-prev:hover, .swiper-button-next:hover {
            color: #ffdd57; /* Đổi màu biểu tượng khi hover */
        }

        .swiper-button-prev {
            left: 10px;
        }

        .swiper-button-next {
            right: 10px;
        }

        /* Chấm phân trang của Swiper */
        .swiper-pagination-bullet {
            background: #fff;
            opacity: 0.5;
        }

        .swiper-pagination-bullet-active {
            opacity: 1;
            background: #ffdd57;
        }

        /* Nội dung sản phẩm */
        .container {
            padding-top: 30px; /* Giảm padding-top để không bị khoảng trống lớn sau banner */
            display: flex;
            justify-content: center;
            flex-grow: 1; /* Đảm bảo nội dung chiếm hết không gian còn lại */
        }

        .sanpham-container {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
            gap: 20px;
            max-width: 80%; /* Đảm bảo chiều rộng tối đa là 80% */
            margin: auto;
        }

        .sanpham {
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            align-items: center;
            text-align: center;
            width: 200px;
            height: 350px;
            border: 1px solid #ddd;
            padding: 15px;
            background: #fff;
            box-shadow: 2px 2px 10px rgba(0, 0, 0, 0.1);
            border-radius: 5px;
        }

        .sanpham img {
            width: 100%;
            height: 150px;
            object-fit: cover;
        }

        .sanpham h3 {
            font-size: 16px;
            margin: 10px 0;
            min-height: 40px;
        }

        .sanpham p {
            font-size: 14px;
            color: #555;
            margin: 5px 0;
        }

        .sanpham .button-container {
            display: flex;
            justify-content: space-between;
            gap: 8px;
            width: 100%;
            margin-top: 5px;
        }

        .sanpham a, .sanpham button {
            display: inline-block;
            padding: 8px 12px;
            text-decoration: none;
            border-radius: 15px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.3s ease, background 0.3s ease, box-shadow 0.3s ease;
            flex: 1;
            text-align: center;
        }

        .sanpham a {
            background: #007bff;
            color: white;
        }

        .sanpham a:hover {
            background: #0056b3;
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .sanpham button {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: white;
            border: none;
            padding: 8px 12px;
            position: relative;
            overflow: hidden;
        }

        .sanpham button:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .sanpham button::after {
            content: '';
            position: absolute;
            top: 50%;
            left: 50%;
            width: 0;
            height: 0;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            transform: translate(-50%, -50%);
            transition: width 0.4s ease, height 0.4s ease;
        }

        .sanpham button:hover::after {
            width: 200px;
            height: 200px;
        }

        .pagination {
            text-align: center;
            margin: 20px 0;
        }

        .btn-page {
            padding: 10px 15px;
            margin: 5px;
            background: #007bff;
            color: white;
            border: none;
            cursor: pointer;
            border-radius: 5px;
        }

        .btn-page:hover {
            background: #0056b3;
        }

        .btn-them-gio {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: white;
            border: none;
            padding: 8px 12px;
            border-radius: 15px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.3s ease, background 0.3s ease, box-shadow 0.3s ease;
            min-width: 80px;
            white-space: nowrap;
            text-align: center;
        }
        
        .btn-them-gio:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        /* Footer được thiết kế lại */
        .footer {
            background: linear-gradient(135deg, #0056b3, #00c4cc); /* Gradient đẹp */
            color: #fff;
            padding: 30px 20px;
            text-align: center;
            box-shadow: 0 -4px 15px rgba(0, 0, 0, 0.2); /* Bóng đổ phía trên */
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

            .banner-container {
                height: 200px; /* Giảm chiều cao banner trên thiết bị di động */
                margin-top: 60px; /* Điều chỉnh margin-top trên thiết bị di động để sát navbar */
                width: 80%; /* Khớp với max-width của .sanpham-container */
                max-width: 80%; /* Giới hạn tối đa giống .sanpham-container */
                margin-left: auto;
                margin-right: auto;
            }

            .banner-overlay h2 {
                font-size: 24px;
            }

            .banner-overlay p {
                font-size: 14px;
            }

            .banner-overlay a {
                padding: 8px 16px;
                font-size: 14px;
            }

            .swiper-button-prev, .swiper-button-next {
                width: 25px;
                height: 25px;
            }

            .swiper-button-prev {
                left: 5px;
            }

            .swiper-button-next {
                right: 5px;
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
                <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                    <asp:ListItem Text="Tất cả" Value="all"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Nhập thông tin cần tìm..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
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

        <!-- Banner ngang với Swiper -->
        <div class="banner-container swiper">
            <div class="swiper-wrapper">
                <!-- Banner 1 -->
                <div class="swiper-slide">
                    <img src="images/banner1.jpg" alt="Banner 1" />
                    <div class="banner-overlay">
                        <h2>Khuyến Mãi Lớn!</h2>
                        <p>Giảm giá lên đến 50% cho tất cả sản phẩm</p>
                        <a href="Home.aspx">Mua Sắm Ngay</a>
                    </div>
                </div>
                <!-- Banner 2 -->
                <div class="swiper-slide">
                    <img src="images/banner2.jpg" alt="Banner 2" />
                    <div class="banner-overlay">
                        <h2>Mua 1 Tặng 1</h2>
                        <p>Ưu đãi đặc biệt trong tuần này</p>
                        <a href="Home.aspx">Xem Ngay</a>
                    </div>
                </div>
                <!-- Banner 3 -->
                <div class="swiper-slide">
                    <img src="images/banner3.jpg" alt="Banner 3" />
                    <div class="banner-overlay">
                        <h2>Sản Phẩm Mới!</h2>
                        <p>Khám phá bộ sưu tập mới nhất</p>
                        <a href="Home.aspx">Khám Phá</a>
                    </div>
                </div>
            </div>
            <!-- Nút điều hướng -->
            <div class="swiper-button-prev"></div>
            <div class="swiper-button-next"></div>
            <!-- Chấm phân trang -->
            <div class="swiper-pagination"></div>
        </div>

        <!-- Hiển thị sản phẩm -->
        <div class="container">
            <div class="sanpham-container">
                <asp:Repeater ID="rptSanPham" runat="server">
                    <ItemTemplate>
                        <div class="sanpham">
                            <img src='<%# Eval("HinhAnh") %>' alt="Hình sản phẩm" />
                            <h3><%# Eval("TenSanPham") %></h3>
                            <p>Giá: <%# Eval("Gia", "{0:N0}") %> VNĐ</p>
                            <div class="button-container">
                                <a href='ChiTietSanPham.aspx?id=<%# Eval("ID") %>'>Xem chi tiết</a>
                                <asp:Button ID="btnThemVaoGioHang" runat="server" 
                                    Text="Thêm vào giỏ" 
                                    CssClass="btn-them-gio"
                                    CommandArgument='<%# Eval("ID") %>' 
                                    CommandName="AddToCart" 
                                    OnCommand="btnThemVaoGioHang_Command" />
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Phân trang -->
        <div class="pagination">
            <asp:Button ID="btnPrevious" runat="server" Text="Trang trước" OnClick="btnPrevious_Click" CssClass="btn-page" />
            <asp:Button ID="btnNext" runat="server" Text="Trang sau" OnClick="btnNext_Click" CssClass="btn-page" />
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

    <!-- Thêm Swiper JS và khởi tạo -->
    <script src="https://cdn.jsdelivr.net/npm/swiper@11/swiper-bundle.min.js"></script>
    <script>
        new Swiper('.swiper', {
            loop: true, // Lặp lại banner
            autoplay: {
                delay: 5000, // Chuyển banner sau 5 giây
                disableOnInteraction: false, // Tiếp tục autoplay sau khi người dùng tương tác
            },
            pagination: {
                el: '.swiper-pagination',
                clickable: true, // Người dùng có thể nhấp vào chấm để chuyển banner
            },
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev',
            },
        });
    </script>
</body>
</html>