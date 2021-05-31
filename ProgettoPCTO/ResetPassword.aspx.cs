using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProgettoPCTO
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            SQLHandler handler = new SQLHandler((string)Session["connection"]);

            if (txtOtp.Enabled == false)
            {
                if (!CheckEmail(txtEmail.Text.TrimEnd(null), handler))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Indirizzo email non valido.');", true);
                    return;
                }

                txtOtp.Enabled = true;
                txtOtp.Visible = true;
                txtEmail.Enabled = false;
                lblOtp.Visible = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("assistance.videogame@gmail.com", "denisenanni"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                };

                string otp = (new Random()).Next(1000000).ToString();
                if (otp.Length < 6)
                {
                    var filler = new string('0', 6 - otp.Length);
                    otp = filler + otp;
                }

                Session["otp"] = otp;

                smtpClient.Send("assistance.videogame@gmail.com", txtEmail.Text, "Reimpostazione password", 
                    "Il tuo codice di reimpostazione è " + otp + ".");
            }
            else if(lblEmail.Text == "Email")
            {
                if(txtOtp.Text == (string)Session["otp"])
                {
                    Session["email"] = txtEmail.Text;
                    txtEmail.Enabled = true;
                    lblEmail.Text = "Password";
                    lblOtp.Text = "Conferma password";
                    txtEmail.Attributes.Remove("placeholder");
                    txtEmail.Attributes.Add("placeholder", "Enter password");
                    txtEmail.TextMode = TextBoxMode.Password;
                    txtOtp.Attributes.Remove("placeholder");
                    txtOtp.Attributes.Add("placeholder", "Renter password");
                    txtOtp.TextMode = TextBoxMode.Password;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Codice OTP errato.');", true);
                    return;
                }
            }
            else
            {
                if(!(txtEmail.Text == txtOtp.Text))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Le password non corrispondono.');", true);
                    return;
                }

                string username = handler.GetUsernameFromEmail((string)Session["email"]);
                handler.UpdatePassword(username, Helper.HashPassword(txtOtp.Text));

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AlertBox", "alert('Password modificata con successo.');", true);
                Response.Redirect("~/Login.aspx", true);
            }
        }

        private bool CheckEmail(string email, SQLHandler handler)
        {
            // Check if there are the name and the domain
            string[] tmp = email.Split('@');
            if (tmp[0] is null || tmp[1] is null || tmp.Length == 1)
                return false;

            // Checks if there is a top level domain 
            string[] tmp2 = tmp[1].Split('.');
            if (tmp2[tmp2.Length - 1] is null)
                return false;

            // Checks if the email exists in the database
            string user = handler.GetUsernameFromEmail(email);
            if (user is null)
                return false;

            return true;
        }
    }
}