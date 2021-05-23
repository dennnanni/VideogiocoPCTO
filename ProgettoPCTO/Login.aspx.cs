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
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = Helper.HashPassword(txtPassword.Text);

            // change
            if (true || Helper.Authenticate(username, txtPassword.Text, "Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;"))
            {
                Session["user"] = username;
                //SQLCommands command = new SQLCommands("Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;");
                //command.InsertAccount(username, "provaa", password);
                Page.Response.Redirect("~/Default.aspx", true);
            }
            else
            {

            }
        }
    }
}