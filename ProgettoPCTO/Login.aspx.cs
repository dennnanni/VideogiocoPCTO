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
            //if (!Page.IsPostBack)
            //{
            //    if (Request.QueryString["action"] == "reset")
            //    {
            //        pnlContainer.Visible = false;
            //        // pnlContainerReset.Visible = true;
            //        // pnlContainerSignUp.Visible = false;
            //    }
            //    else if (Request.QueryString["action"] == "signup")
            //    {
            //        pnlContainer.Visible = false;
            //        // pnlContainerReset.Visible = false;
            //        // pnlContainerSignUp.Visible = true;
            //    }
            //    else
            //    {
            //        pnlContainer.Visible = true;
            //        // pnlContainerReset.Visible = false;
            //        // pnlContainerSignUp.Visible = false;
            //    }

            //}
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = Helper.HashPassword(txtPassword.Text);

            if (true)//Helper.Authenticate(username, password, "Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;"))
            {
                Session["user"] = username;
                Response.Redirect(String.Format("Default.aspx"));
            }
            else
            {

            }
        }
    }
}