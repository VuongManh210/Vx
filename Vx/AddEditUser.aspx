<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditUser.aspx.cs" Inherits="Vx.AddEditUser" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Thêm/Sửa Người Dùng</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background: #f4f7fa;
            font-family: 'Arial', sans-serif;
            padding: 20px;
        }

        .container {
            max-width: 600px;
            margin: 0 auto;
            background: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }

        .form-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            text-align: center;
            margin-bottom: 20px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            font-weight: 600;
            color: #333;
        }

        .form-control {
            border-radius: 5px;
        }

        .btn-save {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            font-weight: 600;
            transition: transform 0.3s ease, background 0.3s ease;
        }

        .btn-save:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
        }

        .btn-back {
            background: linear-gradient(90deg, #6c757d, #829099);
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            font-weight: 600;
            transition: transform 0.3s ease, background 0.3s ease;
        }

        .btn-back:hover {
            background: linear-gradient(90deg, #5a6268, #6c757d);
            transform: scale(1.05);
        }

        .text-danger {
            font-size: 14px;
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="container">
            <asp:Label ID="lblTitle" runat="server" CssClass="form-title" Text="Thêm Người Dùng" />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" CssClass="text-danger" />

            <div class="form-group">
                <label for="txtUsername">Tên Đăng Nhập</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtPassword">Mật Khẩu</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtFullName">Họ Tên</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="txtEmail">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" />
            </div>

            <div class="form-group">
                <label for="ddlRole">Vai Trò</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                    <asp:ListItem Value="Admin">Admin</asp:ListItem>
                    <asp:ListItem Value="User">User</asp:ListItem>
                    <asp:ListItem Value="ShopOwner">ShopOwner</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label for="ddlShop">Cửa Hàng (Tùy chọn)</label>
                <asp:DropDownList ID="ddlShop" runat="server" CssClass="form-control" />
            </div>

            <div class="form-group text-center">
                <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-save" OnClick="btnSave_Click" />
                <asp:Button ID="btnBack" runat="server" Text="Quay Lại" CssClass="btn-back" OnClick="btnBack_Click" />
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>