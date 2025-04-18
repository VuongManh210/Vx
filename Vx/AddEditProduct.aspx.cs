using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class AddEditProduct : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Sản Phẩm";
                    LoadProduct(Request.QueryString["id"]);
                }
                else
                {
                    lblTitle.Text = "Thêm Sản Phẩm";
                }
            }
        }

        private void LoadCategories()
        {
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
                    string query = "SELECT CategoryId, CategoryName FROM Categories";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                    ddlCategory.DataSource = dt;
                    ddlCategory.DataTextField = "CategoryName";
                    ddlCategory.DataValueField = "CategoryId";
                    ddlCategory.DataBind();
                    ddlCategory.Items.Insert(0, new ListItem("-- Chọn Danh Mục --", ""));
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải danh mục: {ex.Message}");
                }
            }
        }

        private void LoadProduct(string productId)
        {
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
                    string query = @"
                        SELECT ProductName, Price, CategoryId, ImageUrl, Description, Stock 
                        FROM Products 
                        WHERE ProductId = @ProductId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtProductName.Text = reader["ProductName"].ToString();
                        txtPrice.Text = reader["Price"].ToString();
                        ddlCategory.SelectedValue = reader["CategoryId"].ToString();
                        txtImageUrl.Text = reader["ImageUrl"].ToString();
                        txtDescription.Text = reader["Description"].ToString();
                        txtStock.Text = reader["Stock"].ToString();
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy sản phẩm!");
                        Response.Redirect("AdminDashboard.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải thông tin sản phẩm: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra hợp lệ dữ liệu
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || string.IsNullOrWhiteSpace(ddlCategory.SelectedValue))
            {
                ShowAlert("Tên sản phẩm, giá và danh mục không được để trống!");
                return;
            }

            if (!int.TryParse(txtPrice.Text, out int price) || price < 0)
            {
                ShowAlert("Giá phải là số không âm!");
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                ShowAlert("Số lượng tồn kho phải là số không âm!");
                return;
            }

            string mode = Request.QueryString["mode"];
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
                    string query = mode == "add" ?
                        @"INSERT INTO Products (ProductId, ProductName, Price, CategoryId, ImageUrl, Description, Stock, CreatedDate, ShopId) 
                          VALUES (@ProductId, @ProductName, @Price, @CategoryId, @ImageUrl, @Description, @Stock, @CreatedDate, @ShopId)" :
                        @"UPDATE Products 
                          SET ProductName = @ProductName, Price = @Price, CategoryId = @CategoryId, ImageUrl = @ImageUrl, 
                              Description = @Description, Stock = @Stock 
                          WHERE ProductId = @ProductId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    string productId = mode == "add" ? Guid.NewGuid().ToString() : Request.QueryString["id"];
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@ProductName", txtProductName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@CategoryId", ddlCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@ImageUrl", txtImageUrl.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Stock", stock);
                    if (mode == "add")
                    {
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ShopId", Session["UserId"].ToString()); // ShopId là UserId của ShopOwner
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    ShowAlert(mode == "add" ? "Thêm sản phẩm thành công!" : "Cập nhật sản phẩm thành công!");
                    Response.Redirect("AdminDashboard.aspx");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi lưu thông tin sản phẩm: {ex.Message}");
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