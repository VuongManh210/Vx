<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddEditProduct.aspx.cs" Inherits="Vx.AddEditProduct" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Thêm/Sửa Sản Phẩm</title>
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
        }

        .main-content {
            margin-top: 90px;
            padding: 20px;
            flex-grow: 1;
        }

        .form-container {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
            padding: 20px;
            max-width: 600px;
            margin: 0 auto;
        }

        .section-title {
            font-size: 28px;
            font-weight: 600;
            color: #333;
            margin-bottom: 20px;
            text-align: center;
            text-transform: uppercase;
        }

        .form-label {
            font-weight: 600;
            color: #333;
        }

        .form-control, .form-select {
            border-radius: 5px;
            border: 1px solid #ced4da;
            padding: 8px;
            margin-bottom: 15px;
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

        .current-image {
            max-width: 100px;
            max-height: 100px;
            object-fit: cover;
            margin-bottom: 15px;
            border-radius: 5px;
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <nav class="navbar">
            <a href="AdminDashboard.aspx" class="navbar-brand">Admin Dashboard</a>
        </nav>

        <div class="main-content">
            <asp:Label ID="lblTitle" runat="server" CssClass="section-title" />
            <div class="form-container">
                <div class="mb-3">
                    <label for="txtProductName" class="form-label">Tên Sản Phẩm</label>
                    <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" placeholder="Nhập tên sản phẩm" />
                </div>
                <div class="mb-3">
                    <label for="txtPrice" class="form-label">Giá (VNĐ)</label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="Nhập giá sản phẩm" TextMode="Number" />
                </div>
                <div class="mb-3">
                    <label for="ddlCategory" class="form-label">Danh Mục</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select" />
                </div>
                <div class="mb-3">
                    <label for="ddlShop" class="form-label">Cửa Hàng</label>
                    <asp:DropDownList ID="ddlShop" runat="server" CssClass="form-select" />
                </div>
                <div class="mb-3">
                    <label for="fuImage" class="form-label">Hình Ảnh Sản Phẩm</label>
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" Accept=".jpg,.jpeg,.png" />
                    <small class="form-text text-muted">Hỗ trợ .jpg, .jpeg, .png. Tối đa 5MB.</small>
                    <asp:Label ID="lblCurrentImage" runat="server" Text="Hình ảnh hiện tại:" Visible="false" CssClass="form-label mt-2" />
                    <asp:Image ID="imgCurrent" runat="server" CssClass="current-image" Visible="false" />
                </div>
                <div class="mb-3">
                    <label for="txtDescription" class="form-label">Mô Tả</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Nhập mô tả sản phẩm" />
                </div>
                <div class="mb-3">
                    <label for="txtStock" class="form-label">Tồn Kho</label>
                    <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" TextMode="Number" placeholder="Nhập số lượng tồn kho" />
                </div>
                <div class="text-center">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu" CssClass="btn-action btn-save" OnClick="btnSave_Click" />
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