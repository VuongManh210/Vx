<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Vx.Search" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <title>Kết quả tìm kiếm</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <h2>Kết quả tìm kiếm</h2>
            <asp:Label ID="lblResult" runat="server" CssClass="alert alert-info" />
            <br />
            <a href="Home.aspx" class="btn btn-primary">Quay lại Home</a>
        </div>
    </form>
</body>
</html>
