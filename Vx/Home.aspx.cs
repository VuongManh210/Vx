using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class Home : System.Web.UI.Page
    {
        private int currentPage
        {
            get { return ViewState["CurrentPage"] != null ? (int)ViewState["CurrentPage"] : 1; }
            set { ViewState["CurrentPage"] = value; }
        }
        private const int pageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null || Session["Role"] == null)
                {
                    Response.Redirect("Login.aspx");
                }

                if (Session["Username"] != null)
                {
                    lblUsername.Text = Session["Username"].ToString();
                }

                LoadCategories(); // Tải danh mục từ Categories
                LoadProducts("", "all", currentPage); // Tải tất cả sản phẩm
            }
        }

        private void LoadCategories()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT CategoryId, CategoryName FROM Categories";
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlCategory.DataSource = dt;
                    ddlCategory.DataTextField = "CategoryName"; // Hiển thị tên danh mục
                    ddlCategory.DataValueField = "CategoryId";  // Giá trị là CategoryId
                    ddlCategory.DataBind();

                    // Thêm "Tất cả" vào đầu danh sách
                    ddlCategory.Items.Insert(0, new ListItem("Tất cả", "all"));
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải danh mục: {ex.Message}");
                }
            }
        }

        private void LoadProducts(string searchTerm, string category, int page)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT ProductId AS ID, ProductName AS TenSanPham, Price AS Gia, ImageUrl AS HinhAnh
                    FROM Products
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND ProductName LIKE @SearchTerm";
                }
                else if (category != "all")
                {
                    query += " AND CategoryId = @Category";
                }
                query += " ORDER BY ProductId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                }
                else if (category != "all")
                {
                    cmd.Parameters.AddWithValue("@Category", Convert.ToInt32(category));
                }
                cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptSanPham.DataSource = dt;
                    rptSanPham.DataBind();

                    btnPrevious.Enabled = page > 1;
                    btnNext.Enabled = dt.Rows.Count == pageSize;

                    currentPage = page;

                    if (dt.Rows.Count == 0)
                    {
                        ShowAlert("Không có sản phẩm nào phù hợp!");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            string searchTerm = txtSearch.Text.Trim();
            LoadProducts(searchTerm, "all", currentPage);
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            txtSearch.Text = ""; // Xóa TextBox khi chọn danh mục
            string category = ddlCategory.SelectedValue;
            LoadProducts("", category, currentPage);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            string searchTerm = txtSearch.Text.Trim();
            LoadProducts(searchTerm, "all", currentPage);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                string searchTerm = txtSearch.Text.Trim();
                string category = string.IsNullOrEmpty(searchTerm) ? ddlCategory.SelectedValue : "all";
                LoadProducts(searchTerm, category, currentPage);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            string searchTerm = txtSearch.Text.Trim();
            string category = string.IsNullOrEmpty(searchTerm) ? ddlCategory.SelectedValue : "all";
            LoadProducts(searchTerm, category, currentPage);
        }

        protected void btnThemVaoGioHang_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                string userId = Session["UserId"].ToString();
                int quantity = 1;

                AddToCart(userId, productId, quantity);
                ShowAlert("Đã thêm sản phẩm vào giỏ hàng!");
            }
        }

        private void AddToCart(string userId, int productId, int quantity)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string checkQuery = "SELECT Quantity FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    conn.Open();
                    object result = checkCmd.ExecuteScalar();
                    int newQuantity;

                    if (result != null)
                    {
                        int currentQuantity = Convert.ToInt32(result);
                        newQuantity = currentQuantity + quantity;
                        string updateQuery = "UPDATE Cart SET Quantity = @Quantity WHERE UserId = @UserId AND ProductId = @ProductId";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@Quantity", newQuantity);
                        updateCmd.Parameters.AddWithValue("@UserId", userId);
                        updateCmd.Parameters.AddWithValue("@ProductId", productId);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        newQuantity = quantity;
                        string insertQuery = "INSERT INTO Cart (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@UserId", userId);
                        insertCmd.Parameters.AddWithValue("@ProductId", productId);
                        insertCmd.Parameters.AddWithValue("@Quantity", newQuantity);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi thêm vào giỏ hàng: {ex.Message}");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            ShowAlertAndRedirect("Đăng xuất thành công! Chuyển đến trang đăng nhập trong 2 giây...", "Login.aspx", 2000);
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }

        private void ShowAlertAndRedirect(string message, string url, int milliseconds)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alertAndRedirect",
                $"alert('{message}'); setTimeout(function(){{window.location.href='{url}';}}, {milliseconds});", true);
        }
    }
}