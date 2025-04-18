using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Vx
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] != null)
                {
                    Response.Redirect("Home.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowAlert("Tên đăng nhập và mật khẩu là bắt buộc!");
                return;
            }

            if (password != confirmPassword)
            {
                ShowAlert("Mật khẩu xác nhận không khớp!");
                return;
            }

            // Kiểm tra định dạng email nếu có nhập
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                ShowAlert("Email không đúng định dạng!");
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Kiểm tra trùng Username
                    string checkUsernameQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SqlCommand checkUsernameCmd = new SqlCommand(checkUsernameQuery, conn))
                    {
                        checkUsernameCmd.Parameters.AddWithValue("@Username", username);
                        int usernameCount = (int)checkUsernameCmd.ExecuteScalar();
                        if (usernameCount > 0)
                        {
                            ShowAlert("Tên đăng nhập đã tồn tại!");
                            return;
                        }
                    }

                    // Kiểm tra trùng Email (nếu có nhập)
                    if (!string.IsNullOrEmpty(email))
                    {
                        string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                        using (SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, conn))
                        {
                            checkEmailCmd.Parameters.AddWithValue("@Email", email);
                            int emailCount = (int)checkEmailCmd.ExecuteScalar();
                            if (emailCount > 0)
                            {
                                ShowAlert("Email đã được sử dụng!");
                                return;
                            }
                        }
                    }

                    // Thêm người dùng mới
                    string insertQuery = "INSERT INTO Users (UserId, Username, PasswordHash, FullName, Email, Phone, Role, CreatedDate) " +
                                        "VALUES (@UserId, @Username, @PasswordHash, @FullName, @Email, @Phone, @Role, @CreatedDate)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", password); // Trong thực tế, nên mã hóa password
                        cmd.Parameters.AddWithValue("@FullName", string.IsNullOrEmpty(fullName) ? (object)DBNull.Value : fullName);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                        cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone);
                        cmd.Parameters.AddWithValue("@Role", "User");
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        cmd.ExecuteNonQuery();
                        ShowAlertAndRedirect("Đăng ký thành công! Chuyển đến trang đăng nhập trong 3 giây...", "Login.aspx", 3000);
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Biểu thức chính quy kiểm tra định dạng email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
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

        private void ClearForm()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
        }
    }
}