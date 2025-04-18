using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Vx
{
    public partial class AddEditShop : System.Web.UI.Page
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

                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Cửa Hàng";
                    if (!int.TryParse(Request.QueryString["id"], out int shopId))
                    {
                        ShowAlert("ID cửa hàng không hợp lệ!");
                        Response.Redirect("AdminDashboard.aspx");
                        return;
                    }
                    LoadShop(shopId);
                }
                else
                {
                    lblTitle.Text = "Thêm Cửa Hàng";
                }
            }
        }

        private void LoadShop(int shopId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ShopName FROM Shops WHERE ShopId = @ShopId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtShopName.Text = reader["ShopName"].ToString();
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy cửa hàng!");
                        Response.Redirect("AdminDashboard.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi tải thông tin cửa hàng: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                ShowAlert("Tên cửa hàng không được để trống!");
                return;
            }

            string mode = Request.QueryString["mode"];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;
                    SqlCommand cmd;

                    if (mode == "add")
                    {
                        query = "INSERT INTO Shops (ShopName) VALUES (@ShopName)";
                        cmd = new SqlCommand(query, conn);
                    }
                    else
                    {
                        if (!int.TryParse(Request.QueryString["id"], out int shopId))
                        {
                            ShowAlert("ID cửa hàng không hợp lệ!");
                            return;
                        }
                        query = "UPDATE Shops SET ShopName = @ShopName WHERE ShopId = @ShopId";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ShopId", shopId);
                    }

                    cmd.Parameters.AddWithValue("@ShopName", txtShopName.Text.Trim());
                    cmd.ExecuteNonQuery();

                    ShowAlert(mode == "add" ? "Thêm cửa hàng thành công!" : "Cập nhật cửa hàng thành công!");
                    Response.Redirect("AdminDashboard.aspx");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Lỗi khi lưu cửa hàng: {ex.Message}");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminDashboard.aspx");
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }
    }
}