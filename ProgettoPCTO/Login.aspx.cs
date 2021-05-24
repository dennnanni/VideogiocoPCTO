using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            if (!IsPostBack)
            {
                // Test the connection
                try
                {
                    using (SqlConnection conn = new SqlConnection("Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;"))
                    {
                        conn.Open();
                        Session["connection"] = "Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;";
                    }
                }
                catch
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection("Data Source = (local);Initial Catalog = Videogame;User ID=sa;Password=burbero2020"))
                        {
                            conn.Open();
                            Session["connection"] = "Data Source = (local);Initial Catalog = Videogame;User ID=sa;Password=burbero2020";
                        }
                    }
                    catch
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Impossibile raggiungere il database.');", true);
                    }
                }

                using (SqlConnection conn = new SqlConnection((string)Session["connection"]))
                {
                    conn.Open();
                    // caricare parte data.xml
                }
            }
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = Helper.HashPassword(txtPassword.Text);

            // change
            if (Helper.Authenticate(username, txtPassword.Text, "Data Source = (local);Initial Catalog = Videogame;Integrated Security = True;"))
            {
                Session["user"] = username;
                Page.Response.Redirect("~/Default.aspx", true);
            }
            else
            {

            }
        }
    }
}