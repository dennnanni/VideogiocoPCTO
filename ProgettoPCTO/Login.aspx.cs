using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProgettoPCTO
{
    public partial class Login : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.Response.Redirect("~/Default.aspx", true);
            Session["user"] = "den";
            Response.Redirect(String.Format("Default.aspx?user"));
        }
    }
}