<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditUser.aspx.cs" Inherits="Vx.AddEditUser" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Thêm/Sửa Người Dùng</title>
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

        .main-content {
            margin-top: 90px;
            padding: 20px;
            flex-grow: 1;
        }

        .form-container {
            max-width: 600px;
            margin: 0 auto;
            background: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }

        .form-title {
            font-size: 28px;
            font-weight: 600;
            color: #333;
            text-align: center;
            margin-bottom: 20px;
            text-transform: uppercase;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            font-weight: 600;
            color: #333;
        }

        .form-control, .form-select {
            border-radius: 5px;
            border: 1px solid #ced4da;
            padding: 8px;
        }

        .btn-action {
            padding: 8px 15px;
            border: none;
            border-radius: 15px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.3s ease, background 0.3s ease;
            margin-right: 10px;
        }

        .btn-save {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: #fff;
        }

        .btn-save:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
        }

        .btn-back {
            background: linear-gradient(90deg, #6c757d, #829099);
            color: #fff;
        }

        .btn-back:hover {
            background: linear-gradient(90deg, #5a6268, #6c757d);
            transform: scale(1.05);
        }

        .btn-delete {
            background: linear-gradient(90deg, #dc3545, #e4606d);
            color: #fff;
        }

        .btn-delete:hover {
            background: linear-gradient(90deg, #c82333, #d6384e);
            transform: scale(1.05);
        }

        .text-danger {
            font-size: 14px;
            margin-top: 5px;
            display: block;
            text-align: center;
        }

        .footer {
            background: linear-gradient(135deg, #0056b3, #00c4cc);
            color: #fff;
            padding: 20px;
            text-align: center;
            box-shadow: 0 -4px 15px rgba(0, 0, 0, 0.2);
            width: 100%;
            flex-shrink: 0;
        }

        @media (max-width: 768px) {
            .form-container {
                padding: 15px;
            }

            .form-title {
                font-size: 24px;
            }

            .btn-action {
                padding: 6px 12px;
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
        </nav>

        <div class="main-content">
            <asp:Label ID="lblTitle" runat="server" CssClass="form-title" Text="Thêm Người Dùng" />
            <div class="form-container">
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="text-danger" />

                <div class="form-group">
                    <label for="txtUsername">Tên Đăng Nhập</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Nhập tên đăng nhập" />
                </div>

                <div class="form-group">
                    <label for="txtPassword">Mật Khẩu</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập mật khẩu" />
                </div>

                <div class="form-group">
                    <label for="txtFullName">Họ Tên</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Nhập họ tên" />
                </div>

                <div class="form-group">
                    <label for="txtEmail">Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="Nhập email" />
                </div>

                <div class="form-group">
                    <label for="ddlRole">Vai Trò</label>
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Admin">Admin</asp:ListItem>
                        <asp:ListItem Value="User">User</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group text-center">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-action btn-save" OnClick="btnSave_Click" />
                    <asp:Button ID="btnDelete" runat="server" Text="Xóa" CssClass="btn-action btn-delete" OnClick="btnDelete_Click" Visible="false" OnClientClick="return confirm('Bạn có chắc chắn muốn xóa người dùng này?');" />
                    <asp:Button ID="btnBack" runat="server" Text="Quay Lại" CssClass="btn-action btn-back" OnClick="btnBack_Click" />
                </div>
            </div>
        </div>

        <div class="footer">
            <span>© 2025 Manhdz Store. All rights reserved.</span>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>