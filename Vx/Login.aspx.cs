using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Vx
{
    public partial class Login : System.Web.UI.Page
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

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowAlert("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "SELECT UserId, Username, Role FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", password); // Trong thực tế, nên mã hóa password

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Session["UserId"] = reader["UserId"].ToString();
                                Session["Username"] = reader["Username"].ToString();
                                Session["Role"] = reader["Role"].ToString();

                                // Kiểm tra vai trò và chuyển hướng phù hợp
                                string role = Session["Role"].ToString();
                                string redirectUrl = role == "Admin" ? "AdminDashboard.aspx" : "Home.aspx";
                                ShowAlertAndRedirect("Đăng nhập thành công! Chuyển đến trang chính trong 2 giây...", redirectUrl, 2000);
                            }
                            else
                            {
                                ShowAlert("Tên đăng nhập hoặc mật khẩu không đúng!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Có lỗi xảy ra: " + ex.Message);
            }
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