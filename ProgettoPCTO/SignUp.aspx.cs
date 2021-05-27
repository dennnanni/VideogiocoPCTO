using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProgettoPCTO
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string email = txtEmail.Text;

            if (!CheckEmail(email))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Indirizzo email non valido.');", true);
                return;
            }

            if(username == "default")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Username non valido, sceglierne un altro.');", true);
                return;
            }

            SQLHandler command = new SQLHandler((string)Session["connection"]);

            if (command.ValidateUsernameAndEmail(username, email))
            {
                command.InsertAccount(username, email, Helper.HashPassword(password));
                Gameplay g = command.ReadData("default");
                command.WriteData(username, g);

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Registrazione avvenuta con successo.');", true);

                Response.Redirect("~/Login.aspx");
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Nome utente o email già assegnati, riprovare con altri.');", true);
                txtEmail.Text = "";
                txtPassword.Text = "";
                txtUsername.Text = "";
            }
        }

        private bool CheckEmail(string email)
        {
            // Check if there are the name and the domain
            string[] tmp = email.Split('@');
            if (tmp[0] is null || tmp[1] is null || tmp.Length == 1)
                return false;

            // Checks if there is a top level domain 
            string[] tmp2 = tmp[1].Split('.');
            if (tmp2[tmp2.Length - 1] is null)
                return false;

            return true;
        }
    }
}