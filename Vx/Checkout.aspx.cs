using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để thanh toán!");
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

                LoadCart();
            }
        }

        private void LoadCart()
        {
            DataTable cartTable = new DataTable();
            cartTable.Columns.Add("ProductId", typeof(int));
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("Price", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("Total", typeof(decimal));
            cartTable.Columns.Add("ImageUrl", typeof(string));
            cartTable.Columns.Add("Stock", typeof(int));

            decimal totalAmount = 0;
            string userId = Session["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                ShowAlert("Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại!");
                Response.Redirect("Login.aspx");
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
                    string query = @"
                        SELECT c.ProductId, p.ProductName, p.Price, c.Quantity, p.ImageUrl, p.Stock
                        FROM Cart c
                        JOIN Products p ON c.ProductId = p.ProductId
                        WHERE c.UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow row = cartTable.NewRow();
                        row["ProductId"] = reader["ProductId"];
                        row["ProductName"] = reader["ProductName"];
                        row["Price"] = reader["Price"];
                        row["Quantity"] = reader["Quantity"];
                        row["Total"] = Convert.ToDecimal(reader["Price"]) * Convert.ToInt32(reader["Quantity"]);
                        row["ImageUrl"] = reader["ImageUrl"];
                        row["Stock"] = reader["Stock"];
                        totalAmount += Convert.ToDecimal(row["Total"]);
                        cartTable.Rows.Add(row);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải giỏ hàng: {ex.Message}");
                    return;
                }
            }

            if (cartTable.Rows.Count == 0)
            {
                ShowAlert("Giỏ hàng của bạn đang trống! Vui lòng thêm sản phẩm trước khi thanh toán.");
                Response.Redirect("Cart.aspx");
                return;
            }

            rptCart.DataSource = cartTable;
            rptCart.DataBind();
            lblTongTien.Text = totalAmount.ToString("N0") + " VND";
        }

        protected void btnXacNhan_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin giao hàng
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                ShowAlert("Vui lòng nhập họ và tên!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                ShowAlert("Vui lòng nhập số điện thoại!");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtSoDienThoai.Text, @"^\d{10,11}$"))
            {
                ShowAlert("Số điện thoại không hợp lệ! Vui lòng nhập 10-11 chữ số.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
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
                        // Kiểm tra giỏ hàng
                        DataTable cartItems = GetCartItems(userId, conn, transaction);
                        if (cartItems.Rows.Count == 0)
                        {
                            ShowAlert("Giỏ hàng của bạn đang trống! Vui lòng thêm sản phẩm trước khi thanh toán.");
                            transaction.Rollback();
                            Response.Redirect("Cart.aspx");
                            return;
                        }

                        // Kiểm tra tồn kho
                        foreach (DataRow row in cartItems.Rows)
                        {
                            int productId = Convert.ToInt32(row["ProductId"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            int stock = GetLatestStock(productId, conn, transaction);

                            if (stock < 0)
                            {
                                ShowAlert($"Có lỗi xảy ra: Sản phẩm {row["ProductName"]} có số lượng tồn kho không hợp lệ!");
                                transaction.Rollback();
                                return;
                            }

                            if (quantity > stock)
                            {
                                ShowAlert($"Sản phẩm {row["ProductName"]} chỉ còn {stock} trong kho, không đủ để đặt hàng!");
                                transaction.Rollback();
                                return;
                            }
                        }

                        // Tính tổng tiền
                        decimal totalAmount = 0;
                        foreach (DataRow row in cartItems.Rows)
                        {
                            totalAmount += Convert.ToDecimal(row["Price"]) * Convert.ToInt32(row["Quantity"]);
                        }

                        // Lưu đơn hàng vào bảng Orders
                        string insertOrderQuery = @"
                            INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PhoneNumber)
                            OUTPUT INSERTED.OrderId
                            VALUES (@UserId, GETDATE(), @TotalAmount, 'Pending', @ShippingAddress, @PhoneNumber)";
                        SqlCommand insertOrderCmd = new SqlCommand(insertOrderQuery, conn, transaction);
                        insertOrderCmd.Parameters.AddWithValue("@UserId", userId);
                        insertOrderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        insertOrderCmd.Parameters.AddWithValue("@ShippingAddress", txtDiaChi.Text.Trim());
                        insertOrderCmd.Parameters.AddWithValue("@PhoneNumber", txtSoDienThoai.Text.Trim());
                        int orderId = (int)insertOrderCmd.ExecuteScalar();

                        // Lưu chi tiết đơn hàng vào bảng OrderDetails
                        foreach (DataRow row in cartItems.Rows)
                        {
                            int productId = Convert.ToInt32(row["ProductId"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            decimal unitPrice = Convert.ToDecimal(row["Price"]);

                            string insertDetailQuery = @"
                                INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice)
                                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";
                            SqlCommand insertDetailCmd = new SqlCommand(insertDetailQuery, conn, transaction);
                            insertDetailCmd.Parameters.AddWithValue("@OrderId", orderId);
                            insertDetailCmd.Parameters.AddWithValue("@ProductId", productId);
                            insertDetailCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertDetailCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                            insertDetailCmd.ExecuteNonQuery();

                            // Cập nhật tồn kho
                            string updateStockQuery = @"
                                UPDATE Products 
                                SET Stock = Stock - @Quantity 
                                WHERE ProductId = @ProductId AND Stock >= @Quantity";
                            SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn, transaction);
                            updateStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                            updateStockCmd.Parameters.AddWithValue("@ProductId", productId);
                            int rowsAffected = updateStockCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception($"Không thể cập nhật tồn kho cho sản phẩm {row["ProductName"]}. Có thể sản phẩm không tồn tại hoặc tồn kho không đủ.");
                            }
                        }

                        // Xóa giỏ hàng
                        string deleteCartQuery = "DELETE FROM Cart WHERE UserId = @UserId";
                        SqlCommand deleteCartCmd = new SqlCommand(deleteCartQuery, conn, transaction);
                        deleteCartCmd.Parameters.AddWithValue("@UserId", userId);
                        deleteCartCmd.ExecuteNonQuery();

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

        private DataTable GetCartItems(string userId, SqlConnection conn, SqlTransaction transaction)
        {
            DataTable cartTable = new DataTable();
            cartTable.Columns.Add("ProductId", typeof(int));
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("Price", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));

            string query = @"
                SELECT c.ProductId, p.ProductName, p.Price, c.Quantity
                FROM Cart c
                JOIN Products p ON c.ProductId = p.ProductId
                WHERE c.UserId = @UserId";
            SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@UserId", userId);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DataRow row = cartTable.NewRow();
                    row["ProductId"] = reader["ProductId"];
                    row["ProductName"] = reader["ProductName"];
                    row["Price"] = reader["Price"];
                    row["Quantity"] = reader["Quantity"];
                    cartTable.Rows.Add(row);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra khi lấy giỏ hàng: {ex.Message}");
            }

            return cartTable;
        }

        private int GetLatestStock(int productId, SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT Stock FROM Products WHERE ProductId = @ProductId";
            SqlCommand cmd = new SqlCommand(query, conn, transaction);
            cmd.Parameters.AddWithValue("@ProductId", productId);

            try
            {
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra khi kiểm tra tồn kho: {ex.Message}");
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