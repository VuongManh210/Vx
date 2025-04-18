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
        private int productPage
        {
            get { return ViewState["ProductPage"] != null ? (int)ViewState["ProductPage"] : 1; }
            set { ViewState["ProductPage"] = value; }
        }

        private int userPage
        {
            get { return ViewState["UserPage"] != null ? (int)ViewState["UserPage"] : 1; }
            set { ViewState["UserPage"] = value; }
        }

        private int orderPage
        {
            get { return ViewState["OrderPage"] != null ? (int)ViewState["OrderPage"] : 1; }
            set { ViewState["OrderPage"] = value; }
        }

        private int shopPage
        {
            get { return ViewState["ShopPage"] != null ? (int)ViewState["ShopPage"] : 1; }
            set { ViewState["ShopPage"] = value; }
        }

        private const int pageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra đăng nhập
                if (Session["UserId"] == null || Session["Role"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để truy cập trang quản trị!");
                    Response.Redirect("Login.aspx");
                    return;
                }

                // Đảm bảo chỉ Admin truy cập được
                string role = Session["Role"].ToString();
                if (role != "Admin")
                {
                    ShowAlert("Chỉ Admin mới có quyền truy cập trang này!");
                    Response.Redirect("Home.aspx");
                    return;
                }

                LoadProducts();
                LoadUsers();
                LoadOrders();
                LoadShops();
            }
        }

        private void LoadProducts()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB' trong web.config!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string countQuery = "SELECT COUNT(*) FROM Products";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalProducts = (int)countCmd.ExecuteScalar();

                    string query = @"
                        SELECT ProductId, ProductName, Price, Description, Stock 
                        FROM Products
                        ORDER BY ProductId 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (productPage - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvProducts.DataSource = dt;
                    gvProducts.DataBind();

                    btnPrevProducts.Enabled = productPage > 1;
                    btnNextProducts.Enabled = productPage * pageSize < totalProducts;

                    if (dt.Rows.Count == 0)
                    {
                        ShowAlert("Không có sản phẩm nào để hiển thị.");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải danh sách sản phẩm: {ex.Message}");
                }
            }
        }

        private void LoadUsers()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB' trong web.config!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Đếm tổng số người dùng
                    string countQuery = "SELECT COUNT(*) FROM Users";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalUsers = (int)countCmd.ExecuteScalar();
                    ShowAlert($"Tổng số người dùng trong bảng Users: {totalUsers}");

                    // Truy vấn danh sách người dùng
                    string query = @"
                        SELECT UserId, Username, FullName, Email, Role, ShopId 
                        FROM Users 
                        ORDER BY UserId 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (userPage - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        ShowAlert("Không có người dùng nào để hiển thị. Vui lòng thêm người dùng mới!");
                    }
                    else
                    {
                        ShowAlert($"Đã tải thành công {dt.Rows.Count} người dùng.");
                    }

                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();

                    btnPrevUsers.Enabled = userPage > 1;
                    btnNextUsers.Enabled = userPage * pageSize < totalUsers;
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải danh sách người dùng: {ex.Message}");
                }
            }
        }

        private void LoadOrders()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB' trong web.config!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string countQuery = "SELECT COUNT(*) FROM Orders o";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalOrders = (int)countCmd.ExecuteScalar();

                    string query = @"
                        SELECT o.OrderId, o.UserId, u.Username, o.OrderDate, o.TotalAmount, o.Status, o.ShippingAddress, o.PhoneNumber
                        FROM Orders o
                        JOIN Users u ON o.UserId = u.UserId
                        ORDER BY o.OrderId 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (orderPage - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvOrders.DataSource = dt;
                    gvOrders.DataBind();

                    btnPrevOrders.Enabled = orderPage > 1;
                    btnNextOrders.Enabled = orderPage * pageSize < totalOrders;

                    if (dt.Rows.Count == 0)
                    {
                        ShowAlert("Không có đơn hàng nào để hiển thị.");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải danh sách đơn hàng: {ex.Message}");
                }
            }
        }

        private void LoadShops()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB' trong web.config!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string countQuery = "SELECT COUNT(*) FROM Shops";
                    SqlCommand countCmd = new SqlCommand(countQuery, conn);
                    int totalShops = (int)countCmd.ExecuteScalar();

                    string query = @"
                        SELECT s.ShopId, s.ShopName, 
                               (SELECT COUNT(*) FROM Products p WHERE p.ShopId = s.ShopId) AS ProductCount
                        FROM Shops s
                        ORDER BY s.ShopId 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Offset", (shopPage - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvShops.DataSource = dt;
                    gvShops.DataBind();

                    btnPrevShops.Enabled = shopPage > 1;
                    btnNextShops.Enabled = shopPage * pageSize < totalShops;

                    if (dt.Rows.Count == 0)
                    {
                        ShowAlert("Không có shop nào để hiển thị. Vui lòng thêm shop mới!");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải danh sách shop: {ex.Message}");
                }
            }
        }

        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string orderId = btn.CommandArgument;

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
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
                        ShowAlert("Đã xác nhận đơn hàng thành công!");
                    }
                    else
                    {
                        ShowAlert("Không thể xác nhận đơn hàng. Đơn hàng có thể đã được xử lý trước đó!");
                    }

                    LoadOrders();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi xác nhận đơn hàng: {ex.Message}");
                }
            }
        }

        protected void btnCancelOrder_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string orderId = btn.CommandArgument;

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
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
                        ShowAlert("Đã hủy đơn hàng thành công!");
                    }
                    else
                    {
                        ShowAlert("Không thể hủy đơn hàng. Đơn hàng có thể đã được xử lý trước đó!");
                    }

                    LoadOrders();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi hủy đơn hàng: {ex.Message}");
                }
            }
        }

        protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditProduct.aspx?mode=add");
        }

        protected void btnEditProduct_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;
            Response.Redirect($"AddEditProduct.aspx?mode=edit&id={productId}");
        }

        protected void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string productId = btn.CommandArgument;

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
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

                    DeleteProduct(productId);
                    ShowAlert("Xóa sản phẩm thành công!");
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi xóa sản phẩm: {ex.Message}");
                }
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditUser.aspx?mode=add");
        }

        protected void btnEditUser_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string userId = btn.CommandArgument;

            if (string.IsNullOrEmpty(userId))
            {
                ShowAlert("ID người dùng không được để trống!");
                return;
            }

            Response.Redirect($"AddEditUser.aspx?mode=edit&id={userId}");
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string userId = btn.CommandArgument;

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
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

                    DeleteUser(userId);
                    ShowAlert("Xóa người dùng thành công!");
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi xóa người dùng: {ex.Message}");
                }
            }
        }

        protected void btnAddShop_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddEditShop.aspx?mode=add");
        }

        protected void btnEditShop_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string shopId = btn.CommandArgument;
            if (!string.IsNullOrEmpty(shopId))
            {
                Response.Redirect($"AddEditShop.aspx?mode=edit&id={shopId}");
            }
            else
            {
                ShowAlert("ID shop không hợp lệ!");
            }
        }

        private void DeleteProduct(string productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Products WHERE ProductId = @ProductId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa sản phẩm: {ex.Message}");
                }
            }
        }

        private void DeleteUser(string userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Users WHERE UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa người dùng: {ex.Message}");
                }
            }
        }

        protected void btnPrevProducts_Click(object sender, EventArgs e)
        {
            if (productPage > 1)
            {
                productPage--;
                LoadProducts();
            }
        }

        protected void btnNextProducts_Click(object sender, EventArgs e)
        {
            productPage++;
            LoadProducts();
        }

        protected void btnPrevUsers_Click(object sender, EventArgs e)
        {
            if (userPage > 1)
            {
                userPage--;
                LoadUsers();
            }
        }

        protected void btnNextUsers_Click(object sender, EventArgs e)
        {
            userPage++;
            LoadUsers();
        }

        protected void btnPrevOrders_Click(object sender, EventArgs e)
        {
            if (orderPage > 1)
            {
                orderPage--;
                LoadOrders();
            }
        }

        protected void btnNextOrders_Click(object sender, EventArgs e)
        {
            orderPage++;
            LoadOrders();
        }

        protected void btnPrevShops_Click(object sender, EventArgs e)
        {
            if (shopPage > 1)
            {
                shopPage--;
                LoadShops();
            }
        }

        protected void btnNextShops_Click(object sender, EventArgs e)
        {
            shopPage++;
            LoadShops();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}