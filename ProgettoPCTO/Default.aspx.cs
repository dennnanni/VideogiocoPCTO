using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProgettoPCTO
{
    enum Cardinals : int { North, East, South, West }
    public partial class Default : System.Web.UI.Page
    {
        private Gameplay _game = null;
        private string currentAreaID = "area1";
        private Situation _currentSituation;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Gameplay game = new Gameplay();
                XMLHandler.Write(game, Server);
                //// Creates the gameplay and loads the first situation
                //_game = new Gameplay();
                //Session["game"] = _game;
                //Session["currentArea"] = currentAreaID;
                //_currentSituation = _game[currentAreaID];
                //this.LoadSituation(currentAreaID);
            }
            else
            {
                // Restores last changes 
                currentAreaID = Session["currentArea"].ToString();
                _game = (Gameplay)Session["game"];
                _currentSituation = _game[currentAreaID];
                this.LoadSituation(currentAreaID);

            }
        }

        protected void btnCardinal_Click(object sender, EventArgs e)
        {
            // Gets clicked direction
            string btnName = ((Button)sender).ID.Substring(3);
            int index = -1;
            bool exit = false;
            for (int i = 0; i < 4 && !exit; i++)
                if (btnName == ((Cardinals)i).ToString())
                {
                    index = i;
                    exit = true;
                }


            this.LoadSituation(_currentSituation.Areas[index]);

            Session["currentArea"] = currentAreaID;
            Session["gameplay"] = _game;
        }

        public void LoadSituation(string name) // Loading by name is much more easy to reuse code
        {
            Situation s = _game[name];
            pnlImages.Controls.Clear();
            pnlImages.BackImageUrl = s.ImageURL; // Load background
            currentAreaID = name; // Sets the global variable

            for (int i = 0; i < 4; i++)
            {
                if (s.Areas[i] == null)
                {
                    foreach (Control btn in pnlCardinals.Controls)
                    {
                        // Sorting buttons 
                        if (btn as Button != null && (btn as Button).ID == "btn" + (Cardinals)i)
                            (btn as Button).Enabled = false;
                    }
                }
                else
                {
                    foreach (Control btn in pnlCardinals.Controls)
                    {
                        if ((btn as Button) != null && (btn as Button).ID == "btn" + (Cardinals)i)
                            (btn as Button).Enabled = true;
                    }
                }

            }

            // Loading all entities in the situation if there are
            if(s.Entities != null)
                foreach (Entity e in s.Entities)
                {
                    ImageButton img = new ImageButton();
                    img.ImageUrl = e.ImageURL;
                    img.Style["position"] = "absolute";
                    img.Style["width"] = e.Width + "px";
                    img.Style["height"] = e.Height + "px";
                    img.Style["left"] = e.X + "px";
                    img.Style["top"] = e.Y + "px";
                    img.ID = "img" + e.Name;
                    pnlImages.Controls.Add(img);
                }

            if(s.Items != null)
                foreach(Item i in s.Items)
                {
                    ImageButton img = new ImageButton();
                    img.ImageUrl = i.ImageURL;
                    img.Style["position"] = "absolute";
                    img.Style["width"] = i.Width + "px";
                    img.Style["height"] = i.Height + "px";
                    img.Style["left"] = i.X + "px";
                    img.Style["top"] = i.Y + "px";
                    img.ID = "img" + i.Name;
                    pnlImages.Controls.Add(img);
                }
        }
    }
}