<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderDetails.aspx.cs" Inherits="Vx.OrderDetails" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Chi Tiết Đơn Hàng</title>
    <style>
        html, body {
            margin: 0;
            padding: 0;
            height: 100%;
            font-family: 'Arial', sans-serif;
        }

        #form1 {
            display: flex;
            flex-direction: column;
            min-height: 100vh;
        }

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

        .container {
            max-width: 80%;
            margin: 100px auto 30px auto;
            padding: 30px;
            background: white;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
            flex-grow: 1;
        }

        h2 {
            font-size: 28px;
            color: #333;
            margin-bottom: 20px;
            text-align: center;
        }

        .order-info {
            margin-bottom: 30px;
            padding: 20px;
            background: #f9f9f9;
            border-radius: 10px;
        }

        .order-info p {
            font-size: 16px;
            color: #555;
            margin: 5px 0;
        }

        .order-info p span {
            font-weight: bold;
            color: #333;
        }

        .order-details-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 30px;
        }

        .order-details-table th, .order-details-table td {
            padding: 15px;
            text-align: center;
            border-bottom: 1px solid #ddd;
        }

        .order-details-table th {
            background: #f1f1f1;
            font-size: 16px;
            color: #333;
        }

        .order-details-table td {
            font-size: 16px;
            color: #555;
        }

        .order-details-table img {
            width: 80px;
            height: 80px;
            object-fit: cover;
            border-radius: 5px;
        }

        .total-container {
            text-align: right;
            font-size: 20px;
            color: #333;
            margin-bottom: 20px;
        }

        .total-container span {
            font-weight: bold;
            color: #e60000;
        }

        .cancel-btn {
            background: #dc3545;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 20px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 600;
            transition: transform 0.3s ease, background 0.3s ease;
            margin-top: 10px;
        }

        .cancel-btn:hover {
            background: #c82333;
            transform: scale(1.05);
        }

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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navbar -->
        <div class="navbar">
            <a href="Home.aspx" class="logo">HOME</a>
            <div class="search-container">
                <asp:DropDownList ID="ddlCategory" runat="server">
                    <asp:ListItem Text="Tất cả" Value="all"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Nhập thông tin cần tìm..."></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="🔍" OnClick="btnSearch_Click" CausesValidation="false" />
            </div>
            <div class="user-info">
                <span>Xin chào, <a href="Profile.aspx" style="color: #fff; text-decoration: none;" onmouseover="this.style.textDecoration='underline';" onmouseout="this.style.textDecoration='none';"><asp:Label ID="lblUsername" runat="server" Text="Khách"></asp:Label></a></span>
                <asp:Button ID="btnLogout" runat="server" Text="Đăng Xuất" CssClass="logout-btn" OnClick="btnLogout_Click" CausesValidation="false" />
            </div>
            <div class="cart" onclick="window.location.href='Cart.aspx';">
                GIỎ HÀNG
                <i class="fa fa-shopping-cart"></i>
            </div>
        </div>

        <!-- Container chi tiết đơn hàng -->
        <div class="container">
            <h2>Chi Tiết Đơn Hàng</h2>
            <div class="order-info">
                <p>Mã đơn hàng: <span><asp:Label ID="lblOrderId" runat="server" Text=""></asp:Label></span></p>
                <p>Người nhận: <span><asp:Label ID="lblFullName" runat="server" Text=""></asp:Label></span></p>
                <p>Số điện thoại: <span><asp:Label ID="lblPhoneNumber" runat="server" Text=""></asp:Label></span></p>
                <p>Địa chỉ giao hàng: <span><asp:Label ID="lblShippingAddress" runat="server" Text=""></asp:Label></span></p>
                <p>Ngày đặt hàng: <span><asp:Label ID="lblOrderDate" runat="server" Text=""></asp:Label></span></p>
                <p>Trạng thái: <span><asp:Label ID="lblStatus" runat="server" Text=""></asp:Label></span></p>
                <asp:Button ID="btnCancelOrder" runat="server" Text="Hủy Đơn Hàng" CssClass="cancel-btn" OnClick="btnCancelOrder_Click" Visible="false" />
            </div>

            <asp:Repeater ID="rptOrderDetails" runat="server">
                <HeaderTemplate>
                    <table class="order-details-table">
                        <tr>
                            <th>Sản phẩm</th>
                            <th>Hình ảnh</th>
                            <th>Giá</th>
                            <th>Số lượng</th>
                            <th>Tổng</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("ProductName") %></td>
                        <td><img src='<%# Eval("ImageUrl") %>' alt='<%# Eval("ProductName") %>' /></td>
                        <td><%# Eval("UnitPrice", "{0:N0} VND") %></td>
                        <td><%# Eval("Quantity") %></td>
                        <td><%# Eval("Total", "{0:N0} VND") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <div class="total-container">
                Tổng tiền: <span><asp:Label ID="lblTotalAmount" runat="server" Text="0 VND"></asp:Label></span>
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

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>