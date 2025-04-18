<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QLBH.aspx.cs" Inherits="Vx.QLBH" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style>
#Button1 {
    position: absolute;
    top: 20px; /* Cách mép trên 20px */
    left: 20px; /* Cách mép trái 20px */
    background-color: #dc3545; /* Màu đỏ */
    color: white;
    padding: 10px 15px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
}

#Button1:hover {
    background-color: #c82333;
}

        body {
    font-family: Arial, sans-serif;
    background-color: #f4f4f4;
    margin: 0;
    padding: 0;
    text-align: center;
}

h1 {
    background-color: #007bff;
    color: white;
    padding: 15px;
    margin: 0;
    font-size: 24px;
}

form {
    background: white;
    width: 50%;
    margin: 20px auto;
    padding: 20px;
    box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
    border-radius: 8px;
}

asp:TextBox {
    display: block;
    width: 100%;
    padding: 10px;
    margin: 15px 0; 
    border: 1px solid #ccc;
    border-radius: 5px;
}

asp:Button {
    background-color: #28a745;
    color: white;
    border: none;
    padding: 10px 15px;
    cursor: pointer;
    border-radius: 5px;
    font-size: 16px;
    margin-top: 15px; 

asp:Button:hover {
    background-color: #218838;
}

asp:GridView {
    width: 80%;
    margin: 40px auto; 
    border-collapse: collapse;
}

asp:GridView th, asp:GridView td {
    border: 1px solid #ddd;
    padding: 8px;
}

asp:GridView th {
    background-color: #007bff;
    color: white;
}


    </style>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="Button1" runat="server" style="margin-bottom: 0px" Text="Quay lai" OnClick="Button1_Click" />
        <h1>Chỉnh sửa thông tin</h1>

        <div>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="MaHang" DataSourceID="SqlDataSource1">
                <Columns>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                    <asp:BoundField DataField="MaHang" HeaderText="MaHang" ReadOnly="True" SortExpression="MaHang" />
                    <asp:BoundField DataField="TenHang" HeaderText="TenHang" SortExpression="TenHang" />
                    <asp:BoundField DataField="MoTa" HeaderText="MoTa" SortExpression="MoTa" />
                    <asp:BoundField DataField="DonGiaNhap" HeaderText="DonGiaNhap" SortExpression="DonGiaNhap" />
                    <asp:BoundField DataField="DonGiaBan" HeaderText="DonGiaBan" SortExpression="DonGiaBan" />
                    <asp:BoundField DataField="GhiChu" HeaderText="GhiChu" SortExpression="GhiChu" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConflictDetection="CompareAllValues" ConnectionString="<%$ ConnectionStrings:QuanLyBanHangConnectionString4 %>" DeleteCommand="DELETE FROM [tblHang] WHERE [MaHang] = @original_MaHang AND [TenHang] = @original_TenHang AND (([MoTa] = @original_MoTa) OR ([MoTa] IS NULL AND @original_MoTa IS NULL)) AND (([DonGiaNhap] = @original_DonGiaNhap) OR ([DonGiaNhap] IS NULL AND @original_DonGiaNhap IS NULL)) AND (([DonGiaBan] = @original_DonGiaBan) OR ([DonGiaBan] IS NULL AND @original_DonGiaBan IS NULL)) AND (([GhiChu] = @original_GhiChu) OR ([GhiChu] IS NULL AND @original_GhiChu IS NULL))" InsertCommand="INSERT INTO [tblHang] ([MaHang], [TenHang], [MoTa], [DonGiaNhap], [DonGiaBan], [GhiChu]) VALUES (@MaHang, @TenHang, @MoTa, @DonGiaNhap, @DonGiaBan, @GhiChu)" OldValuesParameterFormatString="original_{0}" ProviderName="<%$ ConnectionStrings:QuanLyBanHangConnectionString4.ProviderName %>" SelectCommand="SELECT [MaHang], [TenHang], [MoTa], [DonGiaNhap], [DonGiaBan], [GhiChu] FROM [tblHang]" UpdateCommand="UPDATE [tblHang] SET [TenHang] = @TenHang, [MoTa] = @MoTa, [DonGiaNhap] = @DonGiaNhap, [DonGiaBan] = @DonGiaBan, [GhiChu] = @GhiChu WHERE [MaHang] = @original_MaHang AND [TenHang] = @original_TenHang AND (([MoTa] = @original_MoTa) OR ([MoTa] IS NULL AND @original_MoTa IS NULL)) AND (([DonGiaNhap] = @original_DonGiaNhap) OR ([DonGiaNhap] IS NULL AND @original_DonGiaNhap IS NULL)) AND (([DonGiaBan] = @original_DonGiaBan) OR ([DonGiaBan] IS NULL AND @original_DonGiaBan IS NULL)) AND (([GhiChu] = @original_GhiChu) OR ([GhiChu] IS NULL AND @original_GhiChu IS NULL))">
                <DeleteParameters>
                    <asp:Parameter Name="original_MaHang" Type="Int32" />
                    <asp:Parameter Name="original_TenHang" Type="String" />
                    <asp:Parameter Name="original_MoTa" Type="String" />
                    <asp:Parameter Name="original_DonGiaNhap" Type="Decimal" />
                    <asp:Parameter Name="original_DonGiaBan" Type="Decimal" />
                    <asp:Parameter Name="original_GhiChu" Type="String" />
                </DeleteParameters>
                <InsertParameters>
                    <asp:Parameter Name="MaHang" Type="Int32" />
                    <asp:Parameter Name="TenHang" Type="String" />
                    <asp:Parameter Name="MoTa" Type="String" />
                    <asp:Parameter Name="DonGiaNhap" Type="Decimal" />
                    <asp:Parameter Name="DonGiaBan" Type="Decimal" />
                    <asp:Parameter Name="GhiChu" Type="String" />
                </InsertParameters>
                <UpdateParameters>
                    <asp:Parameter Name="TenHang" Type="String" />
                    <asp:Parameter Name="MoTa" Type="String" />
                    <asp:Parameter Name="DonGiaNhap" Type="Decimal" />
                    <asp:Parameter Name="DonGiaBan" Type="Decimal" />
                    <asp:Parameter Name="GhiChu" Type="String" />
                    <asp:Parameter Name="original_MaHang" Type="Int32" />
                    <asp:Parameter Name="original_TenHang" Type="String" />
                    <asp:Parameter Name="original_MoTa" Type="String" />
                    <asp:Parameter Name="original_DonGiaNhap" Type="Decimal" />
                    <asp:Parameter Name="original_DonGiaBan" Type="Decimal" />
                    <asp:Parameter Name="original_GhiChu" Type="String" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>


            <asp:TextBox ID="txtMahang" runat="server" Placeholder="Nhập mã hàng" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="txtTenhang" runat="server" Placeholder="Nhập tên hàng" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="txtMoTa" runat="server" Placeholder="Nhập mô tả" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="txtDongianhap" runat="server" Placeholder="Nhập đơn giá nhập" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="txtDongiaban" runat="server" Placeholder="Nhập đơn giá bán" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="txtGhichu" runat="server" Placeholder="Nhập ghi chú" AutoCompleteType="Disabled"></asp:TextBox>

            <asp:Button ID="btnThem" runat="server" Text="Thêm" OnClick="btnThem_Click" style="height: 29px" />


        <p>
            &nbsp;</p>


    </form>
</body>
</html>
