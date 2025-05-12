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
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                string role = Session["Role"].ToString();
                if (role != "Admin")
                {
                    ShowAlert("Bạn không có quyền truy cập trang này!");
                    Response.Redirect("Home.aspx", false);
                    return;
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB'!");
                    return;
                }

                LoadCategories();

                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Sản Phẩm";
                    if (!int.TryParse(Request.QueryString["id"], out int productId))
                    {
                        ShowAlert("ID sản phẩm không hợp lệ!");
                        Response.Redirect("AdminDashboard.aspx", false);
                        return;
                    }
                    LoadProduct(productId);
                }
                else
                {
                    lblTitle.Text = "Thêm Sản Phẩm";
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

                    if (dt.Rows.Count == 0)
                    {
                        ShowAlert("Không tìm thấy danh mục nào trong cơ sở dữ liệu!");
                        return;
                    }

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

        private void LoadProduct(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT ProductName, Price, CategoryId, ImageUrl, Description, Stock 
                        FROM Products 
                        WHERE ProductId = @ProductId AND IsDeleted = 0";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtProductName.Text = reader["ProductName"].ToString();
                        txtPrice.Text = Convert.ToDecimal(reader["Price"]).ToString("F2");
                        ddlCategory.SelectedValue = reader["CategoryId"].ToString();
                        string imageUrl = reader["ImageUrl"].ToString();
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            lblCurrentImage.Visible = true;
                            imgCurrent.Visible = true;
                            imgCurrent.ImageUrl = "~/" + imageUrl; // Ví dụ: ~/images/laptop_dell_xps_13.jpg
                        }
                        txtDescription.Text = reader["Description"].ToString();
                        txtStock.Text = reader["Stock"].ToString();
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy sản phẩm!");
                        Response.Redirect("AdminDashboard.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải thông tin sản phẩm: {ex.Message}");
                }
            }
        }

        private bool IsProductNameExists(string productName, string categoryId, int? productId = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Products 
                        WHERE ProductName = @ProductName 
                        AND CategoryId = @CategoryId 
                        AND IsDeleted = 0";
                    if (productId.HasValue)
                    {
                        query += " AND ProductId != @ProductId";
                    }
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    if (productId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@ProductId", productId.Value);
                    }
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Reset thông báo lỗi
            lblProductNameError.Visible = false;
            lblPriceError.Visible = false;
            lblCategoryError.Visible = false;
            lblImageError.Visible = false;
            lblDescriptionError.Visible = false;
            lblStockError.Visible = false;

            // Kiểm tra hợp lệ dữ liệu
            bool isValid = true;
            string productName = txtProductName.Text.Trim();
            if (string.IsNullOrWhiteSpace(productName))
            {
                lblProductNameError.Text = "Tên sản phẩm không được để trống!";
                lblProductNameError.Visible = true;
                isValid = false;
            }
            else if (productName.Length > 150)
            {
                lblProductNameError.Text = "Tên sản phẩm không được vượt quá 150 ký tự!";
                lblProductNameError.Visible = true;
                isValid = false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                lblPriceError.Text = "Giá phải là số không âm!";
                lblPriceError.Visible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(ddlCategory.SelectedValue))
            {
                lblCategoryError.Text = "Vui lòng chọn danh mục!";
                lblCategoryError.Visible = true;
                isValid = false;
            }

            string description = txtDescription.Text.Trim();
            if (description.Length > 1000)
            {
                lblDescriptionError.Text = "Mô tả không được vượt quá 1000 ký tự!";
                lblDescriptionError.Visible = true;
                isValid = false;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                lblStockError.Text = "Số lượng tồn kho phải là số không âm!";
                lblStockError.Visible = true;
                isValid = false;
            }

            // Kiểm tra trùng tên sản phẩm trong cùng danh mục
            string mode = Request.QueryString["mode"];
            int? productId = null;
            if (mode == "edit" && int.TryParse(Request.QueryString["id"], out int parsedProductId))
            {
                productId = parsedProductId;
            }
            if (IsProductNameExists(productName, ddlCategory.SelectedValue, productId))
            {
                lblProductNameError.Text = "Tên sản phẩm đã tồn tại trong danh mục này!";
                lblProductNameError.Visible = true;
                isValid = false;
            }

            // Kiểm tra file upload
            string imageUrl = null;
            string oldImageUrl = null;
            if (fuImage.HasFile)
            {
                string fileExtension = Path.GetExtension(fuImage.FileName).ToLower();
                if (!AllowedExtensions.Contains(fileExtension))
                {
                    lblImageError.Text = "Chỉ hỗ trợ file .jpg, .jpeg, .png!";
                    lblImageError.Visible = true;
                    isValid = false;
                }
                else if (fuImage.PostedFile.ContentLength > MaxFileSize)
                {
                    lblImageError.Text = "Kích thước file không được vượt quá 5MB!";
                    lblImageError.Visible = true;
                    isValid = false;
                }
                else
                {
                    try
                    {
                        string originalFileName = Path.GetFileNameWithoutExtension(fuImage.FileName);
                        string fileName = $"{Guid.NewGuid().ToString()}_{originalFileName}{fileExtension}";
                        string savePath = Path.Combine(Server.MapPath("~/images/"), fileName);
                        fuImage.SaveAs(savePath);
                        imageUrl = $"images/{fileName}";

                        // Lưu đường dẫn hình ảnh cũ để xóa sau
                        if (mode == "edit" && imgCurrent.Visible)
                        {
                            oldImageUrl = imgCurrent.ImageUrl?.Replace("~/", "");
                        }
                    }
                    catch (Exception ex)
                    {
                        lblImageError.Text = $"Lỗi khi lưu hình ảnh: {ex.Message}";
                        lblImageError.Visible = true;
                        isValid = false;
                    }
                }
            }
            else if (mode == "add")
            {
                lblImageError.Text = "Vui lòng chọn hình ảnh cho sản phẩm mới!";
                lblImageError.Visible = true;
                isValid = false;
            }
            else if (mode == "edit")
            {
                imageUrl = imgCurrent.ImageUrl?.Replace("~/", "");
            }

            if (!isValid)
            {
                return;
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
                            INSERT INTO Products (ProductName, Price, CategoryId, ImageUrl, Description, Stock, CreatedDate)
                            VALUES (@ProductName, @Price, @CategoryId, @ImageUrl, @Description, @Stock, @CreatedDate)";
                        cmd = new SqlCommand(query, conn);
                    }
                    else
                    {
                        query = @"
                            UPDATE Products 
                            SET ProductName = @ProductName, Price = @Price, CategoryId = @CategoryId, 
                                ImageUrl = @ImageUrl, Description = @Description, Stock = @Stock
                            WHERE ProductId = @ProductId";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ProductId", productId.Value);
                    }

                    cmd.Parameters.AddWithValue("@ProductName", productName);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@CategoryId", ddlCategory.SelectedValue);

                    // Xử lý ImageUrl
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);
                    }

                    // Xử lý Description
                    if (string.IsNullOrEmpty(description))
                    {
                        cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Description", description);
                    }

                    cmd.Parameters.AddWithValue("@Stock", stock);
                    if (mode == "add")
                    {
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }

                    cmd.ExecuteNonQuery();

                    // Xóa hình ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(oldImageUrl) && !string.IsNullOrEmpty(imageUrl))
                    {
                        string oldImagePath = Server.MapPath($"~/{oldImageUrl}");
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    ShowAlert(mode == "add" ? "Thêm sản phẩm thành công!" : "Cập nhật sản phẩm thành công!");
                    Response.Redirect("AdminDashboard.aspx", false);
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi lưu sản phẩm: {ex.Message}");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx", false);
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}