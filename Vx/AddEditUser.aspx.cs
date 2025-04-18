using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Vx
{
    public partial class AddEditUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Người Dùng";
                    LoadUser(Request.QueryString["id"]);
                }
                else
                {
                    lblTitle.Text = "Thêm Người Dùng";
                }
            }
        }

        private void LoadUser(string userId)
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
                        SELECT Username, PasswordHash, FullName, Email, Phone, Address, Role 
                        FROM Users 
                        WHERE UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtUsername.Text = reader["Username"].ToString();
                        txtPassword.Text = reader["PasswordHash"].ToString();
                        txtFullName.Text = reader["FullName"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        txtPhone.Text = reader["Phone"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        ddlRole.SelectedValue = reader["Role"].ToString();

                        // Nếu là ShopOwner, hiển thị thêm các trường thông tin shop
                        if (ddlRole.SelectedValue == "ShopOwner")
                        {
                            LoadShopDetails(userId);
                            divShopDetails.Visible = true;
                        }
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy người dùng!");
                        Response.Redirect("AdminDashboard.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải thông tin người dùng: {ex.Message}");
                }
            }
        }

        private void LoadShopDetails(string userId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string query = "SELECT ShopName, Description, Address, PhoneNumber FROM Shops WHERE ShopId = @ShopId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ShopId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtShopName.Text = reader["ShopName"].ToString();
                        txtShopDescription.Text = reader["Description"].ToString();
                        txtShopAddress.Text = reader["Address"].ToString();
                        txtShopPhone.Text = reader["PhoneNumber"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải thông tin shop: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra hợp lệ dữ liệu
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowAlert("Tên đăng nhập và mật khẩu không được để trống!");
                return;
            }

            if (ddlRole.SelectedValue == "ShopOwner" && string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                ShowAlert("Tên shop không được để trống khi tạo ShopOwner!");
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
                        @"INSERT INTO Users (UserId, Username, PasswordHash, FullName, Email, Phone, Address, Role, CreatedDate) 
                          VALUES (@UserId, @Username, @PasswordHash, @FullName, @Email, @Phone, @Address, @Role, @CreatedDate)" :
                        @"UPDATE Users 
                          SET Username = @Username, PasswordHash = @PasswordHash, FullName = @FullName, 
                              Email = @Email, Phone = @Phone, Address = @Address, Role = @Role 
                          WHERE UserId = @UserId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    string userId = mode == "add" ? Guid.NewGuid().ToString() : Request.QueryString["id"];
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@PasswordHash", txtPassword.Text.Trim()); // Nên mã hóa trong thực tế
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@Role", ddlRole.SelectedValue);
                    if (mode == "add")
                    {
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Nếu là ShopOwner, thêm hoặc cập nhật thông tin shop
                    if (ddlRole.SelectedValue == "ShopOwner")
                    {
                        if (mode == "add")
                        {
                            CreateShopForUser(userId, txtShopName.Text.Trim(), txtShopDescription.Text.Trim(), txtShopAddress.Text.Trim(), txtShopPhone.Text.Trim());
                        }
                        else
                        {
                            UpdateShopForUser(userId, txtShopName.Text.Trim(), txtShopDescription.Text.Trim(), txtShopAddress.Text.Trim(), txtShopPhone.Text.Trim());
                        }
                    }

                    ShowAlert(mode == "add" ? "Thêm người dùng thành công!" : "Cập nhật người dùng thành công!");
                    Response.Redirect("AdminDashboard.aspx");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi lưu thông tin người dùng: {ex.Message}");
                }
            }
        }

        private void CreateShopForUser(string userId, string shopName, string description, string address, string phoneNumber)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string query = @"
                        INSERT INTO Shops (ShopId, ShopName, Description, Address, PhoneNumber, CreatedDate)
                        VALUES (@ShopId, @ShopName, @Description, @Address, @PhoneNumber, @CreatedDate)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ShopId", userId); // ShopId trùng với UserId
                    cmd.Parameters.AddWithValue("@ShopName", shopName);
                    cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi tạo shop: {ex.Message}");
                }
            }
        }

        private void UpdateShopForUser(string userId, string shopName, string description, string address, string phoneNumber)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string query = @"
                        UPDATE Shops 
                        SET ShopName = @ShopName, Description = @Description, Address = @Address, PhoneNumber = @PhoneNumber
                        WHERE ShopId = @ShopId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ShopId", userId);
                    cmd.Parameters.AddWithValue("@ShopName", shopName);
                    cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber ?? (object)DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật shop: {ex.Message}");
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

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            divShopDetails.Visible = ddlRole.SelectedValue == "ShopOwner";
        }
    }
}