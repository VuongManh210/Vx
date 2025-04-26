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

        private string SortOrder
        {
            get { return ViewState["SortOrder"] != null ? ViewState["SortOrder"].ToString() : "default"; }
            set { ViewState["SortOrder"] = value; }
        }

        private const int pageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Debug Session để kiểm tra trạng thái đăng nhập
            System.Diagnostics.Debug.WriteLine($"Page_Load Start - Session[UserId]: {(Session["UserId"] != null ? Session["UserId"].ToString() : "null")}");
            System.Diagnostics.Debug.WriteLine($"Page_Load Start - hdnIsLoggedIn: {hdnIsLoggedIn.Value}");

            // Cập nhật trạng thái đăng nhập
            hdnIsLoggedIn.Value = (Session["UserId"] != null).ToString();
            lblUsername.Text = Session["Username"] != null ? Session["Username"].ToString() : "Khách";
            btnLogout.Visible = Session["UserId"] != null;
            lnkLogin.Visible = Session["UserId"] == null;

            System.Diagnostics.Debug.WriteLine($"Page_Load End - Session[UserId]: {(Session["UserId"] != null ? Session["UserId"].ToString() : "null")}");
            System.Diagnostics.Debug.WriteLine($"Page_Load End - hdnIsLoggedIn: {hdnIsLoggedIn.Value}");

            if (!IsPostBack)
            {
                LoadCategories();
                LoadProducts("", "all", currentPage, SortOrder);
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
                    ddlCategory.DataTextField = "CategoryName";
                    ddlCategory.DataValueField = "CategoryId";
                    ddlCategory.DataBind();

                    ddlCategory.Items.Insert(0, new ListItem("Tất cả", "all"));
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải danh mục: {ex.Message}");
                }
            }
        }

        private void LoadProducts(string searchTerm, string category, int page, string sortOrder)
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

                if (sortOrder == "asc")
                {
                    query += " ORDER BY Price ASC";
                }
                else if (sortOrder == "desc")
                {
                    query += " ORDER BY Price DESC";
                }
                else
                {
                    query += " ORDER BY ProductId";
                }

                query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

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
            LoadProducts(searchTerm, "all", currentPage, SortOrder);
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            txtSearch.Text = "";
            string category = ddlCategory.SelectedValue;
            LoadProducts("", category, currentPage, SortOrder);
        }

        protected void ddlSortPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            SortOrder = ddlSortPrice.SelectedValue;
            string searchTerm = txtSearch.Text.Trim();
            string category = string.IsNullOrEmpty(searchTerm) ? ddlCategory.SelectedValue : "all";
            LoadProducts(searchTerm, category, currentPage, SortOrder);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            string searchTerm = txtSearch.Text.Trim();
            LoadProducts(searchTerm, "all", currentPage, SortOrder);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                string searchTerm = txtSearch.Text.Trim();
                string category = string.IsNullOrEmpty(searchTerm) ? ddlCategory.SelectedValue : "all";
                LoadProducts(searchTerm, category, currentPage, SortOrder);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            string searchTerm = txtSearch.Text.Trim();
            string category = string.IsNullOrEmpty(searchTerm) ? ddlCategory.SelectedValue : "all";
            LoadProducts(searchTerm, category, currentPage, SortOrder);
        }

        protected void btnThemVaoGioHang_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "AddToCart")
            {
                int productId = Convert.ToInt32(e.CommandArgument);
                int quantity = 1;

                // Kiểm tra đăng nhập
                if (Session["UserId"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để thêm vào giỏ hàng!");
                    Response.Redirect("Login.aspx?ReturnUrl=Home.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Kiểm tra tồn kho
                int latestStock = GetLatestStock(productId);
                if (quantity > latestStock)
                {
                    ShowAlert($"Sản phẩm này chỉ còn {latestStock} trong kho!");
                    return;
                }

                // Thêm vào giỏ hàng trong cơ sở dữ liệu
                string userId = Session["UserId"].ToString();
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
                        int latestStock = GetLatestStock(productId);
                        if (newQuantity > latestStock)
                        {
                            ShowAlert($"Số lượng vượt quá tồn kho ({latestStock})!");
                            newQuantity = latestStock;
                        }
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

        private int GetLatestStock(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Stock FROM Products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi kiểm tra tồn kho: {ex.Message}");
                    return 0;
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            ShowAlert("Đăng xuất thành công!");
            Response.Redirect("Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}