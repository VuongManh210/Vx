using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class AddEditUser : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

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

                if (Session["Role"].ToString() != "Admin")
                {
                    ShowAlert("Chỉ Admin mới có quyền truy cập trang này!");
                    Response.Redirect("AdminDashboard.aspx");
                    return;
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình chuỗi kết nối 'MyDB'!");
                    return;
                }

                LoadShops();

                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Người Dùng";
                    txtUsername.ReadOnly = true; // Chỉ đọc Username
                    if (!int.TryParse(Request.QueryString["id"], out int userId))
                    {
                        ShowAlert("ID người dùng không hợp lệ!");
                        Response.Redirect("AdminDashboard.aspx");
                        return;
                    }
                    LoadUser(userId);
                }
                else
                {
                    lblTitle.Text = "Thêm Người Dùng";
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

        private void LoadUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT Username, FullName, Email, Role, ShopId
                        FROM Users 
                        WHERE UserId = @UserId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtUsername.Text = reader["Username"].ToString();
                        txtFullName.Text = reader["FullName"].ToString();
                        txtEmail.Text = reader["Email"].ToString();
                        ddlRole.SelectedValue = reader["Role"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("ShopId")))
                        {
                            ddlShop.SelectedValue = reader["ShopId"].ToString();
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
                    ShowAlert($"Lỗi khi tải thông tin người dùng: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra hợp lệ dữ liệu
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(ddlRole.SelectedValue))
            {
                ShowAlert("Vui lòng điền đầy đủ các trường bắt buộc (Họ Tên, Email, Vai Trò)!");
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                ShowAlert("Email không hợp lệ!");
                return;
            }

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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (Request.QueryString["mode"] == "add")
                    {
                        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            ShowAlert("Tên đăng nhập và mật khẩu không được để trống khi thêm người dùng!");
                            return;
                        }

                        // Kiểm tra username đã tồn tại
                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            ShowAlert("Tên đăng nhập đã tồn tại!");
                            return;
                        }

                        query = @"
                            INSERT INTO Users (Username, Password, FullName, Email, Role, ShopId)
                            VALUES (@Username, @Password, @FullName, @Email, @Role, @ShopId)";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text); // Nên mã hóa mật khẩu
                    }
                    else
                    {
                        if (!int.TryParse(Request.QueryString["id"], out int userId))
                        {
                            ShowAlert("ID người dùng không hợp lệ!");
                            return;
                        }
                        query = @"
                            UPDATE Users 
                            SET FullName = @FullName, 
                                Email = @Email, 
                                Role = @Role, 
                                ShopId = @ShopId";
                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            query += ", Password = @Password";
                        }
                        query += " WHERE UserId = @UserId";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            cmd.Parameters.AddWithValue("@Password", txtPassword.Text); // Nên mã hóa mật khẩu
                        }
                    }

                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Role", ddlRole.SelectedValue);
                    cmd.Parameters.AddWithValue("@ShopId", shopId.HasValue ? (object)shopId.Value : DBNull.Value);

                    cmd.ExecuteNonQuery();

                    ShowAlert(Request.QueryString["mode"] == "add" ? "Thêm người dùng thành công!" : "Cập nhật người dùng thành công!");
                    Response.Redirect("AdminDashboard.aspx");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi lưu người dùng: {ex.Message}");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}