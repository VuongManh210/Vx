using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Vx
{
    public partial class QLBH : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnThem_Click(object sender, EventArgs e)
        {
            SqlDataSource1.InsertParameters["Mahang"].DefaultValue = txtMahang.Text;
            SqlDataSource1.InsertParameters["Tenhang"].DefaultValue = txtTenhang.Text;
            SqlDataSource1.InsertParameters["MoTa"].DefaultValue = txtMoTa.Text;

            // Kiểm tra giá trị số
            double dongianhap, dongiaban;
            if (double.TryParse(txtDongianhap.Text, out dongianhap) && double.TryParse(txtDongiaban.Text, out dongiaban))
            {
                SqlDataSource1.InsertParameters["Dongianhap"].DefaultValue = dongianhap.ToString();
                SqlDataSource1.InsertParameters["Dongiaban"].DefaultValue = dongiaban.ToString();
            }
            else
            {
                Response.Write("<script>alert('Vui lòng nhập số hợp lệ cho Đơn giá nhập và Đơn giá bán!');</script>");
                return;
            }

            SqlDataSource1.InsertParameters["Ghichu"].DefaultValue = txtGhichu.Text;

            int result = SqlDataSource1.Insert();
            if (result > 0)
            {
                Response.Write("<script>alert('Thêm thành công!');</script>");

                // XÓA DỮ LIỆU TRONG TEXTBOX
                txtMahang.Text = "";
                txtTenhang.Text = "";
                txtMoTa.Text = "";
                txtDongianhap.Text = "";
                txtDongiaban.Text = "";
                txtGhichu.Text = "";
            }
            else
            {
                Response.Write("<script>alert('Thêm thất bại!');</script>");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}