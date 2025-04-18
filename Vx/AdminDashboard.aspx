<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="Vx.AdminDashboard" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Admin Dashboard</title>
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

        .nav-tabs {
            border-bottom: 2px solid #007bff;
            justify-content: center;
            margin-bottom: 30px;
        }

        .nav-tabs .nav-link {
            font-size: 18px;
            font-weight: 600;
            color: #007bff;
            text-transform: uppercase;
            padding: 10px 20px;
            border: none;
            border-radius: 0;
            transition: color 0.3s ease, background 0.3s ease;
        }

        .nav-tabs .nav-link:hover {
            color: #0056b3;
            background: #e9ecef;
        }

        .nav-tabs .nav-link.active {
            color: #fff;
            background: linear-gradient(90deg, #007bff, #00c4cc);
            border: none;
        }

        .tab-content {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            padding: 20px;
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

        .btn-add {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: #fff;
        }

        .btn-add:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .btn-edit {
            background: linear-gradient(90deg, #ffc107, #ffd154);
            color: #fff;
        }

        .btn-edit:hover {
            background: linear-gradient(90deg, #e0a800, #ffca28);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

        .btn-delete {
            background: linear-gradient(90deg, #dc3545, #e4606d);
            color: #fff;
        }

        .btn-delete:hover {
            background: linear-gradient(90deg, #c82333, #d6384e);
            transform: scale(1.05);
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
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

        .pagination {
            text-align: center;
            margin-top: 20px;
        }

        .btn-page {
            padding: 8px 15px;
            margin: 0 5px;
            background: #007bff;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

        .btn-page:hover {
            background: #0056b3;
        }

        .btn-page:disabled {
            background: #ccc;
            cursor: not-allowed;
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

            .nav-tabs .nav-link {
                font-size: 14px;
                padding: 8px 15px;
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
            <h2 class="section-title">Quản Lý Hệ Thống</h2>

            <ul class="nav nav-tabs" id="adminTabs" role="tablist">
                <li class="nav-item">
                    <a class="nav-link active" id="products-tab" data-bs-toggle="tab" href="#products" role="tab" aria-controls="products" aria-selected="true">Quản Lý Sản Phẩm</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="users-tab" data-bs-toggle="tab" href="#users" role="tab" aria-controls="users" aria-selected="false">Quản Lý Người Dùng</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="shops-tab" data-bs-toggle="tab" href="#shops" role="tab" aria-controls="shops" aria-selected="false">Quản Lý Shop</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="orders-tab" data-bs-toggle="tab" href="#orders" role="tab" aria-controls="orders" aria-selected="false">Quản Lý Đơn Hàng</a>
                </li>
            </ul>

            <div class="tab-content" id="adminTabContent">
                <div class="tab-pane fade show active" id="products" role="tabpanel" aria-labelledby="products-tab">
                    <div class="table-container">
                        <asp:Button ID="btnAddProduct" runat="server" Text="Thêm Sản Phẩm" CssClass="btn-action btn-add mb-3" OnClick="btnAddProduct_Click" />
                        <asp:Label ID="lblProductsMessage" runat="server" ForeColor="Red" CssClass="mb-3 d-block" />
                        <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="false" CssClass="table">
                            <Columns>
                                <asp:BoundField DataField="ProductId" HeaderText="ID" />
                                <asp:BoundField DataField="ProductName" HeaderText="Tên Sản Phẩm" />
                                <asp:BoundField DataField="Price" HeaderText="Giá" DataFormatString="{0:N0} VNĐ" />
                                <asp:BoundField DataField="Description" HeaderText="Mô Tả" />
                                <asp:BoundField DataField="Stock" HeaderText="Tồn Kho" />
                                <asp:TemplateField HeaderText="Hành Động">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditProduct" runat="server" Text="Sửa" CssClass="btn-action btn-edit" CommandArgument='<%# Eval("ProductId") %>' OnClick="btnEditProduct_Click" />
                                        <asp:Button ID="btnDeleteProduct" runat="server" Text="Xóa" CssClass="btn-action btn-delete" CommandArgument='<%# Eval("ProductId") %>' OnClick="btnDeleteProduct_Click" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa sản phẩm này?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div class="pagination">
                            <asp:Button ID="btnPrevProducts" runat="server" Text="Trang trước" CssClass="btn-page" OnClick="btnPrevProducts_Click" />
                            <asp:Button ID="btnNextProducts" runat="server" Text="Trang sau" CssClass="btn-page" OnClick="btnNextProducts_Click" />
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="users" role="tabpanel" aria-labelledby="users-tab">
                    <div class="table-container">
                        <asp:Button ID="btnAddUser" runat="server" Text="Thêm Người Dùng" CssClass="btn-action btn-add mb-3" OnClick="btnAddUser_Click" />
                        <asp:Label ID="lblUsersMessage" runat="server" ForeColor="Red" CssClass="mb-3 d-block" />
                        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" CssClass="table">
                            <Columns>
                                <asp:BoundField DataField="UserId" HeaderText="ID" />
                                <asp:BoundField DataField="Username" HeaderText="Tên Đăng Nhập" />
                                <asp:BoundField DataField="FullName" HeaderText="Họ Tên" />
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                                <asp:BoundField DataField="Role" HeaderText="Vai Trò" />
                                <asp:BoundField DataField="ShopId" HeaderText="Shop ID" NullDisplayText="N/A" />
                                <asp:TemplateField HeaderText="Hành Động">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditUser" runat="server" Text="Sửa" CssClass="btn-action btn-edit" CommandArgument='<%# Eval("UserId") %>' OnClick="btnEditUser_Click" />
                                        <asp:Button ID="btnDeleteUser" runat="server" Text="Xóa" CssClass="btn-action btn-delete" CommandArgument='<%# Eval("UserId") %>' OnClick="btnDeleteUser_Click" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa người dùng này?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div class="pagination">
                            <asp:Button ID="btnPrevUsers" runat="server" Text="Trang trước" CssClass="btn-page" OnClick="btnPrevUsers_Click" />
                            <asp:Button ID="btnNextUsers" runat="server" Text="Trang sau" CssClass="btn-page" OnClick="btnNextUsers_Click" />
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="shops" role="tabpanel" aria-labelledby="shops-tab">
                    <div class="table-container">
                        <asp:Button ID="btnAddShop" runat="server" Text="Thêm Shop" CssClass="btn-action btn-add mb-3" OnClick="btnAddShop_Click" />
                        <asp:Label ID="lblShopsMessage" runat="server" ForeColor="Red" CssClass="mb-3 d-block" />
                        <asp:GridView ID="gvShops" runat="server" AutoGenerateColumns="false" CssClass="table">
                            <Columns>
                                <asp:BoundField DataField="ShopId" HeaderText="Shop ID" />
                                <asp:BoundField DataField="ShopName" HeaderText="Tên Shop" />
                                <asp:BoundField DataField="ProductCount" HeaderText="Số Sản Phẩm" />
                                <asp:TemplateField HeaderText="Hành Động">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEditShop" runat="server" Text="Sửa" CssClass="btn-action btn-edit" CommandArgument='<%# Eval("ShopId") %>' OnClick="btnEditShop_Click" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div class="pagination">
                            <asp:Button ID="btnPrevShops" runat="server" Text="Trang trước" CssClass="btn-page" OnClick="btnPrevShops_Click" />
                            <asp:Button ID="btnNextShops" runat="server" Text="Trang sau" CssClass="btn-page" OnClick="btnNextShops_Click" />
                        </div>
                    </div>
                </div>

                <div class="tab-pane fade" id="orders" role="tabpanel" aria-labelledby="orders-tab">
                    <div class="table-container">
                        <asp:Label ID="lblOrdersMessage" runat="server" ForeColor="Red" CssClass="mb-3 d-block" />
                        <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="false" CssClass="table">
                            <Columns>
                                <asp:BoundField DataField="OrderId" HeaderText="ID Đơn Hàng" />
                                <asp:BoundField DataField="Username" HeaderText="Người Đặt" />
                                <asp:BoundField DataField="OrderDate" HeaderText="Ngày Đặt" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                <asp:BoundField DataField="TotalAmount" HeaderText="Tổng Tiền" DataFormatString="{0:N0} VNĐ" />
                                <asp:BoundField DataField="Status" HeaderText="Trạng Thái" />
                                <asp:BoundField DataField="ShippingAddress" HeaderText="Địa Chỉ Giao Hàng" />
                                <asp:BoundField DataField="PhoneNumber" HeaderText="Số Điện Thoại" />
                                <asp:TemplateField HeaderText="Hành Động">
                                    <ItemTemplate>
                                        <asp:Button ID="btnConfirmOrder" runat="server" Text="Xác Nhận" CssClass="btn-action btn-confirm" CommandArgument='<%# Eval("OrderId") %>' OnClick="btnConfirmOrder_Click" Visible='<%# Eval("Status").ToString() == "Pending" %>' />
                                        <asp:Button ID="btnCancelOrder" runat="server" Text="Hủy" CssClass="btn-action btn-cancel" CommandArgument='<%# Eval("OrderId") %>' OnClick="btnCancelOrder_Click" Visible='<%# Eval("Status").ToString() == "Pending" %>' OnClientClick="return confirm('Bạn có chắc chắn muốn hủy đơn hàng này?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <div class="pagination">
                            <asp:Button ID="btnPrevOrders" runat="server" Text="Trang trước" CssClass="btn-page" OnClick="btnPrevOrders_Click" />
                            <asp:Button ID="btnNextOrders" runat="server" Text="Trang sau" CssClass="btn-page" OnClick="btnNextOrders_Click" />
                        </div>
                    </div>
                </div>
            </div>
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