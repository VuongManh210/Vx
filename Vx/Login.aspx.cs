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
                    string returnUrl = Request.QueryString["ReturnUrl"] ?? "Home.aspx";
                    Response.Redirect(returnUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
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
                        cmd.Parameters.AddWithValue("@PasswordHash", password); // Văn bản thuần, dựa trên dữ liệu mẫu

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Session["UserId"] = reader["UserId"].ToString();
                                Session["Username"] = reader["Username"].ToString();
                                Session["Role"] = reader["Role"].ToString();

                                string role = Session["Role"].ToString();
                                string redirectUrl = role == "Admin" ? "AdminDashboard.aspx" :
                                    (Request.QueryString["ReturnUrl"] ?? "Home.aspx");

                                ShowAlert("Đăng nhập thành công!");
                                Response.Redirect(redirectUrl, false);
                                Context.ApplicationInstance.CompleteRequest();
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
    }
}