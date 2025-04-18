<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Vx.Register" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Đăng Ký - Manhdz Store</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Tahoma, sans-serif;
            background: linear-gradient(135deg, #1e3c72, #2a5298);
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .register-container {
            background: rgba(255, 255, 255, 0.95);
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
            width: 100%;
            max-width: 450px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }

        .register-container::before {
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: linear-gradient(45deg, rgba(0, 123, 255, 0.2), rgba(42, 82, 152, 0.2));
            transform: rotate(30deg);
            z-index: -1;
            animation: shine 6s infinite linear;
        }

        @keyframes shine {
            0% { transform: rotate(30deg) translateX(-50%); }
            50% { transform: rotate(30deg) translateX(50%); }
            100% { transform: rotate(30deg) translateX(-50%); }
        }

        .register-container h2 {
            margin: 0 0 20px;
            font-size: 28px;
            color: #1e3c72;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .textbox {
            position: relative;
            margin-bottom: 25px;
        }

        .textbox input {
            width: 84%;
            padding: 12px 15px 12px 40px;
            border: 2px solid #ddd;
            border-radius: 25px;
            font-size: 16px;
            outline: none;
            transition: border-color 0.3s ease;
        }

        .textbox input:focus {
            border-color: #007bff;
        }

        .textbox i {
            position: absolute;
            top: 50%;
            left: 15px;
            transform: translateY(-50%);
            color: #666;
            font-size: 16px;
            transition: color 0.3s ease;
        }

        .textbox input:focus + i {
            color: #007bff;
        }

        .textbox label {
            position: absolute;
            top: 50%;
            left: 40px;
            transform: translateY(-50%);
            font-size: 16px;
            color: #666;
            pointer-events: none;
            transition: all 0.3s ease;
        }

        .textbox input:focus + i + label,
        .textbox input:not(:placeholder-shown) + i + label {
            top: -10px;
            left: 35px;
            font-size: 12px;
            color: #007bff;
            background: #fff;
            padding: 0 5px;
        }

        .btn-register {
            width: 100%;
            padding: 12px;
            background: linear-gradient(90deg, #28a745, #34c759);
            border: none;
            border-radius: 25px;
            color: white;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .btn-register:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(40, 167, 69, 0.4);
        }

        .footer {
            margin-top: 20px;
            font-size: 14px;
            color: #666;
        }

        .footer a {
            color: #007bff;
            text-decoration: none;
            font-weight: 600;
        }

        .footer a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="register-container">
            <h2>Đăng Ký</h2>
            <div class="textbox">
                <asp:TextBox ID="txtUsername" runat="server" placeholder=" "></asp:TextBox>
                <i class="fas fa-user"></i>
                <label>Tên đăng nhập</label>
            </div>
            <div class="textbox">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder=" "></asp:TextBox>
                <i class="fas fa-lock"></i>
                <label>Mật khẩu</label>
            </div>
            <div class="textbox">
                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" placeholder=" "></asp:TextBox>
                <i class="fas fa-lock"></i>
                <label>Xác nhận mật khẩu</label>
            </div>
            <div class="textbox">
                <asp:TextBox ID="txtFullName" runat="server" placeholder=" "></asp:TextBox>
                <i class="fas fa-id-card"></i>
                <label>Họ và tên</label>
            </div>
            <div class="textbox">
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder=" "></asp:TextBox>
                <i class="fas fa-envelope"></i>
                <label>Email</label>
            </div>
            <div class="textbox">
                <asp:TextBox ID="txtPhone" runat="server" placeholder=" "></asp:TextBox>
                <i class="fas fa-phone"></i>
                <label>Số điện thoại</label>
            </div>
            <asp:Button ID="btnRegister" runat="server" Text="Đăng Ký" CssClass="btn-register" OnClick="btnRegister_Click" />
            <div class="footer">
                Đã có tài khoản? <a href="Login.aspx">Đăng nhập ngay</a>
            </div>
        </div>
    </form>
</body>
</html>