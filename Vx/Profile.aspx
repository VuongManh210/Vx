<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Vx.Profile" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Thông Tin Tài Khoản</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        /* CSS giữ nguyên như trước, không thay đổi */
        body {
            font-family: 'Arial', sans-serif;
            background: #f4f7fa;
            margin: 0;
            padding: 0;
            min-height: 100vh;
            display: flex;
            flex-direction: column;
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
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .navbar-brand {
            font-size: 24px;
            font-weight: 700;
            color: #fff;
            text-transform: uppercase;
            letter-spacing: 1px;
            text-decoration: none;
        }

        .navbar-brand:hover {
            color: #fff;
            transform: scale(1.05);
        }

        .container {
            display: flex;
            margin-top: 70px;
            flex-grow: 1;
        }

        .sidebar {
            width: 250px;
            background: #fff;
            padding: 20px;
            box-shadow: 2px 0 10px rgba(0, 0, 0, 0.1);
            height: calc(100vh - 70px);
            position: fixed;
            top: 70px;
            left: 0;
        }

        .sidebar a {
            display: block;
            padding: 15px 20px;
            color: #333;
            text-decoration: none;
            font-size: 16px;
            font-weight: 500;
            border-radius: 5px;
            margin-bottom: 10px;
            transition: background 0.3s ease, color 0.3s ease;
            cursor: pointer;
        }

        .sidebar a:hover {
            background: #e9ecef;
            color: #007bff;
        }

        .sidebar a.active {
            background: #007bff;
            color: #fff;
        }

        .sidebar .logout {
            background: #ffc107;
            color: #fff;
            text-align: center;
            font-weight: 600;
        }

        .sidebar .logout:hover {
            background: #e0a800;
        }

        .content {
            margin-left: 250px;
            padding: 30px;
            flex-grow: 1;
        }

        .order-history {
            margin-bottom: 40px;
        }

        .order-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 30px;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        .order-table th, .order-table td {
            padding: 15px;
            text-align: center;
            border-bottom: 1px solid #ddd;
        }

        .order-table th {
            background: #f1f1f1;
            font-size: 16px;
            color: #333;
        }

        .order-table td {
            font-size: 16px;
            color: #555;
        }

        .view-details {
            color: #007bff;
            text-decoration: none;
            font-weight: 600;
        }

        .view-details:hover {
            text-decoration: underline;
        }

        .modal-content {
            border-radius: 15px;
            box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
        }

        .modal-header {
            background: linear-gradient(90deg, #007bff, #00c4cc);
            color: #fff;
            border-top-left-radius: 15px;
            border-top-right-radius: 15px;
            justify-content: center;
        }

        .modal-title {
            font-size: 24px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .modal-body {
            padding: 20px;
            background: #f9f9f9;
        }

        .modal-body p {
            margin: 15px 0;
            display: flex;
            align-items: center;
            padding: 10px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }

        .modal-body p:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
        }

        .modal-body label {
            font-weight: 600;
            color: #444;
            width: 150px;
            padding-right: 15px;
            text-align: right;
        }

        .modal-body span {
            color: #333;
            width: 300px;
            font-size: 15px;
        }

        #editModal .modal-body p {
            display: flex;
            align-items: center;
            margin: 10px 0;
            padding: 8px 12px;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
            transition: all 0.3s ease;
        }

        #editModal .modal-body p:hover {
            background: #f0faff;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }

        #editModal .modal-body label {
            width: 150px;
            padding-right: 15px;
            text-align: right;
            font-weight: 600;
            color: #555;
        }

        #editModal .modal-body input, #editModal .modal-body textarea {
            width: 300px;
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background: #fff;
            border: 1px solid #007bff;
            font-size: 15px;
            transition: border-color 0.3s ease, box-shadow 0.3s ease;
        }

        #editModal .modal-body input:focus, #editModal .modal-body textarea:focus {
            border-color: #0056b3;
            box-shadow: 0 0 5px rgba(0, 123, 255, 0.3);
            outline: none;
        }

        #editModal .modal-body textarea {
            height: 80px;
            resize: vertical;
        }

        .btn {
            padding: 12px 25px;
            border: none;
            border-radius: 20px;
            font-weight: 600;
            cursor: pointer;
            transition: background 0.3s ease, transform 0.3s ease, box-shadow 0.3s ease;
            margin: 10px 5px;
            display: flex;
            align-items: center;
            justify-content: center;
            text-align: center;
        }

        .btn-edit {
            background: linear-gradient(90deg, #ffc107, #ffd154);
            color: #fff;
        }

        .btn-edit:hover {
            background: linear-gradient(90deg, #e0a800, #ffca28);
            transform: scale(1.05);
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2);
        }

        .btn-save {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: #fff;
        }

        .btn-save:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2);
        }

        .btn-close {
            background: linear-gradient(90deg, #dc3545, #ff4d4d);
            color: #fff;
        }

        .btn-close:hover {
            background: linear-gradient(90deg, #c82333, #e63946);
            transform: scale(1.05);
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2);
        }

        .modal-footer {
            justify-content: center;
        }

        @media (max-width: 768px) {
            .container {
                flex-direction: column;
            }

            .sidebar {
                width: 100%;
                height: auto;
                position: relative;
                top: 0;
            }

            .content {
                margin-left: 0;
                padding: 20px;
            }

            .modal-body p {
                flex-direction: column;
                align-items: flex-start;
            }

            .modal-body label {
                width: 100%;
                text-align: left;
                padding-right: 0;
                margin-bottom: 5px;
            }

            .modal-body span, #editModal .modal-body input, #editModal .modal-body textarea {
                width: 100%;
            }

            .order-table {
                font-size: 14px;
            }

            .order-table th, .order-table td {
                padding: 10px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navbar -->
        <div class="navbar">
            <a href="Home.aspx" class="navbar-brand">HOME</a>
        </div>

        <!-- Nội dung -->
        <div class="container">
            <!-- Sidebar -->
            <div class="sidebar">
                <a href="Profile.aspx?tab=orders" class='<%= Request.QueryString["tab"] == "orders" ? "active" : "" %>'>Đơn hàng đặt mua</a>
                <a href="#" data-bs-toggle="modal" data-bs-target="#accountModal">Thông tin tài khoản</a>
                <a href="#" data-bs-toggle="modal" data-bs-target="#passwordModal">Thay đổi mật khẩu</a>
                <asp:LinkButton ID="lnkLogout" runat="server" CssClass="logout" OnClick="lnkLogout_Click">Logout</asp:LinkButton>
            </div>

            <!-- Nội dung bên phải -->
            <div class="content">
                <h2 style="text-align: center; color: #333;">Chào mừng đến với trang quản lý tài khoản</h2>

                <!-- Lịch sử đơn hàng -->
                <div class="order-history">
                    <h3 style="color: #333; margin-bottom: 20px;">Lịch Sử Đơn Hàng</h3>
                    <asp:Repeater ID="rptOrders" runat="server">
                        <HeaderTemplate>
                            <table class="order-table">
                                <tr>
                                    <th>Mã đơn hàng</th>
                                    <th>Ngày đặt</th>
                                    <th>Tổng tiền</th>
                                    <th>Trạng thái</th>
                                    <th>Chi tiết</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("OrderId") %></td>
                                <td><%# Eval("OrderDate", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("TotalAmount", "{0:N0} VND") %></td>
                                <td><%# Eval("Status") %></td>
                                <td><a href='<%# "OrderDetails.aspx?OrderId=" + Eval("OrderId") %>' class="view-details">Xem chi tiết</a></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <!-- Modal Thông tin tài khoản (Chỉ hiển thị) -->
        <div class="modal fade" id="accountModal" tabindex="-1" aria-labelledby="accountModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="accountModalLabel">Thông Tin Tài Khoản</h5>
                    </div>
                    <div class="modal-body">
                        <p><label>Họ và tên:</label> <span><asp:Label ID="lblAccountFullName" runat="server" Text="N/A"></asp:Label></span></p>
                        <p><label>Email:</label> <span><asp:Label ID="lblAccountEmail" runat="server" Text="N/A"></asp:Label></span></p>
                        <p><label>Số điện thoại:</label> <span><asp:Label ID="lblAccountPhone" runat="server" Text="N/A"></asp:Label></span></p>
                        <p><label>Địa chỉ:</label> <span><asp:Label ID="lblAccountAddress" runat="server" Text="N/A"></asp:Label></span></p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-edit" data-bs-dismiss="modal" data-bs-toggle="modal" data-bs-target="#editModal">Chỉnh sửa</button>
                        <button type="button" class="btn btn-close" data-bs-dismiss="modal">Đóng</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Thay đổi thông tin cá nhân (Cho phép chỉnh sửa) -->
        <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editModalLabel">Thay Đổi Thông Tin Cá Nhân</h5>
                    </div>
                    <div class="modal-body">
                        <p>
                            <label>Họ và tên:</label>
                            <asp:TextBox ID="txtFullName" runat="server"></asp:TextBox>
                        </p>
                        <p>
                            <label>Email:</label>
                            <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                        </p>
                        <p>
                            <label>Số điện thoại:</label>
                            <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                        </p>
                        <p>
                            <label>Địa chỉ:</label>
                            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </p>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn btn-save" OnClick="btnSave_Click" />
                        <button type="button" class="btn btn-close" data-bs-dismiss="modal">Đóng</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal Thay đổi mật khẩu -->
        <div class="modal fade" id="passwordModal" tabindex="-1" aria-labelledby="passwordModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="passwordModalLabel">Thay Đổi Mật Khẩu</h5>
                    </div>
                    <div class="modal-body">
                        <p><label>Mật khẩu cũ:</label> 
                            <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password"></asp:TextBox>
                        </p>
                        <p><label>Mật khẩu mới:</label> 
                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                        </p>
                        <p><label>Xác nhận mật khẩu:</label> 
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                        </p>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnChangePassword" runat="server" Text="Đổi Mật Khẩu" CssClass="btn btn-save" OnClick="btnChangePassword_Click" />
                        <button type="button" class="btn btn-close" data-bs-dismiss="modal">Đóng</button>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        function openAccountModal() {
            var editModal = bootstrap.Modal.getInstance(document.getElementById('editModal'));
            editModal.hide();
            var accountModal = new bootstrap.Modal(document.getElementById('accountModal'));
            accountModal.show();
        }
    </script>
</body>
</html>