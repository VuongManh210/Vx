using System;
using System.Web.UI;

namespace Vx
{
    public partial class Search : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string searchQuery = Request.QueryString["query"];
            if (!string.IsNullOrEmpty(searchQuery))
            {
                lblResult.Text = "Bạn đã tìm kiếm: <b>" + searchQuery + "</b>";
            }
            else
            {
                lblResult.Text = "Không có từ khóa tìm kiếm.";
            }
        }
    }
}
