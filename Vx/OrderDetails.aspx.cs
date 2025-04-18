using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class OrderDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để xem chi tiết đơn hàng!");
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (Session["Username"] != null)
                {
                    lblUsername.Text = Session["Username"].ToString();
                }
                else
                {
                    lblUsername.Text = "Khách";
                }

                string orderId = Request.QueryString["OrderId"];
                if (string.IsNullOrEmpty(orderId) || !int.TryParse(orderId, out _))
                {
                    ShowAlert("Mã đơn hàng không hợp lệ!");
                    Response.Redirect("Home.aspx");
                    return;
                }

                LoadOrderDetails(orderId);
            }
        }

        private void LoadOrderDetails(string orderId)
        {
            string userId = Session["UserId"]?.ToString();
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Có lỗi xảy ra. Vui lòng thử lại!");
                Response.Redirect("Home.aspx");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    // Lấy thông tin đơn hàng và thông tin người dùng
                    string orderQuery = @"
                        SELECT o.OrderId, o.OrderDate, o.TotalAmount, o.Status, o.ShippingAddress, o.PhoneNumber, u.FullName
                        FROM Orders o
                        JOIN Users u ON o.UserId = u.UserId
                        WHERE o.OrderId = @OrderId AND o.UserId = @UserId";
                    SqlCommand orderCmd = new SqlCommand(orderQuery, conn);
                    orderCmd.Parameters.AddWithValue("@OrderId", orderId);
                    orderCmd.Parameters.AddWithValue("@UserId", userId);

                    SqlDataReader orderReader = orderCmd.ExecuteReader();
                    if (orderReader.Read())
                    {
                        lblOrderId.Text = orderId;
                        lblFullName.Text = orderReader["FullName"].ToString();
                        lblPhoneNumber.Text = orderReader["PhoneNumber"].ToString();
                        lblShippingAddress.Text = orderReader["ShippingAddress"].ToString();
                        lblOrderDate.Text = Convert.ToDateTime(orderReader["OrderDate"]).ToString("dd/MM/yyyy HH:mm:ss");
                        lblStatus.Text = orderReader["Status"].ToString();
                        btnCancelOrder.Visible = lblStatus.Text == "Pending";
                    }
                    else
                    {
                        orderReader.Close();
                        ShowAlert("Đơn hàng không tồn tại hoặc không thuộc về bạn!");
                        Response.Redirect("Home.aspx");
                        return;
                    }
                    orderReader.Close();

                    // Lấy chi tiết đơn hàng và tính tổng tiền
                    DataTable orderDetailsTable = new DataTable();
                    orderDetailsTable.Columns.Add("ProductId", typeof(int));
                    orderDetailsTable.Columns.Add("ProductName", typeof(string));
                    orderDetailsTable.Columns.Add("UnitPrice", typeof(decimal));
                    orderDetailsTable.Columns.Add("Quantity", typeof(int));
                    orderDetailsTable.Columns.Add("Total", typeof(decimal));
                    orderDetailsTable.Columns.Add("ImageUrl", typeof(string));

                    decimal calculatedTotalAmount = 0; // Biến để tính tổng tiền từ OrderDetails

                    string detailsQuery = @"
                        SELECT od.ProductId, p.ProductName, od.UnitPrice, od.Quantity, p.ImageUrl
                        FROM OrderDetails od
                        JOIN Products p ON od.ProductId = p.ProductId
                        WHERE od.OrderId = @OrderId";
                    SqlCommand detailsCmd = new SqlCommand(detailsQuery, conn);
                    detailsCmd.Parameters.AddWithValue("@OrderId", orderId);

                    SqlDataReader detailsReader = detailsCmd.ExecuteReader();
                    while (detailsReader.Read())
                    {
                        DataRow row = orderDetailsTable.NewRow();
                        row["ProductId"] = detailsReader["ProductId"];
                        row["ProductName"] = detailsReader["ProductName"];
                        row["UnitPrice"] = detailsReader["UnitPrice"];
                        row["Quantity"] = detailsReader["Quantity"];
                        decimal unitPrice = Convert.ToDecimal(detailsReader["UnitPrice"]);
                        int quantity = Convert.ToInt32(detailsReader["Quantity"]);
                        decimal total = unitPrice * quantity;
                        row["Total"] = total;
                        row["ImageUrl"] = detailsReader["ImageUrl"];
                        calculatedTotalAmount += total; // Cộng dồn tổng tiền
                        orderDetailsTable.Rows.Add(row);
                    }
                    detailsReader.Close();

                    rptOrderDetails.DataSource = orderDetailsTable;
                    rptOrderDetails.DataBind();

                    // Hiển thị tổng tiền tính từ OrderDetails
                    lblTotalAmount.Text = calculatedTotalAmount.ToString("N0") + " VND";
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải chi tiết đơn hàng: {ex.Message}");
                }
            }
        }

        protected void btnCancelOrder_Click(object sender, EventArgs e)
        {
            string orderId = lblOrderId.Text;
            string userId = Session["UserId"]?.ToString();
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Có lỗi xảy ra. Vui lòng thử lại!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Kiểm tra trạng thái đơn hàng
                        string checkStatusQuery = "SELECT Status FROM Orders WHERE OrderId = @OrderId AND UserId = @UserId";
                        SqlCommand checkStatusCmd = new SqlCommand(checkStatusQuery, conn, transaction);
                        checkStatusCmd.Parameters.AddWithValue("@OrderId", orderId);
                        checkStatusCmd.Parameters.AddWithValue("@UserId", userId);
                        string status = checkStatusCmd.ExecuteScalar()?.ToString();

                        if (status != "Pending")
                        {
                            ShowAlert("Bạn chỉ có thể hủy đơn hàng khi trạng thái là 'Pending'!");
                            transaction.Rollback();
                            return;
                        }

                        // Lấy chi tiết đơn hàng để hoàn tồn kho
                        DataTable orderDetails = new DataTable();
                        orderDetails.Columns.Add("ProductId", typeof(int));
                        orderDetails.Columns.Add("Quantity", typeof(int));

                        string detailsQuery = "SELECT ProductId, Quantity FROM OrderDetails WHERE OrderId = @OrderId";
                        SqlCommand detailsCmd = new SqlCommand(detailsQuery, conn, transaction);
                        detailsCmd.Parameters.AddWithValue("@OrderId", orderId);

                        SqlDataReader detailsReader = detailsCmd.ExecuteReader();
                        while (detailsReader.Read())
                        {
                            DataRow row = orderDetails.NewRow();
                            row["ProductId"] = detailsReader["ProductId"];
                            row["Quantity"] = detailsReader["Quantity"];
                            orderDetails.Rows.Add(row);
                        }
                        detailsReader.Close();

                        // Hoàn tồn kho
                        foreach (DataRow row in orderDetails.Rows)
                        {
                            int productId = Convert.ToInt32(row["ProductId"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);

                            string updateStockQuery = "UPDATE Products SET Stock = Stock + @Quantity WHERE ProductId = @ProductId";
                            SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn, transaction);
                            updateStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                            updateStockCmd.Parameters.AddWithValue("@ProductId", productId);
                            updateStockCmd.ExecuteNonQuery();
                        }

                        // Cập nhật trạng thái đơn hàng
                        string updateOrderQuery = "UPDATE Orders SET Status = 'Cancelled' WHERE OrderId = @OrderId";
                        SqlCommand updateOrderCmd = new SqlCommand(updateOrderQuery, conn, transaction);
                        updateOrderCmd.Parameters.AddWithValue("@OrderId", orderId);
                        updateOrderCmd.ExecuteNonQuery();

                        // Cập nhật trạng thái thanh toán (nếu có)
                        string updatePaymentQuery = "UPDATE Payments SET Status = 'Cancelled' WHERE OrderId = @OrderId";
                        SqlCommand updatePaymentCmd = new SqlCommand(updatePaymentQuery, conn, transaction);
                        updatePaymentCmd.Parameters.AddWithValue("@OrderId", orderId);
                        updatePaymentCmd.ExecuteNonQuery();

                        // Commit giao dịch
                        transaction.Commit();

                        ShowAlert("Đơn hàng đã được hủy thành công!");
                        LoadOrderDetails(orderId); // Tải lại trang để cập nhật trạng thái
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowAlert($"Có lỗi xảy ra khi hủy đơn hàng: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi kết nối cơ sở dữ liệu: {ex.Message}");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string category = ddlCategory.SelectedValue;
            Response.Redirect($"Home.aspx?search={searchText}&category={category}");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}