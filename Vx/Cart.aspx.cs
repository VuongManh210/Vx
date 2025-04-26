using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class Cart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra đăng nhập
                if (Session["UserId"] == null)
                {
                    ShowAlert("Vui lòng đăng nhập để xem giỏ hàng!");
                    Response.Redirect("Login.aspx?ReturnUrl=Cart.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Hiển thị tên người dùng
                lblUsername.Text = Session["Username"]?.ToString() ?? "Khách";

                LoadCart();
            }
        }

        private void LoadCart()
        {
            DataTable cartTable = new DataTable();
            cartTable.Columns.Add("ProductId", typeof(int));
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("Price", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("Total", typeof(decimal));
            cartTable.Columns.Add("ImageUrl", typeof(string));
            cartTable.Columns.Add("Stock", typeof(int));

            decimal totalAmount = 0;

            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT c.ProductId, p.ProductName, p.Price, c.Quantity, p.ImageUrl, p.Stock
                    FROM Cart c
                    JOIN Products p ON c.ProductId = p.ProductId
                    WHERE c.UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", Session["UserId"].ToString());

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow row = cartTable.NewRow();
                        row["ProductId"] = reader["ProductId"];
                        row["ProductName"] = reader["ProductName"];
                        row["Price"] = reader["Price"];
                        row["Quantity"] = reader["Quantity"];
                        row["Total"] = Convert.ToDecimal(reader["Price"]) * Convert.ToInt32(reader["Quantity"]);
                        row["ImageUrl"] = reader["ImageUrl"];
                        row["Stock"] = reader["Stock"];
                        totalAmount += Convert.ToDecimal(row["Total"]);
                        cartTable.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                }
            }

            // Vô hiệu hóa nút tăng nếu số lượng đạt giới hạn tồn kho
            rptCart.DataSource = cartTable;
            rptCart.DataBind();

            foreach (RepeaterItem item in rptCart.Items)
            {
                Button btnTang = (Button)item.FindControl("btnTang");
                int quantity = Convert.ToInt32(((TextBox)item.FindControl("txtSoLuong")).Text);
                int stock = Convert.ToInt32(cartTable.Rows[item.ItemIndex]["Stock"]);
                if (quantity >= stock)
                {
                    btnTang.Enabled = false;
                    btnTang.Style.Add("background", "#ccc");
                    btnTang.Style.Add("cursor", "not-allowed");
                }
            }

            lblTongTien.Text = totalAmount.ToString("N0") + " VND";
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Increase")
            {
                int latestStock = GetLatestStock(productId);
                int currentQuantity = GetCurrentQuantityFromDatabase(Session["UserId"].ToString(), productId);
                if (currentQuantity >= latestStock)
                {
                    ShowAlert("Số lượng không thể vượt quá tồn kho!");
                    return;
                }
                UpdateQuantityInDatabase(Session["UserId"].ToString(), productId, 1);
            }
            else if (e.CommandName == "Decrease")
            {
                UpdateQuantityInDatabase(Session["UserId"].ToString(), productId, -1);
            }
            else if (e.CommandName == "Delete")
            {
                DeleteFromDatabase(Session["UserId"].ToString(), productId);
            }

            LoadCart();
        }

        private int GetCurrentQuantityFromDatabase(string userId, int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Quantity FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                    return 0;
                }
            }
        }

        private void UpdateQuantityInDatabase(string userId, int productId, int change)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Quantity FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int currentQuantity = Convert.ToInt32(result);
                        int newQuantity = currentQuantity + change;
                        if (newQuantity > 0)
                        {
                            string updateQuery = "UPDATE Cart SET Quantity = @Quantity WHERE UserId = @UserId AND ProductId = @ProductId";
                            SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                            updateCmd.Parameters.AddWithValue("@Quantity", newQuantity);
                            updateCmd.Parameters.AddWithValue("@UserId", userId);
                            updateCmd.Parameters.AddWithValue("@ProductId", productId);
                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            DeleteFromDatabase(userId, productId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        private void DeleteFromDatabase(string userId, int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                }
            }
        }

        private int GetLatestStock(int productId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Stock FROM Products WHERE ProductId = @ProductId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi kiểm tra tồn kho: {ex.Message}");
                    return 0;
                }
            }
        }

        protected void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                ShowAlert("Vui lòng đăng nhập để thanh toán!");
                Response.Redirect("Login.aspx?ReturnUrl=Cart.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                Response.Redirect("Checkout.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            ShowAlert("Đăng xuất thành công!");
            Response.Redirect("Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}