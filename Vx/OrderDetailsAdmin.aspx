<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderDetailsAdmin.aspx.cs" Inherits="Vx.OrderDetailsAdmin" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Chi Tiết Đơn Hàng - Admin</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
    <style>
        html, body {
            margin: 0;
            padding: 0;
            height: 100%;
            font-family: 'Arial', sans-serif;
        }

        body {
            background: #f4f7fa;
            display: flex;
            flex-direction: column;
            min-height: 100vh;
        }

        .navbar {
            background: linear-gradient(90deg, #007bff, #00c4cc);
            padding: 0 20px;
            height: 70px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            z-index: 1000;
            transition: background 0.3s ease;
        }

        .navbar:hover {
            background: linear-gradient(90deg, #0056b3, #009faf);
        }

        .navbar-brand {
            font-size: 24px;
            font-weight: 700;
            color: #fff;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .logout-btn {
            background: #dc3545;
            color: white;
            border: none;
            padding: 8px 15px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
            transition: transform 0.3s ease, background 0.3s ease;
        }

        .logout-btn:hover {
            background: #c82333;
            transform: scale(1.05);
        }

        .main-content {
            margin-top: 90px;
            padding: 20px;
            flex-grow: 1;
        }

        .section-title {
            font-size: 28px;
            font-weight: 600;
            color: #333;
            margin-bottom: 20px;
            text-align: center;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .order-info {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            padding: 20px;
            margin-bottom: 20px;
        }

        .order-info p {
            font-size: 16px;
            margin: 5px 0;
        }

        .table-container {
            margin-bottom: 20px;
        }

        .table {
            width: 100%;
            border-collapse: collapse;
        }

        .table th, .table td {
            padding: 12px 15px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        .table th {
            background: linear-gradient(90deg, #007bff, #00c4cc);
            color: #fff;
            font-weight: 600;
            text-transform: uppercase;
        }

        .table tr:hover {
            background: #f8f9fa;
        }

        .btn-action {
            padding: 6px 12px;
            border: none;
            border-radius: 15px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.3s ease, background 0.3s ease, box-shadow 0.3s ease;
            margin-right: 5px;
        }

        .btn-confirm {
            background: linear-gradient(90deg, #007bff, #00c4cc);
            color: #fff;
        }

        .btn-confirm:hover {
            background: linear-gradient(90deg, #0056b3, #009faf);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .btn-cancel {
            background: linear-gradient(90deg, #6c757d, #829099);
            color: #fff;
        }

        .btn-cancel:hover {
            background: linear-gradient(90deg, #5a6268, #6c757d);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .btn-back {
            background: linear-gradient(90deg, #ffc107, #ffd154);
            color: #fff;
        }

        .btn-back:hover {
            background: linear-gradient(90deg, #e0a800, #ffca28);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .footer {
            background: linear-gradient(135deg, #0056b3, #00c4cc);
            color: #fff;
            padding: 30px 20px;
            text-align: center;
            box-shadow: 0 -4px 15px rgba(0, 0, 0, 0.2);
            width: 100%;
            flex-shrink: 0;
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
        }

        @media (max-width: 768px) {
            .footer-content {
                flex-direction: column;
            }

            .table th, .table td {
                font-size: 12px;
                padding: 8px;
            }

            .btn-action {
                padding: 4px 8px;
                font-size: 12px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <nav class="navbar">
            <a href="AdminDashboard.aspx" class="navbar-brand">Admin Dashboard</a>
            <asp:Button ID="btnLogout" runat="server" Text="Đăng Xuất" CssClass="logout-btn" OnClick="btnLogout_Click" />
        </nav>

        <div class="main-content">
            <h2 class="section-title">Chi Tiết Đơn Hàng</h2>

            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="mb-3 d-block" />

            <div class="order-info">
                <h4>Thông Tin Đơn Hàng</h4>
                <p><strong>ID Đơn Hàng:</strong> <asp:Label ID="lblOrderId" runat="server" /></p>
                <p><strong>Người Đặt:</strong> <asp:Label ID="lblUsername" runat="server" /></p>
                <p><strong>Ngày Đặt:</strong> <asp:Label ID="lblOrderDate" runat="server" /></p>
                <p><strong>Tổng Tiền:</strong> <asp:Label ID="lblTotalAmount" runat="server" /></p>
                <p><strong>Trạng Thái:</strong> <asp:Label ID="lblStatus" runat="server" /></p>
                <p><strong>Địa Chỉ Giao Hàng:</strong> <asp:Label ID="lblShippingAddress" runat="server" /></p>
                <p><strong>Số Điện Thoại:</strong> <asp:Label ID="lblPhoneNumber" runat="server" /></p>
                <asp:Panel ID="pnlActions" runat="server" CssClass="mt-3">
                    <asp:Button ID="btnConfirm" runat="server" Text="Xác Nhận Đơn Hàng" CssClass="btn-action btn-confirm" OnClick="btnConfirm_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Hủy Đơn Hàng" CssClass="btn-action btn-cancel" OnClick="btnCancel_Click" OnClientClick="return confirm('Bạn có chắc chắn muốn hủy đơn hàng này?');" />
                </asp:Panel>
            </div>

            <div class="table-container">
                <h4>Thông Tin Thanh Toán</h4>
                <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="false" CssClass="table">
                    <Columns>
                        <asp:BoundField DataField="PaymentId" HeaderText="ID Thanh Toán" />
                        <asp:BoundField DataField="PaymentDate" HeaderText="Ngày Thanh Toán" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:BoundField DataField="Amount" HeaderText="Số Tiền" DataFormatString="{0:N0} VNĐ" />
                        <asp:BoundField DataField="PaymentMethod" HeaderText="Phương Thức" />
                        <asp:BoundField DataField="Status" HeaderText="Trạng Thái" />
                    </Columns>
                </asp:GridView>
            </div>

            <div class="table-container">
                <h4>Sản Phẩm Trong Đơn Hàng</h4>
                <asp:GridView ID="gvOrderDetails" runat="server" AutoGenerateColumns="false" CssClass="table">
                    <Columns>
                        <asp:BoundField DataField="OrderDetailId" HeaderText="ID Chi Tiết" />
                        <asp:BoundField DataField="ProductId" HeaderText="ID Sản Phẩm" />
                        <asp:BoundField DataField="ProductName" HeaderText="Tên Sản Phẩm" />
                        <asp:BoundField DataField="Quantity" HeaderText="Số Lượng" />
                        <asp:BoundField DataField="UnitPrice" HeaderText="Đơn Giá" DataFormatString="{0:N0} VNĐ" />
                        <asp:TemplateField HeaderText="Thành Tiền">
                            <ItemTemplate>
                                <%# (Convert.ToDecimal(Eval("Quantity")) * Convert.ToDecimal(Eval("UnitPrice"))).ToString("N0") + " VNĐ" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <asp:Button ID="btnBack" runat="server" Text="Quay Lại" CssClass="btn-action btn-back" OnClick="btnBack_Click" />
        </div>

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