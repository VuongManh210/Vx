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
                if (Session["UserId"] == null || Session["Role"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để tiếp tục!");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                if (Session["Role"].ToString() != "Admin")
                {
                    ShowAlert("Chỉ Admin mới có quyền truy cập trang này!");
                    Response.Redirect("AdminDashboard.aspx", false);
                    return;
                }

                if (string.IsNullOrEmpty(connectionString))
                {
                    ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra chuỗi kết nối 'MyDB'!");
                    return;
                }

                LoadShops();

                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Người Dùng";
                    string userId = Request.QueryString["id"];
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
                    ddlShop.Items.Insert(0, new ListItem("-- Chọn Cửa Hàng (Tùy chọn) --", ""));
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi tải danh sách cửa hàng: {ex.Message}";
                }
            }
        }

        private void LoadUser(string userId)
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
                        Response.Redirect("AdminDashboard.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi tải thông tin người dùng: {ex.Message}";
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    lblMessage.Text = "Vui lòng điền đầy đủ Tên đăng nhập, Họ Tên và Email!";
                    return;
                }

                if (!IsValidEmail(txtEmail.Text))
                {
                    lblMessage.Text = "Email không hợp lệ!";
                    return;
                }

                int? shopId = null;
                if (!string.IsNullOrEmpty(ddlShop.SelectedValue))
                {
                    if (!int.TryParse(ddlShop.SelectedValue, out int parsedShopId))
                    {
                        lblMessage.Text = "Cửa hàng không hợp lệ!";
                        return;
                    }
                    shopId = parsedShopId;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string checkQuery = "SELECT COUNT(*) FROM Shops WHERE ShopId = @ShopId";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@ShopId", shopId);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count == 0)
                        {
                            lblMessage.Text = "Cửa hàng được chọn không tồn tại!";
                            return;
                        }
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (Request.QueryString["mode"] == "add")
                    {
                        if (string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            lblMessage.Text = "Mật khẩu không được để trống khi thêm người dùng!";
                            return;
                        }

                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email = @Email";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblMessage.Text = "Tên đăng nhập hoặc Email đã tồn tại!";
                            return;
                        }

                        string userId = Guid.NewGuid().ToString(); // Tạo UserId ngẫu nhiên
                        query = @"
                            INSERT INTO Users (UserId, Username, PasswordHash, FullName, Email, Role, ShopId)
                            VALUES (@UserId, @Username, @PasswordHash, @FullName, @Email, @Role, @ShopId)";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@PasswordHash", txtPassword.Text); // Nên mã hóa mật khẩu
                    }
                    else
                    {
                        string userId = Request.QueryString["id"];
                        if (string.IsNullOrEmpty(userId))
                        {
                            lblMessage.Text = "ID người dùng không hợp lệ!";
                            return;
                        }

                        string checkQuery = "SELECT COUNT(*) FROM Users WHERE (Username = @Username OR Email = @Email) AND UserId != @UserId";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            lblMessage.Text = "Tên đăng nhập hoặc Email đã tồn tại!";
                            return;
                        }

                        query = @"
                            UPDATE Users 
                            SET Username = @Username, 
                                FullName = @FullName, 
                                Email = @Email, 
                                Role = @Role, 
                                ShopId = @ShopId";
                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            query += ", PasswordHash = @PasswordHash";
                        }
                        query += " WHERE UserId = @UserId";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            cmd.Parameters.AddWithValue("@PasswordHash", txtPassword.Text); // Nên mã hóa mật khẩu
                        }
                    }

                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Role", ddlRole.SelectedValue);
                    cmd.Parameters.AddWithValue("@ShopId", shopId.HasValue ? (object)shopId.Value : DBNull.Value);

                    cmd.ExecuteNonQuery();

                    ShowAlert(Request.QueryString["mode"] == "add" ? "Thêm người dùng thành công!" : "Cập nhật người dùng thành công!");
                    Response.Redirect("AdminDashboard.aspx", false);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Lỗi khi lưu người dùng: {ex.Message}";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx", false);
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
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}