using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Vx
{
    public partial class AddEditShop : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string mode = Request.QueryString["mode"];
                if (mode == "edit" && !string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    lblTitle.Text = "Sửa Thông Tin Shop";
                    LoadShop(Request.QueryString["id"]);
                }
                else
                {
                    lblTitle.Text = "Thêm Shop";
                }
            }
        }

        private void LoadShop(string shopId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string query = @"
                        SELECT ShopName, Description, Address, PhoneNumber 
                        FROM Shops 
                        WHERE ShopId = @ShopId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtShopName.Text = reader["ShopName"].ToString();
                        txtDescription.Text = reader["Description"].ToString();
                        txtAddress.Text = reader["Address"].ToString();
                        txtPhone.Text = reader["PhoneNumber"].ToString();
                    }
                    else
                    {
                        ShowAlert("Không tìm thấy shop!");
                        Response.Redirect("AdminDashboard.aspx");
                    }
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi tải thông tin shop: {ex.Message}");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Kiểm tra hợp lệ dữ liệu
            if (string.IsNullOrWhiteSpace(txtShopName.Text))
            {
                ShowAlert("Tên shop không được để trống!");
                return;
            }

            string mode = Request.QueryString["mode"];
            string connStr = ConfigurationManager.ConnectionStrings["MyDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                ShowAlert("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra cấu hình!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string query = mode == "add" ?
                        @"INSERT INTO Shops (ShopId, ShopName, Description, Address, PhoneNumber, CreatedDate) 
                          VALUES (@ShopId, @ShopName, @Description, @Address, @PhoneNumber, @CreatedDate)" :
                        @"UPDATE Shops 
                          SET ShopName = @ShopName, Description = @Description, Address = @Address, PhoneNumber = @PhoneNumber 
                          WHERE ShopId = @ShopId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    string shopId = mode == "add" ? Guid.NewGuid().ToString() : Request.QueryString["id"];
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    cmd.Parameters.AddWithValue("@ShopName", txtShopName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtPhone.Text.Trim());
                    if (mode == "add")
                    {
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    ShowAlert(mode == "add" ? "Thêm shop thành công!" : "Cập nhật shop thành công!");
                    Response.Redirect("AdminDashboard.aspx");
                }
                catch (Exception ex)
                {
                    ShowAlert($"Có lỗi xảy ra khi lưu thông tin shop: {ex.Message}");
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