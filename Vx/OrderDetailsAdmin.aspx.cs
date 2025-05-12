using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class OrderDetailsAdmin : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["Role"] == null)
            {
                ShowAlert("Vui lòng đăng nhập để tiếp tục!");
                Response.Redirect("Login.aspx", false);
                return;
            }

            string role = Session["Role"].ToString();
            if (role != "Admin")
            {
                ShowAlert("Chỉ Admin mới có quyền truy cập trang này!");
                Response.Redirect("Home.aspx", false);
                return;
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra chuỗi kết nối 'MyDB'!");
                return;
            }

            if (!IsPostBack)
            {
                string orderId = Request.QueryString["id"];
                if (string.IsNullOrEmpty(orderId))
                {
                    ShowAlert("ID đơn hàng không hợp lệ!");
                    Response.Redirect("AdminDashboard.aspx", false);
                    return;
                }

                LoadOrderDetails(orderId);
                LoadOrderItems(orderId);
                LoadPayments(orderId);
            }
        }

        private void LoadOrderDetails(string orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT o.OrderId, u.Username, o.OrderDate, o.TotalAmount, o.Status, o.ShippingAddress, o.PhoneNumber
                        FROM Orders o
                        INNER JOIN Users u ON o.UserId = u.UserId
                        WHERE o.OrderId = @OrderId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblOrderId.Text = reader["OrderId"].ToString();
                        lblUsername.Text = reader["Username"].ToString();
                        lblOrderDate.Text = Convert.ToDateTime(reader["OrderDate"]).ToString("dd/MM/yyyy HH:mm");
                        lblTotalAmount.Text = Convert.ToDecimal(reader["TotalAmount"]).ToString("N0") + " VNĐ";
                        lblStatus.Text = reader["Status"].ToString();
                        lblShippingAddress.Text = reader["ShippingAddress"].ToString();
                        lblPhoneNumber.Text = reader["PhoneNumber"].ToString();

                        // Hiển thị nút hành động chỉ khi trạng thái là Pending
                        pnlActions.Visible = reader["Status"].ToString() == "Pending";
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy đơn hàng!");
                        Response.Redirect("AdminDashboard.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi tải thông tin đơn hàng: {ex.Message}";
                }
            }
        }

        private void LoadOrderItems(string orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT od.OrderDetailId, od.ProductId, p.ProductName, od.Quantity, od.UnitPrice
                        FROM OrderDetails od
                        INNER JOIN Products p ON od.ProductId = p.ProductId
                        WHERE od.OrderId = @OrderId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvOrderDetails.DataSource = dt;
                    gvOrderDetails.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.Text = "Không có sản phẩm nào trong đơn hàng này.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi tải danh sách sản phẩm: {ex.Message}";
                }
            }
        }

        private void LoadPayments(string orderId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT PaymentId, PaymentDate, Amount, PaymentMethod, Status
                        FROM Payments
                        WHERE OrderId = @OrderId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvPayments.DataSource = dt;
                    gvPayments.DataBind();

                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.Text = "Không có thông tin thanh toán cho đơn hàng này.";
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi tải thông tin thanh toán: {ex.Message}";
                }
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            string orderId = lblOrderId.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Orders SET Status = 'Confirmed' WHERE OrderId = @OrderId AND Status = 'Pending'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Xác nhận đơn hàng thành công!");
                        LoadOrderDetails(orderId); // Tải lại thông tin để cập nhật trạng thái
                    }
                    else
                    {
                        ShowAlert("Không thể xác nhận đơn hàng. Đơn hàng có thể đã được xử lý!");
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi xác nhận đơn hàng: {ex.Message}";
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string orderId = lblOrderId.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Orders SET Status = 'Cancelled' WHERE OrderId = @OrderId AND Status = 'Pending'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowAlert("Hủy đơn hàng thành công!");
                        LoadOrderDetails(orderId); // Tải lại thông tin để cập nhật trạng thái
                    }
                    else
                    {
                        ShowAlert("Không thể hủy đơn hàng. Đơn hàng có thể đã được xử lý!");
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Lỗi khi hủy đơn hàng: {ex.Message}";
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx", false);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx", false);
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message.Replace("'", "\\'")}');", true);
        }
    }
}