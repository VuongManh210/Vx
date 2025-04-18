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
        body {
            font-family: 'Arial', sans-serif;
            background: #f4f7fa;
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
        }

        .navbar-brand {
            font-size: 24px;
            font-weight: 700;
            color: #fff;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .form-container {
            margin-top: 100px;
            padding: 20px;
            max-width: 600px;
            margin-left: auto;
            margin-right: auto;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            flex-grow: 1;
        }

        .form-container h2 {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            text-align: center;
            margin-bottom: 20px;
            text-transform: uppercase;
        }

        .form-group label {
            font-weight: 600;
            color: #555;
        }

        .form-control {
            border-radius: 5px;
            border: 1px solid #ddd;
        }

        .btn-save {
            background: linear-gradient(90deg, #28a745, #34c759);
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 15px;
            font-weight: 600;
            transition: background 0.3s ease, transform 0.3s ease;
        }

        .btn-save:hover {
            background: linear-gradient(90deg, #218838, #2ba84a);
            transform: scale(1.05);
        }

        .btn-back {
            background: linear-gradient(90deg, #007bff, #00c4cc);
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 15px;
            font-weight: 600;
            transition: background 0.3s ease, transform 0.3s ease;
        }

        .btn-back:hover {
            background: linear-gradient(90deg, #0056b3, #009faf);
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar">
            <a href="AdminDashboard.aspx" class="navbar-brand">Admin Dashboard</a>
        </nav>

        <div class="form-container">
            <h2><asp:Label ID="lblTitle" runat="server" Text="Thêm Người Dùng"></asp:Label></h2>
            <div class="form-group mb-3">
                <label>Tên Đăng Nhập</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group mb-3">
                <label>Mật Khẩu</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
            </div>
            <div class="form-group mb-3">
                <label>Họ Tên</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group mb-3">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            </div>
            <div class="form-group mb-3">
                <label>Số Điện Thoại</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group mb-3">
                <label>Địa Chỉ</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            </div>
            <div class="form-group mb-3">
                <label>Vai Trò</label>
                <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                    <asp:ListItem Value="Admin">Admin</asp:ListItem>
                    <asp:ListItem Value="ShopOwner">ShopOwner</asp:ListItem>
                    <asp:ListItem Value="User">User</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div id="divShopDetails" runat="server" visible="false">
                <h4>Thông Tin Shop</h4>
                <div class="form-group mb-3">
                    <label>Tên Shop</label>
                    <asp:TextBox ID="txtShopName" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Mô Tả Shop</label>
                    <asp:TextBox ID="txtShopDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>
                <div class="form-group mb-3">
                    <label>Địa Chỉ Shop</label>
                    <asp:TextBox ID="txtShopAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>
                <div class="form-group mb-3">
                    <label>Số Điện Thoại Shop</label>
                    <asp:TextBox ID="txtShopPhone" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="text-center">
                <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-save me-2" OnClick="btnSave_Click" />
                <asp:Button ID="btnBack" runat="server" Text="Quay Lại" CssClass="btn-back" OnClick="btnBack_Click" />
            </div>
        </div>

        <div class="footer">
            <!-- Footer content giống AdminDashboard.aspx -->
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>