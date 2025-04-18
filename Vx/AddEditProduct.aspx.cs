using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class AddEditProduct : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
        private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra đăng nhập và quyền
                if (Session["UserId"] == null || Session["Role"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để tiếp tục!");
                    Response.Redirect("Login.aspx");
                    return;
                }

                string role = Session["Role"].ToString();
                if (role != "Admin" && role != "ShopOwner")
                {
                    ShowAlert("Bạn không có quyền truy cập trang này!");
                    Response.Redirect("Home.aspx");
                    return;
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB'!");
                    return;
                }

                LoadCategories();
                LoadShops();

                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Sản Phẩm";
                    if (!int.TryParse(Request.QueryString["id"], out int productId))
                    {
                        ShowAlert("ID sản phẩm không hợp lệ!");
                        Response.Redirect("AdminDashboard.aspx");
                        return;
                    }
                    LoadProduct(productId);
                }
                else
                {
                    lblTitle.Text = "Thêm Sản Phẩm";
                    // Nếu là ShopOwner, tự động chọn ShopId và vô hiệu hóa dropdown
                    if (role == "ShopOwner")
                    {
                        string userId = Session["UserId"].ToString();
                        string shopId = GetShopIdByUserId(userId);
                        if (!string.IsNullOrEmpty(shopId))
                        {
                            ddlShop.SelectedValue = shopId;
                            ddlShop.Enabled = false;
                        }
                        else
                        {
                            ShowAlert("Không tìm thấy cửa hàng liên kết với tài khoản của bạn!");
                            Response.Redirect("AdminDashboard.aspx");
                        }
                    }
                }
            }
        }

        private void LoadCategories()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlCategory.DataSource = dt;
                    ddlCategory.DataTextField = "CategoryName";
                    ddlCategory.DataValueField = "CategoryId";
                    ddlCategory.DataBind();
                    ddlCategory.Items.Insert(0, new ListItem("-- Chọn Danh Mục --", ""));
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải danh mục: {ex.Message}");
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
                    string query = "SELECT ShopId, ShopName FROM Shops ORDER BY ShopName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlShop.DataSource = dt;
                    ddlShop.DataTextField = "ShopName";
                    ddlShop.DataValueField = "ShopId";
                    ddlShop.DataBind();
                    ddlShop.Items.Insert(0, new ListItem("-- Chọn Cửa Hàng --", ""));
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải danh sách cửa hàng: {ex.Message}");
                }
            }
        }

        private string GetShopIdByUserId(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ShopId FROM Users WHERE UserId = @UserId AND Role = 'ShopOwner'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi lấy ShopId: {ex.Message}");
                    return null;
                }
            }
        }

        private void LoadProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT ProductName, Price, CategoryId, ShopId, ImageUrl, Description, Stock 
                        FROM Products 
                        WHERE ProductId = @ProductId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtProductName.Text = reader["ProductName"].ToString();
                        txtPrice.Text = reader["Price"].ToString();
                        ddlCategory.SelectedValue = reader["CategoryId"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("ShopId")))
                        {
                            ddlShop.SelectedValue = reader["ShopId"].ToString();
                        }
                        string imageUrl = reader["ImageUrl"].ToString();
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            lblCurrentImage.Visible = true;
                            imgCurrent.Visible = true;
                            imgCurrent.ImageUrl = "~/" + imageUrl; // Ví dụ: ~/images/laptop_dell_xps_13.jpg
                        }
                        txtDescription.Text = reader["Description"].ToString();
                        txtStock.Text = reader["Stock"].ToString();

                        // Nếu là ShopOwner, kiểm tra quyền chỉnh sửa
                        if (Session["Role"].ToString() == "ShopOwner")
                        {
                            string userShopId = GetShopIdByUserId(Session["UserId"].ToString());
                            if (userShopId != ddlShop.SelectedValue)
                            {
                                ShowAlert("Bạn chỉ có thể chỉnh sửa sản phẩm thuộc cửa hàng của mình!");
                                Response.Redirect("AdminDashboard.aspx");
                            }
                            ddlShop.Enabled = false;
                        }
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy sản phẩm!");
                        Response.Redirect("AdminDashboard.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải thông tin sản phẩm: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra hợp lệ dữ liệu
            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(ddlCategory.SelectedValue))
            {
                ShowAlert("Tên sản phẩm, giá và danh mục không được để trống!");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                ShowAlert("Giá phải là số không âm!");
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                ShowAlert("Số lượng tồn kho phải là số không âm!");
                return;
            }

            // Kiểm tra file upload
            string imageUrl = null;
            if (fuImage.HasFile)
            {
                string fileExtension = Path.GetExtension(fuImage.FileName).ToLower();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    ShowAlert("Chỉ hỗ trợ file .jpg, .jpeg, .png!");
                    return;
                }

                if (fuImage.PostedFile.ContentLength > MaxFileSize)
                {
                    ShowAlert("Kích thước file không được vượt quá 5MB!");
                    return;
                }

                try
                {
                    // Giữ tên file gốc, thêm GUID để tránh trùng
                    string originalFileName = Path.GetFileNameWithoutExtension(fuImage.FileName);
                    string fileName = $"{Guid.NewGuid().ToString()}_{originalFileName}{fileExtension}";
                    string savePath = Path.Combine(Server.MapPath("~/images/"), fileName);
                    fuImage.SaveAs(savePath);
                    imageUrl = $"images/{fileName}"; // Ví dụ: images/abc123_laptop_dell_xps_13.jpg
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi lưu hình ảnh: {ex.Message}");
                    return;
                }
            }
            else if (Request.QueryString["mode"] == "add")
            {
                ShowAlert("Vui lòng chọn hình ảnh cho sản phẩm mới!");
                return;
            }

            string mode = Request.QueryString["mode"];
            int? shopId = null;
            if (!string.IsNullOrEmpty(ddlShop.SelectedValue))
            {
                if (!int.TryParse(ddlShop.SelectedValue, out int parsedShopId))
                {
                    ShowAlert("Cửa hàng không hợp lệ!");
                    return;
                }
                shopId = parsedShopId;
            }

            // Kiểm tra quyền của ShopOwner
            if (Session["Role"].ToString() == "ShopOwner")
            {
                string userShopId = GetShopIdByUserId(Session["UserId"].ToString());
                if (shopId.ToString() != userShopId)
                {
                    ShowAlert("Bạn chỉ có thể thêm/sửa sản phẩm cho cửa hàng của mình!");
                    return;
                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (mode == "add")
                    {
                        query = @"
                            INSERT INTO Products (ProductName, Price, CategoryId, ShopId, ImageUrl, Description, Stock, CreatedDate)
                            VALUES (@ProductName, @Price, @CategoryId, @ShopId, @ImageUrl, @Description, @Stock, @CreatedDate)";
                        cmd = new SqlCommand(query, conn);
                    }
                    else
                    {
                        if (!int.TryParse(Request.QueryString["id"], out int productId))
                        {
                            ShowAlert("ID sản phẩm không hợp lệ!");
                            return;
                        }
                        query = @"
                            UPDATE Products 
                            SET ProductName = @ProductName, Price = @Price, CategoryId = @CategoryId, ShopId = @ShopId, 
                                ImageUrl = @ImageUrl, Description = @Description, Stock = @Stock
                            WHERE ProductId = @ProductId";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        // Nếu không upload ảnh mới, giữ ảnh cũ
                        if (!fuImage.HasFile)
                        {
                            string currentImage = imgCurrent.ImageUrl?.Replace("~/", "");
                            imageUrl = string.IsNullOrEmpty(currentImage) ? null : currentImage;
                        }
                    }

                    cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@CategoryId", ddlCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@ShopId", shopId.HasValue ? (object)shopId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageUrl", (object)imageUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    if (mode == "add")
                    {
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }

                    cmd.ExecuteNonQuery();

                    ShowAlert(mode == "add" ? "Thêm sản phẩm thành công!" : "Cập nhật sản phẩm thành công!");
                    Response.Redirect("AdminDashboard.aspx");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi lưu sản phẩm: {ex.Message}");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}