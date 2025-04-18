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

        private int ShopPage
        {
            get => ViewState["ShopPage"] != null ? (int)ViewState["ShopPage"] : 1;
            set => ViewState["ShopPage"] = value;
        }

        private int OrderPage
        {
            get => ViewState["OrderPage"] != null ? (int)ViewState["OrderPage"] : 1;
            set => ViewState["OrderPage"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"] == null)
            {
                ShowAlert("Vui lòng đăng nhập để tiếp tục!");
                Response.Redirect("Login.aspx", false);
                return;
            }

            string role = Session["Role"].ToString();
            if (role != "Admin")
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

            if (!IsPostBack)
            {
                LoadProducts();
                LoadUsers();
                LoadShops();
                LoadOrders();
            }
        }

        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = "SELECT COUNT(*) FROM Products";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = @"
                        SELECT ProductId, ProductName, Price, Description, Stock
                        FROM Products
                        ORDER BY ProductId 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (ProductPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
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

        private void LoadUsers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = "SELECT COUNT(*) FROM Users";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = @"
                        SELECT UserId, Username, FullName, Email, Role, ShopId
                        FROM Users
                        ORDER BY Username 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (UserPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
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

        private void LoadShops()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = "SELECT COUNT(*) FROM Shops";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = @"
                        SELECT s.ShopId, s.ShopName, COUNT(p.ProductId) AS ProductCount
                        FROM Shops s
                        LEFT JOIN Products p ON s.ShopId = p.ShopId
                        GROUP BY s.ShopId, s.ShopName
                        ORDER BY s.ShopId
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (ShopPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvShops.DataSource = dt;
                    gvShops.DataBind();

                    btnPrevShops.Enabled = ShopPage > 1;
                    btnNextShops.Enabled = ShopPage < totalPages;

                    lblShopsMessage.Text = dt.Rows.Count == 0 ? "Không có shop nào để hiển thị." : "";
                }
                catch (Exception ex)
                {
                    lblShopsMessage.Text = $"Lỗi khi tải cửa hàng: {ex.Message}";
                }
            }
        }

        private void LoadOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string countQuery = "SELECT COUNT(*) FROM Orders";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalRecords = (int)countCmd.ExecuteScalar();
                    int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

                    string query = @"
                        SELECT o.OrderId, u.Username, o.OrderDate, o.TotalAmount, o.Status, o.ShippingAddress, o.PhoneNumber
                        FROM Orders o
                        INNER JOIN Users u ON o.UserId = u.UserId
                        ORDER BY o.OrderId
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (OrderPage - 1) * PageSize);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
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
                    string checkQuery = "SELECT COUNT(*) FROM OrderDetails WHERE ProductId = @ProductId";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@ProductId", productId);
                    int orderCount = (int)checkCmd.ExecuteScalar();

                    if (orderCount > 0)
                    {
                        ShowAlert("Không thể xóa sản phẩm này vì nó đang có trong đơn hàng!");
                        return;
                    }

                    string query = "DELETE FROM Products WHERE ProductId = @ProductId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.ExecuteNonQuery();

                    ShowAlert("Xóa sản phẩm thành công!");
                    LoadProducts();
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
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    lblUsersMessage.Text = $"Lỗi khi xóa người dùng: {ex.Message}";
                }
            }
        }

        protected void btnAddShop_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditShop.aspx?mode=add", false);
        }

        protected void btnEditShop_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                string shopId = btn.CommandArgument;
                if (string.IsNullOrEmpty(shopId))
                {
                    ShowAlert("ID shop không hợp lệ!");
                    return;
                }
                Response.Redirect($"AddEditShop.aspx?mode=edit&id={shopId}", false);
            }
            catch (Exception ex)
            {
                ShowAlert($"Lỗi khi chuyển hướng chỉnh sửa shop: {ex.Message}");
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
                    LoadOrders();
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
                    LoadOrders();
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
                LoadProducts();
            }
        }

        protected void btnNextProducts_Click(object sender, EventArgs e)
        {
            ProductPage++;
            LoadProducts();
        }

        protected void btnPrevUsers_Click(object sender, EventArgs e)
        {
            if (UserPage > 1)
            {
                UserPage--;
                LoadUsers();
            }
        }

        protected void btnNextUsers_Click(object sender, EventArgs e)
        {
            UserPage++;
            LoadUsers();
        }

        protected void btnPrevShops_Click(object sender, EventArgs e)
        {
            if (ShopPage > 1)
            {
                ShopPage--;
                LoadShops();
            }
        }

        protected void btnNextShops_Click(object sender, EventArgs e)
        {
            ShopPage++;
            LoadShops();
        }

        protected void btnPrevOrders_Click(object sender, EventArgs e)
        {
            if (OrderPage > 1)
            {
                OrderPage--;
                LoadOrders();
            }
        }

        protected void btnNextOrders_Click(object sender, EventArgs e)
        {
            OrderPage++;
            LoadOrders();
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