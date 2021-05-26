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

                // Checks if the default account in the db exists, if it does not, then reads from the XML file the game config and
                // uploads the game scheme in the db
                using(SqlConnection conn = new SqlConnection((string)Session["connection"]))
                {
                    conn.Open();
                    SqlCommand select = new SqlCommand("SELECT * FROM Account WHERE Username = 'default';", conn);
                    SqlDataReader reader = select.ExecuteReader();
                    if (!reader.Read())
                    {
                        Gameplay g = new Gameplay();
                        g.Initialize();
                        SQLCommands handler = new SQLCommands((string)Session["connection"]);
                        handler.InsertAccount("default", "default", "default");

                        g.Save(Server);
                        // Read from XML file and upload datas into the db
                        g = (new Gameplay()).SetUp(Server);
                        
                        handler.InsertSituation(g.Situations);
                        foreach(string key in g.Situations.Keys)
                        {
                            List<Entity> entities = new List<Entity>();
                            if (!(g.Situations[key].Entities is null))
                                entities.AddRange(g.Situations[key].Entities);
                            if (!(g.Situations[key].Items is null))
                                entities.AddRange(g.Situations[key].Items);
                            handler.InsertImage(g.Situations[key].IdSituation, entities);
                        }
                        handler.WriteData("default", g);
                    }

                }
            }
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = Helper.HashPassword(txtPassword.Text);

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