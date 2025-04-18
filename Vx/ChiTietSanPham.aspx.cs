using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class ChiTietSanPham : System.Web.UI.Page
    {
        private int ProductId
        {
            get { return ViewState["ProductId"] != null ? Convert.ToInt32(ViewState["ProductId"]) : 0; }
            set { ViewState["ProductId"] = value; }
        }

        private decimal ProductPrice
        {
            get { return ViewState["ProductPrice"] != null ? Convert.ToDecimal(ViewState["ProductPrice"]) : 0; }
            set { ViewState["ProductPrice"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (Session["Username"] != null)
                {
                    lblUsername.Text = Session["Username"].ToString();
                }
                else
                {
                    lblUsername.Text = "Khách";
                }

                LoadProductDetails();
            }
        }

        private void LoadProductDetails()
        {
            string productId = Request.QueryString["id"];
            if (string.IsNullOrEmpty(productId) || !int.TryParse(productId, out int id))
            {
                ShowAlert("Sản phẩm không tồn tại!");
                Response.Redirect("Home.aspx");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT ProductId, ProductName, Price, Description, Stock, ImageUrl 
                        FROM Products 
                        WHERE ProductId = @ProductId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ProductId = Convert.ToInt32(reader["ProductId"]);
                        ProductPrice = Convert.ToDecimal(reader["Price"]);
                        lblTenSanPham.InnerText = reader["ProductName"].ToString();
                        lblGia.InnerText = Convert.ToDecimal(reader["Price"]).ToString("N0") + " VNĐ";
                        lblMoTa.InnerText = "Mô tả: " + reader["Description"].ToString();
                        lblTonKho.InnerText = "Tồn kho: " + reader["Stock"].ToString();
                        imgSanPham.Src = reader["ImageUrl"].ToString();

                        int stock = Convert.ToInt32(reader["Stock"]);
                        if (stock <= 0)
                        {
                            btnThemVaoGioHang.Enabled = false;
                            btnThemVaoGioHang.CssClass += " disabled";
                            btnMuaNgay.Enabled = false;
                            btnMuaNgay.CssClass += " disabled";
                        }
                    }
                    else
                    {
                        ShowAlert("Sản phẩm không tồn tại!");
                        Response.Redirect("Home.aspx");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        protected void btnTang_Click(object sender, EventArgs e)
        {
            int quantity = int.Parse(txtSoLuong.Text);
            int stock = GetStock(ProductId);
            if (quantity < stock)
            {
                quantity++;
                txtSoLuong.Text = quantity.ToString();
            }
            else
            {
                ShowAlert("Số lượng vượt quá tồn kho!");
            }
        }

        protected void btnGiam_Click(object sender, EventArgs e)
        {
            int quantity = int.Parse(txtSoLuong.Text);
            if (quantity > 1)
            {
                quantity--;
                txtSoLuong.Text = quantity.ToString();
            }
        }

        protected void btnThemVaoGioHang_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                ShowAlert("Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!");
                Response.Redirect("Login.aspx");
                return;
            }

            int quantity = int.Parse(txtSoLuong.Text);
            int stock = GetStock(ProductId);
            if (quantity > stock)
            {
                ShowAlert("Số lượng vượt quá tồn kho!");
                return;
            }

            string userId = Session["UserId"].ToString();
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        IF EXISTS (SELECT 1 FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId)
                            UPDATE Cart 
                            SET Quantity = Quantity + @Quantity
                            WHERE UserId = @UserId AND ProductId = @ProductId
                        ELSE
                            INSERT INTO Cart (UserId, ProductId, Quantity)
                            VALUES (@UserId, @ProductId, @Quantity)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@ProductId", ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.ExecuteNonQuery();

                    ShowAlert("Đã thêm sản phẩm vào giỏ hàng!");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin giao hàng
            if (string.IsNullOrWhiteSpace(txtModalHoTen.Text))
            {
                ShowAlert("Vui lòng nhập họ và tên!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModalSoDienThoai.Text))
            {
                ShowAlert("Vui lòng nhập số điện thoại!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtModalSoDienThoai.Text, @"^\d{10,11}$"))
            {
                ShowAlert("Số điện thoại không hợp lệ! Vui lòng nhập 10-11 chữ số.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtModalDiaChi.Text))
            {
                ShowAlert("Vui lòng nhập địa chỉ giao hàng!");
                return;
            }

            string userId = Session["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                ShowAlert("Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại!");
                Response.Redirect("Login.aspx");
                return;
            }

            int quantity = int.Parse(txtSoLuong.Text);
            int stock = GetStock(ProductId);
            if (quantity > stock)
            {
                ShowAlert($"Sản phẩm chỉ còn {stock} trong kho, không đủ để đặt hàng!");
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Tính tổng tiền
                        decimal totalAmount = ProductPrice * quantity;

                        // Lưu đơn hàng vào bảng Orders
                        string insertOrderQuery = @"
                            INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PhoneNumber)
                            OUTPUT INSERTED.OrderId
                            VALUES (@UserId, GETDATE(), @TotalAmount, 'Pending', @ShippingAddress, @PhoneNumber)";
                        SqlCommand insertOrderCmd = new SqlCommand(insertOrderQuery, conn, transaction);
                        insertOrderCmd.Parameters.AddWithValue("@UserId", userId);
                        insertOrderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        insertOrderCmd.Parameters.AddWithValue("@ShippingAddress", txtModalDiaChi.Text.Trim());
                        insertOrderCmd.Parameters.AddWithValue("@PhoneNumber", txtModalSoDienThoai.Text.Trim());
                        int orderId = (int)insertOrderCmd.ExecuteScalar();

                        // Lưu chi tiết đơn hàng vào bảng OrderDetails
                        string insertDetailQuery = @"
                            INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice)
                            VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";
                        SqlCommand insertDetailCmd = new SqlCommand(insertDetailQuery, conn, transaction);
                        insertDetailCmd.Parameters.AddWithValue("@OrderId", orderId);
                        insertDetailCmd.Parameters.AddWithValue("@ProductId", ProductId);
                        insertDetailCmd.Parameters.AddWithValue("@Quantity", quantity);
                        insertDetailCmd.Parameters.AddWithValue("@UnitPrice", ProductPrice);
                        insertDetailCmd.ExecuteNonQuery();

                        // Cập nhật tồn kho
                        string updateStockQuery = @"
                            UPDATE Products 
                            SET Stock = Stock - @Quantity 
                            WHERE ProductId = @ProductId AND Stock >= @Quantity";
                        SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn, transaction);
                        updateStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                        updateStockCmd.Parameters.AddWithValue("@ProductId", ProductId);
                        int rowsAffected = updateStockCmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Không thể cập nhật tồn kho. Có thể sản phẩm không tồn tại hoặc tồn kho không đủ.");
                        }

                        // Commit giao dịch
                        transaction.Commit();

                        // Hiển thị thông báo và chuyển hướng
                        ShowAlert("Đặt hàng thành công! Cảm ơn bạn đã mua sắm.");
                        Response.Redirect($"OrderDetails.aspx?OrderId={orderId}");
                    }
                    catch (Exception ex)
                    {
                        // Rollback nếu có lỗi
                        transaction.Rollback();
                        ShowAlert($"Có lỗi xảy ra khi đặt hàng: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi kết nối cơ sở dữ liệu: {ex.Message}");
                }
            }
        }

        private int GetStock(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Stock FROM Products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string category = ddlCategory.SelectedValue;
            Response.Redirect($"Home.aspx?search={searchText}&category={category}");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}