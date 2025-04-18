using System;
using System.Collections.Generic;
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
                // Hiển thị tên người dùng
                if (Session["Username"] != null)
                {
                    lblUsername.Text = Session["Username"].ToString();
                }
                else
                {
                    lblUsername.Text = "Khách";
                }

                // Nếu người dùng vừa đăng nhập, chuyển TempCart sang Cart
                if (Session["UserId"] != null && Session["TempCart"] != null)
                {
                    TransferTempCartToDatabase();
                }

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
            cartTable.Columns.Add("Stock", typeof(int)); // Thêm cột Stock để kiểm tra

            decimal totalAmount = 0;

            if (Session["UserId"] != null)
            {
                // Người dùng đã đăng nhập: Lấy từ bảng Cart
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
            }
            else if (Session["TempCart"] != null)
            {
                // Người dùng chưa đăng nhập: Lấy từ Session
                var tempCart = (Dictionary<int, int>)Session["TempCart"];
                string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    foreach (var item in tempCart)
                    {
                        string query = "SELECT ProductName, Price, ImageUrl, Stock FROM Products WHERE ProductId = @ProductId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ProductId", item.Key);

                        try
                        {
                            conn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                DataRow row = cartTable.NewRow();
                                row["ProductId"] = item.Key;
                                row["ProductName"] = reader["ProductName"];
                                row["Price"] = reader["Price"];
                                row["Quantity"] = item.Value;
                                row["Total"] = Convert.ToDecimal(reader["Price"]) * item.Value;
                                row["ImageUrl"] = reader["ImageUrl"];
                                row["Stock"] = reader["Stock"];
                                totalAmount += Convert.ToDecimal(row["Total"]);
                                cartTable.Rows.Add(row);
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            ShowAlert($"Có lỗi xảy ra: {ex.Message}");
                        }
                    }
                }
            }

            // Vô hiệu hóa nút tăng nếu số lượng đã đạt đến Stock
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

        private void TransferTempCartToDatabase()
        {
            if (Session["UserId"] == null || Session["TempCart"] == null)
                return;

            var tempCart = (Dictionary<int, int>)Session["TempCart"];
            string userId = Session["UserId"].ToString();
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    foreach (var item in tempCart)
                    {
                        int productId = item.Key;
                        int quantity = item.Value;

                        // Kiểm tra tồn kho
                        int latestStock = GetLatestStock(productId);
                        if (quantity > latestStock)
                        {
                            ShowAlert($"Sản phẩm với ID {productId} có số lượng vượt quá tồn kho ({latestStock}). Đã điều chỉnh số lượng.");
                            quantity = latestStock;
                        }

                        if (quantity <= 0)
                            continue;

                        // Kiểm tra xem sản phẩm đã có trong Cart chưa
                        string checkQuery = "SELECT Quantity FROM Cart WHERE UserId = @UserId AND ProductId = @ProductId";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        checkCmd.Parameters.AddWithValue("@ProductId", productId);

                        object result = checkCmd.ExecuteScalar();
                        int newQuantity;

                        if (result != null)
                        {
                            int currentQuantity = Convert.ToInt32(result);
                            newQuantity = currentQuantity + quantity;
                            if (newQuantity > latestStock)
                            {
                                ShowAlert($"Sản phẩm với ID {productId} có số lượng vượt quá tồn kho ({latestStock}). Đã điều chỉnh số lượng.");
                                newQuantity = latestStock;
                            }

                            string updateQuery = "UPDATE Cart SET Quantity = @Quantity WHERE UserId = @UserId AND ProductId = @ProductId";
                            SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                            updateCmd.Parameters.AddWithValue("@Quantity", newQuantity);
                            updateCmd.Parameters.AddWithValue("@UserId", userId);
                            updateCmd.Parameters.AddWithValue("@ProductId", productId);
                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            newQuantity = quantity;
                            string insertQuery = "INSERT INTO Cart (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
                            SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                            insertCmd.Parameters.AddWithValue("@UserId", userId);
                            insertCmd.Parameters.AddWithValue("@ProductId", productId);
                            insertCmd.Parameters.AddWithValue("@Quantity", newQuantity);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    // Xóa TempCart sau khi chuyển
                    Session["TempCart"] = null;
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi chuyển giỏ hàng: {ex.Message}");
                }
            }
        }

        protected void rptCart_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Increase")
            {
                int latestStock = GetLatestStock(productId);
                if (Session["UserId"] != null)
                {
                    int currentQuantity = GetCurrentQuantityFromDatabase(Session["UserId"].ToString(), productId);
                    if (currentQuantity >= latestStock)
                    {
                        ShowAlert("Số lượng không thể vượt quá tồn kho!");
                        return;
                    }
                    UpdateQuantityInDatabase(Session["UserId"].ToString(), productId, 1);
                }
                else if (Session["TempCart"] != null)
                {
                    int currentQuantity = GetCurrentQuantityFromSession(productId);
                    if (currentQuantity >= latestStock)
                    {
                        ShowAlert("Số lượng không thể vượt quá tồn kho!");
                        return;
                    }
                    UpdateQuantityInSession(productId, 1);
                }
            }
            else if (e.CommandName == "Decrease")
            {
                if (Session["UserId"] != null)
                {
                    UpdateQuantityInDatabase(Session["UserId"].ToString(), productId, -1);
                }
                else if (Session["TempCart"] != null)
                {
                    UpdateQuantityInSession(productId, -1);
                }
            }
            else if (e.CommandName == "Delete")
            {
                if (Session["UserId"] != null)
                {
                    DeleteFromDatabase(Session["UserId"].ToString(), productId);
                }
                else if (Session["TempCart"] != null)
                {
                    DeleteFromSession(productId);
                }
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

        private int GetCurrentQuantityFromSession(int productId)
        {
            var tempCart = (Dictionary<int, int>)Session["TempCart"];
            return tempCart.ContainsKey(productId) ? tempCart[productId] : 0;
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

        private void UpdateQuantityInSession(int productId, int change)
        {
            var tempCart = (Dictionary<int, int>)Session["TempCart"];
            if (tempCart.ContainsKey(productId))
            {
                int newQuantity = tempCart[productId] + change;
                if (newQuantity > 0)
                {
                    tempCart[productId] = newQuantity;
                }
                else
                {
                    tempCart.Remove(productId);
                }
                Session["TempCart"] = tempCart;
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

        private void DeleteFromSession(int productId)
        {
            var tempCart = (Dictionary<int, int>)Session["TempCart"];
            tempCart.Remove(productId);
            Session["TempCart"] = tempCart;
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
                Response.Redirect("Login.aspx");
            }
            else
            {
                Response.Redirect("Checkout.aspx"); // Chuyển đến trang thanh toán (tạo riêng nếu cần)
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}