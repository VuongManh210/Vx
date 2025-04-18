using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Vx
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Vui lòng đăng nhập để xem hồ sơ!'); window.location='Login.aspx';", true);
                    return;
                }

                LoadUserProfile();
                LoadOrderHistory();
            }
        }

        private void LoadUserProfile()
        {
            string userId = Session["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không tìm thấy UserId trong Session. Vui lòng đăng nhập lại!'); window.location='Login.aspx';", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT FullName, Email, Phone, Address FROM Users WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblAccountFullName.Text = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "Chưa cập nhật";
                        txtFullName.Text = lblAccountFullName.Text;

                        lblAccountEmail.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "Chưa cập nhật";
                        txtEmail.Text = lblAccountEmail.Text;

                        lblAccountPhone.Text = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : "Chưa cập nhật";
                        txtPhone.Text = lblAccountPhone.Text;

                        lblAccountAddress.Text = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : "Chưa cập nhật";
                        txtAddress.Text = lblAccountAddress.Text;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Không tìm thấy thông tin người dùng với UserId: {userId}. Vui lòng kiểm tra lại!');", true);
                        lblAccountFullName.Text = "N/A";
                        txtFullName.Text = "N/A";
                        lblAccountEmail.Text = "N/A";
                        txtEmail.Text = "N/A";
                        lblAccountPhone.Text = "N/A";
                        txtPhone.Text = "N/A";
                        lblAccountAddress.Text = "N/A";
                        txtAddress.Text = "N/A";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi khi tải thông tin: {ex.Message}');", true);
                    lblAccountFullName.Text = "N/A";
                    txtFullName.Text = "N/A";
                    lblAccountEmail.Text = "N/A";
                    txtEmail.Text = "N/A";
                    lblAccountPhone.Text = "N/A";
                    txtPhone.Text = "N/A";
                    lblAccountAddress.Text = "N/A";
                    txtAddress.Text = "N/A";
                }
            }
        }

        private void LoadOrderHistory()
        {
            string userId = Session["UserId"].ToString();
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT OrderId, OrderDate, TotalAmount, Status
                    FROM Orders
                    WHERE UserId = @UserId
                    ORDER BY OrderDate DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    DataTable ordersTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ordersTable);
                    }
                    rptOrders.DataSource = ordersTable;
                    rptOrders.DataBind();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Có lỗi xảy ra khi tải lịch sử đơn hàng: {ex.Message}');", true);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!'); window.location='Login.aspx';", true);
                return;
            }

            if (string.IsNullOrEmpty(txtFullName.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Họ và tên không được để trống!');", true);
                return;
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Email không được để trống!');", true);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Email không hợp lệ!');", true);
                return;
            }

            if (!string.IsNullOrEmpty(txtPhone.Text.Trim()) && !System.Text.RegularExpressions.Regex.IsMatch(txtPhone.Text, @"^\d{10,11}$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Số điện thoại không hợp lệ! Vui lòng nhập 10-11 chữ số.');", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE Users SET FullName = @FullName, Email = @Email, Phone = @Phone, Address = @Address WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(txtPhone.Text.Trim()) ? (object)DBNull.Value : txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(txtAddress.Text.Trim()) ? (object)DBNull.Value : txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@UserId", Session["UserId"].ToString());

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        LoadUserProfile();
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Cập nhật thông tin thành công!'); openAccountModal();", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Không có thông tin nào được cập nhật. Có thể UserId không tồn tại!');", true);
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Lỗi khi lưu thông tin: {ex.Message}');", true);
                }
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!'); window.location='Login.aspx';", true);
                return;
            }

            if (string.IsNullOrEmpty(txtNewPassword.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Mật khẩu mới không được để trống!');", true);
                return;
            }

            if (txtNewPassword.Text.Length < 8)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Mật khẩu mới phải có ít nhất 8 ký tự!');", true);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtNewPassword.Text, @"[A-Za-z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(txtNewPassword.Text, @"\d"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Mật khẩu mới phải chứa cả chữ cái và số!');", true);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Mật khẩu mới và xác nhận không khớp!');", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string checkQuery = "SELECT PasswordHash FROM Users WHERE UserId = @UserId";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@UserId", Session["UserId"].ToString());

                conn.Open();
                string storedPasswordHash = checkCmd.ExecuteScalar()?.ToString();

                string inputPasswordHash = HashPassword(txtOldPassword.Text);
                if (string.IsNullOrEmpty(storedPasswordHash) || storedPasswordHash != inputPasswordHash)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Mật khẩu cũ không đúng!');", true);
                    return;
                }

                string newPasswordHash = HashPassword(txtNewPassword.Text);
                string updateQuery = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
                updateCmd.Parameters.AddWithValue("@UserId", Session["UserId"].ToString());

                try
                {
                    updateCmd.ExecuteNonQuery();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Đổi mật khẩu thành công!');", true);
                    txtOldPassword.Text = "";
                    txtNewPassword.Text = "";
                    txtConfirmPassword.Text = "";
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Có lỗi xảy ra: {ex.Message}');", true);
                }
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        private string HashPassword(string password)
        {
            return password;
        }
    }
}