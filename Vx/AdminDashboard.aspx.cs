using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
        private const int PageSize = 10;

        private int ProductPage
        {
            get => ViewState["ProductPage"] != null ? (int)ViewState["ProductPage"] : 1;
            set => ViewState["ProductPage"] = value;
        }

        private int UserPage
        {
            get => ViewState["UserPage"] != null ? (int)ViewState["UserPage"] : 1;
            set => ViewState["UserPage"] = value;
        }

        private int OrderPage
        {
            get => ViewState["OrderPage"] != null ? (int)ViewState["OrderPage"] : 1;
            set => ViewState["OrderPage"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra session
                if (Session["UserId"] == null || Session["Role"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để tiếp tục!");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                string role = Session["Role"]?.ToString();
                if (string.IsNullOrEmpty(role) || role != "Admin")
                {
                    ShowAlert("Chỉ Admin mới có quyền truy cập trang này!");
                    Response.Redirect("Home.aspx", false);
                    return;
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra chuỗi kết nối 'MyDB'!");
                    return;
                }

                // Tải dữ liệu lần đầu
                LoadProducts();
                LoadUsers();
                LoadOrders();
            }
        }

        private void LoadProducts(string searchTerm = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = string.IsNullOrEmpty(searchTerm)
                        ? "SELECT COUNT(*) FROM Products WHERE IsDeleted = 0"
                        : @"SELECT COUNT(*) FROM Products p
                           INNER JOIN Categories c ON p.CategoryId = c.CategoryId
                           WHERE p.IsDeleted = 0 AND (p.ProductName LIKE '%' + @SearchTerm + '%' OR c.CategoryName LIKE '%' + @SearchTerm + '%')";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        countCmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = string.IsNullOrEmpty(searchTerm)
                        ? @"
                          SELECT p.ProductId, c.CategoryName, p.ProductName, p.Price, p.Description, p.Stock
                          FROM Products p
                          INNER JOIN Categories c ON p.CategoryId = c.CategoryId
                          WHERE p.IsDeleted = 0
                          ORDER BY p.ProductId 
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
                        : @"
                          SELECT p.ProductId, c.CategoryName, p.ProductName, p.Price, p.Description, p.Stock
                          FROM Products p
                          INNER JOIN Categories c ON p.CategoryId = c.CategoryId
                          WHERE p.IsDeleted = 0 AND (p.ProductName LIKE '%' + @SearchTerm + '%' OR c.CategoryName LIKE '%' + @SearchTerm + '%')
                          ORDER BY p.ProductId 
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (ProductPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvProducts.DataSource = dt;
                    gvProducts.DataBind();

                    btnPrevProducts.Enabled = ProductPage > 1;
                    btnNextProducts.Enabled = ProductPage < totalPages;

                    lblProductsMessage.Text = dt.Rows.Count == 0 ? "Không có sản phẩm nào để hiển thị." : "";
                }
                catch (Exception ex)
                {
                    lblProductsMessage.Text = $"Lỗi khi tải sản phẩm: {ex.Message}";
                }
            }
        }

        private void LoadUsers(string searchTerm = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = string.IsNullOrEmpty(searchTerm)
                        ? "SELECT COUNT(*) FROM Users WHERE Role != 'Admin'"
                        : @"SELECT COUNT(*) FROM Users
                           WHERE (Username LIKE '%' + @SearchTerm + '%' OR FullName LIKE '%' + @SearchTerm + '%') AND Role != 'Admin'";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        countCmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = string.IsNullOrEmpty(searchTerm)
                        ? @"
                          SELECT UserId, Username, FullName, Email, Role
                          FROM Users
                          WHERE Role != 'Admin'
                          ORDER BY Username 
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
                        : @"
                          SELECT UserId, Username, FullName, Email, Role
                          FROM Users
                          WHERE (Username LIKE '%' + @SearchTerm + '%' OR FullName LIKE '%' + @SearchTerm + '%') AND Role != 'Admin'
                          ORDER BY Username 
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (UserPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();

                    btnPrevUsers.Enabled = UserPage > 1;
                    btnNextUsers.Enabled = UserPage < totalPages;

                    lblUsersMessage.Text = dt.Rows.Count == 0 ? "Không có người dùng nào để hiển thị." : "";
                }
                catch (Exception ex)
                {
                    lblUsersMessage.Text = $"Lỗi khi tải người dùng: {ex.Message}";
                }
            }
        }

        private void LoadOrders(string searchTerm = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = string.IsNullOrEmpty(searchTerm)
                        ? "SELECT COUNT(*) FROM Orders"
                        : @"SELECT COUNT(*) FROM Orders o
                           INNER JOIN Users u ON o.UserId = u.UserId
                           WHERE CAST(o.OrderId AS NVARCHAR) LIKE '%' + @SearchTerm + '%' OR u.Username LIKE '%' + @SearchTerm + '%'";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        countCmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = string.IsNullOrEmpty(searchTerm)
                        ? @"
                          SELECT o.OrderId, u.Username, o.OrderDate, o.TotalAmount, o.Status, o.ShippingAddress, o.PhoneNumber
                          FROM Orders o
                          INNER JOIN Users u ON o.UserId = u.UserId
                          ORDER BY o.OrderId
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY"
                        : @"
                          SELECT o.OrderId, u.Username, o.OrderDate, o.TotalAmount, o.Status, o.ShippingAddress, o.PhoneNumber
                          FROM Orders o
                          INNER JOIN Users u ON o.UserId = u.UserId
                          WHERE CAST(o.OrderId AS NVARCHAR) LIKE '%' + @SearchTerm + '%' OR u.Username LIKE '%' + @SearchTerm + '%'
                          ORDER BY o.OrderId
                          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (OrderPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvOrders.DataSource = dt;
                    gvOrders.DataBind();

                    btnPrevOrders.Enabled = OrderPage > 1;
                    btnNextOrders.Enabled = OrderPage < totalPages;

                    lblOrdersMessage.Text = dt.Rows.Count == 0 ? "Không có đơn hàng nào để hiển thị." : "";
                }
                catch (Exception ex)
                {
                    lblOrdersMessage.Text = $"Lỗi khi tải đơn hàng: {ex.Message}";
                }
            }
        }

        protected void txtSearchProducts_TextChanged(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Admin")
            {
                ShowAlert("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!");
                Response.Redirect("Login.aspx", false);
                return;
            }

            string searchTerm = txtSearchProducts.Text.Trim();
            ProductPage = 1; // Reset về trang đầu khi tìm kiếm
            LoadProducts(searchTerm);
        }

        protected void btnClearSearchProducts_Click(object sender, EventArgs e)
        {
            txtSearchProducts.Text = "";
            ProductPage = 1;
            LoadProducts();
        }

        protected void txtSearchUsers_TextChanged(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Admin")
            {
                ShowAlert("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!");
                Response.Redirect("Login.aspx", false);
                return;
            }

            string searchTerm = txtSearchUsers.Text.Trim();
            UserPage = 1; // Reset về trang đầu khi tìm kiếm
            LoadUsers(searchTerm);
        }

        protected void btnClearSearchUsers_Click(object sender, EventArgs e)
        {
            txtSearchUsers.Text = "";
            UserPage = 1;
            LoadUsers();
        }

        protected void txtSearchOrders_TextChanged(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"]?.ToString() != "Admin")
            {
                ShowAlert("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!");
                Response.Redirect("Login.aspx", false);
                return;
            }

            string searchTerm = txtSearchOrders.Text.Trim();
            OrderPage = 1; // Reset về trang đầu khi tìm kiếm
            LoadOrders(searchTerm);
        }

        protected void btnClearSearchOrders_Click(object sender, EventArgs e)
        {
            txtSearchOrders.Text = "";
            OrderPage = 1;
            LoadOrders();
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditProduct.aspx?mode=add", false);
        }

        protected void btnEditProduct_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string productId = btn.CommandArgument;
                if (string.IsNullOrEmpty(productId))
                {
                    ShowAlert("ID sản phẩm không hợp lệ!");
                    return;
                }
                Response.Redirect($"AddEditProduct.aspx?mode=edit&id={productId}", false);
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi khi chuyển hướng chỉnh sửa sản phẩm: {ex.Message}");
            }
        }

        protected void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Kiểm tra xem sản phẩm có trong đơn hàng không
                    string checkOrderQuery = "SELECT COUNT(*) FROM OrderDetails WHERE ProductId = @ProductId";
                    SqlCommand checkOrderCmd = new SqlCommand(checkOrderQuery, conn);
                    checkOrderCmd.Parameters.AddWithValue("@ProductId", productId);
                    int orderCount = (int)checkOrderCmd.ExecuteScalar();

                    if (orderCount > 0)
                    {
                        ShowAlert("Không thể xóa sản phẩm này vì nó đang có trong đơn hàng!");
                        return;
                    }

                    // Lưu thông tin sản phẩm vào bảng DeletedProducts
                    string insertDeletedQuery = @"
                        INSERT INTO DeletedProducts (ProductId, CategoryId, ProductName, Description, Price, Stock, ImageUrl, CreatedDate)
                        SELECT ProductId, CategoryId, ProductName, Description, Price, Stock, ImageUrl, CreatedDate
                        FROM Products WHERE ProductId = @ProductId";
                    SqlCommand insertDeletedCmd = new SqlCommand(insertDeletedQuery, conn);
                    insertDeletedCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertDeletedCmd.ExecuteNonQuery();

                    // Xóa sản phẩm khỏi giỏ hàng
                    string deleteCartQuery = "DELETE FROM Cart WHERE ProductId = @ProductId";
                    SqlCommand deleteCartCmd = new SqlCommand(deleteCartQuery, conn);
                    deleteCartCmd.Parameters.AddWithValue("@ProductId", productId);
                    deleteCartCmd.ExecuteNonQuery();

                    // Đánh dấu sản phẩm là đã xóa
                    string updateProductQuery = "UPDATE Products SET IsDeleted = 1 WHERE ProductId = @ProductId";
                    SqlCommand updateProductCmd = new SqlCommand(updateProductQuery, conn);
                    updateProductCmd.Parameters.AddWithValue("@ProductId", productId);
                    updateProductCmd.ExecuteNonQuery();

                    ShowAlert("Xóa sản phẩm thành công! Sản phẩm đã được lưu vào lịch sử.");
                    LoadProducts(txtSearchProducts.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblProductsMessage.Text = $"Lỗi khi xóa sản phẩm: {ex.Message}";
                }
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditUser.aspx?mode=add", false);
        }

        protected void btnEditUser_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string userId = btn.CommandArgument;
                if (string.IsNullOrEmpty(userId))
                {
                    ShowAlert("ID người dùng không hợp lệ!");
                    return;
                }
                Response.Redirect($"AddEditUser.aspx?mode=edit&id={Server.UrlEncode(userId)}", false);
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi khi chuyển hướng chỉnh sửa người dùng: {ex.Message}");
            }
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string userId = btn.CommandArgument;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string roleQuery = "SELECT Role FROM Users WHERE UserId = @UserId";
                    SqlCommand roleCmd = new SqlCommand(roleQuery, conn);
                    roleCmd.Parameters.AddWithValue("@UserId", userId);
                    string role = roleCmd.ExecuteScalar()?.ToString();

                    if (role == "Admin")
                    {
                        ShowAlert("Không thể xóa người dùng có vai trò Admin!");
                        return;
                    }

                    string checkQuery = "SELECT COUNT(*) FROM Orders WHERE UserId = @UserId";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    int orderCount = (int)checkCmd.ExecuteScalar();

                    if (orderCount > 0)
                    {
                        ShowAlert("Không thể xóa người dùng này vì họ đang có đơn hàng!");
                        return;
                    }

                    string query = "DELETE FROM Users WHERE UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();

                    ShowAlert("Xóa người dùng thành công!");
                    LoadUsers(txtSearchUsers.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblUsersMessage.Text = $"Lỗi khi xóa người dùng: {ex.Message}";
                }
            }
        }

        protected void btnViewDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string orderId = btn.CommandArgument;
                if (string.IsNullOrEmpty(orderId))
                {
                    ShowAlert("ID đơn hàng không hợp lệ!");
                    return;
                }
                Response.Redirect($"OrderDetailsAdmin.aspx?id={orderId}", false);
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi khi chuyển hướng xem chi tiết đơn hàng: {ex.Message}");
            }
        }

        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string orderId = btn.CommandArgument;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Orders SET Status = 'Confirmed' WHERE OrderId = @OrderId AND Status = 'Pending'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Xác nhận đơn hàng thành công!");
                    }
                    else
                    {
                        ShowAlert("Không thể xác nhận đơn hàng. Đơn hàng có thể đã được xử lý!");
                    }
                    LoadOrders(txtSearchOrders.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblOrdersMessage.Text = $"Lỗi khi xác nhận đơn hàng: {ex.Message}";
                }
            }
        }

        protected void btnCancelOrder_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string orderId = btn.CommandArgument;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Orders SET Status = 'Cancelled' WHERE OrderId = @OrderId AND Status = 'Pending'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Hủy đơn hàng thành công!");
                    }
                    else
                    {
                        ShowAlert("Không thể hủy đơn hàng. Đơn hàng có thể đã được xử lý!");
                    }
                    LoadOrders(txtSearchOrders.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblOrdersMessage.Text = $"Lỗi khi hủy đơn hàng: {ex.Message}";
                }
            }
        }

        protected void btnPrevProducts_Click(object sender, EventArgs e)
        {
            if (ProductPage > 1)
            {
                ProductPage--;
                LoadProducts(txtSearchProducts.Text.Trim());
            }
        }

        protected void btnNextProducts_Click(object sender, EventArgs e)
        {
            ProductPage++;
            LoadProducts(txtSearchProducts.Text.Trim());
        }

        protected void btnPrevUsers_Click(object sender, EventArgs e)
        {
            if (UserPage > 1)
            {
                UserPage--;
                LoadUsers(txtSearchUsers.Text.Trim());
            }
        }

        protected void btnNextUsers_Click(object sender, EventArgs e)
        {
            UserPage++;
            LoadUsers(txtSearchUsers.Text.Trim());
        }

        protected void btnPrevOrders_Click(object sender, EventArgs e)
        {
            if (OrderPage > 1)
            {
                OrderPage--;
                LoadOrders(txtSearchOrders.Text.Trim());
            }
        }

        protected void btnNextOrders_Click(object sender, EventArgs e)
        {
            OrderPage++;
            LoadOrders(txtSearchOrders.Text.Trim());
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx", false);
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}